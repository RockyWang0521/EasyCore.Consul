using Consul;

namespace EasyCore.Consul.Cache
{
    public interface IConsulCache
    {
        /// <summary>
        /// 添加一个键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Task<WriteResult<bool>> KVPut(string key, string value);

        /// <summary>
        /// 添加一个键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Task<WriteResult<bool>> KVPut<TValue>(string key, TValue value);

        /// <summary>
        /// 获取一个键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<TValue> KVGet<TValue>(string key);


        /// <summary>
        /// 删除一个键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<WriteResult<bool>> KVDelete(string key);
    }
}
