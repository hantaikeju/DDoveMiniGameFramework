---
type: Playbook
title: 创建 LoopList 标题栏
description: CreateLoopListWindow.Open 用 GetWindow 留一扇。尺寸锁 420×268，打开时居中。
tags: [程序-前, ddoveui]
status: stable
generated: { by: human:cjh, at: 2026-09-26T06:18:00Z }
verified: { by: human:cjh, at: 2026-09-26T06:18:00Z }
sources:
  - id: window
    resource: ../../DDoveMiniGameClient/Assets/Game/Editor/CreateLoopListWindow.cs
    title: CreateLoopListWindow.cs
  - id: host
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Editor/DDoveEditorWindow.cs
    title: DDoveEditorWindow.cs
  - id: create
    resource: /02-程序-前/创建LoopList.md
    title: 创建 LoopList
  - id: req
    resource: /05-需求/创建LoopList标题栏.md
    title: 创建 LoopList 标题栏（需求稿）
---

# 创建 LoopList 标题栏

Concept ID：`/02-程序-前/创建LoopList标题栏`。清单：[index_cjh](/02-程序-前/index_cjh.md)。窗里怎么创建列表看 [创建 LoopList](/02-程序-前/创建LoopList.md)。总窗打开方式看 [DDove Editor](/02-程序-前/DDoveEditor.md)。

[三块](创建LoopList标题栏.html)

## 干什么

「创建 LoopList」是一扇普通编辑器窗口。`CreateLoopListWindow.Open` 用 `GetWindow` 留一扇：已有就聚焦，不另开第二扇。标题是「创建 LoopList」。`minSize` 与 `maxSize` 都是 `420×268`，打开时 `DDoveEditorUi.Centered`。`CreateGUI` 走 `DDoveEditorUi.ApplySheet`，标题栏与 `DDove/Editor` 同一类深色栏。

窗里的方向、条尺寸、间距、边距和「创建」不变。列表仍挂在 `UIRoot` 下。「创建 UI 场景」仍是 `ShowUtility`。

## 有哪些接口

- `CreateLoopListWindow.Open`
- `GetWindow<CreateLoopListWindow>`
- `DDoveEditorUi.Centered`
- `DDoveEditorUi.ApplySheet`

## 每个接口干什么

`Open` 挂着 HotBox「创建 LoopList」。它取出窗口、写上标题「创建 LoopList」、把最小和最大尺寸都锁成 `420×268`，再把 `position` 设成居中。不调用 `ShowUtility`。

`GetWindow<CreateLoopListWindow>` 只留一扇。再次从 HotBox 打开时聚焦已有那一扇。

`DDoveEditorUi.Centered` 按 `420×268` 算出打开时的居中位置。

`ApplySheet` 在 `CreateGUI` 里套到根节点，窗体用与 `DDove/Editor` 同一套深色样式。「创建 UI 场景」不走这条打开方式。
