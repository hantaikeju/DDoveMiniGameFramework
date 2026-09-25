---
type: Playbook
title: WndHome Sample入口
description: WndHome 是三行入口。HomeEntryRowData.Title/Open；HomeEntryRow.Bind/Recycle；NavigateToAsync 打开三页；三页 BtnBack 走 BackAsync；Launch 为 NavigateToAsync<WndHome>。
tags: [程序-前, ddoveui]
status: stable
generated: { by: human:cjh, at: 2026-09-19T18:12:00Z }
verified: { by: human:cjh, at: 2026-09-24T19:40:00Z }
sources:
  - id: home
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/WndHome.cs
    title: WndHome.cs
  - id: home-row
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/HomeEntryRow.cs
    title: HomeEntryRow.cs
  - id: collect
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/WndCollectDemo.cs
    title: WndCollectDemo.cs
  - id: loop-wnd
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/WndLoopDemo.cs
    title: WndLoopDemo.cs
  - id: looph
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/WndLoopHDemo.cs
    title: WndLoopHDemo.cs
  - id: launch
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/GameLaunch.cs
    title: GameLaunch.cs
  - id: kit
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/DDoveUIKit.cs
    title: DDoveUIKit.cs
---

# WndHome Sample入口

Concept ID：`/02-程序-前/WndHomeSample入口`。

## 干什么

`WndHome` 是三行入口。`OnOpen` 写死一份 `IList<HomeEntryRowData>`，交给子节点 `LoopList.Create`。点一行调用该行的 `Open`。

三行标题与打开目标：

| Title | Open |
|-------|------|
| `LoopList` | `NavigateToAsync<WndLoopDemo>` |
| `领取/换图` | `NavigateToAsync<WndCollectDemo>` |
| `LoopH` | `NavigateToAsync<WndLoopHDemo>` |

`WndLoopDemo`、`WndCollectDemo`、`WndLoopHDemo` 的 `BtnBack` 都调 `BackAsync`。`GameLaunch` 在装表、读档、图集 / 音频 / UI 初始化之后，用 `NavigateToAsync<WndHome>` 打开入口。

## 有哪些接口

- `HomeEntryRowData.Title`
- `HomeEntryRowData.Open`
- `HomeEntryRow.Bind`
- `HomeEntryRow.Recycle`
- `DDoveUIKit.NavigateToAsync<T>`
- `DDoveUIKit.BackAsync`

## 每个接口干什么

`HomeEntryRowData.Title` 是这一行要显示的标题字符串。`WndHome` 三行分别写成 `LoopList`、`领取/换图`、`LoopH`。

`HomeEntryRowData.Open` 是点这一行时要跑的 `Action`。三行各自 `Forget` 掉一次 `NavigateToAsync`：`WndLoopDemo`、`WndCollectDemo`、`WndLoopHDemo`。

`HomeEntryRow.Bind(HomeEntryRowData data, int index)` 记下当前行数据，并把 `Title` 写到格子上的 `TMP_Text`。数据为空时文本清空。`index` 不参与显示。

`HomeEntryRow.Recycle()` 把记下的行数据清成 `null`，避免格子复用后还点到上一行的 `Open`。

`DDoveUIKit.NavigateToAsync<T>` 按面板类型名入栈并打开。栈顶已是该面板则直接返回；栈里已有则 `BackToAsync<T>`；否则先 `Hide` 当前顶，再 `OpenAsync<T>` 并压栈。本功能用它打开 `WndHome`（Launch）以及上面三页。

`DDoveUIKit.BackAsync` 弹出栈顶并 `Hide`。栈里还有下一张时 `Show` 它。三页的 `BtnBack` 经 `AddClick` 调用它，从而回到 `WndHome`。
