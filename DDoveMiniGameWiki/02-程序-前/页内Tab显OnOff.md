---
type: Playbook
title: 页内 Tab 显 On Off
description: TabOnOff 和 Toggle 在同一物体上。isOn 为真显示 On，为假显示 Off。两套各有一张图和一行字。
tags: [程序-前, ddoveui]
status: stable
generated: { by: human:cjh, at: 2026-09-26T09:26:00Z }
verified: { by: human:cjh, at: 2026-09-26T09:26:00Z }
sources:
  - id: mono
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/Tab/TabOnOff.cs
    title: TabOnOff.cs
  - id: demo
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/WndTabDemo.cs
    title: WndTabDemo.cs
  - id: builder
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/Editor/DDoveUITabSampleBuilder.cs
    title: DDoveUITabSampleBuilder.cs
  - id: tab
    resource: /02-程序-前/页内Tab子页.md
    title: 页内 Tab 子页
  - id: req
    resource: /05-需求/页内Tab显OnOff.md
    title: 页内 Tab 显 On Off（需求稿）
---

# 页内 Tab 显 On Off

Concept ID：`/02-程序-前/页内Tab显OnOff`。清单：[index_cjh](/02-程序-前/index_cjh.md)。子页挂载仍看 [页内 Tab 子页](/02-程序-前/页内Tab子页.md)。

[三块](页内Tab显OnOff.html)

## 干什么

`Game.Mono.TabOnOff` 和 `Toggle` 挂在同一个按钮上。它引用子节点 `On`、`Off`。两个根各自有一张 `Image` 和一行字。`isOn` 为真时显示 `On`、藏起 `Off`。为假时反过来。图和字在场景里摆好，这个组件不改颜色，也不换 Sprite。

样例 `WndTabDemo` 的 `BtnTabA`、`BtnTabB` 挂着它。`On` 底图白、字 `0.15`；`Off` 底图灰 `0.55`、字白。Sprite 留空。打开时用 `SetIsOnWithoutNotify` 把 `BtnTabA` 设为真、`BtnTabB` 设为假，然后调用 `Refresh`，并只打开一次 `WndTabPageA`。点另一个会打开对应子页。再点已经是真的那个不再次打开。返回仍是 `Button`。

窗口只去掉自己的那一份回调，不清空 `Toggle` 上的全部监听。子场景 `Excluded_Top` 里的 Tab 拷贝同样有 `On` / `Off`，不进子页 Prefab。

## 有哪些接口

- `Game.Mono.TabOnOff`（字段 `onRoot`、`offRoot`）
- `TabOnOff.Refresh()`
- `DDoveUIKit.OpenChildAsync<TChild>(Component parent, IDDoveUIPanelData data = null)`

## 每个接口干什么

`TabOnOff` 放在 `Assets/Game/Mono/Tab/`。`onRoot`、`offRoot` 拖到按钮下的 `On`、`Off`。启用时按当前 `Toggle.isOn` 刷一次显隐，并听 `onValueChanged`。关掉时去掉这份监听。

`Refresh` 再按当前 `isOn` 刷一次。`SetIsOnWithoutNotify` 不会进 `onValueChanged`，所以静默改完选中之后要调用它。

`OpenChildAsync` 只在当前子页换了的时候调用。`WndTabPageA` 记为 1，`WndTabPageB` 记为 2。已经是这个编号就返回。子页仍挂在父页的 `Content` 下。
