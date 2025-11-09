using Consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyCore.Consul.Discovery;

internal sealed class ConsulServiceDiscovery : IConsulServiceDiscovery
{
    private readonly IConsulClient _consulClient;
    private readonly ILoadBalancer _loadBalancer;
    private readonly IOptions<ConsulOptions> _options;
    private readonly ILogger<ConsulServiceDiscovery> _logger;

    public ConsulServiceDiscovery(
        IConsulClient consulClient,
        ILoadBalancer loadBalancer,
        IOptions<ConsulOptions> options,
        ILogger<ConsulServiceDiscovery> logger)
    {
        _consulClient = consulClient;
        _loadBalancer = loadBalancer;
        _options = options;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ServiceEndpoint>> GetHealthyInstancesAsync(
        string serviceName,
        string? tag = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

        var passingOnly = _options.Value.PassingOnly;
        var result = await _consulClient.Health
            .Service(serviceName, tag, passingOnly, cancellationToken)
            .ConfigureAwait(false);

        var endpoints = result.Response
            .Select(entry => new ServiceEndpoint
            {
                ServiceId = entry.Service.ID,
                ServiceName = entry.Service.Service,
                Address = string.IsNullOrWhiteSpace(entry.Service.Address)
                    ? entry.Node.Address
                    : entry.Service.Address,
                Port = entry.Service.Port,
                Tags = entry.Service.Tags ?? Array.Empty<string>()
            })
            .Where(e => !string.IsNullOrWhiteSpace(e.Address) && e.Port > 0)
            .ToList();

        if (endpoints.Count == 0)
        {
            _logger.LogWarning(
                "No {Passing} instances found for service {ServiceName}",
                passingOnly ? "healthy" : "registered",
                serviceName);
        }

        return endpoints;
    }

    public async Task<ServiceEndpoint?> ResolveAsync(
        string serviceName,
        string? tag = null,
        CancellationToken cancellationToken = default)
    {
        var instances = await GetHealthyInstancesAsync(serviceName, tag, cancellationToken)
            .ConfigureAwait(false);
        return _loadBalancer.Select(instances);
    }
}
