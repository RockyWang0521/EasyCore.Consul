using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;

namespace EasyCore.Consul;

/// <summary>ASP.NET Core middleware / endpoint helpers.</summary>
public static class ApplicationBuilderExtensions
{
    public const string DefaultHealthCheckPath = "/healthCheck";

    /// <summary>
    /// Maps a lightweight anonymous health endpoint used by Consul HTTP checks.
    /// Registration itself is performed by the hosted service registered in DI.
    /// </summary>
    public static IEndpointConventionBuilder MapEasyCoreConsulHealthCheck(
        this IEndpointRouteBuilder endpoints,
        string pattern = DefaultHealthCheckPath)
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        return endpoints.MapGet(pattern, () => Results.Ok("ok"));
    }

    /// <summary>
    /// Maps the default health-check endpoint. Service registration runs via <see cref="IHostedService"/>.
    /// </summary>
    public static WebApplication UseEasyCoreConsul(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);
        app.MapEasyCoreConsulHealthCheck();
        return app;
    }
}
