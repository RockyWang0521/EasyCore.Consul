# docs/

README 配图（SVG）存放于此。

## 为何 NuGet 上看不见 Gitee 图？

[NuGet.org](https://learn.microsoft.com/en-us/nuget/nuget-org/package-readme-on-nuget-org#allowed-domains-for-images-and-badges) **只渲染白名单域名**上的图片。

| 来源 | NuGet 能否显示 |
|------|----------------|
| `docs/svg/...` 相对路径 | ❌（包内路径不会被解析成图床） |
| `gitee.com/.../raw/...` | ❌（不在白名单） |
| `raw.githubusercontent.com/...` | ✅ |
| `img.shields.io/...` | ✅ |

因此 README 中的架构图统一使用：

```text
https://raw.githubusercontent.com/RockyWang0521/EasyCore.Consul/master/docs/svg/<file>.svg
```

修改 SVG 后请同时推送到 **GitHub**（供 NuGet / 外链）与 Gitee（仓库浏览）。重新打包发布后，NuGet 页面才会更新。
