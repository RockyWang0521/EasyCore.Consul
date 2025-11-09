using EasyCore.Consul.Discovery;

namespace EasyCore.Consul.Invocation;

/// <summary>Invokes HTTP APIs on other Consul-registered services.</summary>
public interface IConsulServer
{
    Task<ServiceCallResult<TReturn>> ServiceGetAsync<TReturn>(
        RequestScheme scheme,
        string serviceName,
        string apiPath,
        string? token = null,
        CancellationToken cancellationToken = default);

    Task<ServiceCallResult> ServiceGetAsync(
        RequestScheme scheme,
        string serviceName,
        string apiPath,
        string? token = null,
        CancellationToken cancellationToken = default);

    Task<ServiceCallResult<TReturn>> ServicePostAsync<TReturn, TParams>(
        RequestScheme scheme,
        string serviceName,
        string apiPath,
        TParams? body,
        string? token = null,
        CancellationToken cancellationToken = default);

    Task<ServiceCallResult> ServicePostAsync<TParams>(
        RequestScheme scheme,
        string serviceName,
        string apiPath,
        TParams? body,
        string? token = null,
        CancellationToken cancellationToken = default);

    Task<ServiceCallResult<TReturn>> ServicePostAsync<TReturn>(
        RequestScheme scheme,
        string serviceName,
        string apiPath,
        string? token = null,
        CancellationToken cancellationToken = default);

    Task<ServiceCallResult> ServicePostAsync(
        RequestScheme scheme,
        string serviceName,
        string apiPath,
        string? token = null,
        CancellationToken cancellationToken = default);

    Task<ServiceCallResult<TReturn>> ServicePutAsync<TReturn, TParams>(
        RequestScheme scheme,
        string serviceName,
        string apiPath,
        TParams? body,
        string? token = null,
        CancellationToken cancellationToken = default);

    Task<ServiceCallResult> ServicePutAsync<TParams>(
        RequestScheme scheme,
        string serviceName,
        string apiPath,
        TParams? body,
        string? token = null,
        CancellationToken cancellationToken = default);

    Task<ServiceCallResult<TReturn>> ServicePutAsync<TReturn>(
        RequestScheme scheme,
        string serviceName,
        string apiPath,
        string? token = null,
        CancellationToken cancellationToken = default);

    Task<ServiceCallResult> ServicePutAsync(
        RequestScheme scheme,
        string serviceName,
        string apiPath,
        string? token = null,
        CancellationToken cancellationToken = default);

    Task<ServiceCallResult<TReturn>> ServiceDeleteAsync<TReturn>(
        RequestScheme scheme,
        string serviceName,
        string apiPath,
        string? token = null,
        CancellationToken cancellationToken = default);

    Task<ServiceCallResult> ServiceDeleteAsync(
        RequestScheme scheme,
        string serviceName,
        string apiPath,
        string? token = null,
        CancellationToken cancellationToken = default);
}
