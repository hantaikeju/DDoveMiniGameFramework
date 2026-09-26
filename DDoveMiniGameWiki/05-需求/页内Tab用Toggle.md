---
type: Playbook
title: 页内 Tab 用 Toggle（需求稿）
description: 薄记录。用法篇才是接口总结。得分留在本篇。
tags: [程序-前, ddoveui, 需求]
status: deprecated
generated: { by: human:cjh, at: 2026-09-26T03:27:00Z }
sources:
  - id: use
    resource: /02-程序-前/页内Tab用Toggle.md
    title: 页内 Tab 用 Toggle
  - id: demo
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/WndTabDemo.cs
    title: WndTabDemo.cs
---

# 页内 Tab 用 Toggle（需求稿）

用法篇：[页内 Tab 用 Toggle](/02-程序-前/页内Tab用Toggle.md)。

[要什么不要](页内Tab用Toggle.html)

## 要什么

`BtnTabA`、`BtnTabB` 换成 Unity `Toggle`，挂在同一个 `ToggleGroup` 上，`allowSwitchOff` 关掉。节点名不变。`isOn` 为真是 1，为假是 0。组里始终只有一个是 1。

高亮用底图颜色：1 是白色，0 是灰色 `0.55`。不靠勾选标记。两个 Tab 的过渡是 `None`，底图颜色不被自带变色盖掉。返回仍是 `Button`，仍走 `BackAsync`，仍用原来的变色。子页仍走 `OpenChildAsync`。打开时用 `SetIsOnWithoutNotify` 设好 0 / 1，记下当前子页，已经是这一页就不再次打开。

父页打开时 `BtnTabA` 是 1，并打开 `WndTabPageA`，这一次只打开一回。点另一个：它变成 1，原来的变成 0，并打开对应子页。再点已经是 1 的那个：保持 1，不再次打开子页。

子场景 `Excluded_Top` 里的 Tab 拷贝改成同样的底色对照，仍不进子页 Prefab。

## 不要

- 两个 Tab 继续用 `Button`
- 两个都是 1，或两个都是 0
- 用勾选标记表示选中
- 再点已经是 1 的 Tab 时再次 `OpenChildAsync`
- 改 `OpenChildAsync`、`GameLaunch`，或其它窗
- 另做一套可复用的 Tab 组件

## 评分

分数记在本节，不另开文件。尺子：开工评分 v11（`stable`）。

| 轮 | 总分 | 过线 | 记录 |
|----|------|------|------|
| 1 | 100 | 是 | 审查首轮验收 2 不满足。补丁把两个 Tab 过渡改为 None 后复审 PASS。需求验收 20、Wiki 同步 20、职责边界 20、范围 15、信任表述 15、规格增量 10。条文不变。 |
