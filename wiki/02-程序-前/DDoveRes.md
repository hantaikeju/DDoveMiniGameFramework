---
type: Reference
title: DDoveRes
description: YooAsset 运行时门面。建包、初始化、LoadAssetAsync。失败打 DDoveDebug，不进 IOC。
tags: [程序-前, ddoveres, yooasset]
status: stable
generated: { by: human:cjh, at: 2026-09-03T14:44:00Z }
verified: { by: human:cjh, at: 2026-09-03T14:46:00Z }
sources:
  - id: kit
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveRes/DDoveResKit.cs
    title: DDoveResKit.cs
  - id: init-info
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveRes/DDoveResInitInfo.cs
    title: DDoveResInitInfo.cs
  - id: asmdef
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveRes/DDoveFramework.Extension.DDoveRes.asmdef
    title: DDoveFramework.Extension.DDoveRes.asmdef
  - id: eudebug
    resource: /02-程序-前/DDoveDebug.md
    title: DDoveDebug
  - id: core
    resource: /02-程序-前/DDoveFramework-Core.md
    title: DDoveFramework Core
---

# DDoveRes

Concept ID：`/02-程序-前/DDoveRes`。权威实现：[DDoveResKit.cs](../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveRes/DDoveResKit.cs)。结论与代码冲突以代码为准。

`Extension/DDoveRes/` 静态门面，包 YooAsset。不进 [IOC 容器](/02-程序-前/IOC容器.md)，不进 [DDoveFramework Core](/02-程序-前/DDoveFramework-Core.md)。异步用 [UniTask](/02-程序-前/UniTask异步.md)。

## 调用

包名从 Yoo `BundleCollectorSetting` 读（优先 `DefaultPackage`，否则第一个 Package）。收集器没有 Package 时用 `DefaultPackage`，直接调 Yoo 初始化。PlayMode：编辑器 `EditorSimulate`，真机 `Offline`（Host / Web 以后再接）。不要在场景或自建 SO 上手填包名 / PlayMode。总窗 Res 页只展示收集器里的包，见 [DDove Editor](/02-程序-前/DDoveEditor.md)。

`InitializeAsync()` 内部 `Initialize` + 解析包名 / PlayMode + `CreateInitializeOptions` + `InitializePackageAsync`。成功且尚未设默认包时设默认。之后 `LoadAssetAsync` 可省略 `packageName`。

```csharp
await DDoveResKit.InitializeAsync();
var handle = await DDoveResKit.LoadAssetAsync<GameObject>("UI/Home");
if (handle == null)
{
    return;
}
```

| 方法 | 行为 |
|------|------|
| `InitializeAsync()` | 从 Yoo 收集器取包名；没有则 `DefaultPackage`。组 options 失败返回 `false` |
| `InitializeAsync(name, playMode)` | 显式指定。空包名抛 |
| `Initialize()` | `YooAssets` 未初始化才 `Initialize` |
| `CreatePackage(name)` | 空名抛 `ArgumentException`。已有则返回已有 |
| `SetDefaultPackage(name)` | 包不存在则 [DDoveDebug](/02-程序-前/DDoveDebug.md) `LogError` 并 return |
| `GetPackage(name?)` | 未指定用默认包。解析失败或包不存在 `LogError`，返回 `null` |
| `InitializePackageAsync` | `options == null` 抛。失败 `LogError`，返回 `false` |
| `CreateInitializeOptions` | EditorSimulate / Offline 组 options。Host / WebGL 尚未实现，`LogError` 返回 `null` |
| `LoadAssetAsync<T>` | 空 location 抛。包或加载失败 `LogError`，返回 `null`（失败 handle 会 `Release`） |
| `LoadSceneAsync` | 空 location 抛。包或加载失败 `LogError`，返回 `null`（失败 handle 会 `Release`） |

`title` 固定 `DDoveRes`。取消走 `CancellationToken`，不改成 `LogError`。

按需下、收集器 / tag、不接整包 Fsm：见 [DDoveRes 按需加载](/02-程序-前/DDoveRes按需加载.md)。

## 程序集

`DDoveFramework.Extension.DDoveRes`：`DDoveFramework.Core`、`UniTask`、`YooAsset`。Yoo 嵌入见 [UPM 落地](/02-程序-前/UPM落地.md)，本地夹 `Packages/com.tuyoogame.yooasset@3.0.5/`。

## 还没有

bytes / TextAsset、释放全集、Downloader、Host 版本/清单、Luban。小游戏不接整包补丁 Fsm，见 [DDoveRes 按需加载](/02-程序-前/DDoveRes按需加载.md)。
