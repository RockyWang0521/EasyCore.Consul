# 🧭 EasyCore.Consul

> **EasyCore.Consul** is a production-oriented [HashiCorp Consul](https://www.consul.io/) integration library for .NET 8. It provides service registration with health checks, KV storage, distributed locking, healthy-instance discovery with load balancing, and inter-service HTTP invocation.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12-239120?logo=csharp)
![Consul](https://img.shields.io/badge/Consul-1.7+-e03875?logo=consul&logoColor=white)
![Features](https://img.shields.io/badge/Features-Register%20%7C%20KV%20%7C%20Lock%20%7C%20Invoke-blueviolet)
![License](https://img.shields.io/badge/License-MIT-yellow)
![Version](https://img.shields.io/badge/Version-8.3.0-blue)

---

## 🌍 Language

- 🇨🇳 Chinese: [README.md](https://github.com/RockyWang0521/EasyCore.Consul/blob/master/README.md)
- 🇺🇸 **English (this document)**

---

## 📚 Table of Contents

### 🗺️ Part I — Overview & Architecture
- [1. 🎯 Positioning](#1--positioning)
- [2. 🏗️ Architecture](#2-️-architecture)
- [3. 📦 NuGet / Projects](#3--nuget--projects)
- [4. 📊 Capability Matrix](#4--capability-matrix)

### 🚀 Part II — Getting Started
- [5. 💻 Requirements](#5--requirements)
- [6. 📥 Installation](#6--installation)
- [7. ⚡ Quick Start (3 minutes)](#7--quick-start-3-minutes)
- [8. ⚙️ Configuration Reference](#8-️-configuration-reference)

### 🧩 Part III — KV · Lock · Invocation
- [9. 🗄️ KV Cache](#9-️-kv-cache)
- [10. 🔒 Distributed Lock](#10--distributed-lock)
- [11. 🔗 Discovery & Invocation](#11--discovery--invocation)

### 🏭 Part IV — Demos & Production
- [12. 🧪 Demo Projects](#12--demo-projects)
- [13. 🔄 Migrating from older versions](#13--migrating-from-older-versions)
- [14. ✅ Production Checklist](#14--production-checklist)
- [15. ❓ FAQ](#15--faq)
- [16. 📄 License](#16--license)

---

## 1. 🎯 Positioning

EasyCore.Consul makes Consul easy, operable, and production-safe in ASP.NET Core:

| Pain point | EasyCore.Consul approach |
|---|---|
| Easy-to-miss register/deregister | Async `IHostedService` with graceful deregister |
| Catalog may route to unhealthy nodes | Health API + `PassingOnly` |
| Mutating `HttpClient.DefaultRequestHeaders` | Per-request Authorization headers |
| Locks without TTL renewal | Session TTL + `RenewPeriodic` |
| Bad config discovered too late | `ValidateOnStart` options validation |
| Monolithic DI surface | Opt-in `Add*` for Cache / Locking / Server |

### 1.1 ✨ Design Principles

| Principle | Meaning |
|---|---|
| **Low friction** | A few extension methods to register and health-check |
| **Composable** | Cache / Locking / Server register independently |
| **Failure-aware** | Registration throws; calls return `Succeed` / `Message` |
| **Production defaults** | Healthy discovery, stable ServiceId, ACL Token support |
| **Backward compatible** | Keeps `EasyCoreConsul*` / `ServiceIP` aliases |

### 1.2 📁 Repository Layout

```text
EasyCore.Consul/
├── src/EasyCore.Consul/           # Core library
│   ├── Configuration/            # Options + validation
│   ├── Registration/             # HostedService registration
│   ├── Cache/                    # KV
│   ├── Locking/                  # Distributed locks
│   ├── Discovery/                # Healthy discovery + LB
│   ├── Invocation/               # Inter-service HTTP
│   └── DependencyInjection/      # Add* / Use*
├── demo/
│   ├── Web.Consul/               # :5057 — KV / lock / invoke
│   ├── Web.Consul.Server/        # :5058 — downstream API
│   └── Web.Consul.Ocelot/        # Ocelot + Consul
├── tests/EasyCore.Consul.Tests/
└── docs/svg/                     # README diagrams
```

---

## 2. 🏗️ Architecture

### 2.1 🖼️ Component Diagram

![architecture-en](https://raw.githubusercontent.com/RockyWang0521/EasyCore.Consul/master/docs/svg/architecture-en.svg)

### 2.2 🔁 Service Lifecycle

![sequence-en](https://raw.githubusercontent.com/RockyWang0521/EasyCore.Consul/master/docs/svg/sequence-en.svg)

### 2.3 📜 Data Flow

```text
[ASP.NET Core Host]
        │
        ▼
 AddEasyCoreConsul ──► ConsulOptions (ValidateOnStart)
        │
        ├─ HostedService ──► Agent.ServiceRegister / Deregister
        │
        ├─ IConsulCache ────► KV Put / Get / Delete
        ├─ IConsulLocking ──► Session + Acquire / Renew / Release
        └─ IConsulServer ───► Health.Service → LB → HttpClient
                                    │
                                    ▼
                              Downstream Service
```

---

## 3. 📦 NuGet / Projects

| Package / Project | Role | Required |
|---|---|---|
| `EasyCore.Consul` | Register, KV, lock, discover, invoke | ✅ |
| `demo/Web.Consul` | Client sample | Sample |
| `demo/Web.Consul.Server` | Downstream API sample | Sample |
| `demo/Web.Consul.Ocelot` | Gateway + Consul discovery | Sample |
| `tests/EasyCore.Consul.Tests` | Unit tests | Dev |

---

## 4. 📊 Capability Matrix

| Capability | Description | Surface |
|---|---|---|
| Registration | Start register / stop deregister / HTTP check | `AddEasyCoreConsul` |
| KV store | Strings and typed JSON | `IConsulCache` |
| Distributed lock | TTL renewal, `await using` lease | `IConsulLocking` |
| Discovery | Healthy instances only (default) | `IConsulServiceDiscovery` |
| Load balancing | RoundRobin (default) / Random | `LoadBalance` |
| Invocation | GET/POST/PUT/DELETE + Bearer | `IConsulServer` |
| Gateway | Ocelot provider (demo) | `Web.Consul.Ocelot` |

### 4.1 🌳 Decision Tree

```text
Should this process register itself?
├── Yes → AddEasyCoreConsul + Register=true + HealthCheck.Http
└── No  → AddEasyCoreConsul + Register=false (consume only)

Also need?
├── Config / cache → AddEasyCoreConsulCache
├── Critical section → AddEasyCoreConsulLocking
└── Call other services → AddEasyCoreConsulServer
```

---

## 5. 💻 Requirements

| Item | Requirement |
|---|---|
| .NET | 8.0+ |
| Host | ASP.NET Core (Web / API) |
| Consul | Reachable agent (default `http://127.0.0.1:8500`) |
| Dependency | `Consul` NuGet (brought by this package) |

---

## 6. 📥 Installation

```bash
dotnet add package EasyCore.Consul
```

Project reference:

```xml
<ProjectReference Include="..\..\src\EasyCore.Consul\EasyCore.Consul.csproj" />
```

---

## 7. ⚡ Quick Start (3 minutes)

### 7️⃣.1️⃣ Configure `appsettings.json`

```json
{
  "Consul": {
    "ConsulAddress": "http://127.0.0.1:8500",
    "Token": null,
    "Register": true,
    "ServiceName": "my-service",
    "ServiceAddress": "127.0.0.1",
    "ServicePort": 5057,
    "LoadBalance": "RoundRobin",
    "PassingOnly": true,
    "HealthCheck": {
      "Http": "http://127.0.0.1:5057/healthCheck",
      "Interval": "00:00:10",
      "Timeout": "00:00:05",
      "DeregisterCriticalServiceAfter": "00:01:00"
    }
  }
}
```

### 7️⃣.2️⃣ Register services

```csharp
using EasyCore.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.AddEasyCoreConsul()
    .AddEasyCoreConsulCache()
    .AddEasyCoreConsulLocking()
    .AddEasyCoreConsulServer();

builder.Services.AddControllers();

var app = builder.Build();

app.UseEasyCoreConsul(); // maps /healthCheck; registration runs as hosted service
app.MapControllers();
app.Run();
```

Open the Consul UI at `http://127.0.0.1:8500` to see the registered service.

---

## 8. ⚙️ Configuration Reference

| Option | Description |
|---|---|
| `ConsulAddress` | Consul HTTP API base URL |
| `Token` | Optional ACL token |
| `Datacenter` | Optional datacenter |
| `Register` | Self-register (default `true`) |
| `ServiceName` | Logical service name |
| `ServiceId` | Instance id; empty ⇒ `{ServiceName}-{MachineName}-{Port}` |
| `ServiceAddress` / `ServiceIP` | Advertised address |
| `ServicePort` | Advertised port |
| `Tags` / `Meta` | Service tags and metadata |
| `HealthCheck.Http` / `ServiceHealthCheck` | HTTP health-check URL |
| `HealthCheck.Interval` | Interval (default 10s) |
| `HealthCheck.Timeout` | Timeout (default 5s) |
| `HealthCheck.DeregisterCriticalServiceAfter` | Auto-deregister after critical |
| `LoadBalance` | `RoundRobin` / `Random` |
| `PassingOnly` | Discover only passing instances (default `true`) |

---

## 9. 🗄️ KV Cache

```csharp
public class DemoController(IConsulCache cache) : ControllerBase
{
    [HttpPost("{key}")]
    public Task<bool> Put(string key, [FromBody] MyDto dto, CancellationToken ct)
        => cache.PutAsync(key, dto, ct);

    [HttpGet("{key}")]
    public Task<MyDto?> Get(string key, CancellationToken ct)
        => cache.GetAsync<MyDto>(key, ct);

    [HttpGet("raw/{key}")]
    public Task<string?> GetRaw(string key, CancellationToken ct)
        => cache.GetStringAsync(key, ct);
}
```

| API | Description |
|---|---|
| `PutAsync` / `KVPut` | Write string or object (JSON) |
| `GetStringAsync` | Raw string without JSON deserialize |
| `GetAsync<T>` / `KVGet` | Typed read; `T=string` returns raw text |
| `DeleteAsync` / `KVDelete` | Delete key |

---

## 10. 🔒 Distributed Lock

```csharp
// Preferred: acquire / renew / release automatically
await locking.ExecuteLockedAsync("orders:pay", ttlSeconds: 30, async ct =>
{
    await DoWorkAsync(ct);
}, cancellationToken);

// Or manual lease
await using var lease = await locking.TryAcquireAsync(
    "orders:pay", TimeSpan.FromSeconds(30), ct);
if (lease is null) return Conflict();
```

| API | Description |
|---|---|
| `TryAcquireAsync` | Returns `IConsulLock` (with session renew), or `null` |
| `ExecuteLockedAsync` | `Func<CancellationToken, Task>` — properly awaited |
| `AcquireLock` / `ReleaseLock` | Legacy APIs (no auto renew) |

> ⚠️ Keep TTL realistic for your critical section. Always dispose the lease with `await using`.

---

## 11. 🔗 Discovery & Invocation

```csharp
var result = await consulServer.ServiceGetAsync<OrderDto>(
    RequestScheme.Http,
    serviceName: "order-service",
    apiPath: "/api/orders/1",
    cancellationToken: ct);

if (!result.Succeed) return Problem(result.Message);
return Ok(result.Values);
```

| Method | Description |
|---|---|
| `ServiceGetAsync` | GET |
| `ServicePostAsync` | POST (optional body) |
| `ServicePutAsync` | PUT |
| `ServiceDeleteAsync` | DELETE |

Call chain: `Health.Service` → load balancer → `IHttpClientFactory`. By default only **passing** instances are selected.

---

## 12. 🧪 Demo Projects

![demo-topology-en](https://raw.githubusercontent.com/RockyWang0521/EasyCore.Consul/master/docs/svg/demo-topology-en.svg)

| Project | Port | Role | Command |
|---|---|---|---|
| [`Web.Consul`](demo/Web.Consul) | 5057 | KV / lock / invoke | `dotnet run --project demo/Web.Consul` |
| [`Web.Consul.Server`](demo/Web.Consul.Server) | 5058 | Downstream REST API | `dotnet run --project demo/Web.Consul.Server` |
| [`Web.Consul.Ocelot`](demo/Web.Consul.Ocelot) | — | Ocelot gateway | `dotnet run --project demo/Web.Consul.Ocelot` |

```bash
# 1. Start Consul Agent (HTTP :8500)
# 2. Start downstream
dotnet run --project demo/Web.Consul.Server
# 3. Start client
dotnet run --project demo/Web.Consul
# 4. Open http://127.0.0.1:8500
```

---

## 13. 🔄 Migrating from older versions

**8.1.0** is a breaking enhancement over early wrappers (upgrade recommended):

| Older | 8.1 |
|---|---|
| `.Wait()` inside `UseEasyCoreConsul` | Async `IHostedService` registration |
| Catalog discovery | Health API + `PassingOnly` |
| `ExecuteLocked(Action)` | Prefer `ExecuteLockedAsync(Func<..., Task>)` |
| Exposing `WriteResult<bool>` | Returns `bool` |
| Flat `ServiceHealthCheck` | `HealthCheck` object (legacy field still works) |
| `EasyCoreConsul*` | Kept; prefer `AddEasyCoreConsul*` |

---

## 14. ✅ Production Checklist

- [ ] Point `ConsulAddress` at an internal DNS / VIP; configure ACL `Token`
- [ ] Use a `ServiceAddress` reachable by other nodes (not loopback-only)
- [ ] Keep `HealthCheck.Timeout` in seconds, not minutes
- [ ] Set a stable `ServiceId` for multi-instance, or accept `{Name}-{Machine}-{Port}`
- [ ] Keep `PassingOnly=true` on callers
- [ ] Match lock TTL to critical-section duration; always release leases
- [ ] Watch startup logs — registration failure fails host start
- [ ] Run `dotnet test` in CI (workflow included)

---

## 15. ❓ FAQ

**Q: Options validation fails at startup?**  
A: When `Register=true`, you must set `ServiceName`, `ServiceAddress`, `ServicePort`, and a valid `ConsulAddress`.

**Q: Service shows in UI but calls fail?**  
A: Check that the health check is passing; ensure `ServiceAddress` is reachable; ensure the downstream is running.

**Q: KV only — no self-registration?**  
A: Set `Register: false`, then call `AddEasyCoreConsulCache()`.

**Q: Cannot acquire a lock?**  
A: Another holder may own it, or Session/Acquire failed. Check logs; handle `null` from `TryAcquireAsync`.

**Q: How does the Ocelot demo relate?**  
A: Ocelot discovers via `Ocelot.Provider.Consul`; this library handles app-side registration and invocation. They can be used together.

---

## 16. 📄 License

MIT — see [LICENSE](LICENSE).

---

## 🤝 Contributing

1. Fork and create a feature branch  
2. Add tests under `tests/EasyCore.Consul.Tests`  
3. Run `dotnet test` and `dotnet build EasyCore.Consul.sln`  
4. Open a pull request  

Issues and PRs are welcome 🚀
