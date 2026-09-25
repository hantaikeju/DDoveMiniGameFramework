---
type: Playbook
title: DDoveAtlas
description: 散图在 Excluded，Yoo 只收图集产物。工程用 Sprite Atlas V2 - Enabled。独立 Kit 做 late-bind 与按名异步取图。DDoveUI 不引用 Atlas。
tags: [程序-前, ddoveatlas]
status: stable
generated: { by: human:cjh, at: 2026-09-12T10:31:00Z }
verified: { by: human:cjh, at: 2026-09-24T02:46:00Z }
sources:
  - id: grill
    resource: /00-索引/Agent/需求稿工作流.md
    title: 需求稿工作流
  - id: ui
    resource: /02-程序-前/DDoveUI.md
    title: DDoveUI
  - id: res
    resource: /02-程序-前/DDoveRes.md
    title: DDoveRes
  - id: ondemand
    resource: /02-程序-前/DDoveRes按需加载.md
    title: DDoveRes 按需加载
  - id: res-use
    resource: /02-程序-前/DDoveRes配置与使用.md
    title: DDoveRes 配置与使用
  - id: cfg
    resource: /02-程序-前/DDoveCfg.md
    title: DDoveCfg
  - id: boot
    resource: /02-程序-前/DDoveBoot.md
    title: DDoveBoot
  - id: editor
    resource: /02-程序-前/DDoveEditor.md
    title: DDove Editor
  - id: core
    resource: /02-程序-前/DDoveFramework-Core.md
    title: DDoveFramework Core
  - id: unitask
    resource: /02-程序-前/UniTask异步.md
    title: 异步用 UniTask
  - id: atlas-v2
    resource: https://docs.unity3d.com/cn/2023.1/Manual/SpriteAtlasV2.html
    title: Sprite Atlas V2
---

# DDoveAtlas

Concept ID：`/02-程序-前/DDoveAtlas`。清单：[index_cjh](/02-程序-前/index_cjh.md)。与代码冲突以当前代码为准。只取目录 / 生成 / 按名取图；不搬 Builtin + Remote 双包裹。

对照：[DDoveUI](/02-程序-前/DDoveUI.md)、[DDoveRes](/02-程序-前/DDoveRes.md)、[DDoveRes 按需加载](/02-程序-前/DDoveRes按需加载.md)、[DDoveCfg](/02-程序-前/DDoveCfg.md)、[DDove Editor](/02-程序-前/DDoveEditor.md)。官方：[Sprite Atlas V2](https://docs.unity3d.com/cn/2023.1/Manual/SpriteAtlasV2.html)。

## 要什么

散图在 Excluded，Yoo **只收打好的图集**。工程开 **Sprite Atlas V2 - Enabled**（编辑和 Play 都走图集贴图）。删掉一代样例产物，用生成工具重建。

两条线一起做：

| 场景 | 入口 |
|------|------|
| 预制体上已拖好的 Image | `SpriteAtlasManager.atlasRequested`（late-bind） |
| 配表 / 代码动态换图 | `LoadSpriteAsync("图集名/精灵名")` |

V2 改的是 Pack 管线（ADBV2 导入器），不是改成加载散图。运行时仍是图集纹理 + 切割；`GetSprite` 不变。

## 仓库落点

| 路径 | 放什么 | 进收集器 |
|------|--------|----------|
| `GameResExcluded/Atlases/{阶段}/{图集名}/` | 散图（制作源） | 否 |
| `GameRes/Atlases/{阶段}/{图集名}.spriteatlas` 或 `.spriteatlasv2` | 生成产物；packable 挂对应散图文件夹 | 是，只收图集文件 |
| `ProjectSettings/EditorSettings` | Sprite Atlas Mode = **V2 - Enabled** | — |
| `Assets/DDoveFramework/Extension/DDoveAtlas/` | Kit + `DDoveAtlasInitInfo`（`Resources.Load`） | 否 |
| `Assets/DDoveFramework/Extension/DDoveAtlas/Editor/` | 生成图集、HotBox | 否 |
| `Assets/DDoveFramework/Extension/DDoveRes/Editor/` | `CollectSpriteAtlas` Filter | 否 |

菜单：`Edit > Project Settings > Editor > Sprite Atlas > Mode` → **Sprite Atlas V2 - Enabled**。新图集按此 Mode 建。不要用一代、不要用 V2 Enabled for Builds。

本刀只加 **Start** 组 + 一张最小样例（文件名 `StartDemo`，location `StartDemo`）。**删掉**现有一代 `GameRes/Atlases/Start/StartDemo.spriteatlas`，选中 Excluded 散图文件夹再生成。不要让 Unity 自动迁旧文件（迁过不能回一代）。`Common` / Login / Hall **有资产再加组**。

图集文件名 **全局唯一**（[AddressByFileName](/02-程序-前/DDoveRes.md)）。阶段靠目录，不靠文件名前缀。扩展名以 Unity 生成为准。

## 程序集

学 [DDoveCfg](/02-程序-前/DDoveCfg.md)。[DDoveFramework Core](/02-程序-前/DDoveFramework-Core.md) `references` 空。**没装 Atlas 时** Core / Res / Boot / UI / `DDove/Editor` 仍能编。

```
DDoveFramework.Extension.DDoveAtlas           Core, DDoveRes, UniTask, YooAsset
DDoveFramework.Extension.DDoveAtlas.Editor    DDoveAtlas, DDoveFramework.Editor
Game                                          现有引用 + DDoveAtlas
```

| 规则 | |
|------|--|
| Kit | 只走 [DDoveRes](/02-程序-前/DDoveRes.md) `LoadAssetAsync<SpriteAtlas>(图集名)`。不进 [IOC](/02-程序-前/IOC容器.md) |
| [DDoveUI](/02-程序-前/DDoveUI.md) | **不要**引用 Atlas。基类不加 `SetImage(url)` |
| Boot | **不要**引用 Atlas。`InitializeExtensionsAsync` 保持空挂钩 |
| Game | 仍 **不要**引用 `DDoveBoot`、`DDoveRes`、YooAsset、Editor |
| 业务换图 | `await DDoveAtlasKit.LoadSpriteAsync(url, ownerId)`，再走已有 `SetImage(Image, Sprite)` |

日志 [DDoveDebug](/02-程序-前/DDoveDebug.md)，`title` 固定 `DDoveAtlas`。异步 [UniTask](/02-程序-前/UniTask异步.md)。

## 配置与编辑器

一份 SO：`DDoveAtlasInitInfo`，放 `Extension/DDoveAtlas/Resources/`，运行时 `Resources.Load`（名字 `DDoveAtlasInitInfo`）。学 [DDoveResInitInfo](/02-程序-前/DDoveRes配置与使用.md)，不要再拆 EditorConfig + KitConfig。

字段：散图根（默认 `Assets/GameResExcluded/Atlases`）、产物根（默认 `Assets/GameRes/Atlases`）、padding、像素风预设。

**不要**总窗 Atlas 页，**不要**改 [DDove Editor](/02-程序-前/DDoveEditor.md) `DDoveEditorNav.Ids`。生成挂 `[DDoveHotboxEntry]`，饼环在 **HotBox** 页编排。Create 菜单可留 `DDove/Atlas/Init Info`。

选中 Excluded 下的散图文件夹 → 生成/更新同名图集到 `GameRes/Atlases/{阶段}/`。阶段从相对散图根的第一级目录读（`Start/...` → `Start`）。新建时关掉 Include in Build（`SetIncludeInBuild(false)`；当前 Unity 没有 `GetIncludeInBuild`），packable **挂文件夹**。更新只刷 packable，保留 Inspector 里改过的打包设置。不要启动时扫产物目录。

[DDoveRes.Editor](/02-程序-前/DDoveRes配置与使用.md) 的 `CollectSpriteAtlas` 收 `.spriteatlas` **和** `.spriteatlasv2`。收集器：`AddressByFileName`、`PackSeparately`、tag 与同阶段 UI 对齐（本刀 `start`）。构建打开 `TrackSpriteAtlasDependencies`。

## 运行时

`DDoveAtlasKit.Initialize()`：注册 `atlasRequested`。之后 Unity 要图集时按 **文件名** 异步 `LoadAssetAsync`，再回调。不要 `LoadAssetSync`。Include in Build 关时，V2 Enabled 仍要这条 late-bind，真机才会补上图集。

`LoadSpriteAsync("图集名/精灵名", ownerId)`：同一图集全局一份 handle，按 owner **引用计数**。`SpriteAtlas.GetSprite` 的实例按 `图集名/精灵名` 缓存。面板真正销毁时 `ReleaseScope(ownerId)`；[DDoveUI LRU](/02-程序-前/DDoveUI.md) 还活着不要放。引用到 0 再 `Release` handle。

## 启动顺序

```
Boot → DDoveResKit.InitializeAsync → LoadScene(Launch)
GameLaunch
    await DDoveCfgKit.LoadAsync()
    DDoveSaveKit.LoadFile
    GameArchitecture.Interface
    DDoveAtlasKit.Initialize          开任何 UI 之前
    DDoveAudioKit.Initialize
    DDoveUIKit.Initialize
    NavigateToAsync<WndHome>
```

改 [GameLaunch](../../DDoveMiniGameClient/Assets/Game/Mono/GameLaunch.cs)。不要手写 `GameArchitecture.Init`。

## 不要

- 搬 Builtin + Remote、`isRemote`、完整资产路径当 location
- `LoadAssetSync`、启动预载全部图集、一张总图集
- 一条收集器扫整个 `GameRes/Atlases`，或把散图收成 addressable
- 散图进 `GameRes/` 当正式资源；图集进 `GameResExcluded`
- `DDoveUI` / Boot / Core 引用 Atlas
- 基类 `SetImage(url)`、Scriban 扩展模板、OSA / 第三方骨骼图集 Provider
- 总窗 Atlas 页、双份 Config SO
- 留着一代 `StartDemo` 让 Unity 自动迁；Mode 用一代或 V2 Enabled for Builds
- `GetIncludeInBuild`（当前编辑器 API 没有）

## 还没有（本刀之后）

按 tag 预下图集、阶段退出整批卸、Host/Web、`04-美术` 命名规范篇。未实现前不要把未落地 API 当已有方法。

## 验收

1. Project Settings Sprite Atlas Mode 是 **V2 - Enabled**。没装 `DDoveAtlas` 时 Core / Res / Boot / UI / `DDove/Editor` 仍能编。
2. 仓库里没有旧的一代 `StartDemo.spriteatlas`（`isAtlasV2: 0`）。选中 `GameResExcluded/Atlases/Start/StartDemo/` 能生成产物；Include in Build 关；收集器只打到图集、打不到散图。
3. Play：`WndHome` 上拖好的样例 Sprite 可见（不白）。
4. 业务 `LoadSpriteAsync("StartDemo/…")` 能换图；关面板后引用计数掉干净。
5. `DDoveUI` / Boot / Core asmdef 没有 Atlas。`Game` 没有 `DDoveRes` / YooAsset。总窗侧栏仍是 HotBox / Architecture / Res / UI。
