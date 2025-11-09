namespace EasyCore.Consul.Discovery;

/// <summary>Resolved service endpoint.</summary>
public sealed class ServiceEndpoint
{
    public required string ServiceId { get; init; }
    public required string ServiceName { get; init; }
    public required string Address { get; init; }
    public required int Port { get; init; }
    public string[] Tags { get; init; } = Array.Empty<string>();

    public Uri BuildUri(RequestScheme scheme, string pathAndQuery)
    {
        var builder = new UriBuilder
        {
            Scheme = scheme == RequestScheme.Https ? Uri.UriSchemeHttps : Uri.UriSchemeHttp,
            Host = Address,
            Port = Port
        };

        if (string.IsNullOrEmpty(pathAndQuery))
        {
            return builder.Uri;
        }

        if (pathAndQuery.StartsWith('?'))
        {
            builder.Query = pathAndQuery.TrimStart('?');
            return builder.Uri;
        }

        var split = pathAndQuery.Split('?', 2);
        builder.Path = split[0].StartsWith('/') ? split[0] : "/" + split[0];
        if (split.Length > 1)
        {
            builder.Query = split[1];
        }

        return builder.Uri;
    }
}

/// <summary>HTTP / HTTPS scheme for inter-service calls.</summary>
public enum RequestScheme
{
    Http = 0,
    Https = 1
}

/// <summary>Discovers healthy service instances from Consul.</summary>
public interface IConsulServiceDiscovery
{
    Task<IReadOnlyList<ServiceEndpoint>> GetHealthyInstancesAsync(
        string serviceName,
        string? tag = null,
        CancellationToken cancellationToken = default);

    Task<ServiceEndpoint?> ResolveAsync(
        string serviceName,
        string? tag = null,
        CancellationToken cancellationToken = default);
}

internal interface ILoadBalancer
{
    ServiceEndpoint? Select(IReadOnlyList<ServiceEndpoint> endpoints);
}
