using EasyCore.Consul.Servers;

namespace EasyCore.Consul.Servers
{
    internal interface IHttpClient
    {
        HttpClient GetHttpClient(string? token);

        Task<ConsulReturn<TReturn>> SendRequestAsync<TReturn>(HttpMethod method, RequestType type, string serviceName, string apiAddr, object? content = null, string? token = null);

        Task<ConsulReturn> SendRequestAsync(HttpMethod method, RequestType type, string serviceName, string apiAddr, object? content = null, string? token = null);
    }
}
