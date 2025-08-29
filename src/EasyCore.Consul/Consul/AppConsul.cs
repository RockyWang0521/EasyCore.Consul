using EasyCore.Consul.Cache;
using EasyCore.Consul.Locking;
using EasyCore.Consul.Servers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace EasyCore.Consul
{
    public static class AppConsul
    {
        public static IServiceCollection EasyCoreConsul(this IHostApplicationBuilder builder, string[] args)
        {
            builder.Services.Configure<ConsulOptions>(builder.Configuration.GetSection("Consul"));

            builder.Configuration.AddCommandLine(args);

            builder.Configuration.AddEnvironmentVariables();

            return builder.Services;
        }

        public static IServiceCollection EasyCoreConsul(this IHostApplicationBuilder builder)
        {
            builder.Services.Configure<ConsulOptions>(builder.Configuration.GetSection("Consul"));

            return builder.Services;
        }

        public static IServiceCollection EasyCoreConsul(this IServiceCollection service, IConfiguration configuration)
        {
            service.Configure<ConsulOptions>(configuration);

            return service;
        }

        public static IServiceCollection EasyCoreConsulCache(this IServiceCollection service)
        {
            service.TryAddSingleton<IConsulCache, ConsulCache>();

            return service;
        }

        public static IServiceCollection EasyCoreConsulLocking(this IServiceCollection service)
        {
            service.TryAddSingleton<IConsulLocking, ConsulLocking>();

            return service;
        }

        public static IServiceCollection EasyCoreConsulServer(this IServiceCollection service)
        {
            service.AddHttpClient("ConsulServer");

            service.TryAddSingleton<IConsulServer, ConsulServer>();

            return service;
        }
    }
}
