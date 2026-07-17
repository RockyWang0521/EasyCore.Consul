using EasyCore.Consul.Cache;
using EasyCore.Consul.Client;
using EasyCore.Consul.Discovery;
using EasyCore.Consul.Invocation;
using EasyCore.Consul.Locking;
using EasyCore.Consul.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace EasyCore.Consul;

/// <summary>DI registration extensions for EasyCore.Consul.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Consul options, shared Consul client, and optional service registration hosted service.
    /// </summary>
    public static IServiceCollection AddEasyCoreConsul(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ConsulOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var builder = services.AddOptions<ConsulOptions>()
            .Bind(configuration.GetSection(ConsulOptions.SectionName))
            .ValidateOnStart();

        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IValidateOptions<ConsulOptions>, ConsulOptionsValidator>());

        if (configure is not null)
        {
            builder.Configure(configure);
        }

        services.AddConsulClient();
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IHostedService, ConsulServiceRegistrationHostedService>());

        return services;
    }

    /// <summary>
    /// Registers Consul using <see cref="IHostApplicationBuilder.Configuration"/>.
    /// </summary>
    public static IServiceCollection AddEasyCoreConsul(
        this IHostApplicationBuilder builder,
        Action<ConsulOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.Services.AddEasyCoreConsul(builder.Configuration, configure);
    }

    /// <summary>Registers Consul KV cache services.</summary>
    public static IServiceCollection AddEasyCoreConsulCache(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddConsulClient();
        services.TryAddSingleton<IConsulCache, Cache.ConsulCache>();
        return services;
    }

    /// <summary>Registers Consul distributed locking services.</summary>
    public static IServiceCollection AddEasyCoreConsulLocking(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddConsulClient();
        services.TryAddSingleton<IConsulLocking, Locking.ConsulLocking>();
        return services;
    }

    /// <summary>
    /// Registers service discovery, load balancing, and inter-service HTTP invocation.
    /// </summary>
    public static IServiceCollection AddEasyCoreConsulServer(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddConsulClient();
        services.AddHttpContextAccessor();
        services.AddHttpClient(ConsulServer.HttpClientName, client =>
        {
            client.Timeout = TimeSpan.FromSeconds(100);
        });

        services.TryAddSingleton<IConsulServiceDiscovery, ConsulServiceDiscovery>();
        services.TryAddSingleton<ILoadBalancer>(sp =>
        {
            var policy = sp.GetRequiredService<IOptions<ConsulOptions>>().Value.LoadBalance;
            return policy == LoadBalancePolicy.Random
                ? new RandomLoadBalancer()
                : new RoundRobinLoadBalancer();
        });
        services.TryAddSingleton<IConsulServer, ConsulServer>();
        return services;
    }
}
