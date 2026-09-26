---
type: Playbook
title: 页内 Tab 用 Toggle
description: WndTabDemo 两个 Tab 是同一 ToggleGroup 的 Toggle。选中外观由 TabOnOff 显示 On 或 Off。再点已选中的不再次打开子页。
tags: [程序-前, ddoveui]
status: stable
generated: { by: human:cjh, at: 2026-09-26T04:06:00Z }
verified: { by: human:cjh, at: 2026-09-26T09:46:00Z }
sources:
  - id: demo
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/WndTabDemo.cs
    title: WndTabDemo.cs
  - id: onoff
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/Tab/TabOnOff.cs
    title: TabOnOff.cs
  - id: look
    resource: /02-程序-前/页内Tab显OnOff.md
    title: 页内 Tab 显 On Off
  - id: tab
    resource: /02-程序-前/页内Tab子页.md
    title: 页内 Tab 子页
  - id: req
    resource: /05-需求/页内Tab用Toggle.md
    title: 页内 Tab 用 Toggle（需求稿）
---

# 页内 Tab 用 Toggle

Concept ID：`/02-程序-前/页内Tab用Toggle`。清单：[index_cjh](/02-程序-前/index_cjh.md)。子页挂载仍看 [页内 Tab 子页](/02-程序-前/页内Tab子页.md)。选中外观看 [页内 Tab 显 On Off](/02-程序-前/页内Tab显OnOff.md)。

[三块](页内Tab用Toggle.html)

## 干什么

`WndTabDemo` 的 `BtnTabA`、`BtnTabB` 是同一个 `ToggleGroup` 上的 `Toggle`，`allowSwitchOff` 关掉。`isOn` 为真是 1，为假是 0。组里始终只有一个是 1。

选中外观由 `TabOnOff` 显示 `On` 或 `Off`。本篇不再改 `Image.color`。返回仍是 `Button`，仍走 `BackAsync`。

打开时用 `SetIsOnWithoutNotify` 把 `BtnTabA` 设为 1、`BtnTabB` 设为 0，再 `Refresh`，并只打开一次 `WndTabPageA`。点另一个：它变成 1，原来的变成 0，并 `OpenChildAsync` 打开对应子页。再点已经是 1 的那个：保持 1，不再次打开。

## 有哪些接口

- `WndTabDemo` 上的 `ToggleGroup`（`allowSwitchOff` 为关）
- `BtnTabA`、`BtnTabB` 的 `Toggle.isOn`
- `TabOnOff.Refresh()`
- `DDoveUIKit.OpenChildAsync<TChild>(Component parent, IDDoveUIPanelData data = null)`

## 每个接口干什么

`ToggleGroup` 挂在 `WndTabDemo` 的根上。两个 Tab 都指向它。关掉 `allowSwitchOff` 之后，组里不能一个都不选。窗口只去掉自己的那一份回调，不清空 `Toggle` 上的全部监听。

`Toggle.isOn` 为真是当前子页。打开父页时用 `SetIsOnWithoutNotify` 写入这对值，避免回调把 `WndTabPageA` 再打开一次。某个 Tab 变成真时，另一个若仍为真就 `SetIsOnWithoutNotify(false)`。静默改完之后调用 `Refresh`。`On` / `Off` 怎么显示见 [页内 Tab 显 On Off](/02-程序-前/页内Tab显OnOff.md)。

`OpenChildAsync` 只在当前子页换了的时候调用。`WndTabPageA` 记为 1，`WndTabPageB` 记为 2。已经是这个编号就返回。子页仍挂在父页的 `Content` 下。
