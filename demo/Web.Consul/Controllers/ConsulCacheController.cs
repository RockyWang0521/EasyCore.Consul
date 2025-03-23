using Consul;
using EasyCore.Consul.Cache;
using Microsoft.AspNetCore.Mvc;
using Web.Consul.Pojo;

namespace Web.Consul.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsulCacheController : ControllerBase
    {
        private readonly IConsulCache _consulCache;

        public ConsulCacheController(IConsulCache consulCache) => _consulCache = consulCache;

        [HttpPost("string:{key}/{value}")]
        public async Task<WriteResult<bool>> Post(string key, string value)
        {
            return await _consulCache.KVPut(key, value);
        }

        [HttpPost("dto:{key}")]
        public async Task<WriteResult<bool>> Post(string key, ConsulCacheDto value)
        {
            return await _consulCache.KVPut(key, value);
        }

        [HttpGet("string:{key}")]
        public async Task<string> KVGet(string key)
        {
            return await _consulCache.KVGet<string>(key);
        }

        [HttpGet("dto:{key}")]
        public async Task<ConsulCacheDto> KVGetDto(string key)
        {
            return await _consulCache.KVGet<ConsulCacheDto>(key);
        }

        [HttpDelete("{key}")]
        public async Task<WriteResult<bool>> Delete(string key)
        {
            return await _consulCache.KVDelete(key);
        }
    }
}
