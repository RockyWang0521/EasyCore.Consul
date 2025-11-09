namespace EasyCore.Consul.Discovery;

internal sealed class RandomLoadBalancer : ILoadBalancer
{
    public ServiceEndpoint? Select(IReadOnlyList<ServiceEndpoint> endpoints)
    {
        if (endpoints.Count == 0)
        {
            return null;
        }

        if (endpoints.Count == 1)
        {
            return endpoints[0];
        }

        return endpoints[Random.Shared.Next(endpoints.Count)];
    }
}

internal sealed class RoundRobinLoadBalancer : ILoadBalancer
{
    private int _index = -1;

    public ServiceEndpoint? Select(IReadOnlyList<ServiceEndpoint> endpoints)
    {
        if (endpoints.Count == 0)
        {
            return null;
        }

        if (endpoints.Count == 1)
        {
            return endpoints[0];
        }

        var next = Interlocked.Increment(ref _index);
        if (next < 0)
        {
            Interlocked.Exchange(ref _index, 0);
            next = 0;
        }

        return endpoints[next % endpoints.Count];
    }
}
