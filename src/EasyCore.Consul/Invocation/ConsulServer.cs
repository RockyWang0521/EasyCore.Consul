using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EasyCore.Consul.Discovery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EasyCore.Consul.Invocation;

internal sealed class ConsulServer : IConsulServer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConsulServiceDiscovery _discovery;
    private readonly ILogger<ConsulServer> _logger;

    public const string HttpClientName = "EasyCore.Consul.ServiceInvoker";

    public ConsulServer(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IConsulServiceDiscovery discovery,
        ILogger<ConsulServer> logger)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _discovery = discovery;
        _logger = logger;
    }

    public Task<ServiceCallResult<TReturn>> ServiceGetAsync<TReturn>(
        RequestScheme scheme, string serviceName, string apiPath, string? token = null, CancellationToken cancellationToken = default)
        => SendAsync<TReturn>(HttpMethod.Get, scheme, serviceName, apiPath, null, token, cancellationToken);

    public Task<ServiceCallResult> ServiceGetAsync(
        RequestScheme scheme, string serviceName, string apiPath, string? token = null, CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Get, scheme, serviceName, apiPath, null, token, cancellationToken);

    public Task<ServiceCallResult<TReturn>> ServicePostAsync<TReturn, TParams>(
        RequestScheme scheme, string serviceName, string apiPath, TParams? body, string? token = null, CancellationToken cancellationToken = default)
        => SendAsync<TReturn>(HttpMethod.Post, scheme, serviceName, apiPath, body, token, cancellationToken);

    public Task<ServiceCallResult> ServicePostAsync<TParams>(
        RequestScheme scheme, string serviceName, string apiPath, TParams? body, string? token = null, CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Post, scheme, serviceName, apiPath, body, token, cancellationToken);

    public Task<ServiceCallResult<TReturn>> ServicePostAsync<TReturn>(
        RequestScheme scheme, string serviceName, string apiPath, string? token = null, CancellationToken cancellationToken = default)
        => SendAsync<TReturn>(HttpMethod.Post, scheme, serviceName, apiPath, null, token, cancellationToken);

    public Task<ServiceCallResult> ServicePostAsync(
        RequestScheme scheme, string serviceName, string apiPath, string? token = null, CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Post, scheme, serviceName, apiPath, null, token, cancellationToken);

    public Task<ServiceCallResult<TReturn>> ServicePutAsync<TReturn, TParams>(
        RequestScheme scheme, string serviceName, string apiPath, TParams? body, string? token = null, CancellationToken cancellationToken = default)
        => SendAsync<TReturn>(HttpMethod.Put, scheme, serviceName, apiPath, body, token, cancellationToken);

    public Task<ServiceCallResult> ServicePutAsync<TParams>(
        RequestScheme scheme, string serviceName, string apiPath, TParams? body, string? token = null, CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Put, scheme, serviceName, apiPath, body, token, cancellationToken);

    public Task<ServiceCallResult<TReturn>> ServicePutAsync<TReturn>(
        RequestScheme scheme, string serviceName, string apiPath, string? token = null, CancellationToken cancellationToken = default)
        => SendAsync<TReturn>(HttpMethod.Put, scheme, serviceName, apiPath, null, token, cancellationToken);

    public Task<ServiceCallResult> ServicePutAsync(
        RequestScheme scheme, string serviceName, string apiPath, string? token = null, CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Put, scheme, serviceName, apiPath, null, token, cancellationToken);

    public Task<ServiceCallResult<TReturn>> ServiceDeleteAsync<TReturn>(
        RequestScheme scheme, string serviceName, string apiPath, string? token = null, CancellationToken cancellationToken = default)
        => SendAsync<TReturn>(HttpMethod.Delete, scheme, serviceName, apiPath, null, token, cancellationToken);

    public Task<ServiceCallResult> ServiceDeleteAsync(
        RequestScheme scheme, string serviceName, string apiPath, string? token = null, CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Delete, scheme, serviceName, apiPath, null, token, cancellationToken);

    private async Task<ServiceCallResult<TReturn>> SendAsync<TReturn>(
        HttpMethod method,
        RequestScheme scheme,
        string serviceName,
        string apiPath,
        object? content,
        string? token,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiPath);

        var endpoint = await _discovery.ResolveAsync(serviceName, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        if (endpoint is null)
        {
            return ServiceCallResult<TReturn>.Fail($"Service '{serviceName}' not found or unhealthy.");
        }

        var uri = endpoint.BuildUri(scheme, apiPath);
        using var request = CreateRequest(method, uri, content, ResolveToken(token));

        try
        {
            var client = _httpClientFactory.CreateClient(HttpClientName);
            using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var status = (int)response.StatusCode;

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Service call {Method} {Uri} failed with {StatusCode}: {Body}",
                    method, uri, status, Truncate(body));
                return ServiceCallResult<TReturn>.Fail($"Request failed with status {status}.", status);
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                return ServiceCallResult<TReturn>.Ok(default!, "Request successful (empty body).", status);
            }

            if (typeof(TReturn) == typeof(string))
            {
                return ServiceCallResult<TReturn>.Ok((TReturn)(object)body, statusCode: status);
            }

            if (!LooksLikeJson(body))
            {
                return ServiceCallResult<TReturn>.Fail("Response body is not valid JSON.", status);
            }

            var value = JsonSerializer.Deserialize<TReturn>(body, JsonOptions);
            return ServiceCallResult<TReturn>.Ok(value!, statusCode: status);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Service call {Method} {Uri} threw", method, uri);
            return ServiceCallResult<TReturn>.Fail(ex.Message);
        }
    }

    private async Task<ServiceCallResult> SendAsync(
        HttpMethod method,
        RequestScheme scheme,
        string serviceName,
        string apiPath,
        object? content,
        string? token,
        CancellationToken cancellationToken)
    {
        var typed = await SendAsync<object>(method, scheme, serviceName, apiPath, content, token, cancellationToken)
            .ConfigureAwait(false);

        return typed.Succeed
            ? ServiceCallResult.Ok(typed.Message, typed.StatusCode)
            : ServiceCallResult.Fail(typed.Message, typed.StatusCode);
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, Uri uri, object? content, string? token)
    {
        var request = new HttpRequestMessage(method, uri);

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (content is not null)
        {
            var json = JsonSerializer.Serialize(content, JsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return request;
    }

    private string? ResolveToken(string? token)
    {
        if (!string.IsNullOrWhiteSpace(token))
        {
            return token;
        }

        var header = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(header))
        {
            return null;
        }

        const string bearerPrefix = "Bearer ";
        return header.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase)
            ? header[bearerPrefix.Length..].Trim()
            : header.Trim();
    }

    private static bool LooksLikeJson(string input)
    {
        var trimmed = input.AsSpan().Trim();
        return trimmed.Length > 0 &&
               ((trimmed[0] == '{' && trimmed[^1] == '}') ||
                (trimmed[0] == '[' && trimmed[^1] == ']') ||
                (trimmed[0] == '"' && trimmed[^1] == '"') ||
                trimmed.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("false", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("null", StringComparison.OrdinalIgnoreCase) ||
                char.IsDigit(trimmed[0]) ||
                trimmed[0] == '-');
    }

    private static string Truncate(string? value, int max = 256)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= max)
        {
            return value ?? string.Empty;
        }

        return value[..max] + "...";
    }
}
