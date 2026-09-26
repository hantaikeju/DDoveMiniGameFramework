---
type: Playbook
title: 页内 Tab 显 On Off（需求稿）
description: 薄记录。用法篇才是接口总结。得分留在本篇。
tags: [程序-前, ddoveui, 需求]
status: deprecated
generated: { by: human:cjh, at: 2026-09-26T06:03:00Z }
sources:
  - id: use
    resource: /02-程序-前/页内Tab显OnOff.md
    title: 页内 Tab 显 On Off
  - id: mono
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/Tab/TabOnOff.cs
    title: TabOnOff.cs
---

# 页内 Tab 显 On Off（需求稿）

用法篇：[页内 Tab 显 On Off](/02-程序-前/页内Tab显OnOff.md)。

[要什么不要](页内Tab显OnOff.html)

## 要什么

每个 Tab 按钮挂一个 Mono，放在 `Assets/Game/Mono/` 下、与 `Scroll` 并列的目录里，命名空间 `Game.Mono`。不进 DDoveUI。它和 `Toggle` 挂在同一个物体上，引用该按钮下的 `On`、`Off` 两个根。两个根各自有一张 `Image` 和一行字。

`Toggle.isOn` 为真时显示 `On`、藏起 `Off`。为假时显示 `Off`、藏起 `On`。图和字在场景里摆好。Mono 只切换这两套的显示，不改它们的颜色，也不换 Sprite。启用时按当前 `isOn` 刷一次，这样 `SetIsOnWithoutNotify` 之后外观也是对的。

样例 `WndTabDemo` 的 `BtnTabA`、`BtnTabB` 挂上它。`On` 底图白、字深色 `0.15`；`Off` 底图灰 `0.55`、字白。两套字都是该 Tab 的名字。两张 `Image` 的 Sprite 留空，以后在 `Image` 上换图。`WndTabDemo` 不再写 `Image.color`。打开时仍用 `SetIsOnWithoutNotify` 把 `BtnTabA` 设为真、`BtnTabB` 设为假，并只打开一次 `WndTabPageA`。

`ToggleGroup` 仍在父页上，`allowSwitchOff` 仍关掉。子页仍走 `OpenChildAsync`。点另一个：它变成真并显示 `On`，原来的变成假并显示 `Off`，并打开对应子页。再点已经是真的那个：保持，不再次打开。返回仍是 `Button`，仍走 `BackAsync`。

子场景 `Excluded_Top` 里的 Tab 拷贝同样摆 `On` / `Off`，仍不进子页 Prefab。

## 不要

- 继续用 `ApplyTabColor` 改底图颜色来表示选中
- 用 `Toggle` 的变色或 Sprite Swap 表示选中
- 把这个 Mono 放进 DDoveUI
- 给样例另做 Sprite 文件
- `On` 和 `Off` 同时显示，或同时藏起
- 再点已经是真的 Tab 时再次 `OpenChildAsync`
- 改 `OpenChildAsync`、`GameLaunch`，或其它窗

## 评分

分数记在本节，不另开文件。尺子：开工评分 v11（`stable`）。

| 轮 | 总分 | 过线 | 记录 |
|----|------|------|------|
| 1 | 100 | 是 | 审查首轮验收 3 不满足。补丁改为静默改 isOn 后调用 Refresh，并不再清空全部监听。复审 PASS。需求验收 20、Wiki 同步 20、职责边界 20、范围 15、信任表述 15、规格增量 10。条文不变。 |
