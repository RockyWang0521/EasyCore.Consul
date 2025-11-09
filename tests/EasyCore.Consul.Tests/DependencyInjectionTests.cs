using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace EasyCore.Consul.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddEasyCoreConsul_registers_options_and_validates()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Consul:ConsulAddress"] = "http://127.0.0.1:8500",
                ["Consul:Register"] = "false"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddEasyCoreConsul(configuration);
        services.AddEasyCoreConsulCache();
        services.AddEasyCoreConsulLocking();
        services.AddEasyCoreConsulServer();

        using var sp = services.BuildServiceProvider();
        var options = sp.GetRequiredService<IOptions<ConsulOptions>>().Value;
        options.ConsulAddress.Should().Be("http://127.0.0.1:8500");
        options.Register.Should().BeFalse();

        sp.GetRequiredService<Cache.IConsulCache>().Should().NotBeNull();
        sp.GetRequiredService<Locking.IConsulLocking>().Should().NotBeNull();
        sp.GetRequiredService<Invocation.IConsulServer>().Should().NotBeNull();
        sp.GetRequiredService<Discovery.IConsulServiceDiscovery>().Should().NotBeNull();
    }
}
