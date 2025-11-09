using EasyCore.Consul.Discovery;
using EasyCore.Consul.Invocation;
using Microsoft.AspNetCore.Mvc;
using Web.Consul.Pojo;

namespace Web.Consul.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConsulServerController : ControllerBase
{
    private readonly IConsulServer _consulServer;
    private const string ServerName = "Web.Consul.Server";

    public ConsulServerController(IConsulServer consulServer) => _consulServer = consulServer;

    [HttpGet]
    public Task<ServiceCallResult<ConsulServerDto>> Get(CancellationToken cancellationToken)
        => _consulServer.ServiceGetAsync<ConsulServerDto>(
            RequestScheme.Http, ServerName, "/RESTfulApi?id=1", cancellationToken: cancellationToken);

    [HttpPost]
    public Task<ServiceCallResult<string>> Post(CancellationToken cancellationToken)
    {
        var dto = new ConsulServerDto
        {
            BoolDto = true,
            IntDto = 200,
            StringDto = "this is Web.Consul ConsulServerController.Post method"
        };

        return _consulServer.ServicePostAsync<string, ConsulServerDto>(
            RequestScheme.Http, ServerName, "/RESTfulApi", dto, cancellationToken: cancellationToken);
    }

    [HttpPut]
    public Task<ServiceCallResult<ConsulServerDto>> Put(CancellationToken cancellationToken)
    {
        var dto = new ConsulServerDto
        {
            BoolDto = true,
            IntDto = 300,
            StringDto = "this is Web.Consul ConsulServerController.Put method"
        };

        return _consulServer.ServicePutAsync<ConsulServerDto, ConsulServerDto>(
            RequestScheme.Http, ServerName, "/RESTfulApi", dto, cancellationToken: cancellationToken);
    }

    [HttpDelete]
    public Task<ServiceCallResult<string>> Delete(CancellationToken cancellationToken)
        => _consulServer.ServiceDeleteAsync<string>(
            RequestScheme.Http, ServerName, "/RESTfulApi?id=2", cancellationToken: cancellationToken);
}
