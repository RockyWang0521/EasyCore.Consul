using EasyCore.Consul;

namespace Web.Consul.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add EasyCoreConsul
            builder.EasyCoreConsul(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthorization();

            // Use EasyCoreConsul
            app.UseEasyCoreConsul();

            app.MapControllers();
            app.Run();
        }
    }
}
