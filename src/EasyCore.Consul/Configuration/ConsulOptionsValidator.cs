using Microsoft.Extensions.Options;

namespace EasyCore.Consul;

internal sealed class ConsulOptionsValidator : IValidateOptions<ConsulOptions>
{
    public ValidateOptionsResult Validate(string? name, ConsulOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ConsulAddress) ||
            !Uri.TryCreate(options.ConsulAddress, UriKind.Absolute, out var consulUri) ||
            (consulUri.Scheme != Uri.UriSchemeHttp && consulUri.Scheme != Uri.UriSchemeHttps))
        {
            failures.Add("Consul:ConsulAddress must be an absolute http/https URI.");
        }

        if (options.Register)
        {
            if (string.IsNullOrWhiteSpace(options.ServiceName))
            {
                failures.Add("Consul:ServiceName is required when Register is true.");
            }

            if (string.IsNullOrWhiteSpace(options.ServiceAddress))
            {
                failures.Add("Consul:ServiceAddress (or ServiceIP) is required when Register is true.");
            }

            if (options.ServicePort is < 1 or > 65535)
            {
                failures.Add("Consul:ServicePort must be between 1 and 65535 when Register is true.");
            }

            if (options.HealthCheck.Interval <= TimeSpan.Zero)
            {
                failures.Add("Consul:HealthCheck:Interval must be positive.");
            }

            if (options.HealthCheck.Timeout <= TimeSpan.Zero)
            {
                failures.Add("Consul:HealthCheck:Timeout must be positive.");
            }

            if (!string.IsNullOrWhiteSpace(options.HealthCheck.Http) &&
                !Uri.TryCreate(options.HealthCheck.Http, UriKind.Absolute, out _))
            {
                failures.Add("Consul:HealthCheck:Http (or ServiceHealthCheck) must be an absolute URI when set.");
            }
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
