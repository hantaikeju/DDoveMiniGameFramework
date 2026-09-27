---
type: Playbook
title: TMP 中文 Fallback
description: 默认字体仍是 LiberationSans。缺的汉字从动态 Noto Serif CJK SC 的 Fallback 里补。
tags: [程序-前, ddoveui]
status: stable
generated: { by: human:cjh, at: 2026-09-26T23:06:00Z }
verified: { by: human:cjh, at: 2026-09-26T23:06:00Z }
sources:
  - id: noto
    resource: ../../DDoveMiniGameClient/Assets/TextMesh Pro/Resources/Fonts & Materials/NotoSerifCJKsc-Medium SDF.asset
    title: NotoSerifCJKsc-Medium SDF.asset
  - id: font
    resource: ../../DDoveMiniGameClient/Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset
    title: LiberationSans SDF.asset
  - id: tmp
    resource: ../../DDoveMiniGameClient/Assets/TextMesh Pro/Resources/TMP Settings.asset
    title: TMP Settings.asset
  - id: input
    resource: /02-程序-前/DDoveUI切InputSystem.md
    title: DDoveUI 切 Input System
  - id: req
    resource: /05-需求/TMP中文Fallback.md
    title: TMP 中文 Fallback（需求稿）
---

# TMP 中文 Fallback

Concept ID：`/02-程序-前/TMP中文Fallback`。清单：[index_cjh](/02-程序-前/index_cjh.md)。文本仍只留 TMP，见 [DDoveUI 切 Input System](/02-程序-前/DDoveUI切InputSystem.md)。

[三块](TMP中文Fallback.html)

## 干什么

`LiberationSans SDF` 只收拉丁字母和少量标点。缺的汉字从 Fallback 里的动态字体 `NotoSerifCJKsc-Medium SDF` 补上。`TMP Settings` 的默认字体仍是 LiberationSans。已经摆好的文本保持现在的字体引用。`WndHome` 上的「领取/换图」靠这条 Fallback 显示四个汉字。

字库和 SDF 在 `Assets/TextMesh Pro/Resources/Fonts & Materials/`。图集是动态的，采样 `90`，边距 `9`，宽高 `1024`，多图集开着。打包时清掉已经写进图集的字形，运行时按用到的字生成。

## 有哪些接口

- `NotoSerifCJKsc-Medium SDF`（源字库 `NotoSerifCJKsc-Medium.otf`）
- `LiberationSans SDF` 的 Fallback Font Assets
- `TMP Settings` 的默认字体
- `TMP Settings` 的 Fallback Font Assets

## 每个接口干什么

`NotoSerifCJKsc-Medium SDF` 的字面是 Noto Serif CJK SC Medium。源字库 guid 是 `d7708880da592f44a9fbede871ccc681`，SDF guid 是 `5542ae728e650cb4baf2bcf707f4d9d4`。用到的字写入图集，一页满了再开下一页。

`LiberationSans SDF` 的 Fallback 先保留原来的西文 `LiberationSans SDF - Fallback`，再追加这份 Noto。组件上指定的仍是 LiberationSans 时，缺字走这条表。

`TMP Settings` 的默认字体仍指向 `LiberationSans SDF`（guid `8f586378b4e144a9851e7b34d9b748ee`）。新建文本继续用它。

`TMP Settings` 的 Fallback Font Assets 也追加同一份 Noto。当前字体自己的 Fallback 里没有这个字时，再查这份全局表。
