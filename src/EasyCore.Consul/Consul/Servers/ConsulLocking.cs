using Consul;
using EasyCore.Consul.Servers;
using Microsoft.Extensions.Options;
using System.Text;

namespace EasyCore.Consul.Locking
{
    public class ConsulLocking : ConsulBase, IConsulLocking
    {
        private readonly ConsulOptions _options;

        public ConsulLocking(IOptions<ConsulOptions> options) => _options = options.Value;

        public async Task<string?> AcquireLock(string lockKey, int ttl, CancellationToken cancellationToken = default)
        {
            if (ttl <= 0) throw new ArgumentException("TTL must be a positive integer.", nameof(ttl));

            var time = TimeSpan.FromSeconds(ttl);

            return await AcquireLock(lockKey, time, cancellationToken);
        }

        public async Task<string?> AcquireLock(string lockKey, TimeSpan ttl, CancellationToken cancellationToken = default)
        {
            using var client = base.GetConsulClient(_options.ConsulAddress);

            var sessionEntry = new SessionEntry
            {
                Name = "ConsulLockSession",
                TTL = ttl,
                Behavior = SessionBehavior.Delete
            };

            var sessionIdResponse = await client.Session.Create(sessionEntry, cancellationToken);

            var sessionId = sessionIdResponse.Response;

            if (string.IsNullOrEmpty(sessionId)) return null;

            var keyValue = new KVPair(lockKey)
            {
                Session = sessionId,
                Value = Encoding.UTF8.GetBytes("locked")
            };

            var acquireResponse = await client.KV.Acquire(keyValue, cancellationToken);

            return acquireResponse.Response ? sessionId : null;
        }

        public async Task<bool> ReleaseLock(string lockKey, string sessionId, CancellationToken cancellationToken = default)
        {
            using var client = base.GetConsulClient(_options.ConsulAddress);

            var keyValue = new KVPair(lockKey) { Session = sessionId };

            var released = await client.KV.Release(keyValue, cancellationToken);

            if (released.Response)
            {
                await client.Session.Destroy(sessionId, cancellationToken);

                return true;
            }
            return false;
        }

        public async Task ExecuteLocked(string lockKey, int ttl, Action action, CancellationToken cancellationToken = default)
        {
            if (ttl <= 0) throw new ArgumentException("TTL must be a positive integer.", nameof(ttl));

            var time = TimeSpan.FromSeconds(ttl);

            await ExecuteLocked(lockKey, time, action, cancellationToken);
        }

        public async Task ExecuteLocked(string lockKey, TimeSpan ttl, Action action, CancellationToken cancellationToken = default)
        {
            using var client = base.GetConsulClient(_options.ConsulAddress);

            if (action == null) throw new ArgumentNullException(nameof(action));

            var sessionId = await AcquireLock(lockKey, ttl, cancellationToken);

            if (string.IsNullOrEmpty(sessionId)) throw new Exception("Could not obtain the lock");

            try
            {
                action();
            }
            finally
            {
                await ReleaseLock(lockKey, sessionId, cancellationToken);
            }
        }
    }
}
