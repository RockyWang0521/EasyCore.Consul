namespace EasyCore.Consul.Invocation;

/// <summary>Result of an inter-service HTTP call.</summary>
public class ServiceCallResult
{
    public bool Succeed { get; init; }
    public string Message { get; init; } = string.Empty;
    public int? StatusCode { get; init; }

    public static ServiceCallResult Ok(string message = "Request successful", int? statusCode = null)
        => new() { Succeed = true, Message = message, StatusCode = statusCode };

    public static ServiceCallResult Fail(string message, int? statusCode = null)
        => new() { Succeed = false, Message = message, StatusCode = statusCode };
}

/// <summary>Typed result of an inter-service HTTP call.</summary>
public class ServiceCallResult<T> : ServiceCallResult
{
    public T? Values { get; init; }

    public static ServiceCallResult<T> Ok(T values, string message = "Request successful", int? statusCode = null)
        => new() { Succeed = true, Message = message, Values = values, StatusCode = statusCode };

    public new static ServiceCallResult<T> Fail(string message, int? statusCode = null)
        => new() { Succeed = false, Message = message, StatusCode = statusCode };
}

/// <summary>
/// Backward-compatible aliases for <see cref="ServiceCallResult"/> / <see cref="ServiceCallResult{T}"/>.
/// </summary>
public class ConsulReturn : ServiceCallResult
{
}

public sealed class ConsulReturn<TReturn> : ServiceCallResult<TReturn>
{
}
