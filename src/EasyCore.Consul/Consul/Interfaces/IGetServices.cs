namespace EasyCore.Consul.Servers
{
    internal interface IGetServices
    {
        Task<string?> GetService(RequestType type, string serviceNamein, string apiAddr);
    }
}
