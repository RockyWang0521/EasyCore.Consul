namespace EasyCore.Consul.Locking
{
    public interface IConsulLocking
    {
        /// <summary>
        /// Consul加锁
        /// </summary>
        /// <param name="lockKey">锁名字</param>
        /// <param name="ttl">锁的有效时间</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string?> AcquireLock(string lockKey, TimeSpan ttl, CancellationToken cancellationToken = default);

        /// <summary>
        /// Consul加锁
        /// </summary>
        /// <param name="lockKey">锁名字</param>
        /// <param name="ttl">锁的有效时间</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string?> AcquireLock(string lockKey, int ttl, CancellationToken cancellationToken = default);

        /// <summary>
        /// Consul释放锁
        /// </summary>
        /// <param name="lockKey">锁名字</param>
        /// <param name="sessionId">sessionid</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ReleaseLock(string lockKey, string sessionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Consul自动管理锁
        /// </summary>
        /// <param name="lockKey">锁名字</param>
        /// <param name="ttl">锁的有效时间</param>
        /// <param name="action">要执行的操作</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ExecuteLocked(string lockKey, TimeSpan ttl, Action action, CancellationToken cancellationToken = default);

        /// <summary>
        /// Consul自动管理锁
        /// </summary>
        /// <param name="lockKey">锁名字</param>
        /// <param name="ttl">锁的有效时间</param>
        /// <param name="action">要执行的操作</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ExecuteLocked(string lockKey, int ttl, Action action, CancellationToken cancellationToken = default);
    }
}
