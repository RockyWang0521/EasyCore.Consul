using EasyCore.Consul.Discovery;
using FluentAssertions;
using Xunit;

namespace EasyCore.Consul.Tests;

public class ServiceEndpointTests
{
    [Theory]
    [InlineData("/api/values", "http://10.0.0.1:8080/api/values")]
    [InlineData("/api/values?id=1", "http://10.0.0.1:8080/api/values?id=1")]
    [InlineData("api/values", "http://10.0.0.1:8080/api/values")]
    public void BuildUri_http_paths(string path, string expected)
    {
        var endpoint = new ServiceEndpoint
        {
            ServiceId = "1",
            ServiceName = "demo",
            Address = "10.0.0.1",
            Port = 8080
        };

        endpoint.BuildUri(RequestScheme.Http, path).ToString().Should().Be(expected);
    }

    [Fact]
    public void BuildUri_https()
    {
        var endpoint = new ServiceEndpoint
        {
            ServiceId = "1",
            ServiceName = "demo",
            Address = "example.com",
            Port = 443
        };

        endpoint.BuildUri(RequestScheme.Https, "/health")
            .ToString()
            .Should()
            .Be("https://example.com/health");
    }
}
