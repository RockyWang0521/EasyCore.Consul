using System.Text;
using System.Text.Json;
using Consul;
using Microsoft.Extensions.Logging;

namespace EasyCore.Consul.Cache;

internal sealed class ConsulCache : IConsulCache
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IConsulClient _consulClient;
    private readonly ILogger<ConsulCache> _logger;

    public ConsulCache(IConsulClient consulClient, ILogger<ConsulCache> logger)
    {
        _consulClient = consulClient;
        _logger = logger;
    }

    public async Task<bool> PutAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);

        var pair = new KVPair(key) { Value = Encoding.UTF8.GetBytes(value) };
        var result = await _consulClient.KV.Put(pair, cancellationToken).ConfigureAwait(false);
        return result.Response;
    }

    public Task<bool> PutAsync<TValue>(string key, TValue value, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is string s)
        {
            return PutAsync(key, s, cancellationToken);
        }

        var json = JsonSerializer.Serialize(value, JsonOptions);
        return PutAsync(key, json, cancellationToken);
    }

    public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var result = await _consulClient.KV.Get(key, cancellationToken).ConfigureAwait(false);
        if (result.Response?.Value is null)
        {
            return null;
        }

        return Encoding.UTF8.GetString(result.Response.Value);
    }

    public async Task<TValue?> GetAsync<TValue>(string key, CancellationToken cancellationToken = default)
    {
        var raw = await GetStringAsync(key, cancellationToken).ConfigureAwait(false);
        if (raw is null)
        {
            return default;
        }

        if (typeof(TValue) == typeof(string))
        {
            return (TValue)(object)raw;
        }

        try
        {
            return JsonSerializer.Deserialize<TValue>(raw, JsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize Consul KV value for key {Key}", key);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var result = await _consulClient.KV.Delete(key, cancellationToken).ConfigureAwait(false);
        return result.Response;
    }
}
