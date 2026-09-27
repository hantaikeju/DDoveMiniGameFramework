---
type: Playbook
title: TMP 中文 Fallback（需求稿）
description: 薄记录。用法篇才是接口总结。得分留在本篇。
tags: [程序-前, ddoveui, 需求]
status: deprecated
generated: { by: human:cjh, at: 2026-09-26T22:25:00Z }
sources:
  - id: use
    resource: /02-程序-前/TMP中文Fallback.md
    title: TMP 中文 Fallback
  - id: noto
    resource: ../../DDoveMiniGameClient/Assets/TextMesh Pro/Resources/Fonts & Materials/NotoSerifCJKsc-Medium SDF.asset
    title: NotoSerifCJKsc-Medium SDF.asset
  - id: font
    resource: ../../DDoveMiniGameClient/Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset
    title: LiberationSans SDF.asset
  - id: tmp
    resource: ../../DDoveMiniGameClient/Assets/TextMesh Pro/Resources/TMP Settings.asset
    title: TMP Settings.asset
---

# TMP 中文 Fallback（需求稿）

用法篇：[TMP 中文 Fallback](/02-程序-前/TMP中文Fallback.md)。

[要什么不要](TMP中文Fallback.html)

## 要什么

`LiberationSans SDF` 只收拉丁字母和少量标点。中文由动态字体 `NotoSerifCJKsc-Medium SDF` 从 Fallback 补上。文件已在 `Assets/TextMesh Pro/Resources/Fonts & Materials/`：otf 与 SDF 的 guid 分别是 `d7708880da592f44a9fbede871ccc681`、`5542ae728e650cb4baf2bcf707f4d9d4`。SDF 为动态图集，采样 `90`，边距 `9`，`1024`，多图集开着，Clear Dynamic Data On Build 开着。

从同级工程 MeowPantry 的 `MeowPantryClient/Assets/GameRes/Fonts/` 拷四份进本库 `Assets/TextMesh Pro/Resources/Fonts & Materials/`：

- `NotoSerifCJKsc-Medium.otf` 与它的 `.meta`（guid `d7708880da592f44a9fbede871ccc681`）
- `NotoSerifCJKsc-Medium SDF.asset` 与它的 `.meta`（guid `5542ae728e650cb4baf2bcf707f4d9d4`）

带 `.meta` 拷，SDF 里的源字库引用保持指向这份 otf。字面是 Noto Serif CJK SC Medium。

拷入后改这份 SDF：图集保持动态，采样 `90`，边距 `9`，宽高 `1024`。打开多图集，一页满了再开下一页。打开 Clear Dynamic Data On Build，打包时清掉已经写进图集的字形，运行时按用到的字生成。

挂两处，都是追加，留下 LiberationSans 自己那条西文 Fallback：

- `LiberationSans SDF` 的 Fallback Font Assets 加上这份 Noto
- `TMP Settings` 的 Fallback Font Assets 加上同一份

`TMP Settings` 的默认字体仍是 `LiberationSans SDF`。已经摆好的文本保持现在的字体引用。`WndHome` 上的「领取/换图」靠这条 Fallback 显示出四个汉字。

当前收集路径是场景、`UI/Start`、图集和配表。这份字放在 TextMesh Pro 的 Resources 里，由上述引用带进播放器包，不另做收集项。

## 不要

- 把 `TMP Settings` 的默认字体改成这份 Noto
- 逐个改 Prefab、场景里 TextMesh Pro 的字体引用
- 拷 `fusion-pixel-12px-proportional`
- 放进 `Assets/GameRes`，或加进 BundleCollector 收集路径
- 做成静态图集，或关掉多图集
- 改样板 Builder、`WndHome`、`LoopList`
- 运行时用 DDoveRes 或 YooAsset 按名 Load 这份字体

## 评分

分数记在本节，不另开文件。尺子：开工评分 v11。

| 轮 | 总分 | 过线 | 记录 |
|----|------|------|------|
| 1 | 100 | 是 | 审查 PASS。需求验收 20、Wiki 同步 20、职责边界 20、范围 15、信任表述 15、规格增量 10。条文不变。 |
