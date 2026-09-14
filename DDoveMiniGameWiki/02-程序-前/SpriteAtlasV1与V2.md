---
type: Reference
title: Sprite Atlas V1 与 V2
description: Unity 一代/二代图集运行时相同，只换 Pack 管线。Mode、Include in Build、第三方图集与代数正交。本库落地见 DDoveAtlas。
tags: [程序-前, ddoveatlas]
status: draft
generated: { by: human:cjh, at: 2026-09-14T06:51:00Z }
sources:
  - id: atlas
    resource: /02-程序-前/DDoveAtlas.md
    title: DDoveAtlas
  - id: ondemand
    resource: /02-程序-前/DDoveRes按需加载.md
    title: DDoveRes 按需加载
  - id: sprite-atlas
    resource: https://docs.unity3d.com/cn/2023.1/Manual/sprite-atlas.html
    title: 精灵图集
  - id: atlas-v2
    resource: https://docs.unity3d.com/cn/2023.1/Manual/SpriteAtlasV2.html
    title: Sprite Atlas V2
  - id: packer-modes
    resource: https://docs.unity3d.com/cn/2023.1/Manual/SpritePackerModes.html
    title: Sprite Packer 模式
  - id: workflow
    resource: https://docs.unity3d.com/cn/2023.1/Manual/SpriteAtlasWorkflow.html
    title: 精灵图集工作流程
  - id: distribution
    resource: https://docs.unity3d.com/cn/2023.1/Manual/SpriteAtlasDistribution.html
    title: 准备要分发的精灵图集
  - id: scenarios
    resource: https://docs.unity3d.com/cn/2023.1/Manual/SpriteAtlasScenarios.html
    title: 解析不同的精灵图集情形
  - id: properties
    resource: https://docs.unity3d.com/cn/2023.1/Manual/class-SpriteAtlas.html
    title: Sprite Atlas 属性
---

# Sprite Atlas V1 与 V2

Concept ID：`/02-程序-前/SpriteAtlasV1与V2`。清单：[index_cjh](/02-程序-前/index_cjh.md)。本篇讲 Unity 代数和易混开关。Kit、目录、late-bind 实现以 [DDoveAtlas](/02-程序-前/DDoveAtlas.md) 为准；与代码冲突以当前代码为准。

对照：[DDoveAtlas](/02-程序-前/DDoveAtlas.md)、[DDoveRes 按需加载](/02-程序-前/DDoveRes按需加载.md)。官方：[精灵图集](https://docs.unity3d.com/cn/2023.1/Manual/sprite-atlas.html)、[Sprite Atlas V2](https://docs.unity3d.com/cn/2023.1/Manual/SpriteAtlasV2.html)、[Sprite Packer 模式](https://docs.unity3d.com/cn/2023.1/Manual/SpritePackerModes.html)。

## 要什么

游戏里一代和二代功能一样：原料是散图，产物是整图 + 切割，API 是 `SpriteAtlas.GetSprite`。二代不是改成加载散图，也不是多一套运行时。

差在编辑器 **谁打、何时打、打完算不算正规导入**。2022.2 起默认 V2。本库选 **V2 - Enabled**，见 [DDoveAtlas](/02-程序-前/DDoveAtlas.md)。

更老的 Sprite Packer（Packing Tag）不是这篇的一代。

## 先分三层

代数只动中间一层。另两件常被当成「代数」的事，其实正交。

```
① 原料     PNG / Sprite（永远是散图）
② 打包     谁打、何时打、打完放哪     ← 只有这里 V1 ≠ V2
③ 运行时   SpriteAtlas + 切割 + Draw Call  ← 完全一样
```

| 现象 | 由谁决定 |
|------|----------|
| 编辑器里是散图还是整图 | **Mode**（Enabled / Enabled for Builds） |
| 真机自动有图还是先白再补 | **Include in Build** + 有没有 late-bind |
| 改 PNG 会不会马上重打、能不能走导入缓存 | **代数** |

不要记成「一代 = 整图，二代 = 散图、出包才整图」。出包两边都是整图 + 切割。「编辑器仍看散图」是某个 Mode。

## 两边相同

[目录页](https://docs.unity3d.com/cn/2023.1/Manual/sprite-atlas.html) 只定目标：少 Draw Call。这些对 V1 / V2 没有区别：

- Inspector：Type / Include in Build / Rotation / Tight Packing / Padding / 平台 Override / Objects for Packing（可挂文件夹）。见 [属性](https://docs.unity3d.com/cn/2023.1/Manual/class-SpriteAtlas.html)。
- 运行时类型 `UnityEngine.U2D.SpriteAtlas`；`GetSprite` 从大图切一块（每次 new，Kit 才按名缓存）。
- 激活一张 Sprite 会加载 **整本图集**。按阶段拆，不要一张总图集。见 [工作流](https://docs.unity3d.com/cn/2023.1/Manual/SpriteAtlasWorkflow.html)、[DDoveRes 按需加载](/02-程序-前/DDoveRes按需加载.md)。
- 关 Include in Build：Player 不自动带、不自动加载；预制体图会白，直到 `SpriteAtlasManager.atlasRequested` 补上。见 [分发](https://docs.unity3d.com/cn/2023.1/Manual/SpriteAtlasDistribution.html)、[情形 2](https://docs.unity3d.com/cn/2023.1/Manual/SpriteAtlasScenarios.html)。
- 同一 Sprite 进两本都 Include in Build 的图集，Unity **随机**选哪本。见 [情形 3](https://docs.unity3d.com/cn/2023.1/Manual/SpriteAtlasScenarios.html)。

## 一代：菜单 + 私房打包

`.spriteatlas` 更像菜单（packable、排法、Include in Build）。设置写在 `SpriteAtlas` 对象上。

AssetDatabase V1 没有依赖、没有 named object 导入器，Unity 另写 Packer：进 Play 或出 Player / AssetBundle 才打，大图进 `Library/AtlasCache`。见 [Sprite Atlas V2](https://docs.unity3d.com/cn/2023.1/Manual/SpriteAtlasV2.html)。

```
改 PNG → 菜单文件通常不变 → 点 Play / 出包 → 自定义 Packer → AtlasCache → 运行时再取
```

编辑器 API：`new SpriteAtlas()` + `SpriteAtlasExtensions.Add / SetIncludeInBuild`。

## 二代：图集是可导入资产

`.spriteatlasv2` 带 `SpriteAtlasImporter`，打图集 = 导入。设置写在 Importer 上。散图变了按依赖重导；能走 Cache Server / Accelerator。

```
改 PNG → 导入器发现依赖变了 → 立刻重打 → V2 Enabled 下编辑器已是新整图
```

开 V2：新图集按当前 Mode 建；已有 V1 **自动迁**，迁过 **不能回**。先备份，不要留一代样例给 Unity 迁。

编辑器 API：`SpriteAtlasAsset.Save` + `SpriteAtlasImporter`。运行时仍是 `SpriteAtlas` + `GetSprite`。

属性页仍写创建出来是 `.spriteatlas`；V2 工程常见 `.spriteatlasv2`。扩展名以 Unity 生成为准。收集器两种都收，见 [DDoveAtlas](/02-程序-前/DDoveAtlas.md)。

## 为什么还要二代

运行时没有新能力。要二代是因为一代接不进后来的导入系统，2022.2 默认已切走。

| 一代疼点 | 二代 |
|----------|------|
| 改 PNG 不保证马上重打 | 依赖变了就重导 |
| 产物在本机 `AtlasCache`，同事 / CI 各打一遍 | 导入产物可进加速缓存 |
| 编辑器常看散图，Play 才整图 | `V2 - Enabled` 编辑和 Play 都看整图 |
| 停在弃用路径 | 默认 Mode，新工具按 V2 写 |

一个人、小项目、只点 Play 看图，会觉得「不就是一样吗」——游戏侧这个感觉对。

## Mode 不是代数

[Sprite Packer Modes](https://docs.unity3d.com/cn/2023.1/Manual/SpritePackerModes.html)：`Edit > Project Settings > Editor > Sprite Packer > Mode`。一代、二代各有「一直打 / 只出包打」。

| Mode | 何时 Pack | 编辑器 / Play 看什么 |
|------|-----------|----------------------|
| Disabled | 不打，无 Pack Preview | 散图 |
| V1 Always Enabled | 进 Play / 出包 | 编辑器常看散图 |
| V1 Enabled for Builds | 只出包 | 编辑器和 Play 都看散图 |
| **V2 - Enabled** | 源图一变就打 | **编辑和 Play 都看整图** |
| V2 Enabled for Builds | 只出 Player / Bundle / Addressable | 编辑器和 Play 都看散图 |

`V2 Enabled for Builds` 不是「二代本质是散图」，只是二代的出包档。本库禁用，避免编辑器和真机不一致。

两页「默认 Mode」打架：Packer Modes 写 V1 Always Enabled；V2 页写 2022.2 起默认 V2 Enabled。以工程 `ProjectSettings/EditorSettings.m_SpritePackerMode` 为准。本库 `5` = V2 Enabled。

## Include in Build 与 late-bind

和代数无关。官方小项目默认 Include in Build 开，打进 Player，自动加载。

[分发三步](https://docs.unity3d.com/cn/2023.1/Manual/SpriteAtlasDistribution.html)（热更 / 分包）：

1. 关 Include in Build
2. 自己发文件（本库 Yoo）
3. 脚本 late-bind

关了之后编辑器仍 Pack 出文件，发布包不带。V2 Enabled 也要 `atlasRequested`，真机才会补上。源图保持未压缩，只压图集贴图，避免 Pack 先解再压掉精度。

本库两条线见 [DDoveAtlas](/02-程序-前/DDoveAtlas.md)：预制体拖图走 late-bind；配表 / 代码走 `LoadSpriteAsync("图集名/精灵名")`。

## 本库怎么选

```
官方小项目：Include in Build 开，自动加载
官方分发路径：关 Include in Build + 自己发 + late-bind
DDove：走分发路径；Mode = V2 Enabled
```

选 V2 不是运行时更强：编辑器和真机都看整图；生成走 Importer；不把样例停在 `isAtlasV2: 0`。

没选：Variant、`Enabled for Builds`、一张总图集、散图进收集器、启动预载全部图集。UI 图集关 Allow Rotation（Canvas 会跟着转）、关 Tight Packing。

运行时 Kit 不分代数。生成工具按 Mode / 已有扩展名走 V1 或 V2 写盘。

## 外部运行时各用各的图集

「整图 + 切割」是通用模式，**账本不能互换**。

| 谁 | 图集 | 给谁用 |
|----|------|--------|
| Unity UI / Sprite | Sprite Atlas（本库 DDoveAtlas） | `Image` / `SpriteRenderer` |
| Spine | 编辑器导出的 `.png` + `.atlas` / AtlasAsset | spine-unity；骨骼绑的是导出时的页和 UV |
| DragonBones / Live2D / TMP 字体 | 各自图集 | 各自运行时 |

Spine 的 PNG **不要**进 `GameResExcluded/Atlases`，不要当 packable 挂到 Unity 图集上。再打一次布局变了，骨骼还按旧 UV 取，会花、会错。`GetSprite` 也不懂 mesh attachment。

V2 Enabled 时，误把 Spine 贴图设成 Sprite 又被 Unity 图集扫进去，编辑器会换贴图源。导入成 Default/Texture，并从 Unity 图集排除。

本库没有 Spine。以后另开资源组 / Kit，用 Yoo 加载骨骼 Data + 自带图集，不要扩 `LoadSpriteAsync`。

## 不要

- 把 V2 理解成「散图出包」或「加载散图」
- 用 Mode / Include in Build 当代数
- 开 V2 留着一代文件让 Unity 自动迁
- 按手册某一页的 “default” 改 Mode，不看 `m_SpritePackerMode`
- 同一 Sprite 进两本 Include in Build 的图集
- 第三方骨骼 / 字体图集打进 Unity Sprite Atlas
- 把本篇当 Kit 步骤；落地步骤在 [DDoveAtlas](/02-程序-前/DDoveAtlas.md)
