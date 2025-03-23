using Consul;
using EasyCore.Consul.Servers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace EasyCore.Consul.Cache
{
    public class ConsulCache : ConsulBase, IConsulCache
    {
        private readonly ConsulOptions _options;

        public ConsulCache(IOptions<ConsulOptions> options) => _options = options.Value;

        public async Task<WriteResult<bool>> KVPut(string key, string value)
        {
            using var client = base.GetConsulClient(_options.ConsulAddress);
               
            return await client.KV.Put(new KVPair(key) { Value = Encoding.UTF8.GetBytes(value) });
        }

        public async Task<TValue> KVGet<TValue>(string key)
        {
            using var client = base.GetConsulClient(_options.ConsulAddress);

            var result = await client.KV.Get(key);

            if (result.Response is null) return default!;

            string? valueString = Encoding.UTF8.GetString(result.Response.Value);

            return string.IsNullOrEmpty(valueString) ? default! : JsonConvert.DeserializeObject<TValue>(valueString)!;
        }

        public async Task<WriteResult<bool>> KVDelete(string key)
        {
            using var client = base.GetConsulClient(_options.ConsulAddress);

            return await client.KV.Delete(key);          
        }

        public async Task<WriteResult<bool>> KVPut<TValue>(string key, TValue value)
        {
            var valueString = JsonConvert.SerializeObject(value);

            return await KVPut(key, valueString);
        }
    }
}
