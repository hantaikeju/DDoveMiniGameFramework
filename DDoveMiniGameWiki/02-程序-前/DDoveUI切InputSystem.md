---
type: Playbook
title: DDoveUI 切 Input System
description: 第二刀已落地：UI EventSystem 只开 Input System Package，文本只留 TMP。不用 Both，不搬 Probe。
tags: [程序-前, ddoveui]
status: stable
generated: { by: human:cjh, at: 2026-09-17T08:17:00Z }
verified: { by: human:cjh, at: 2026-09-17T10:25:00Z }
sources:
  - id: grill
    resource: /00-索引/Agent/需求稿工作流.md
    title: 需求稿工作流
  - id: ui
    resource: /02-程序-前/DDoveUI.md
    title: DDoveUI
  - id: ui-biz
    resource: /02-程序-前/UI业务封装.md
    title: UI 业务封装
  - id: upm
    resource: /02-程序-前/UPM落地.md
    title: UPM 落地
  - id: play
    resource: /02-程序-前/Play到WndHome.md
    title: Play 到 WndHome
  - id: kit
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/DDoveUIKit.cs
    title: DDoveUIKit.cs
  - id: scene-editor
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/Editor/DDoveUISceneEditor.cs
    title: DDoveUISceneEditor.cs
  - id: panel-base
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/DDoveUIPanelBase.cs
    title: DDoveUIPanelBase.cs
  - id: node-bind
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/DDoveUINodeBind.cs
    title: DDoveUINodeBind.cs
  - id: wnd-home
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/WndHome.cs
    title: WndHome.cs
  - id: sample
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/Editor/DDoveUISampleBuilder.cs
    title: DDoveUISampleBuilder.cs
  - id: ui-asmdef
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/DDoveFramework.Extension.DDoveUI.asmdef
    title: DDoveUI.asmdef
  - id: manifest
    resource: ../../DDoveMiniGameClient/Packages/manifest.json
    title: Packages/manifest.json
  - id: player-settings
    resource: ../../DDoveMiniGameClient/ProjectSettings/ProjectSettings.asset
    title: ProjectSettings.asset
---

# DDoveUI 切 Input System

Concept ID：`/02-程序-前/DDoveUI切InputSystem`。清单：[index_cjh](/02-程序-前/index_cjh.md)。结论与代码冲突以当前代码为准。模块总述见 [DDoveUI](/02-程序-前/DDoveUI.md)。

对照：[DDoveUI](/02-程序-前/DDoveUI.md)、[UI 业务封装](/02-程序-前/UI业务封装.md)、[UPM 落地](/02-程序-前/UPM落地.md)、[Play 到 WndHome](/02-程序-前/Play到WndHome.md)。

## 约定

只开新 Input System，文本只留 TMP。运行时 Canvas 仍是 Camera。Play 点击已按只开新包验收通过。不搬 Probe，不用 Both。

| 项 | 现行 |
|----|------|
| 包 | `com.unity.inputsystem` 1.7.0，官方源，不嵌入 `Packages/` |
| Active Input Handling | `activeInputHandler: 2`（Input System Package） |
| EventSystem | `InputSystemUIInputModule` + 默认 UI Actions 并 `Enable()`。拆掉 `StandaloneInputModule` |
| 程序集 | `DDoveUI` / `DDoveUI.Editor` 引 `Unity.InputSystem`；`Game` 不引 |
| Canvas | 制作场景 Overlay；运行时 `ScreenSpaceCamera` + UI 相机 |
| 文本 | 只 `TextMeshProUGUI`；默认 LiberationSans SDF。无 `DDoveUINodeBindType.Text` / `SetText(Text)` |

`AddComponent` 的 `OnEnable` 已赋默认 Actions，不要马上 `AssignDefaultActions()`。Debugger 里 `UI/Click` 可能出现两份，不要为此禁用模块。`ClickSound` / `AddClick` / 导航仍走 EventSystem 指针，见 [UI 业务封装](/02-程序-前/UI业务封装.md)。

## 不要

- Both 或只开旧 Input Manager
- 搬 `DDoveUIPointerProbe`
- 禁用 `InputSystemUIInputModule`
- 运行时改 Overlay、删 UI 相机
- `Game` 引 `Unity.InputSystem`；玩法键盘 / 手柄 Actions
- 拷 Input System 进 `Packages/`、拷 MeowPantry 字体
- 改 `ClickSound` / `AddClick` / 导航 API

## 验收

1. `manifest.json` 有 `com.unity.inputsystem` 1.7.x；`Packages/` 下没有该包嵌入夹。
2. `activeInputHandler` 为 `2`。
3. `DDoveUI` / `DDoveUI.Editor` 引 `Unity.InputSystem`；`Game.asmdef` 不引。
4. 运行时与新建制作场景是 `InputSystemUIInputModule`，没有 `StandaloneInputModule`，没有 `DDoveUIPointerProbe`。
5. 运行时 Canvas 仍是 `ScreenSpaceCamera` + UI 相机。
6. 绑定 / `SetText` / 导出不再认 `UnityEngine.UI.Text`；`Detect` 先认 TMP。
7. 样板、运行时新建、`WndHome` 用 TMP + LiberationSans SDF。
8. Play：看见 `WndHome`，只开新包时能点领取。
9. `ClickSound` / `AddClick` / 导航与切包前相同。
