---
type: Playbook
title: 创建 LoopList 标题栏（需求稿）
description: 薄记录。用法篇才是接口总结。得分留在本篇。
tags: [程序-前, ddoveui, 需求]
status: deprecated
generated: { by: human:cjh, at: 2026-09-25T19:30:00Z }
sources:
  - id: use
    resource: /02-程序-前/创建LoopList标题栏.md
    title: 创建 LoopList 标题栏
  - id: window
    resource: ../../DDoveMiniGameClient/Assets/Game/Editor/CreateLoopListWindow.cs
    title: CreateLoopListWindow.cs
  - id: host
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Editor/DDoveEditorWindow.cs
    title: DDoveEditorWindow.cs
---

# 创建 LoopList 标题栏（需求稿）

用法篇：[创建 LoopList 标题栏](/02-程序-前/创建LoopList标题栏.md)。

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

## 评分

分数记在本节，不另开文件。尺子：开工评分 v11。

| 轮 | 总分 | 过线 | 记录 |
|----|------|------|------|
| 1 | 100 | 是 | 审查 PASS。需求验收 20、Wiki 同步 20、职责边界 20、范围 15、信任表述 15、规格增量 10。条文不变。 |
| 2 | 100 | 是 | 对照验收，改动路径无。审查 PASS。需求验收 20、Wiki 同步 20、职责边界 20、范围 15、信任表述 15、规格增量 10。条文不变。 |
