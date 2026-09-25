---
type: Playbook
title: 创建 LoopList 标题栏
description: 已落地。创建 LoopList 用 GetWindow 留一扇普通编辑器窗口，再次打开聚焦它。尺寸仍锁 420×268，打开时居中。
tags: [程序-前, ddoveui, 需求]
status: draft
generated: { by: human:cjh, at: 2026-09-25T19:30:00Z }
sources:
  - id: grill
    resource: /00-索引/Agent/需求稿工作流.md
    title: 需求稿工作流
  - id: create
    resource: /02-程序-前/创建LoopList.md
    title: 创建 LoopList
  - id: editor
    resource: /02-程序-前/DDoveEditor.md
    title: DDove Editor
  - id: window
    resource: ../../DDoveMiniGameClient/Assets/Game/Editor/CreateLoopListWindow.cs
    title: CreateLoopListWindow.cs
  - id: host
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Editor/DDoveEditorWindow.cs
    title: DDoveEditorWindow.cs
---

# 创建 LoopList 标题栏

Concept ID：`/05-需求/创建LoopList标题栏`。清单：[index_cjh](/05-需求/index_cjh.md)。创建行为仍看 [创建 LoopList](/02-程序-前/创建LoopList.md)。总窗打开方式看 [DDove Editor](/02-程序-前/DDoveEditor.md)。

已按本稿落地。`CreateLoopListWindow.Open` 用 `GetWindow` 留一扇，标题仍是「创建 LoopList」，`minSize` 与 `maxSize` 都是 `420×268`，打开时 `DDoveEditorUi.Centered`。不再 `ShowUtility`。窗内字段和创建逻辑未改。「创建 UI 场景」仍是 `ShowUtility`。

对照过期，本刀未改：[创建 LoopList](/02-程序-前/创建LoopList.md) 仍只写小窗，未写普通编辑器窗口和再次打开聚焦。

[要什么不要](创建LoopList标题栏.html)

## 要什么

「创建 LoopList」改成普通编辑器窗口，标题栏与 `DDove/Editor` 同一类深色栏。打开方式与总窗一样：已有一扇就聚焦它，不另开第二扇。

尺寸仍锁在 `420×268`，打开时居中。窗里的方向、条尺寸、间距、边距和「创建」不动。列表仍挂在 `UIRoot` 下。

「创建 UI 场景」保持现在的系统小窗。

## 不要

- 继续用系统小窗（白底红叉）打开「创建 LoopList」
- 每次从 HotBox 再开一扇
- 改窗内字段、默认值、创建逻辑或父节点规则
- 改「创建 UI 场景」
- 改 DDove 总窗本身

## 落点

| | 放哪 |
|--|------|
| 打开方式 | `Game.Editor.CreateLoopListWindow`。与 `DDoveEditorWindow` 一样只留一扇并聚焦 |
| 尺寸 | 仍锁 `420×268`，打开时居中 |
| 不动 | 「创建 UI 场景」仍是系统小窗 |

## 验收

1. 「创建 LoopList」打开后是普通编辑器窗口，标题栏与 DDove 编辑器同一类深色栏，不是白底红叉。
2. 再次从 HotBox 打开，聚焦已有那一扇，不另开第二扇。
3. 窗口尺寸仍是 `420×268`，打开时居中。
4. 窗内字段和创建行为与现在相同。「创建 UI 场景」仍用系统小窗。

## 评分

分数记在本节，不另开文件。尺子：开工评分 v11。

| 轮 | 总分 | 过线 | 记录 |
|----|------|------|------|
| 1 | 100 | 是 | 审查 PASS。需求验收 20、Wiki 同步 20、职责边界 20、范围 15、信任表述 15、规格增量 10。条文不变。 |
