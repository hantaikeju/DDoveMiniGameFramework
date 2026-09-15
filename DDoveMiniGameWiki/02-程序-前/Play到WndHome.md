---
type: Playbook
title: Play 到 WndHome
description: 点 Play 到看见 WndHome。Boot 加载 Launch 后必须有 GameLaunch；Yoo 3.0.5 要 Init + 要版本 + 加载清单。收集器有资源不等于运行时已有清单。
tags: [程序-前, ddoveboot, ddoveres, ddoveui]
status: stable
generated: { by: human:cjh, at: 2026-09-14T09:20:00Z }
verified: { by: human:cjh, at: 2026-09-15T03:20:00Z }
sources:
  - id: launch
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/GameLaunch.cs
    title: GameLaunch.cs
  - id: res-kit
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveRes/DDoveResKit.cs
    title: DDoveResKit.cs
  - id: boot
    resource: /02-程序-前/DDoveBoot.md
    title: DDoveBoot
  - id: res
    resource: /02-程序-前/DDoveRes.md
    title: DDoveRes
  - id: res-use
    resource: /02-程序-前/DDoveRes配置与使用.md
    title: DDoveRes 配置与使用
  - id: ondemand
    resource: /02-程序-前/DDoveRes按需加载.md
    title: DDoveRes 按需加载
  - id: ui
    resource: /02-程序-前/DDoveUI.md
    title: DDoveUI
  - id: atlas
    resource: /02-程序-前/DDoveAtlas.md
    title: DDoveAtlas
  - id: arch-bind
    resource: /02-程序-前/Architecture自动注册.md
    title: Architecture 自动注册
---

# Play 到 WndHome

Concept ID：`/02-程序-前/Play到WndHome`。清单：[index_cjh](/02-程序-前/index_cjh.md)。Boot / Res / UI 各篇仍是模块权威；本篇只写**编辑器点 Play 到看见面板**这条链。与代码冲突以当前代码为准。

对照：[DDoveBoot](/02-程序-前/DDoveBoot.md)、[DDoveRes](/02-程序-前/DDoveRes.md)、[DDoveRes 配置与使用](/02-程序-前/DDoveRes配置与使用.md)、[DDoveUI](/02-程序-前/DDoveUI.md)、[DDoveAtlas](/02-程序-前/DDoveAtlas.md)、[Architecture 自动注册](/02-程序-前/Architecture自动注册.md)。

## 要什么

Play 后：`DDoveBoot` → 包可用 → `Launch` → `GameLaunch` → 开 `WndHome`。Canvas 运行时叫 **`DDoveUIRoot`**（DontDestroyOnLoad）；预制体导出根才叫 `UIRoot`。

## 整条链

```
点 Play（起步 DDoveBoot，不要只开当前编辑场景当入口）
  DDoveResKit.InitializeAsync
    EditorSimulate 构建
    InitializePackageAsync
    RequestPackageVersionAsync
    LoadPackageManifestAsync      Yoo 3.0.5 三步；缺后两步 ActiveManifest 空
  LoadSceneAsync("Launch")
  GameLaunch（场景上已挂，或 sceneLoaded 补挂）
    DDoveCfgKit.LoadAsync
    GameArchitecture.Interface
    DDoveAtlasKit.Initialize
    DDoveUIKit.Initialize         建 DDoveUIRoot
    OpenAsync<WndHome>            Load 预制体 WndHome
```

收集器是编辑器菜单。Yoo Collector 里 Scene 组有 `Launch.unity`，只说明**以后能打到**。运行时还要加载清单，`LoadScene("Launch")` 才找得到名字。

## GameLaunch 必须在 Launch 之后

[DDoveUI](/02-程序-前/DDoveUI.md) 写：`GameLaunch` **挂 Launch 场景**。权威实现：[GameLaunch.cs](../../DDoveMiniGameClient/Assets/Game/Mono/GameLaunch.cs)。

`RuntimeInitializeOnLoadMethod(AfterSceneLoad)` **只在第一场跑一次**。起步是 Boot 时，当时场景名不是 `Launch`，会直接 return。Boot 再用 Yoo 加载 Launch，**不会再触发** AfterSceneLoad。场景上没有组件、又只靠 AfterSceneLoad，就会停在空 Launch：没有 `DDoveUIRoot`，没有 `WndHome`。

现行两道：

1. `Launch.unity` 顶层 `Launch` 物体挂 `GameLaunch`
2. `SceneManager.sceneLoaded`：场景名是 `Launch` 且场上没有组件时补一个

不要在 Boot 场景挂业务面板，不要让 Boot 引用 UI。

## Yoo 3.0.5 清单

Yoo **3.0.5** 的 EditorSimulate / Offline **也要**要版本 + 清单，否则 `LoadSceneAsync` 抛 `Active package manifest not found`。实现在 [DDoveResKit](../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveRes/DDoveResKit.cs) 的 `LoadActiveManifestAsync`。约定见 [DDoveRes](/02-程序-前/DDoveRes.md)、[DDoveRes 按需加载](/02-程序-前/DDoveRes按需加载.md)。

```
InitializePackageAsync
  → RequestPackageVersionAsync
  → LoadPackageManifestAsync(version)
  → 这时 PackageValid，才能 Load
```

## 进 Play 被静默掐掉

[Architecture 自动注册](/02-程序-前/Architecture自动注册.md)：清单和生成文件对不上就取消本次 Play，编完再进。比对读 `// BIND:角色:键:实现`。键是 `global::Game.CfgUtility` 这种名字时，**不能**按 `:` 整段切开，否则解析永远是空的，Play 会取消→再进→再取消。解析按角色后的最后一个 `:global::` 切开。生成文件没改动则放行，不要死循环。

Console 刷 `[Architecture] play cancelled: architecture bind stale` 就是这条。

## 现象对照

| 看见 | 先查 |
|------|------|
| Play 立刻退、刷 Architecture stale | BIND 解析 / PlayGuard |
| 停在 Boot 黑屏，`Active package manifest not found` | Init 后没加载清单；不是收集器空 |
| 进了 Launch，只有 Camera + YooAssets，没有 DDoveUIRoot | `GameLaunch` 没跑 |
| 有 DDoveUIRoot，没有 WndHome | `OpenAsync` / 收集器 Group Start 能否 `Load("WndHome")` |
| WndHome 在、图白 | [DDoveAtlas](/02-程序-前/DDoveAtlas.md) late-bind / `LoadSpriteAsync` |

## 不要

- 把收集器窗口当成运行时已 Init
- 只靠 AfterSceneLoad 创建 `GameLaunch`
- 在 Boot 里 `DDoveUIKit.Initialize`
- 把本篇当 Boot / Res / UI 模块说明书

## 验收

1. Play：Boot → Launch → DontDestroyOnLoad 有 `DDoveUIRoot`，下面有 `WndHome`。
2. 场景 `Launch` 物体上有 `GameLaunch`；卸掉后 `sceneLoaded` 仍会补挂。
3. 收集器有 `Launch` 但故意不加载清单时，Load 失败且打 `[DDoveRes]`，不要未处理异常。
4. Architecture BIND 行含 `global::` 时，点 Play 能留下，不刷 stale。
