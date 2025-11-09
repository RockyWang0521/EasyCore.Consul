using System.ComponentModel.DataAnnotations;

namespace EasyCore.Consul;

/// <summary>
/// Consul integration options bound from configuration section <see cref="SectionName"/>.
/// </summary>
public sealed class ConsulOptions
{
    public const string SectionName = "Consul";

    /// <summary>Consul HTTP API address, e.g. http://127.0.0.1:8500.</summary>
    [Required]
    public string ConsulAddress { get; set; } = "http://127.0.0.1:8500";

    /// <summary>Optional ACL token.</summary>
    public string? Token { get; set; }

    /// <summary>Optional datacenter override.</summary>
    public string? Datacenter { get; set; }

    /// <summary>When true, registers this process as a Consul service via hosted service.</summary>
    public bool Register { get; set; } = true;

    /// <summary>Logical service name used for discovery.</summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Stable service instance id. When empty, generated as {ServiceName}-{MachineName}-{Port}.
    /// </summary>
    public string? ServiceId { get; set; }

    /// <summary>Advertised service host / IP.</summary>
    public string ServiceAddress { get; set; } = string.Empty;

    /// <summary>Advertised service port.</summary>
    [Range(1, 65535)]
    public int ServicePort { get; set; }

    /// <summary>Optional Consul service tags.</summary>
    public string[]? Tags { get; set; }

    /// <summary>Optional Consul service metadata.</summary>
    public Dictionary<string, string>? Meta { get; set; }

    /// <summary>Health check settings for registration.</summary>
    public ConsulHealthCheckOptions HealthCheck { get; set; } = new();

    /// <summary>Service discovery / load-balancing policy.</summary>
    public LoadBalancePolicy LoadBalance { get; set; } = LoadBalancePolicy.RoundRobin;

    /// <summary>Only return instances that pass health checks when discovering services.</summary>
    public bool PassingOnly { get; set; } = true;

    /// <summary>
    /// Backward-compatible alias of <see cref="ServiceAddress"/>.
    /// Prefer <see cref="ServiceAddress"/>.
    /// </summary>
    public string ServiceIP
    {
        get => ServiceAddress;
        set => ServiceAddress = value;
    }

    /// <summary>
    /// Backward-compatible alias of <see cref="ConsulHealthCheckOptions.Http"/>.
    /// Prefer <see cref="HealthCheck"/>.
    /// </summary>
    public string? ServiceHealthCheck
    {
        get => HealthCheck.Http;
        set => HealthCheck.Http = value;
    }

    internal string ResolveServiceId()
    {
        if (!string.IsNullOrWhiteSpace(ServiceId))
        {
            return ServiceId!;
        }

        var host = Environment.MachineName;
        return $"{ServiceName}-{host}-{ServicePort}";
    }
}

/// <summary>HTTP health-check options used during service registration.</summary>
public sealed class ConsulHealthCheckOptions
{
    /// <summary>HTTP health-check URL. When null/empty, no HTTP check is registered.</summary>
    public string? Http { get; set; }

    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(10);

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

    public TimeSpan DeregisterCriticalServiceAfter { get; set; } = TimeSpan.FromMinutes(1);
}

/// <summary>Load-balancing strategy when multiple healthy instances exist.</summary>
public enum LoadBalancePolicy
{
    RoundRobin = 0,
    Random = 1
}
