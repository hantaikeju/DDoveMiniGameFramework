---
type: Playbook
title: 页内 Tab 子页
description: OpenChildAsync 把子页挂到父页 Content。切 Tab 只隐藏。关父页先 Release 子页，该父页不进 LRU。
tags: [程序-前, ddoveui]
status: stable
generated: { by: human:cjh, at: 2026-09-26T03:02:00Z }
verified: { by: human:cjh, at: 2026-09-26T03:02:00Z }
sources:
  - id: kit
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/DDoveUIKit.cs
    title: DDoveUIKit.cs
  - id: lru
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/DDoveUIKit.LRU.cs
    title: DDoveUIKit.LRU.cs
  - id: demo
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/WndTabDemo.cs
    title: WndTabDemo.cs
  - id: home
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/WndHome.cs
    title: WndHome.cs
  - id: req
    resource: /05-需求/页内Tab子页.md
    title: 页内 Tab 子页（需求稿）
---

# 页内 Tab 子页

Concept ID：`/02-程序-前/页内Tab子页`。清单：[index_cjh](/02-程序-前/index_cjh.md)。整页打开仍看 [DDoveUI](/02-程序-前/DDoveUI.md)。入口列表仍看 [WndHome Sample入口](/02-程序-前/WndHomeSample入口.md)。

[三块](页内Tab子页.html)

## 干什么

`DDoveUIKit.OpenChildAsync` 打开一扇子页面板，挂到父页组件根下名为 `Content` 的直接子节点。子页有自己的类、Prefab 和 Yoo 地址，登记在 Kit 里，不进导航栈，也不使用 `DefaultLayer`。

`parent` 传父页面板自己。父页为空、没有这个 `Content`，或该类型已经作为别的窗打开：打开失败并打日志，不挂到 `DDoveUIRoot` 的层上。该类型已是这个父页的子页：改为 `Show`。显示出一个子页时，同父的其余子页 `Hide`，实例留着，不重新 Load。

`NavigateToAsync` 离开父页只 `Hide` 父页，子页留在 `Content` 里，回来仍是刚才那一页。父页 `Close`、`CloseAll`，以及 `BackAsync` 走到关闭时，先 `Close` 全部子页并 `Release`，含已经 `Hide` 的。登记过子页的父页不进 LRU。下次再打开父页会重新 `OnOpen`。

样板在 `Start`。`WndHome` 第四行标题 `Tab`，`NavigateToAsync<WndTabDemo>`。`WndTabDemo.OnOpen` 打开 `WndTabPageA`。两个 Tab 分别 `OpenChildAsync` 打开 `WndTabPageA`、`WndTabPageB`。返回走 `BackAsync`。

## 有哪些接口

- `DDoveUIKit.OpenChildAsync<TChild>(Component parent, IDDoveUIPanelData data = null)`
- 父页上的 `Close`、`CloseAll`、`BackAsync`
- `DDoveUIKit.NavigateToAsync<T>`

## 每个接口干什么

`OpenChildAsync` 用 `parent.GetType().Name` 记下父子关系。`Content` 只找该组件的直接子节点。已是这个父页的子页就 `Show`，并 `Hide` 其余子页。类型已在别处打开则失败，日志原因是 `already open`。没有 `Content` 的原因是 `missing Content`。父页为空的原因是 `missing parent`。缓存里已有该子页时，把它挂到 `Content` 再 `Show`，不重新 Load。

父页 `Close`、`CloseAll`，以及 `BackAsync` 在 `EnableClose` 时，先关掉登记在该父页下的全部子页并 `Release` handle，再处理父页自己。`TryCachePanel` 见到登记过子页的父页直接返回，父页不进 LRU。

`NavigateToAsync` 离开当前页时只 `Hide` 栈顶。子页不被 `Close`，仍挂在父页的 `Content` 下。再回到这扇父页时，离开前显示的那个子页还在。
