---
type: Playbook
title: 创建 LoopList（需求稿）
description: 薄记录。用法篇才是接口总结。得分留在本篇。
tags: [程序-前, ddoveui, 需求]
status: deprecated
generated: { by: human:cjh, at: 2026-09-25T08:45:00Z }
sources:
  - id: use
    resource: /02-程序-前/创建LoopList.md
    title: 创建 LoopList
  - id: create
    resource: ../../DDoveMiniGameClient/Assets/Game/Editor/CreateLoopListWindow.cs
    title: CreateLoopListWindow.cs
---

# 创建 LoopList（需求稿）

用法篇：[创建 LoopList](/02-程序-前/创建LoopList.md)。

[要什么不要](创建LoopList.html)

## 要什么

HotBox 增加「创建 LoopList」，分组「UI 制作」。实现放 `Game.Editor`。点开后是小窗，字段为方向（横 / 竖）、`itemSize`、`spacing`、一个 padding。初值：竖、`100`、间距 `8`、padding `8`。这一个 padding 写进四边。`previewCount` 保持组件默认 `6`，不进窗口。

没选中物体：挂到场景里名为 `UIRoot` 的节点下。场景里没有 `UIRoot` 就停并提示。选中恰好一个带 `RectTransform` 的节点：挂在该节点下。选中多个，或选中的没有 `RectTransform`：停并提示，不创建。

父节点下已有名为 `LoopList` 的子物体时，新物体命名为 `LoopList (1)` 再创建。再重名则继续加序号。

树是 `ScrollRect` + `Viewport` + `Content` + 一份失活的空 `itemTemplate`。列表铺满父节点。方向只开对应滚动轴：竖关水平，横关垂直。Content 竖为顶对齐，横为左对齐。不加 `NodeBind`。不挂 `DemoRow`，不挂 `DefaultTween_V` / `DefaultTween_H`，模板里不放文字。

不改 [LoopList](/02-程序-前/LoopList.md) 运行时。不改 `Game/Build WndLoopDemo Assets` 与 `Game/Build WndLoopHDemo Assets`。

## 不要

- 把这条指令放进 `DDoveUI` 或任何会引用 `Game` 的框架程序集
- 给列表根加 `NodeBind`
- 模板里放样板格子、`Anim`、文字或入场 tween
- 改 `LoopList` 的 `Create`、回收、夹紧或对外签名
- 改两条 Build 样板菜单，或改已导出的 `WndLoopDemo` / `WndLoopHDemo` Prefab
- 窗口里把 padding 拆成四边，或让人选 `previewCount`
- 用 `VerticalLayoutGroup` / `HorizontalLayoutGroup` / `ContentSizeFitter` 排格子
- 选中多个时挂到第一个

## 评分

分数记在本节，不另开文件。尺子：开工评分 v11。

| 轮 | 总分 | 过线 | 记录 |
|----|------|------|------|
| 1 | 100 | 是 | 审查 PASS。需求验收 20、Wiki 同步 20、职责边界 20、范围 15、信任表述 15、规格增量 10。条文不变。 |
