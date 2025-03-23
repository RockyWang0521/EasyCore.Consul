using EasyCore.Consul;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Web.Consul.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptionsController : ControllerBase
    {
        private readonly ConsulOptions _options;

        public OptionsController(IOptions<ConsulOptions> options) => _options = options.Value;

        [HttpGet("GetOptions")]
        public Task<ConsulOptions> GetOptions()
        {
            return Task.FromResult(_options);
        }
    }
}
