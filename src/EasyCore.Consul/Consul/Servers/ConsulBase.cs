using Consul;

namespace EasyCore.Consul.Servers
{
    public class ConsulBase
    {
        public IConsulClient GetConsulClient(string url)
            => new ConsulClient(x => { x.Address = new Uri(url); });
    }
}
