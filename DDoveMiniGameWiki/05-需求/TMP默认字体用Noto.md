---
type: Playbook
title: TMP 默认字体用 Noto（需求稿）
description: 薄记录。用法篇才是接口总结。得分留在本篇。
tags: [程序-前, ddoveui, 需求]
status: deprecated
generated: { by: human:cjh, at: 2026-09-27T07:28:00Z }
sources:
  - id: use
    resource: /02-程序-前/TMP默认字体用Noto.md
    title: TMP 默认字体用 Noto
  - id: noto
    resource: ../../DDoveMiniGameClient/Assets/TextMesh Pro/Resources/Fonts & Materials/NotoSerifCJKsc-Medium SDF.asset
    title: NotoSerifCJKsc-Medium SDF.asset
  - id: tmp
    resource: ../../DDoveMiniGameClient/Assets/TextMesh Pro/Resources/TMP Settings.asset
    title: TMP Settings.asset
  - id: font
    resource: ../../DDoveMiniGameClient/Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset
    title: LiberationSans SDF.asset
  - id: home
    resource: ../../DDoveMiniGameClient/Assets/GameRes/UI/Start/WndHome.prefab
    title: WndHome.prefab
---

# TMP 默认字体用 Noto（需求稿）

用法篇：[TMP 默认字体用 Noto](/02-程序-前/TMP默认字体用Noto.md)。

[要什么不要](TMP默认字体用Noto.html)

## 要什么

`TMP Settings` 的默认字体已是 `NotoSerifCJKsc-Medium SDF`（guid `5542ae728e650cb4baf2bcf707f4d9d4`）。新建文本走这份默认字体。`Assets/GameRes/UI/Start/` 下列出的 Prefab，以及 `Assets/GameResExcluded/CreateUIScenes/Start/` 下同名场景，文本字体和材质已改挂这份 Noto，不再出现 LiberationSans SDF 的 guid。

下面这些文件里，凡是 `m_fontAsset` 指向 LiberationSans SDF（guid `8f586378b4e144a9851e7b34d9b748ee`）的，字体改挂 Noto，`m_sharedMaterial` 改挂同一份资源里的 Atlas Material（fileID `-2056736997786491373`）。

Prefab：`WndHome`、`WndLoopDemo`、`WndLoopHDemo`、`WndCollectDemo`、`WndTabDemo`、`WndTabPageA`、`WndTabPageB`，都在 `Assets/GameRes/UI/Start/`。

制作场景：同名 `.unity`，都在 `Assets/GameResExcluded/CreateUIScenes/Start/`。

`LiberationSans SDF` 的 Fallback 继续留着这份 Noto。`TMP Settings` 的 Fallback Font Assets 也继续留着它。动态图集、采样 `90`、边距 `9`、`1024`、多图集、Clear Dynamic Data On Build 都不改。

## 不要

- 只改 `TMP Settings`，Prefab 和制作场景继续挂 LiberationSans
- 拿掉 LiberationSans 或 `TMP Settings` 上的 Noto Fallback
- 改 `LoopList` 里格子的 `SetActive`
- 改样板 Builder、`WndHome.cs`
- 再拷像素字，或把图集改成静态

## 评分

分数记在本节，不另开文件。尺子：开工评分 v11。

| 轮 | 总分 | 过线 | 记录 |
|----|------|------|------|
| 1 | 100 | 是 | 审查 PASS。需求验收 20、Wiki 同步 20、职责边界 20、范围 15、信任表述 15、规格增量 10。条文不变。 |
