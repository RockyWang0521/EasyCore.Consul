namespace EasyCore.Consul.Cache;

/// <summary>Consul KV store abstraction.</summary>
public interface IConsulCache
{
    Task<bool> PutAsync(string key, string value, CancellationToken cancellationToken = default);

    Task<bool> PutAsync<TValue>(string key, TValue value, CancellationToken cancellationToken = default);

    Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);

    Task<TValue?> GetAsync<TValue>(string key, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>Legacy alias of <see cref="PutAsync(string, string, CancellationToken)"/>.</summary>
    Task<bool> KVPut(string key, string value, CancellationToken cancellationToken = default)
        => PutAsync(key, value, cancellationToken);

    /// <summary>Legacy alias of <see cref="PutAsync{TValue}"/>.</summary>
    Task<bool> KVPut<TValue>(string key, TValue value, CancellationToken cancellationToken = default)
        => PutAsync(key, value, cancellationToken);

    /// <summary>Legacy alias of <see cref="GetAsync{TValue}"/>.</summary>
    Task<TValue?> KVGet<TValue>(string key, CancellationToken cancellationToken = default)
        => GetAsync<TValue>(key, cancellationToken);

    /// <summary>Legacy alias of <see cref="DeleteAsync"/>.</summary>
    Task<bool> KVDelete(string key, CancellationToken cancellationToken = default)
        => DeleteAsync(key, cancellationToken);
}
