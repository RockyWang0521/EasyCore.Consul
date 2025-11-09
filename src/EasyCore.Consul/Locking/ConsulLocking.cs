using System.Text;
using Consul;
using Microsoft.Extensions.Logging;

namespace EasyCore.Consul.Locking;

internal sealed class ConsulLocking : IConsulLocking
{
    private readonly IConsulClient _consulClient;
    private readonly ILogger<ConsulLocking> _logger;

    public ConsulLocking(IConsulClient consulClient, ILogger<ConsulLocking> logger)
    {
        _consulClient = consulClient;
        _logger = logger;
    }

    public Task<IConsulLock?> TryAcquireAsync(
        string lockKey,
        int ttlSeconds,
        CancellationToken cancellationToken = default)
    {
        if (ttlSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ttlSeconds), "TTL must be positive.");
        }

        return TryAcquireAsync(lockKey, TimeSpan.FromSeconds(ttlSeconds), cancellationToken);
    }

    public async Task<IConsulLock?> TryAcquireAsync(
        string lockKey,
        TimeSpan ttl,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lockKey);
        if (ttl <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(ttl), "TTL must be positive.");
        }

        var sessionId = await CreateAndAcquireAsync(lockKey, ttl, cancellationToken).ConfigureAwait(false);
        if (sessionId is null)
        {
            return null;
        }

        return new ConsulLock(_consulClient, lockKey, sessionId, ttl, _logger);
    }

    public async Task ExecuteLockedAsync(
        string lockKey,
        TimeSpan ttl,
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        await using var lease = await TryAcquireAsync(lockKey, ttl, cancellationToken).ConfigureAwait(false);
        if (lease is null)
        {
            throw new ConsulLockException($"Could not obtain lock for key '{lockKey}'.");
        }

        await action(cancellationToken).ConfigureAwait(false);
    }

    public Task ExecuteLockedAsync(
        string lockKey,
        int ttlSeconds,
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        if (ttlSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ttlSeconds), "TTL must be positive.");
        }

        return ExecuteLockedAsync(lockKey, TimeSpan.FromSeconds(ttlSeconds), action, cancellationToken);
    }

    public async Task<string?> AcquireLock(
        string lockKey,
        TimeSpan ttl,
        CancellationToken cancellationToken = default)
        => await CreateAndAcquireAsync(lockKey, ttl, cancellationToken).ConfigureAwait(false);

    public Task<string?> AcquireLock(
        string lockKey,
        int ttlSeconds,
        CancellationToken cancellationToken = default)
    {
        if (ttlSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ttlSeconds), "TTL must be positive.");
        }

        return AcquireLock(lockKey, TimeSpan.FromSeconds(ttlSeconds), cancellationToken);
    }

    public async Task<bool> ReleaseLock(
        string lockKey,
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lockKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);

        var pair = new KVPair(lockKey) { Session = sessionId };
        var released = await _consulClient.KV.Release(pair, cancellationToken).ConfigureAwait(false);
        if (!released.Response)
        {
            return false;
        }

        await _consulClient.Session.Destroy(sessionId, cancellationToken).ConfigureAwait(false);
        return true;
    }

    public Task ExecuteLocked(
        string lockKey,
        TimeSpan ttl,
        Action action,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        return ExecuteLockedAsync(lockKey, ttl, _ =>
        {
            action();
            return Task.CompletedTask;
        }, cancellationToken);
    }

    public Task ExecuteLocked(
        string lockKey,
        int ttlSeconds,
        Action action,
        CancellationToken cancellationToken = default)
    {
        if (ttlSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ttlSeconds), "TTL must be positive.");
        }

        return ExecuteLocked(lockKey, TimeSpan.FromSeconds(ttlSeconds), action, cancellationToken);
    }

    private async Task<string?> CreateAndAcquireAsync(
        string lockKey,
        TimeSpan ttl,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lockKey);

        var sessionEntry = new SessionEntry
        {
            Name = $"EasyCore.Consul.lock.{lockKey}",
            TTL = ttl,
            Behavior = SessionBehavior.Delete
        };

        var sessionResponse = await _consulClient.Session.Create(sessionEntry, cancellationToken)
            .ConfigureAwait(false);
        var sessionId = sessionResponse.Response;
        if (string.IsNullOrEmpty(sessionId))
        {
            return null;
        }

        var pair = new KVPair(lockKey)
        {
            Session = sessionId,
            Value = Encoding.UTF8.GetBytes("locked")
        };

        var acquire = await _consulClient.KV.Acquire(pair, cancellationToken).ConfigureAwait(false);
        if (!acquire.Response)
        {
            try
            {
                await _consulClient.Session.Destroy(sessionId, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to destroy unused Consul session {SessionId}", sessionId);
            }

            return null;
        }

        return sessionId;
    }
}

internal sealed class ConsulLock : IConsulLock
{
    private readonly IConsulClient _consulClient;
    private readonly ILogger _logger;
    private readonly CancellationTokenSource _renewCts = new();
    private readonly Task _renewTask;
    private int _disposed;

    public string LockKey { get; }
    public string SessionId { get; }

    public ConsulLock(
        IConsulClient consulClient,
        string lockKey,
        string sessionId,
        TimeSpan ttl,
        ILogger logger)
    {
        _consulClient = consulClient;
        LockKey = lockKey;
        SessionId = sessionId;
        _logger = logger;
        _renewTask = RenewLoopAsync(ttl, _renewCts.Token);
    }

    private async Task RenewLoopAsync(TimeSpan ttl, CancellationToken cancellationToken)
    {
        try
        {
            await _consulClient.Session
                .RenewPeriodic(ttl, SessionId, WriteOptions.Default, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // expected on release
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Consul session renew failed for lock {LockKey} / session {SessionId}", LockKey, SessionId);
        }
    }

    public void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1)
        {
            return;
        }

        _renewCts.Cancel();
        try
        {
            await _renewTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }

        try
        {
            var pair = new KVPair(LockKey) { Session = SessionId };
            await _consulClient.KV.Release(pair).ConfigureAwait(false);
            await _consulClient.Session.Destroy(SessionId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to release Consul lock {LockKey}", LockKey);
        }
        finally
        {
            _renewCts.Dispose();
        }
    }
}
