---
type: Playbook
title: DDoveRes 配置与使用
description: 总窗 Res 页怎么配默认包、加载模式、启动场景；Boot 怎么读；运行时怎么 Load。接口表仍看 DDoveRes。
tags: [程序-前, ddoveres, yooasset]
status: stable
generated: { by: human:cjh, at: 2026-09-10T07:35:00Z }
sources:
  - id: init-info
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveRes/DDoveResInitInfo.cs
    title: DDoveResInitInfo.cs
  - id: res-panel
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveRes/Editor/DDoveResEditorPanel.cs
    title: DDoveResEditorPanel.cs
  - id: kit
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveRes/DDoveResKit.cs
    title: DDoveResKit.cs
  - id: boot
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveBoot/DDoveBootLogic.cs
    title: DDoveBootLogic.cs
  - id: res
    resource: /02-程序-前/DDoveRes.md
    title: DDoveRes
  - id: editor
    resource: /02-程序-前/DDoveEditor.md
    title: DDove Editor
  - id: ondemand
    resource: /02-程序-前/DDoveRes按需加载.md
    title: DDoveRes 按需加载
---

# DDoveRes 配置与使用

Concept ID：`/02-程序-前/DDoveRes配置与使用`。清单：[index_cjh](/02-程序-前/index_cjh.md)。接口表看 [DDoveRes](/02-程序-前/DDoveRes.md)；边玩边下看 [DDoveRes 按需加载](/02-程序-前/DDoveRes按需加载.md)。结论与代码冲突以代码为准。

本篇只写**怎么配、怎么用**。不要在 Boot 场景上填包名 / PlayMode / 启动场景。

## 配置落在哪

一份 SO：[DDoveResInitInfo](../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveRes/Resources/DDoveResInitInfo.asset)。运行时 `Resources.Load`（名字 `DDoveResInitInfo`）。这是框架自己的 Init 资产，**不是** `Assets/GameRes/` 那棵树。

| 字段 | 默认 | 谁读 |
|------|------|------|
| 默认包 `_packageName` | `DefaultPackage` | `DDoveResKit.InitializeAsync()` |
| 加载模式 `_playMode` | 编辑器 `EditorSimulate` | 同上 |
| 启动场景 `_launchSceneLocation` | `Launch` | [DDoveBoot](/02-程序-前/DDoveBoot.md) 进真实场景 |

SO 缺失或包名为空时回退：编辑器从 Yoo 收集器取包名（优先 `DefaultPackage`，否则第一个）；真机 `DefaultPackage`。PlayMode 回退：编辑器 `EditorSimulate`，真机 `Offline`。启动场景空则回退 `Launch`。

Create 菜单仍可 `DDove/Res/Init Info`。日常改走总窗，不要在 Boot 上拖这份资产。门面约定见 [DDove Editor](/02-程序-前/DDoveEditor.md)。

## 总窗 Res 页

菜单 **`DDove/Editor`** → 左侧 **Res**。

1. 先用 **Yoo Collector** 建包裹、收目录（官方窗，不要再包一层）。
2. 回到 Res：标题栏 **刷新**，或把焦点切回这扇窗，包裹列表会重画。
3. 勾一个 **默认包**（互斥）。
4. 选 **加载模式**（`EPlayMode`）。现在能跑的是 `EditorSimulate`、`Offline`。`Host` / `Web` 会 Init 失败。
5. **启动场景**填 Yoo location，默认 `Launch`。清空会写回 `Launch`。

没有包裹时页上提示去 Yoo 加 Package。改完写入上面那份 SO，不必进 Play 再存。

## 收集器怎么收

只要一个 `DefaultPackage`。Boot 场景 **不要进**任何收集器。真实入口 `Assets/GameRes/Scenes/Launch.unity` **必须进**（`DefaultPackage` / Scene / tag `launch`），**不要**进 Build Settings。

资源根：`Assets/GameRes/` 进收集器；`Assets/GameResExcluded/` 不进。不要一条收集器扫整个 `UI/`。阶段目录（Common / Login / Hall）有了再加组。细则见 [DDoveRes 按需加载](/02-程序-前/DDoveRes按需加载.md)。

## 运行时怎么走

Play：**起步** `Assets/Scenes/DDoveBoot.unity`（Build Settings index 0）→ `DDoveResKit.InitializeAsync()` 读 SO → 其它 Extension Init → `LoadSceneAsync(启动场景)`。

业务 Load（Boot 已经 Init 成功之后）：

```csharp
var handle = await DDoveResKit.LoadAssetAsync<GameObject>("WndHome");
if (handle == null)
{
    return;
}
```

空 location 抛。失败 [DDoveDebug](/02-程序-前/DDoveDebug.md) `LogError`，`title` 固定 `DDoveRes`，返回 `null`。取消走 `CancellationToken`。方法表见 [DDoveRes](/02-程序-前/DDoveRes.md)。异步用 [UniTask](/02-程序-前/UniTask异步.md)。

## 不要

- 在 `DDoveBoot` Inspector 上填包名 / PlayMode / 启动场景
- 把 Boot 打进 Yoo，或把 Launch 再丢进 Build Settings
- 按模块拆第二个 Package，或 Builtin + Remote 双包裹
- 启动全量 `CreateDownloader()` + 补丁 Fsm
- 在 Editor 里另写一套 Load

## 还没有

Host / Web 的版本与清单、Downloader、进度 UI、bytes / TextAsset、释放全集、Luban。未实现前不要把按需加载篇里的预下 API 当已有方法。
