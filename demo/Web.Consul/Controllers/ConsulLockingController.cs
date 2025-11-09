using EasyCore.Consul.Locking;
using Microsoft.AspNetCore.Mvc;

namespace Web.Consul.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConsulLockingController : ControllerBase
{
    private readonly IConsulLocking _consulLocking;
    private readonly ILogger<ConsulLockingController> _logger;

    public ConsulLockingController(IConsulLocking consulLocking, ILogger<ConsulLockingController> logger)
    {
        _consulLocking = consulLocking;
        _logger = logger;
    }

    [HttpPost("lock:{lockKey}/{lockTTL}")]
    public async Task<IActionResult> Acquire(string lockKey, int lockTTL, CancellationToken cancellationToken)
    {
        await using var lease = await _consulLocking.TryAcquireAsync(lockKey, lockTTL, cancellationToken);
        if (lease is null)
        {
            return Conflict(new { message = "Lock not acquired" });
        }

        _logger.LogInformation("Lock acquired with session id: {SessionId}", lease.SessionId);
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        return Ok(new { lease.SessionId, lease.LockKey });
    }

    [HttpPost("ExecuteLocked:{lockKey}/{lockTTL}")]
    public async Task<IActionResult> ExecuteLocked(string lockKey, int lockTTL, CancellationToken cancellationToken)
    {
        await _consulLocking.ExecuteLockedAsync(lockKey, lockTTL, async ct =>
        {
            _logger.LogInformation("Executing critical section under lock.");
            await Task.Delay(TimeSpan.FromSeconds(5), ct);
        }, cancellationToken);

        _logger.LogInformation("Execution finished.");
        return Ok();
    }
}
