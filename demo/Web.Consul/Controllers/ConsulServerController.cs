using EasyCore.Consul.Servers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Consul.Pojo;

namespace Web.Consul.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsulServerController : ControllerBase
    {
        private readonly IConsulServer _consulServer;

        const string serverName = "Web.Consul.Server";

        public ConsulServerController(IConsulServer consulServer)
            => _consulServer = consulServer;


        [HttpGet]
        public async Task<ConsulReturn<ConsulServerDto>> GetConsulServer()
        {
            return await _consulServer.ServiceGetAsync<ConsulServerDto>(RequestType.Http, serverName, "/RESTfulApi?id=1");
        }

        [HttpPost]
        public async Task<ConsulReturn<string>> Post()
        {
            var dto = new ConsulServerDto()
            {
                BoolDto = true,
                IntDto = 200,
                StringDto = $"this is Web.Consul ConsulServerController.Post method "
            };

            return await _consulServer.ServicePostAsync<string, ConsulServerDto>(RequestType.Http, serverName, "/RESTfulApi", dto);
        }

        [HttpPut]
        public async Task<ConsulReturn<ConsulServerDto>> Put()
        {
            var dto = new ConsulServerDto
            {
                BoolDto = true,
                IntDto = 300,
                StringDto = $"this is Web.Consul ConsulServerController.Put method "
            };

            return await _consulServer.ServicePutAsync<ConsulServerDto, ConsulServerDto>(RequestType.Http, serverName, "/RESTfulApi", dto);
        }

        [HttpDelete]
        public async Task<ConsulReturn<string>> Delete()
        {
            return await _consulServer.ServiceDeleteAsync<string>(RequestType.Http, serverName, "/RESTfulApi?id=2");
        }
    }
}
