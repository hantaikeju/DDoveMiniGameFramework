---
type: Playbook
title: TMP 默认字体用 Noto
description: TMP Settings 的默认字体是动态 Noto。已有 Start 文本的字体和材质都挂这份。两处 Fallback 留着。
tags: [程序-前, ddoveui]
status: stable
generated: { by: human:cjh, at: 2026-09-27T08:10:00Z }
verified: { by: human:cjh, at: 2026-09-27T08:10:00Z }
sources:
  - id: req
    resource: /05-需求/TMP默认字体用Noto.md
    title: TMP 默认字体用 Noto（需求稿）
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

# TMP 默认字体用 Noto

Concept ID：`/02-程序-前/TMP默认字体用Noto`。清单：[index_cjh](/02-程序-前/index_cjh.md)。

[三块](TMP默认字体用Noto.html)

## 干什么

`TMP Settings` 的默认字体是 `NotoSerifCJKsc-Medium SDF`（guid `5542ae728e650cb4baf2bcf707f4d9d4`）。新建文本走这份字体。`Assets/GameRes/UI/Start/` 里的 `WndHome`、`WndLoopDemo`、`WndLoopHDemo`、`WndCollectDemo`、`WndTabDemo`、`WndTabPageA`、`WndTabPageB`，以及 `Assets/GameResExcluded/CreateUIScenes/Start/` 下同名场景，文本的字体和材质都指向这份 Noto。汉字写进这份字体自己的动态图集。

`LiberationSans SDF` 的 Fallback 和 `TMP Settings` 的 Fallback Font Assets 仍留着这份 Noto。动态图集、采样 `90`、边距 `9`、`1024`、多图集、Clear Dynamic Data On Build 保持原样。

## 有哪些接口

- `NotoSerifCJKsc-Medium SDF`
- `TMP Settings` 的默认字体
- 已有文本的字体和材质
- `LiberationSans SDF` 的 Fallback Font Assets
- `TMP Settings` 的 Fallback Font Assets

## 每个接口干什么

`NotoSerifCJKsc-Medium SDF` 的字面是 Noto Serif CJK SC Medium。SDF guid 是 `5542ae728e650cb4baf2bcf707f4d9d4`。图集是动态的。用到的字在这次生成文字时写入图集，一页满了再开下一页。Atlas Material 的 fileID 是 `-2056736997786491373`。

`TMP Settings` 的默认字体指向这份 Noto。没有单独指定字体的新建文本用它。

已有文本：上面列出的 Prefab 和制作场景里，字体指向这份 Noto，材质指向同一份资源里的 Atlas Material。这些文本不再引用 LiberationSans SDF（guid `8f586378b4e144a9851e7b34d9b748ee`）。

`LiberationSans SDF` 的 Fallback 仍先是西文 `LiberationSans SDF - Fallback`，再是这份 Noto。组件上仍指定 LiberationSans 时，缺字走这条表。

`TMP Settings` 的 Fallback Font Assets 也仍有这份 Noto。当前字体自己的 Fallback 里没有这个字时，再查这份全局表。
