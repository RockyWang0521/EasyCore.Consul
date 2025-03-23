using EasyCore.Consul.Locking;
using Microsoft.AspNetCore.Mvc;

namespace Web.Consul.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsulLockingController : ControllerBase
    {
        private readonly IConsulLocking _consulLocking;
        private readonly ILogger<ConsulLockingController> _logger;

        public ConsulLockingController(IConsulLocking consulLocking,
                ILogger<ConsulLockingController> logger)
        {
            _consulLocking = consulLocking;
            _logger = logger;
        }

        [HttpPost("lock:{lockKey}/{lockTTL}")]
        public async Task ConsulLocking(string lockKey, int lockTTL)
        {
            string? sessionId = await _consulLocking.AcquireLock(lockKey, lockTTL);
            if (sessionId != null)
            {
                _logger.LogInformation($"Lock acquired with session id: {sessionId}");
                await Task.Delay(5000);
                await _consulLocking.ReleaseLock(lockKey, sessionId);
            }
        }

        [HttpPost("ExecuteLocked:{lockKey}/{lockTTL}")]
        public async Task ExecuteLocked(string lockKey, int lockTTL)
        {
            await _consulLocking.ExecuteLocked(lockKey, lockTTL, async () =>
            {
                _logger.LogInformation("Executing critical section under lock.");
                await Task.Delay(5000);
            });

            _logger.LogInformation("Execution finished.");
        }
    }
}
