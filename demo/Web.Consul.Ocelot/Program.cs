using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;

namespace Web.Consul.Ocelot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add Ocelot
            builder.Services.AddOcelot(new ConfigurationBuilder().AddJsonFile("ocelot.json", optional: false, reloadOnChange: true).Build())
                                     .AddConsul()
                                     .AddPolly()
                                     .AddCacheManager(x => x.WithDictionaryHandle()
                                     );

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Use Ocelot middleware
            app.UseOcelot().Wait();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
