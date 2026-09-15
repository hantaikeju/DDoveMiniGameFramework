---
type: Reference
title: DDoveRes
description: YooAsset 运行时门面。建包、初始化、LoadAssetAsync、Shutdown。失败打 DDoveDebug，不进 IOC。
tags: [程序-前, ddoveres, yooasset]
status: stable
generated: { by: human:cjh, at: 2026-09-03T14:44:00Z }
verified: { by: human:cjh, at: 2026-09-15T03:20:00Z }
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
  - id: debug
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

无参 `InitializeAsync()` 读 [DDoveResInitInfo](../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveRes/Resources/DDoveResInitInfo.asset)（`Resources.Load`）：默认包名 + PlayMode。总窗 Res 页改这份 SO：默认包互斥（勾另一个则当前取消），加载模式用 `EPlayMode`。SO 缺失或包名为空时回退：编辑器从 Yoo 收集器取包名（优先 `DefaultPackage`，否则第一个）；真机 `DefaultPackage`。PlayMode 回退：编辑器 `EditorSimulate`，真机 `Offline`。Host / Web 尚未实现。不要在 Boot 场景上填包名 / PlayMode，见 [DDove Editor](/02-程序-前/DDoveEditor.md)。

`InitializeAsync()` 内部 `Initialize` + 解析包名 / PlayMode + `CreateInitializeOptions` + `InitializePackageAsync`。包 Init 成功后若还没有激活清单，再 `LoadActiveManifestAsync`（`RequestPackageVersionAsync` → `LoadPackageManifestAsync`）。成功且尚未设默认包时设默认。之后 `LoadAssetAsync` 可省略 `packageName`。EditorSimulate / Offline 这条主链（Init + 清单 + Load + Shutdown）**已落地**；进 Play 看见面板见 [Play 到 WndHome](/02-程序-前/Play到WndHome.md)。

```csharp
await DDoveResKit.InitializeAsync();
var handle = await DDoveResKit.LoadAssetAsync<GameObject>("WndHome");
if (handle == null)
{
    return;
}
```

| 方法 | 行为 |
|------|------|
| `InitializeAsync()` | 读 Init Info SO 的包名 / PlayMode；没有则收集器 / `DefaultPackage`。组 options 失败返回 `false` |
| `InitializeAsync(name, playMode)` | 显式指定。空包名抛 |
| `Initialize()` | `YooAssets` 未初始化才 `Initialize` |
| `CreatePackage(name)` | 空名抛 `ArgumentException`。已有则返回已有 |
| `SetDefaultPackage(name)` | 包不存在则 [DDoveDebug](/02-程序-前/DDoveDebug.md) `LogError` 并 return |
| `GetPackage(name?)` | 未指定用默认包。解析失败或包不存在 `LogError`，返回 `null` |
| `InitializePackageAsync` | `options == null` 抛。失败 `LogError`，返回 `false`。成功后必要时 `LoadActiveManifestAsync` |
| `CreateInitializeOptions` | EditorSimulate / Offline 组 options。Host / WebGL 尚未实现，`LogError` 返回 `null` |
| `LoadAssetAsync<T>` | 空 location 抛。包或加载失败 `LogError`，返回 `null`（失败 handle 会 `Release`） |
| `LoadSceneAsync` | 空 location 抛。包或加载失败 `LogError`，返回 `null`（失败 handle 会 `Release`） |
| `Shutdown()` | 已创建的包走官方 `DestroyPackageAsync` → `RemovePackage`，再 `YooAssets.Destroy()`，清空默认包名 |

`title` 固定 `DDoveRes`。取消走 `CancellationToken`，不改成 `LogError`。

按需下、收集器 / tag、不接整包 Fsm：见 [DDoveRes 按需加载](/02-程序-前/DDoveRes按需加载.md)。配法和进 Play：见 [DDoveRes 配置与使用](/02-程序-前/DDoveRes配置与使用.md)。

## 退出

停 Play / 退出：`Shutdown()`。不要改嵌入的 `Packages/com.tuyoogame.yooasset@3.0.5`。

谁调：`Initialize()` 挂 `[DDoveRes]`（`DDoveResLifetime`，`OnApplicationQuit`，执行序 `-10000`，赶在 Yoo `YooAssetsDriver` 前）。不要再挂编辑器 `ExitingPlayMode`。停 Play 当帧没有下一帧，门面反射调 `YooAssets.Update` 把 `DestroyPackageAsync` 泵完。

`DownloadSchedulerOperation` 是包 **Init** 拉起的常驻泵，不是一次 CDN 下载。Yoo **没有**公开停泵接口（`PauseScheduler` 不结束；`AbortOperation` 是 `internal`，只在文件系统 `OnDestroy` 里调）。边玩边下：单次文件任务 `IsDone` 会出队；泵自己不 `SetResult`，下一次 `Load` 还可能缺文件。`DestroyPackage` → `EditorFileSystem.OnDestroy` 对泵 `AbortOperation`，Yoo 打 Warning `Async operation 'DownloadSchedulerOperation' has been aborted.`。不是业务下载没停完。本库默认 [按需加载](/02-程序-前/DDoveRes按需加载.md)，这条 Warning **跟着承受**。启动前整批下完再进游戏才不必靠泵一直挂，那是另一套启动，默认不接。

不要：

- 等「所有 AsyncOperation」再 Destroy（泵永不完成，会卡死）
- 门面 `CancelDownload` / `PauseDownload`（只属于 `CreateResourceDownloader`，本库没建过）
- 门面反射 `Abort` / 自造 `StopDownloadSchedulers`（先停仍打同一条 Warning）
- 用 `YooAssets.Initialize(ILogger)` 滤这条 Abort
- 下完一批就停调度器（和 [按需加载](/02-程序-前/DDoveRes按需加载.md) 对着干）

`DestroyPackageAsync` 内部会先 `UnloadAllAssets` 再拆文件系统。`Shutdown` **还没有**防重入。Init 仍是 `Processing` 时官方 Destroy 会失败。

## 程序集

`DDoveFramework.Extension.DDoveRes`：`DDoveFramework.Core`、`UniTask`、`YooAsset`。Yoo 嵌入见 [UPM 落地](/02-程序-前/UPM落地.md)，本地夹 `Packages/com.tuyoogame.yooasset@3.0.5/`。

## 还没有

`UnloadAllAssets` / `CancelDownload` 门面、`Shutdown` 防重入、Downloader、Host / Web 的远程 Init options（`CreateInitializeOptions` 现返回 `null`）。EditorSimulate / Offline 的要版本 + 清单已在 `InitializePackageAsync` 里做完。json 表走已有 `LoadAssetAsync<TextAsset>`，见 [DDoveCfg](/02-程序-前/DDoveCfg.md)。bytes / RawFile 留给 bin。小游戏不接整包补丁 Fsm，见 [DDoveRes 按需加载](/02-程序-前/DDoveRes按需加载.md)。
