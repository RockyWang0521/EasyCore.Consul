using EasyCore.Consul.Discovery;
using FluentAssertions;
using Xunit;

namespace EasyCore.Consul.Tests;

public class LoadBalancerTests
{
    private static ServiceEndpoint Ep(string id) => new()
    {
        ServiceId = id,
        ServiceName = "svc",
        Address = "127.0.0.1",
        Port = 80
    };

    [Fact]
    public void RoundRobin_cycles_through_instances()
    {
        var balancer = new RoundRobinLoadBalancer();
        var endpoints = new[] { Ep("a"), Ep("b"), Ep("c") };

        balancer.Select(endpoints)!.ServiceId.Should().Be("a");
        balancer.Select(endpoints)!.ServiceId.Should().Be("b");
        balancer.Select(endpoints)!.ServiceId.Should().Be("c");
        balancer.Select(endpoints)!.ServiceId.Should().Be("a");
    }

    [Fact]
    public void RoundRobin_returns_null_for_empty()
    {
        var balancer = new RoundRobinLoadBalancer();
        balancer.Select(Array.Empty<ServiceEndpoint>()).Should().BeNull();
    }

    [Fact]
    public void Random_returns_one_of_instances()
    {
        var balancer = new RandomLoadBalancer();
        var endpoints = new[] { Ep("a"), Ep("b") };

        var selected = balancer.Select(endpoints);
        selected.Should().NotBeNull();
        endpoints.Select(e => e.ServiceId).Should().Contain(selected!.ServiceId);
    }
}
