---
type: Playbook
title: TMP 默认字体用 Noto（需求稿）
description: 已落地。TMP Settings 默认字体和已有文本都挂动态 Noto，两处 Fallback 留着。
tags: [程序-前, ddoveui, 需求]
status: draft
generated: { by: human:cjh, at: 2026-09-27T07:28:00Z }
sources:
  - id: fallback
    resource: /02-程序-前/TMP中文Fallback.md
    title: TMP 中文 Fallback
  - id: input
    resource: /02-程序-前/DDoveUI切InputSystem.md
    title: DDoveUI 切 Input System
  - id: tmp
    resource: ../../DDoveMiniGameClient/Assets/TextMesh Pro/Resources/TMP Settings.asset
    title: TMP Settings.asset
  - id: noto
    resource: ../../DDoveMiniGameClient/Assets/TextMesh Pro/Resources/Fonts & Materials/NotoSerifCJKsc-Medium SDF.asset
    title: NotoSerifCJKsc-Medium SDF.asset
  - id: font
    resource: ../../DDoveMiniGameClient/Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset
    title: LiberationSans SDF.asset
  - id: home
    resource: ../../DDoveMiniGameClient/Assets/GameRes/UI/Start/WndHome.prefab
    title: WndHome.prefab
---

# TMP 默认字体用 Noto（需求稿）

Concept ID：`/05-需求/TMP默认字体用Noto`。清单：[index_cjh](/05-需求/index_cjh.md)。

对照：[TMP 中文 Fallback](/02-程序-前/TMP中文Fallback.md) 过期、[DDoveUI 切 Input System](/02-程序-前/DDoveUI切InputSystem.md) 过期。两篇仍写默认字体是 LiberationSans。现稿已把默认字体和已有文本改挂 Noto。汉字在这份字体自己的网格里写入图集。

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

## 验收

1. `TMP Settings` 的默认字体是 `NotoSerifCJKsc-Medium SDF`。
2. 上面列出的 Prefab 和制作场景里，不再有文本挂 LiberationSans SDF。这些文本的字体和材质都指向 Noto 那份资源。
3. LiberationSans 与 `TMP Settings` 的 Fallback 仍有这份 Noto。
4. Play 打开 `WndHome`，「领取/换图」第一次就显示，不用再把物体关开。

## 落点

| | 放哪 |
|--|------|
| 默认字体 | `Assets/TextMesh Pro/Resources/TMP Settings.asset` |
| 已有文本 | `Assets/GameRes/UI/Start/` 下列出的 Prefab，以及 `Assets/GameResExcluded/CreateUIScenes/Start/` 下同名场景 |
| 留下的 Fallback | `LiberationSans SDF.asset` 与 `TMP Settings.asset` |

## 评分

分数记在本节，不另开文件。尺子：开工评分 v11。

| 轮 | 总分 | 过线 | 记录 |
|----|------|------|------|
| 1 | 100 | 是 | 审查 PASS。需求验收 20、Wiki 同步 20、职责边界 20、范围 15、信任表述 15、规格增量 10。条文不变。 |
