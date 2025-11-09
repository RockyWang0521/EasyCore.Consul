using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace EasyCore.Consul.Tests;

public class ConsulOptionsValidatorTests
{
    private readonly ConsulOptionsValidator _validator = new();

    [Fact]
    public void Valid_options_pass()
    {
        var options = new ConsulOptions
        {
            ConsulAddress = "http://127.0.0.1:8500",
            Register = true,
            ServiceName = "demo",
            ServiceAddress = "127.0.0.1",
            ServicePort = 5000,
            HealthCheck =
            {
                Http = "http://127.0.0.1:5000/healthCheck",
                Interval = TimeSpan.FromSeconds(10),
                Timeout = TimeSpan.FromSeconds(5)
            }
        };

        _validator.Validate(null, options).Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Invalid_consul_address_fails()
    {
        var options = new ConsulOptions
        {
            ConsulAddress = "not-a-uri",
            Register = false
        };

        var result = _validator.Validate(null, options);
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(f => f.Contains("ConsulAddress"));
    }

    [Fact]
    public void Register_without_service_name_fails()
    {
        var options = new ConsulOptions
        {
            ConsulAddress = "http://127.0.0.1:8500",
            Register = true,
            ServiceName = "",
            ServiceAddress = "127.0.0.1",
            ServicePort = 5000
        };

        var result = _validator.Validate(null, options);
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(f => f.Contains("ServiceName"));
    }

    [Fact]
    public void ResolveServiceId_uses_explicit_id()
    {
        var options = new ConsulOptions
        {
            ServiceName = "demo",
            ServicePort = 1,
            ServiceId = "fixed-id"
        };

        options.ResolveServiceId().Should().Be("fixed-id");
    }

    [Fact]
    public void ServiceIP_alias_maps_to_ServiceAddress()
    {
        var options = new ConsulOptions { ServiceIP = "10.0.0.2" };
        options.ServiceAddress.Should().Be("10.0.0.2");
    }
}
