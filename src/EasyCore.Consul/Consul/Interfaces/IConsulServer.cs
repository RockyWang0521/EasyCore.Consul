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
        Task<ConsulReturn<TReturn>> ServiceGetAsync<TReturn>(RequestType type, string serviceNamein, string apiAddr, string? token = null);

        /// <summary>
        /// 服务之间调用Get方法
        /// </summary>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <returns></returns>
        Task<ConsulReturn> ServiceGetAsync(RequestType type, string serviceNamein, string apiAddr, string? token = null);

        /// <summary>
        /// 服务之间调用Post方法
        /// </summary>
        /// <typeparam name="TParams">泛型输入</typeparam>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <param name="genericParam">输入参数</param>
        /// <returns></returns>
        Task<ConsulReturn<TReturn>> ServicePostAsync<TReturn, TParams>(RequestType type, string serviceNamein, string apiAddr, TParams? genericParam, string? token = null);

        /// <summary>
        /// 服务之间调用Post方法
        /// </summary>
        /// <typeparam name="TParams">泛型输入</typeparam>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <param name="genericParam">输入参数</param>
        /// <returns></returns>
        Task<ConsulReturn> ServicePostAsync<TParams>(RequestType type, string serviceNamein, string apiAddr, TParams? genericParam, string? token = null);


        /// <summary>
        /// 服务之间调用Post方法
        /// </summary>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <returns></returns>
        Task<ConsulReturn<TReturn>> ServicePostAsync<TReturn>(RequestType type, string serviceNamein, string apiAddr, string? token = null);

        /// <summary>
        /// 服务之间调用Post方法
        /// </summary>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <returns></returns>
        Task<ConsulReturn> ServicePostAsync(RequestType type, string serviceNamein, string apiAddr, string? token = null);

        /// <summary>
        /// 服务之间调用Put方法
        /// </summary>
        /// <typeparam name="TIn">泛型输入</typeparam>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <param name="genericParam">输入参数</param>
        /// <returns></returns>
        Task<ConsulReturn<TReturn>> ServicePutAsync<TReturn, TParams>(RequestType type, string serviceNamein, string apiAddr, TParams? genericParam, string? token = null);

        /// <summary>
        /// 服务之间调用Put方法
        /// </summary>
        /// <typeparam name="TParams">泛型参数输入</typeparam>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <param name="genericParam">输入参数</param>
        /// <returns></returns>
        Task<ConsulReturn> ServicePutAsync<TParams>(RequestType type, string serviceNamein, string apiAddr, TParams? genericParam, string? token = null);

        /// <summary>
        /// 服务之间调用Put方法
        /// </summary>
        /// <typeparam name="TReturn">泛型返回</typeparam>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <returns></returns>
        Task<ConsulReturn<TReturn>> ServicePutAsync<TReturn>(RequestType type, string serviceNamein, string apiAddr, string? token = null);

        /// <summary>
        /// 服务之间调用Put方法
        /// </summary>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <returns></returns>
        Task<ConsulReturn> ServicePutAsync(RequestType type, string serviceNamein, string apiAddr, string? token = null);

        /// <summary>
        /// 服务之间调用Delete方法
        /// </summary>
        /// <typeparam name="TReturn">泛型返回</typeparam>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <returns></returns>
        Task<ConsulReturn<TReturn>> ServiceDeleteAsync<TReturn>(RequestType type, string serviceNamein, string apiAddr, string? token = null);

        /// <summary>
        /// 服务之间调用Delete方法
        /// </summary>
        /// <typeparam name="TReturn">泛型返回</typeparam>
        /// <param name="serviceNamein">服务名</param>
        /// <param name="apiAddr">api地址</param>
        /// <returns></returns>
        Task<ConsulReturn> ServiceDeleteAsync(RequestType type, string serviceNamein, string apiAddr, string? token = null);
    }
}
