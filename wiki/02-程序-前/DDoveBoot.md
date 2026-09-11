---
type: Reference
title: DDoveBoot
description: 起步场景与启动编排。只初始化资源包再 Yoo 加载真实场景。不进 IOC，不进 DDoveRes。
tags: [程序-前, ddoveboot, yooasset]
status: stable
generated: { by: human:cjh, at: 2026-09-04T01:28:00Z }
sources:
  - id: logic
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveBoot/DDoveBootLogic.cs
    title: DDoveBootLogic.cs
  - id: asmdef
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveBoot/DDoveFramework.Extension.DDoveBoot.asmdef
    title: DDoveFramework.Extension.DDoveBoot.asmdef
  - id: scene
    resource: ../../DDoveMiniGameClient/Assets/Scenes/DDoveBoot.unity
    title: DDoveBoot.unity
  - id: res
    resource: /02-程序-前/DDoveRes.md
    title: DDoveRes
  - id: debug
    resource: /02-程序-前/DDoveDebug.md
    title: DDoveDebug
---

# DDoveBoot

Concept ID：`/02-程序-前/DDoveBoot`。权威实现：[DDoveBootLogic.cs](../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveBoot/DDoveBootLogic.cs)。结论与代码冲突以代码为准。

`Extension/DDoveBoot/` 是启动编排，不是资源门面。资源走 [DDoveRes](/02-程序-前/DDoveRes.md)。不进 [IOC 容器](/02-程序-前/IOC容器.md)，不进 [DDoveFramework Core](/02-程序-前/DDoveFramework-Core.md)。异步用 [UniTask](/02-程序-前/UniTask异步.md)。

## 场景放哪

| 场景 | 路径 | Build Settings | Yoo 收集器 |
|------|------|----------------|------------|
| 起步 `DDoveBoot` | `Assets/Scenes/DDoveBoot.unity` | **必须，index 0** | **不要进** |
| 真实入口（默认 location `Launch`） | `Assets/GameRes/Scenes/Launch.unity` | **不要进** | **必须进**（`DefaultPackage` / Scene / tag `launch`） |

`DDoveBoot` 可以、也应该直接挂在 `Assets/Scenes/`。引擎只能先打开 Build Settings 里的起步场景；包初始化完再 `LoadSceneAsync`。不要把 Boot 打进 Yoo Builtin，也不要放 `Resources/`。

Hierarchy 顶层并列即可：`DDoveBoot`（挂 `DDoveBootLogic`）+ `Main Camera`（纯色底）。不要业务 Prefab、登录 UI、第二套业务场景进 Build Settings。`SampleScene` 可留作编辑器测试，不要当启动场景。

## Logic 只编排 Extension

```
DDoveResKit.InitializeAsync()
  → InitializeExtensionsAsync（虚方法，以后其它 Extension）
  → OnExtensionsReadyAsync（默认 LoadScene Launch，业务可覆写）
```

启动场景、包名、PlayMode **不在** Boot Inspector 上填。Boot 读 [DDoveResInitInfo](/02-程序-前/DDoveRes配置与使用.md)：`LaunchSceneLocation` 空则回退 `Launch`。见 [DDoveRes](/02-程序-前/DDoveRes.md)。

资源 Init 失败则 return。场景加载失败 [DDoveDebug](/02-程序-前/DDoveDebug.md) `LogError`，`title` 固定 `DDoveBoot`。取消走 `CancellationToken`。

不创建进度 UI，不 `DontDestroyOnLoad`。资源怎么起包只在 Res / Yoo 收集器，不要塞回 Boot。

## 程序集

`DDoveFramework.Extension.DDoveBoot`：`DDoveFramework.Core`、`DDoveFramework.Extension.DDoveRes`、`UniTask`、`YooAsset`。

## 还没有

Host / Web 要版本与清单、进度 UI。业务 Architecture 已由 `GameLaunch` 碰 `Interface`，见 [Architecture 自动注册](/02-程序-前/Architecture自动注册.md)。不接整包下载 Fsm；按需约定见 [DDoveRes 按需加载](/02-程序-前/DDoveRes按需加载.md)。
