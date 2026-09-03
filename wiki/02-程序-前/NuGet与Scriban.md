---
type: Playbook
title: NuGet 与 Scriban
description: 编辑器用 NuGetForUnity 从 nuget 源装 Scriban。出站失败是 UnityTls / 系统代理，和 manifest.json（UPM URL 拉包）不是同一条线。
tags: [程序-前, nuget, scriban]
status: stable
generated: { by: human:cjh, at: 2026-09-03T02:00:00Z }
verified: { by: human:cjh, at: 2026-09-03T02:15:00Z }
sources:
  - id: nuget-config
    resource: ../../DDoveMiniGameClient/Assets/NuGet.config
    title: Assets/NuGet.config
  - id: packages-config
    resource: ../../DDoveMiniGameClient/Assets/packages.config
    title: Assets/packages.config
  - id: nugetforunity-pkg
    resource: ../../DDoveMiniGameClient/Packages/com.github-glitchenzo.nugetforunity@acc1c7bc9e/package.json
    title: NuGetForUnity 4.5.0 package.json
  - id: core
    resource: /02-程序-前/EUFramework-Core.md
    title: EUFramework Core
---

# NuGet 与 Scriban

Concept ID：`/02-程序-前/NuGet与Scriban`。清单：[index_cjh](/02-程序-前/index_cjh.md)。

编辑器代码生成用 **Scriban**。装包走 **NuGetForUnity** → nuget 源 → `Assets/Packages/` + `Assets/packages.config`。不要写进 [EUFramework Core](/02-程序-前/EUFramework-Core.md)。

当前已装：Scriban **7.2.7**（`manuallyInstalled`）及依赖。

`Packages/manifest.json` 是 Unity Package Manager 的 **URL / 注册表拉包**。GitHub 包装完后拷到工程根 `Packages/`，见 [UPM 落地](/02-程序-前/UPM落地.md)。**和 Scriban 不是一个问题**，这次排障、改 nuget 源都不要去动 manifest。

## 这条线管什么

| | 作用 |
|---|---|
| `Assets/NuGet.config` | nuget 源、安装目录（`repositoryPath` = `./Packages`，相对 `Assets/`） |
| `Assets/packages.config` | 已装 nuget 包锁版本 |
| `Assets/Packages/Scriban.*` | 装下来的 dll |
| 菜单 `NuGet → Manage NuGet Packages` | 搜 / 装 / 卸 |

NuGetForUnity 本身是 UPM 工具（可以按 URL 进工程），只负责打开窗口；**Scriban 不写进 manifest**。

## 这次的问题

窗口 Online 刷新 / 点 Install 失败。不是 Scriban 版本配错，也不是 Unity 拦了 nuget.org。

常见栈：

- `HttpRequestException` → `WebException: ReadDoneAsync2 ReceiveFailure`（`SearchPackageAsync` / `UpdateOnlinePackages`）
- `TlsException: Failed to read data to TLS context — UNITYTLS_INTERNAL_ERROR`
- 连带 `GUI Error: Invalid GUILayout state in NugetWindow`（异常砸在 `OnGUI`，不必单独修）

原因：插件用编辑器里的 **Mono `HttpClient` + UnityTls** 访问 nuget.org（搜索还会转到 `azuresearch-*.nuget.org`）。梯子开了系统代理时，Unity 在 Windows 上套 `GetSystemWebProxy()`，读流容易被掐。浏览器能开 nuget.org **不代表** 编辑器能下完 `.nupkg`。

`Select all from clipboard` 只搜 JSON，有时能勾上包；Install 再下二进制，仍可能 TLS 失败。

## 这次的处理

1. **关梯子**（本机 Clash Verge / 系统代理）。
2. **打开 Windows「自动检测设置」**（设置 → 网络和 Internet → 代理）。不要用梯子的本地端口当系统代理给 Unity。
3. 重开 NuGet 窗口，再搜 / Install。

备选：改 `Assets/NuGet.config` 的源，或浏览器下 `.nupkg` 加本地目录。已核对可用：

```
https://repo.huaweicloud.com/repository/nuget/v3/index.json
```

刷新若仍打 nuget.org，在 **NuGet.config** 里禁用官方源。当时不通、别抄：`nuget.cdn.azure.cn`、`nuget.cnblogs.com`、腾讯 `…/nuget/v3/index.json`。

装上后把 `Assets/Packages/` 与 `packages.config` 进 Git。

## 不要

- 把这次报错当成 UPM / `manifest.json` 的事
- 把 nuget 包丢进工程根 `Packages/`（那是 UPM 目录）
- Core / 热更运行时引用 Scriban；dll 建议只开 Editor，避免进微信/抖音包
