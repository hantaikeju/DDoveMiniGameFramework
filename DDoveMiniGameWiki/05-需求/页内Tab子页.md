---
type: Playbook
title: 页内 Tab 子页（需求稿）
description: 薄记录。用法篇才是接口总结。得分留在本篇。
tags: [程序-前, ddoveui, 需求]
status: deprecated
generated: { by: human:cjh, at: 2026-09-25T19:43:00Z }
sources:
  - id: use
    resource: /02-程序-前/页内Tab子页.md
    title: 页内 Tab 子页
  - id: kit
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/DDoveUIKit.cs
    title: DDoveUIKit.cs
  - id: demo
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/WndTabDemo.cs
    title: WndTabDemo.cs
---

# 页内 Tab 子页（需求稿）

用法篇：[页内 Tab 子页](/02-程序-前/页内Tab子页.md)。

[要什么不要](页内Tab子页.html)

## 要什么

`DDoveUIKit.OpenChildAsync<TChild>(Component parent, IDDoveUIPanelData data = null)`：子页仍是独立面板，有自己的类、Prefab、Yoo 地址和 `OnOpen` / `OnClose`，登记在 Kit 里。不进导航栈。`parent` 用面板组件自己。实例挂到该组件根下的直接子节点 `Content`。这条路径不使用子页的 `DefaultLayer`。父页为空时打开失败并打日志。

没有 `Content`，或该类型已经作为别的窗打开着：打开失败，打日志，不挂到 `DDoveUIRoot` 的层上。该类型已是这个父页的子页：改为 `Show`。显示出一个子页时，这个父页下其余子页 `Hide`，实例留着，不重新 Load。

父页 `OnOpen` 里打开并显示约定的第一个子页。`Navigate` 离开父页只 `Hide` 父页，子页留在 `Content` 里，回来仍是刚才那一页。父页真正关闭时（返回、`Close`、`CloseAll`）先 `Close` 全部子页并 `Release`，含已经 `Hide` 的。登记过子页的父页不进 LRU。下次从 `WndHome` 进来会重新 `OnOpen`，再打开第一个子页。

样板在 `Start`。`WndHome` 在现有三行后再加一行，标题 `Tab`，`NavigateToAsync<WndTabDemo>`。`WndTabDemo` 上有返回、两个 Tab，以及直接子节点 `Content`。返回走 `BackAsync`。两个 Tab 分别打开 `WndTabPageA`、`WndTabPageB`。`OnOpen` 打开 `WndTabPageA`。

两个子页各自场景、各自 Prefab。`UIRoot` 里只放自己的标题，文案分别是 `Tab A`、`Tab B`。父级 Tab 的拷贝放在子场景的 `Excluded_Top`，只用于对齐，不进子页 Prefab。`GameLaunch` 不改。业务继续直接调 Kit。

## 不要

- 用 `NavigateToAsync` 切换 Tab
- 子页挂到 `DDoveUIRoot` 的层上，或写进导航栈
- 把父级可点按钮放进子页 `UIRoot`
- 切 Tab 时 `Close` 并 `Release`
- `Navigate` 离开父页时关掉子页
- 父页进入 LRU 后带着空的 `Content` 再 `Show`
- 只销毁父物体来收子页，不走 Kit 的 `Close` / `Release`
- 改 `GameLaunch`
- 为开子页加只转发 Kit 的 System

## 评分

分数记在本节，不另开文件。尺子：开工评分 v11（`stable`）。

| 轮 | 总分 | 过线 | 记录 |
|----|------|------|------|
| 1 | 100 | 是 | 审查 PASS。需求验收 20、Wiki 同步 20、职责边界 20、范围 15、信任表述 15、规格增量 10。条文不变。 |
