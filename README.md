# EasyCore.Consul

Consul是一种用于服务发现、服务注册和配置管理的开源工具，在微服务架构中有着广泛的应用。

核心功能

服务发现：Consul提供了通过DNS或者HTTP接口的方式来注册服务和发现服务的功能。这使得外部的服务能够轻松地找到它所依赖的其他服务。
健康检查：Consul的Client可以提供任意数量的健康检查，既可以与给定的服务相关联（如“webserver是否返回200 OK”），也可以与本地节点相关联（如“内存利用率是否低于90%”）。操作员可以使用这些信息来监视集群的健康状况，服务发现组件可以使用这些信息将流量从不健康的主机路由出去。
Key/Value存储：应用程序可以根据自己的需要使用Consul提供的Key/Value存储。Consul提供了简单易用的HTTP接口，结合其他工具可以实现动态配置、功能标记、领袖选举等功能。
安全服务通信：Consul可以为服务生成和分发TLS证书，以建立相互的TLS连接。意图可用于定义允许哪些服务通信。服务分割可以很容易地进行管理，其目的是可以实时更改的，而不是使用复杂的网络拓扑和静态防火墙规则。
架构组件

代理（Agent）：代理是Consul集群的每个成员上长时间运行的守护程序。它可以以客户端或服务器模式运行。所有节点都必须运行代理，因此将节点称为客户端或服务器更简单。代理能够以客户端或服务器模式运行，并且可以运行DNS或HTTP接口，并负责运行检查并保持服务同步。
数据中心（Datacenter）：数据中心被定义为专用、低延迟和高带宽的网络环境。这排除了通过公共互联网的通信，但出于我们的目的，单个EC2区域内的多个可用区域将被视为单个数据中心的一部分。
共识（Consensus）：在Consul中，共识表示对当选领导者的协议以及对交易顺序的协议。作为一个分布式系统，Consul使用Raft算法来实现共识，确保复制状态机的一致性。
使用场景

服务发现：在微服务架构中，服务实例通常是动态创建和销毁的。Consul作为服务注册中心，服务地址被注册到Consul中以后，可以使用Consul提供的DNS、HTTP接口查询，支持Health Check。
服务隔离：Consul支持以服务为单位设置访问策略，能同时支持经典的平台和新兴的平台，支持TLS证书分发，service-to-service加密。
服务配置：Consul提供Key/Value数据存储功能，并且能将变动迅速地通知出去，通过工具consul-template可以更方便地实时渲染配置文件。

EasyCore.Consul 提供了简单快捷的应用。

1.注册consul

```
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add EasyCoreConsul
        builder.EasyCoreConsul(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseAuthorization();

        // Use EasyCoreConsul
        app.UseEasyCoreConsul();

        app.MapControllers();
        app.Run();
    }
}
```
服务就会注册至consul中。

2.注册consul缓存

```
builder.EasyCoreConsul(args).EasyCoreConsulCache();
```
使用consul缓存

```
 [Route("api/[controller]")]
 [ApiController]
 public class ConsulCacheController : ControllerBase
 {
     private readonly IConsulCache _consulCache;

     public ConsulCacheController(IConsulCache consulCache) => _consulCache = consulCache;

     [HttpPost("string:{key}/{value}")]
     public async Task<WriteResult<bool>> Post(string key, string value)
     {
         return await _consulCache.KVPut(key, value);
     }

     [HttpPost("dto:{key}")]
     public async Task<WriteResult<bool>> Post(string key, ConsulCacheDto value)
     {
         return await _consulCache.KVPut(key, value);
     }

     [HttpGet("string:{key}")]
     public async Task<string> KVGet(string key)
     {
         return await _consulCache.KVGet<string>(key);
     }

     [HttpGet("dto:{key}")]
     public async Task<ConsulCacheDto> KVGetDto(string key)
     {
         return await _consulCache.KVGet<ConsulCacheDto>(key);
     }

     [HttpDelete("{key}")]
     public async Task<WriteResult<bool>> Delete(string key)
     {
         return await _consulCache.KVDelete(key);
     }
 }
```
3.注册consul锁

```
builder.EasyCoreConsul(args).EasyCoreConsulLocking();
```
使用consul锁

```
 [Route("api/[controller]")]
 [ApiController]
 public class ConsulLockingController : ControllerBase
 {
     private readonly IConsulLocking _consulLocking;
     private readonly ILogger<ConsulLockingController> _logger;

     public ConsulLockingController(IConsulLocking consulLocking,
             ILogger<ConsulLockingController> logger)
     {
         _consulLocking = consulLocking;
         _logger = logger;
     }

     [HttpPost("lock:{lockKey}/{lockTTL}")]
     public async Task ConsulLocking(string lockKey, int lockTTL)
     {
         string? sessionId = await _consulLocking.AcquireLock(lockKey, lockTTL);
         if (sessionId != null)
         {
             _logger.LogInformation($"Lock acquired with session id: {sessionId}");
             await Task.Delay(5000);
             await _consulLocking.ReleaseLock(lockKey, sessionId);
         }
     }

     [HttpPost("ExecuteLocked:{lockKey}/{lockTTL}")]
     public async Task ExecuteLocked(string lockKey, int lockTTL)
     {
         await _consulLocking.ExecuteLocked(lockKey, lockTTL, async () =>
         {
             _logger.LogInformation("Executing critical section under lock.");
             await Task.Delay(5000);
         });

         _logger.LogInformation("Execution finished.");
     }
 }
```
4.注册consul服务间调用

```
builder.EasyCoreConsul(args).EasyCoreConsulServer();
```
使用consul服务间调用

```
  [Route("api/[controller]")]
  [ApiController]
  public class ConsulServerController : ControllerBase
  {
      private readonly IConsulServer _consulServer;

      const string serverName = "Web.Consul.Server";

      public ConsulServerController(IConsulServer consulServer)
          => _consulServer = consulServer;


      [HttpGet]
      public async Task<ConsulReturn<ConsulServerDto>> GetConsulServer()
      {
          return await _consulServer.ServiceGet<ConsulServerDto>(RequestType.Http, serverName, "/RESTfulApi?id=1");
      }

      [HttpPost]
      public async Task<ConsulReturn<string>> Post()
      {
          var dto = new ConsulServerDto()
          {
              BoolDto = true,
              IntDto = 200,
              StringDto = $"this is Web.Consul ConsulServerController.Post method "
          };

          return await _consulServer.ServicePost<string, ConsulServerDto>(RequestType.Http, serverName, "/RESTfulApi", dto);
      }

      [HttpPut]
      public async Task<ConsulReturn<ConsulServerDto>> Put()
      {
          var dto = new ConsulServerDto
          {
              BoolDto = true,
              IntDto = 300,
              StringDto = $"this is Web.Consul ConsulServerController.Put method "
          };

          return await _consulServer.ServicePut<ConsulServerDto, ConsulServerDto>(RequestType.Http, serverName, "/RESTfulApi", dto);
      }

      [HttpDelete]
      public async Task<ConsulReturn<string>> Delete()
      {
          return await _consulServer.ServiceDelete<string>(RequestType.Http, serverName, "/RESTfulApi?id=2");
      }
  }
```





