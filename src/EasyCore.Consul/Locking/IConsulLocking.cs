namespace EasyCore.Consul.Locking;

/// <summary>Distributed lock built on Consul sessions + KV acquire/release.</summary>
public interface IConsulLocking
{
    /// <summary>
    /// Acquires a lock. Returns an <see cref="IConsulLock"/> that renews the session until disposed,
    /// or null when the lock could not be acquired.
    /// </summary>
    Task<IConsulLock?> TryAcquireAsync(
        string lockKey,
        TimeSpan ttl,
        CancellationToken cancellationToken = default);

    Task<IConsulLock?> TryAcquireAsync(
        string lockKey,
        int ttlSeconds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes <paramref name="action"/> under a lock. Throws <see cref="ConsulLockException"/> if acquire fails.
    /// </summary>
    Task ExecuteLockedAsync(
        string lockKey,
        TimeSpan ttl,
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default);

    Task ExecuteLockedAsync(
        string lockKey,
        int ttlSeconds,
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default);

    /// <summary>Legacy acquire that returns session id only (no automatic renewal).</summary>
    Task<string?> AcquireLock(string lockKey, TimeSpan ttl, CancellationToken cancellationToken = default);

    Task<string?> AcquireLock(string lockKey, int ttlSeconds, CancellationToken cancellationToken = default);

    Task<bool> ReleaseLock(string lockKey, string sessionId, CancellationToken cancellationToken = default);

    /// <summary>Legacy sync callback wrapper.</summary>
    Task ExecuteLocked(string lockKey, TimeSpan ttl, Action action, CancellationToken cancellationToken = default);

    Task ExecuteLocked(string lockKey, int ttlSeconds, Action action, CancellationToken cancellationToken = default);
}

/// <summary>Held Consul lock lease. Dispose / dispose async to release.</summary>
public interface IConsulLock : IAsyncDisposable, IDisposable
{
    string LockKey { get; }
    string SessionId { get; }
}

public sealed class ConsulLockException : Exception
{
    public ConsulLockException(string message) : base(message)
    {
    }

    public ConsulLockException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
