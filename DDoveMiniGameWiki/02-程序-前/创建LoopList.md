---
type: Playbook
title: 创建 LoopList
description: HotBox「创建 LoopList」在制作场景的 UIRoot 下挂一条空列表。窗口选横竖、itemSize、spacing 和一个四边相同的 padding。创建后自己拖位置。
tags: [程序-前, ddoveui]
status: stable
generated: { by: human:cjh, at: 2026-09-25T08:45:00Z }
verified: { by: human:cjh, at: 2026-09-25T17:58:00Z }
sources:
  - id: create
    resource: ../../DDoveMiniGameClient/Assets/Game/Editor/CreateLoopListWindow.cs
    title: CreateLoopListWindow.cs
  - id: loop
    resource: /02-程序-前/LoopList.md
    title: LoopList
  - id: req
    resource: /05-需求/创建LoopList.md
    title: 创建 LoopList（需求稿）
---

# 创建 LoopList

Concept ID：`/02-程序-前/创建LoopList`。清单：[index_cjh](/02-程序-前/index_cjh.md)。运行时列表看 [LoopList](/02-程序-前/LoopList.md)。

[三块](创建LoopList.html)

## 干什么

HotBox「UI 制作」里的「创建 LoopList」在当前制作场景挂一条空 `LoopList`。点开小窗，选方向、`itemSize`、`spacing` 和一个 padding。确认后列表挂在 `UIRoot` 下并铺满它，然后选中这条新列表，自己再拖位置。

不看当前选中了什么。场景里没有名为 `UIRoot` 的节点就停并提示。`previewCount` 保持组件默认 `6`。

## 有哪些接口

- `CreateLoopListWindow.Open`
- 窗口字段：方向、`itemSize`、`spacing`、`padding`

## 每个接口干什么

`CreateLoopListWindow.Open` 挂着 `[DDoveHotboxEntry("创建 LoopList", "UI 制作", ...)]`。它打开小窗，不直接造物体。

方向是 `LoopDirection`。初值竖。竖只开垂直滚动，Content 顶对齐。横只开水平滚动，Content 左对齐。

`itemSize` 初值 `100`。竖是条高，横是条宽。`spacing` 初值 `8`。`padding` 是一个整数，初值 `8`，确认后写入四边。

没有 `UIRoot` 时不创建，并弹出提示。父节点下已有名为 `LoopList` 的子物体时，新物体名为 `LoopList (1)`，再重名继续加序号。创建成功后选中新列表。

造出的树是 `ScrollRect` + `Viewport` + `Content` + 一份失活的空 `itemTemplate`。列表铺满父节点。没有 `NodeBind`、样板格子、文字和入场 tween。
