using EasyCore.Consul;

namespace Web.Consul.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add EasyCoreConsul
            //builder.EasyCoreConsul();

            builder.Services.EasyCoreConsul(builder.Configuration.GetSection("Consul"));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthorization();

            // Use EasyCoreConsul
            //app.UseEasyCoreConsul();

            UseConsul.UseEasyCoreConsul(builder.Configuration, app.Lifetime);

            app.MapControllers();
            app.Run();
        }
    }
}
