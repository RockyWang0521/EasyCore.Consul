using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace EasyCore.Consul
{
    public static class UseConsul
    {
        public static void UseEasyCoreConsul(this WebApplication app)
        {
            var options = app.Configuration.GetSection("Consul").Get<ConsulOptions>()!;

            var consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri(options.ConsulAddress);
            });

            var registration = new AgentServiceRegistration()
            {
                ID = options.ServiceName + Guid.NewGuid().ToString(),
                Name = options.ServiceName,
                Address = options.ServiceIP,
                Port = options.ServicePort,
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    Interval = TimeSpan.FromSeconds(10),
                    HTTP = options.ServiceHealthCheck,
                    Timeout = TimeSpan.FromSeconds(300)
                }
            };

            consulClient.Agent.ServiceRegister(registration).Wait();

            app.Lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });
        }

        public static void UseEasyCoreConsul(IConfiguration configuration,IHostApplicationLifetime lifetime)
        {
            var options = configuration.GetSection("Consul").Get<ConsulOptions>()!;

            var consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri(options.ConsulAddress);
            });

            var registration = new AgentServiceRegistration()
            {
                ID = options.ServiceName + Guid.NewGuid().ToString(),
                Name = options.ServiceName,
                Address = options.ServiceIP,
                Port = options.ServicePort,
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    Interval = TimeSpan.FromSeconds(10),
                    HTTP = options.ServiceHealthCheck,
                    Timeout = TimeSpan.FromSeconds(300)
                }
            };

            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });
        }
    }
}
