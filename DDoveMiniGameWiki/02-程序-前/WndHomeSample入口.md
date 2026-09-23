---
type: Playbook
title: WndHome Sample入口
description: 已按稿落地。WndHome 是 LoopList 入口；Game 写死 IList 两行 Navigate。Launch 入栈。WndCollectDemo / WndLoopDemo 各有 BtnBack。
tags: [程序-前, ddoveui]
status: draft
generated: { by: human:cjh, at: 2026-09-19T18:12:00Z }
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
  - id: loop
    resource: /02-程序-前/LoopList.md
    title: LoopList
  - id: play
    resource: /02-程序-前/Play到WndHome.md
    title: Play 到 WndHome
  - id: kit
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/DDoveUIKit.cs
    title: DDoveUIKit.cs
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
  - id: launch
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/GameLaunch.cs
    title: GameLaunch.cs
  - id: home-menu
    resource: ../../DDoveMiniGameClient/Assets/Game/Editor/OpenHomeEntryMenu.cs
    title: OpenHomeEntryMenu.cs
  - id: collect-menu
    resource: ../../DDoveMiniGameClient/Assets/Game/Editor/OpenCollectDemoMenu.cs
    title: OpenCollectDemoMenu.cs
  - id: sample
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/Editor/DDoveUISampleBuilder.cs
    title: DDoveUISampleBuilder.cs
---

# WndHome Sample入口

Concept ID：`/02-程序-前/WndHomeSample入口`。清单：[index_cjh](/02-程序-前/index_cjh.md)。制作导出仍看 [DDoveUI](/02-程序-前/DDoveUI.md)。列表挂件看 [LoopList](/02-程序-前/LoopList.md)。开窗门槛看 [UI 业务封装](/02-程序-前/UI业务封装.md)。Play 链看 [Play 到 WndHome](/02-程序-前/Play到WndHome.md)。

对照：[DDoveAtlas](/02-程序-前/DDoveAtlas.md)、[PrimeTween](/02-程序-前/PrimeTween.md)。过期对照（本刀不改正文）：[LoopList](/02-程序-前/LoopList.md) 仍写「不改 `WndHome` / `GameLaunch`」、只靠菜单打开样板；[DDoveUI](/02-程序-前/DDoveUI.md) 仍把 `WndHome` 当领取 / 换图样板，Launch 只 `OpenAsync`；[Play 到 WndHome](/02-程序-前/Play到WndHome.md) / [UI 业务封装](/02-程序-前/UI业务封装.md) 仍写 `OpenAsync<WndHome>`。

权威实现：[WndHome.cs](../../DDoveMiniGameClient/Assets/Game/UI/Start/WndHome.cs)。结论与代码冲突以当前代码为准。

`OpenAsync` **不入** `PanelStack`。`GameLaunch` 已 `NavigateToAsync<WndHome>`，Home 在栈底，`BackAsync` 才能 `Show` 回来。

## 要什么

`WndHome` 是 sample 入口，不再放领取 / 左右换图。制作场景摆 `LoopList`。窗 `Create` 一份 Game 里写死的 `IList`（标题 + 打开动作）。点一行 `NavigateToAsync` 打开对应 sample。

本刀两行：`WndLoopDemo`；领取 / 换图在 **`WndCollectDemo`**。每页一个 `BtnBack`，点了 `BackAsync` 回 `WndHome`。返回无门槛，直调 Kit，不走 Command。

不配表。不加空占位行。不改 Kit 签名。不改 [LoopList](/02-程序-前/LoopList.md) 内核。

## 落点

| | 放哪 |
|--|------|
| 入口窗 | [`WndHome`](../../DDoveMiniGameClient/Assets/Game/UI/Start/WndHome.cs)。`GetComponentInChildren<LoopList>()` 后 `Create` 两行 |
| 入口数据 | [`HomeEntryRowData`](../../DDoveMiniGameClient/Assets/Game/UI/Start/HomeEntryRow.cs)：`Title` + `Action Open`。OnOpen 里 `new List`，不进 Luban，不进 Model |
| 入口格子 | [`HomeEntryRow`](../../DDoveMiniGameClient/Assets/Game/UI/Start/HomeEntryRow.cs) 实现 `ILoopItem<HomeEntryRowData>`。`Bind` 写标题；点击调 `Open` |
| 领取样板 | [`WndCollectDemo`](../../DDoveMiniGameClient/Assets/Game/UI/Start/WndCollectDemo.cs)。领取走已有 `CollectDemoItemCommand`；左右图 + 脉冲与旧 `WndHome` 相同 |
| 列表样板 | [`WndLoopDemo`](../../DDoveMiniGameClient/Assets/Game/UI/Start/WndLoopDemo.cs)。加了 `BtnBack` |
| 返回 | `FindNode("BtnBack")` + `AddClick` → `DDoveUIKit.BackAsync()` |
| Launch | [`GameLaunch`](../../DDoveMiniGameClient/Assets/Game/Mono/GameLaunch.cs) `NavigateToAsync<WndHome>` |
| 入口构建 | [`OpenHomeEntryMenu`](../../DDoveMiniGameClient/Assets/Game/Editor/OpenHomeEntryMenu.cs)：`Game/Build WndHome Assets` 剥领取树、摆 LoopList + `HomeEntryRow`、导出 |
| 领取构建 | [`OpenCollectDemoMenu`](../../DDoveMiniGameClient/Assets/Game/Editor/OpenCollectDemoMenu.cs) |
| 框架样板 | [`DDoveUISampleBuilder`](../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/Editor/DDoveUISampleBuilder.cs) 只去掉 `TxtBag` / `BtnCollect` / `ImgLeft` / `ImgRight`，不在 DDoveUI.Editor 里拼 LoopList |

调试菜单可留：`Game/Open WndHome`（Navigate）、`Game/Open WndLoopDemo` / `Game/Open WndCollectDemo`（OpenAsync）。主路径是 Play → Home 列表。

改布局回制作场景再导出。`Start` 收集器 AddressByFileName 能 Load `WndHome` / `WndLoopDemo` / `WndCollectDemo`。

## 不要

- 配表驱动 sample 列表
- 本刀加空占位行
- `OpenWndCommand` / 只转发 Kit 的 System
- 改 [DDoveUI](/02-程序-前/DDoveUI.md) Kit 签名
- 改 [LoopList](/02-程序-前/LoopList.md) 内核
- Launch 用 `OpenAsync<WndHome>`
- 点条目 `OpenAsync` 再开一次 `WndHome` 当返回
- 领取 / 换图留在 `WndHome`
- DDoveUI.Editor 引用 Game、在框架里拼 `HomeEntryRow`

## 还没有

更多 sample、配表、系统返回键 / 手柄返回。未实现前不要当已有。

## 验收

1. Play：Boot → Launch → 看见 `WndHome`，上面是 `LoopList` 入口，没有领取钮 / 左右换图。
2. 列表数据是 Game 里写死的 `IList`，至少两行（LoopList、领取/换图）。没有配表，没有空占位行。
3. 点 LoopList 行打开 `WndLoopDemo`；点领取/换图行打开 `WndCollectDemo`。走 `NavigateToAsync`，不走 `OpenAsync`。
4. 两页都有 `BtnBack`；点了 `BackAsync` 回到 `WndHome`（Home 被 `Show`，不是再 `OpenAsync` 一张）。
5. `GameLaunch` 用 `NavigateToAsync<WndHome>`。
6. `WndCollectDemo` 仍能领取、左右换图（原 `WndHome` 行为）。
7. 窗与按钮来自制作场景导出；Play 里不 `new` 控件。`Start` 组能 Load `WndHome` / `WndLoopDemo` / `WndCollectDemo`。
8. 没有改 Kit 签名；没有为开窗加 Command。
