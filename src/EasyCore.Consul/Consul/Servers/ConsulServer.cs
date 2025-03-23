using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace EasyCore.Consul.Servers
{
    public class ConsulServer : ConsulBase, IConsulServer, IGetServices, IJsonJudgment, IHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ConsulOptions _options;

        const string clientName = "ConsulServer";

        public ConsulServer(IHttpClientFactory httpClientFactory,
            IOptions<ConsulOptions> options)
        {
            _httpClientFactory = httpClientFactory;

            _options = options.Value;
        }

        public async Task<string?> GetService(RequestType type, string serviceNamein, string apiAddr)
        {
            using var consulClient = base.GetConsulClient(_options.ConsulAddress);

            var services = (await consulClient.Catalog.Service(serviceNamein)).Response;

            if (services.Length == 0) return null;

            var randomNumber = services.Length > 1 ? Random.Shared.Next(0, services.Length) : 0;

            var service = services[randomNumber];

            var serviceUrl = new StringBuilder()
                .Append(type == RequestType.Http ? "http://" : "https://")
                .Append(service.ServiceAddress)
                .Append(":")
                .Append(service.ServicePort)
                .Append(apiAddr).ToString();

            return serviceUrl;
        }

        public bool IsJson(string input)
        {
            try
            {
                JToken.Parse(input);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        public HttpClient GetHttpClient(string? token)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);

            if (!string.IsNullOrEmpty(token)) httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return httpClient;
        }

        public async Task<ConsulReturn<TReturn>> SendRequestAsync<TReturn>(HttpMethod method, RequestType type, string serviceName, string apiAddr, object? content = null, string? token = null)
        {
            var serviceUrl = await GetService(type, serviceName, apiAddr);

            if (serviceUrl is null)
            {
                return new ConsulReturn<TReturn> { Succeed = false, Message = "Service not found" };
            }

            var httpClient = GetHttpClient(token);

            var request = new HttpRequestMessage(method, serviceUrl);

            if (content is not null)
            {
                var json = JsonConvert.SerializeObject(content);

                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await httpClient.SendAsync(request);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode || string.IsNullOrEmpty(responseBody))
            {
                return new ConsulReturn<TReturn> { Succeed = false, Message = "Request failed" };
            }

            var result = IsJson(responseBody) ? JsonConvert.DeserializeObject<TReturn>(responseBody)! : (TReturn)(object)responseBody!;

            return new ConsulReturn<TReturn> { Succeed = true, Message = "Request successful", Values = result };
        }

        public Task<ConsulReturn<TReturn>> ServiceGet<TReturn>(RequestType type, string serviceName, string apiAddr, string? token = null)
            => SendRequestAsync<TReturn>(HttpMethod.Get, type, serviceName, apiAddr, null, token);

        public Task<ConsulReturn<TReturn>> ServiceDelete<TReturn>(RequestType type, string serviceName, string apiAddr, string? token = null)
            => SendRequestAsync<TReturn>(HttpMethod.Delete, type, serviceName, apiAddr, null, token);

        public Task<ConsulReturn<TReturn>> ServicePost<TReturn, TParams>(RequestType type, string serviceName, string apiAddr, TParams? genericParam, string? token = null)
            => SendRequestAsync<TReturn>(HttpMethod.Post, type, serviceName, apiAddr, genericParam, token);

        public Task<ConsulReturn<TReturn>> ServicePut<TReturn, TParams>(RequestType type, string serviceName, string apiAddr, TParams? genericParam, string? token = null)
            => SendRequestAsync<TReturn>(HttpMethod.Put, type, serviceName, apiAddr, genericParam, token);

        public async Task<ConsulReturn<TReturn>> ServicePost<TReturn, TParams>(RequestType type, string serviceNamein, string apiAddr, string? token = null)
            => await ServicePost<TReturn, TParams>(type, serviceNamein, apiAddr, default, token);

        public async Task<ConsulReturn<TReturn>> ServicePut<TReturn, TParams>(RequestType type, string serviceNamein, string apiAddr, string? token = null)
            => await ServicePut<TReturn, TParams>(type, serviceNamein, apiAddr, default, token);
    }
}
