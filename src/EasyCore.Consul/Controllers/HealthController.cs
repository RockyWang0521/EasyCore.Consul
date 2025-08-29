using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyCore.Consul.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController, AllowAnonymous]
    public class HealthController : ControllerBase
    {
        [HttpGet("/healthCheck")]
        public IActionResult Check() => Ok("ok");
    }
}
