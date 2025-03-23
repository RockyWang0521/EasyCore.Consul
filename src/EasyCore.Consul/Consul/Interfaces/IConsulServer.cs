namespace EasyCore.Consul.Servers
{
    public interface IConsulServer
    {
        /// <summary>
        /// 服务之间调用Get方法
        /// </summary>
        /// <typeparam name="TReturn">泛型返回</typeparam>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <returns></returns>
        Task<ConsulReturn<TReturn>> ServiceGet<TReturn>(RequestType type, string serviceNamein, string apiAddr, string? token = null);

        /// <summary>
        /// 服务之间调用Get方法
        /// </summary>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <returns></returns>
        Task<ConsulReturn> ServiceGet(RequestType type, string serviceNamein, string apiAddr, string? token = null);

        /// <summary>
        /// 服务之间调用Post方法
        /// </summary>
        /// <typeparam name="TParams">泛型输入</typeparam>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <param name="genericParam">输入参数</param>
        /// <returns></returns>
        Task<ConsulReturn<TReturn>> ServicePost<TReturn, TParams>(RequestType type, string serviceNamein, string apiAddr, TParams? genericParam, string? token = null);

        /// <summary>
        /// 服务之间调用Post方法
        /// </summary>
        /// <typeparam name="TParams">泛型输入</typeparam>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <param name="genericParam">输入参数</param>
        /// <returns></returns>
        Task<ConsulReturn> ServicePost<TParams>(RequestType type, string serviceNamein, string apiAddr, TParams? genericParam, string? token = null);


        /// <summary>
        /// 服务之间调用Post方法
        /// </summary>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <returns></returns>
        Task<ConsulReturn<TReturn>> ServicePost<TReturn>(RequestType type, string serviceNamein, string apiAddr, string? token = null);

        /// <summary>
        /// 服务之间调用Post方法
        /// </summary>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <returns></returns>
        Task<ConsulReturn> ServicePost(RequestType type, string serviceNamein, string apiAddr, string? token = null);

        /// <summary>
        /// 服务之间调用Put方法
        /// </summary>
        /// <typeparam name="TIn">泛型输入</typeparam>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <param name="genericParam">输入参数</param>
        /// <returns></returns>
        Task<ConsulReturn<TReturn>> ServicePut<TReturn, TParams>(RequestType type, string serviceNamein, string apiAddr, TParams? genericParam, string? token = null);

        /// <summary>
        /// 服务之间调用Put方法
        /// </summary>
        /// <typeparam name="TParams">泛型参数输入</typeparam>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <param name="genericParam">输入参数</param>
        /// <returns></returns>
        Task<ConsulReturn> ServicePut<TParams>(RequestType type, string serviceNamein, string apiAddr, TParams? genericParam, string? token = null);

        /// <summary>
        /// 服务之间调用Put方法
        /// </summary>
        /// <typeparam name="TReturn">泛型返回</typeparam>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <returns></returns>
        Task<ConsulReturn<TReturn>> ServicePut<TReturn>(RequestType type, string serviceNamein, string apiAddr, string? token = null);

        /// <summary>
        /// 服务之间调用Put方法
        /// </summary>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <returns></returns>
        Task<ConsulReturn> ServicePut(RequestType type, string serviceNamein, string apiAddr, string? token = null);

        /// <summary>
        /// 服务之间调用Delete方法
        /// </summary>
        /// <typeparam name="TReturn">泛型返回</typeparam>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <returns></returns>
        Task<ConsulReturn<TReturn>> ServiceDelete<TReturn>(RequestType type, string serviceNamein, string apiAddr, string? token = null);

        /// <summary>
        /// 服务之间调用Delete方法
        /// </summary>
        /// <typeparam name="TReturn">泛型返回</typeparam>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <returns></returns>
        Task<ConsulReturn> ServiceDelete(RequestType type, string serviceNamein, string apiAddr, string? token = null);
    }
}
