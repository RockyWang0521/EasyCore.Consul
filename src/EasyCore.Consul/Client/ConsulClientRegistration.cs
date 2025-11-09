using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EasyCore.Consul.Client;

internal static class ConsulClientRegistration
{
    public static IServiceCollection AddConsulClient(this IServiceCollection services)
    {
        services.TryAddSingleton<IConsulClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<ConsulOptions>>().Value;

            return new ConsulClient(config =>
            {
                config.Address = new Uri(options.ConsulAddress);

                if (!string.IsNullOrWhiteSpace(options.Token))
                {
                    config.Token = options.Token;
                }

                if (!string.IsNullOrWhiteSpace(options.Datacenter))
                {
                    config.Datacenter = options.Datacenter;
                }
            });
        });

        return services;
    }
}
