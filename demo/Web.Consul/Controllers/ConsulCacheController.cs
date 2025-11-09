using EasyCore.Consul.Cache;
using Microsoft.AspNetCore.Mvc;
using Web.Consul.Pojo;

namespace Web.Consul.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConsulCacheController : ControllerBase
{
    private readonly IConsulCache _consulCache;

    public ConsulCacheController(IConsulCache consulCache) => _consulCache = consulCache;

    [HttpPost("string:{key}/{value}")]
    public Task<bool> Post(string key, string value, CancellationToken cancellationToken)
        => _consulCache.PutAsync(key, value, cancellationToken);

    [HttpPost("dto:{key}")]
    public Task<bool> Post(string key, ConsulCacheDto value, CancellationToken cancellationToken)
        => _consulCache.PutAsync(key, value, cancellationToken);

    [HttpGet("string:{key}")]
    public Task<string?> GetString(string key, CancellationToken cancellationToken)
        => _consulCache.GetStringAsync(key, cancellationToken);

    [HttpGet("dto:{key}")]
    public Task<ConsulCacheDto?> GetDto(string key, CancellationToken cancellationToken)
        => _consulCache.GetAsync<ConsulCacheDto>(key, cancellationToken);

    [HttpDelete("{key}")]
    public Task<bool> Delete(string key, CancellationToken cancellationToken)
        => _consulCache.DeleteAsync(key, cancellationToken);
}
