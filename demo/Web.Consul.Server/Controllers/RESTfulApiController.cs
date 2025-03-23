using Microsoft.AspNetCore.Mvc;
using Web.Consul.Server.Pojo;

namespace Web.Consul.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RESTfulApiController : ControllerBase
    {
        private readonly ILogger<RESTfulApiController> _logger;
        public RESTfulApiController(ILogger<RESTfulApiController> logger)
            => _logger = logger;

        [HttpGet]
        public async Task<ConsulServerDto> Get(int id)
        {
            var result = new ConsulServerDto
            {
                BoolDto = true,
                IntDto = id,
                StringDto = "this is Web.Consul.Server RESTfulApiController.Get method"
            };

            return await Task.FromResult(result);
        }

        [HttpPost]
        public async Task<string> Post(ConsulServerDto dto)
        {
            return await Task.FromResult($"this is Web.Consul.Server RESTfulApiController.Post method --dto :  {dto.StringDto}--{dto.IntDto}--{dto.BoolDto}");
        }

        [HttpPut]
        public async Task<ConsulServerDto> Put(ConsulServerDto dto)
        {
            var result = new ConsulServerDto
            {
                BoolDto = true,
                IntDto = 100,
                StringDto = $"this is Web.Consul.Server RESTfulApiController.Get method --dto :  {dto.StringDto}--{dto.IntDto}--{dto.BoolDto}"
            };

            return await Task.FromResult(result);
        }

        [HttpDelete]
        public async Task<string> Delete(int id)
        {
            return await Task.FromResult("this is Web.Consul.Server RESTfulApiController.Delete method");
        }
    }
}
