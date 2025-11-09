using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyCore.Consul.Registration;

/// <summary>
/// Registers the current process with Consul on start and deregisters on graceful shutdown.
/// </summary>
internal sealed class ConsulServiceRegistrationHostedService : IHostedService
{
    private readonly IConsulClient _consulClient;
    private readonly IOptions<ConsulOptions> _options;
    private readonly ILogger<ConsulServiceRegistrationHostedService> _logger;
    private string? _serviceId;

    public ConsulServiceRegistrationHostedService(
        IConsulClient consulClient,
        IOptions<ConsulOptions> options,
        ILogger<ConsulServiceRegistrationHostedService> logger)
    {
        _consulClient = consulClient;
        _options = options;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var options = _options.Value;
        if (!options.Register)
        {
            _logger.LogInformation("Consul service registration is disabled.");
            return;
        }

        _serviceId = options.ResolveServiceId();

        var registration = new AgentServiceRegistration
        {
            ID = _serviceId,
            Name = options.ServiceName,
            Address = options.ServiceAddress,
            Port = options.ServicePort,
            Tags = options.Tags,
            Meta = options.Meta
        };

        if (!string.IsNullOrWhiteSpace(options.HealthCheck.Http))
        {
            registration.Check = new AgentServiceCheck
            {
                HTTP = options.HealthCheck.Http,
                Interval = options.HealthCheck.Interval,
                Timeout = options.HealthCheck.Timeout,
                DeregisterCriticalServiceAfter = options.HealthCheck.DeregisterCriticalServiceAfter
            };
        }

        try
        {
            await _consulClient.Agent.ServiceRegister(registration, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation(
                "Registered Consul service {ServiceName} ({ServiceId}) at {Address}:{Port}",
                options.ServiceName,
                _serviceId,
                options.ServiceAddress,
                options.ServicePort);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to register Consul service {ServiceName} ({ServiceId})",
                options.ServiceName,
                _serviceId);
            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_serviceId))
        {
            return;
        }

        try
        {
            await _consulClient.Agent.ServiceDeregister(_serviceId, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Deregistered Consul service {ServiceId}", _serviceId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to deregister Consul service {ServiceId}", _serviceId);
        }
    }
}
