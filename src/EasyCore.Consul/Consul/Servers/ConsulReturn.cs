namespace EasyCore.Consul.Servers
{
#pragma warning disable CS8618

    public class ConsulReturn<TReturn>
    {
        public bool Succeed { get; set; }

        public string Message { get; set; }

        public TReturn Values { get; set; }
    }

    public class ConsulReturn
    {
        public bool Succeed { get; set; }

        public string Message { get; set; }
    }

#pragma warning restore CS8618

    public enum RequestType
    {
        Http = 0,

        Https = 1
    }
}
