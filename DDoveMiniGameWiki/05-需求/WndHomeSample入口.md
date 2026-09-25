---
type: Playbook
title: WndHome Sample入口（需求稿）
description: 薄记录。用法篇才是接口总结。
tags: [程序-前, ddoveui, 需求]
status: deprecated
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

# WndHome Sample入口（需求稿）

用法篇 [WndHome Sample入口](/02-程序-前/WndHomeSample入口.md) 才是接口总结。本篇只留得分。

## 要什么

`WndHome` 是 sample 入口，不再放领取 / 左右换图。制作场景摆 `LoopList`。窗 `Create` 一份 Game 里写死的 `IList`（标题 + 打开动作）。点一行 `NavigateToAsync` 打开对应 sample。

本刀两行：`WndLoopDemo`；领取 / 换图在 **`WndCollectDemo`**。每页一个 `BtnBack`，点了 `BackAsync` 回 `WndHome`。返回无门槛，直调 Kit，不走 Command。

不配表。不加空占位行。不改 Kit 签名。不改 [LoopList](/02-程序-前/LoopList.md) 内核。

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

## 评分

分数记在本节，不另开文件。尺子：开工评分 v10。

| 轮 | 总分 | 过线 | 记录 |
|----|------|------|------|
| 1 | 100 | 是 | 审查 PASS。票为空，八条验收代码已满足。未改用法篇。 |
