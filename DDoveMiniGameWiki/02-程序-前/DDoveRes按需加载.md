---
type: Playbook
title: DDoveRes 按需加载
description: 小游戏默认边玩边下。一个 DefaultPackage，多条 Collector + tag；用资源名 Load。不接整包下载 Fsm。
tags: [程序-前, ddoveres, yooasset]
status: stable
generated: { by: human:cjh, at: 2026-09-04T08:39:00Z }
verified: { by: human:cjh, at: 2026-09-15T03:20:00Z }
sources:
  - id: res
    resource: /02-程序-前/DDoveRes.md
    title: DDoveRes
  - id: boot
    resource: /02-程序-前/DDoveBoot.md
    title: DDoveBoot
  - id: play
    resource: /02-程序-前/Play到WndHome.md
    title: Play 到 WndHome
  - id: fsm
    resource: /02-程序-前/CoreFsm.md
    title: Core Fsm
  - id: kit
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveRes/DDoveResKit.cs
    title: DDoveResKit.cs
  - id: yoo-dl
    resource: ../../DDoveMiniGameClient/Packages/com.tuyoogame.yooasset@3.0.5/Runtime/ResourcePackage/Operations/DownloaderOptions.cs
    title: DownloaderOptions.cs
  - id: tl2
    resource: /02-程序-前/TL2场景流式对照.md
    title: TL2 场景流式对照
---

# DDoveRes 按需加载

Concept ID：`/02-程序-前/DDoveRes按需加载`。约定与 [DDoveRes](/02-程序-前/DDoveRes.md) 门面、[DDoveBoot](/02-程序-前/DDoveBoot.md) 配合。结论与代码冲突以当前代码为准。

门面上 **还没有** `CreateResourceDownloader` / 按 location 预下。下面预下 / tag 是现行设计，不是已落地 API。

## 结论

小游戏（及默认 App）**边玩边下**：`LoadAssetAsync` / `LoadSceneAsync` 缺文件再拉对应 bundle。  
**不接**启动全量 `CreateDownloader()` + 补丁 Fsm。  
**仍要**包裹 Init + **要版本 + 加载清单**。Yoo 3.0.5 的 EditorSimulate / Offline 已在 [DDoveRes](/02-程序-前/DDoveRes.md) `LoadActiveManifestAsync` 里做完。Host / Web 的远程 Init options **未接**（`CreateInitializeOptions` 返回 `null`）。缺清单会 `Active package manifest not found`，见 [Play 到 WndHome](/02-程序-前/Play到WndHome.md)。

```
DDoveBoot：Init（含要版本 + 清单）→ LoadScene(Launch)
之后：Load(资源名)；没有就下这个资源的包 + 依赖
可选：业务进大厅 / 进本前，按 tag 或配表名字预下一批（门面还没有预下 API）
```

释放 `Release` 卸的是内存。磁盘缓存还在则下次 Load 走本地；换清单或清缓存才再打 CDN。不是每次 Load 都对远端。

## 一个包裹，多条收集器

只要一个 Yoo `DefaultPackage`（远程）。不要 Builtin + Remote 双包裹，也不要按模块拆 Package。  
[DDoveBoot](/02-程序-前/DDoveBoot.md) **不进**任何收集器。

| | 职责 |
|--|------|
| Collector | 收哪些目录、打成哪些 bundle |
| Tag | 以后 `CreateDownloader(tag)` 分批预下（门面还没有） |
| location | 日常 `LoadAssetAsync(名字)` |

资源根：`Assets/GameRes/`（进收集器）与 `Assets/GameResExcluded/`（不进，设计稿 / 临时候选）。不要扫整个 `GameRes/`，按子目录收。不要用 Unity 保留名 `Resources/` 当这棵树。

```
DefaultPackage
  Group Scene      CollectPath: Assets/GameRes/Scenes                   tag: launch
  Group Start      CollectPath: Assets/GameRes/UI/Start                 tag: start
  Group Common     CollectPath: Assets/GameRes/UI/…、Assets/GameRes/Atlases/…   tag: common
  Group Login      CollectPath: Assets/GameRes/UI/…、Assets/GameRes/Atlases/…   tag: login
  Group Hall       CollectPath: Assets/GameRes/UI/…、Assets/GameRes/Atlases/…   tag: hall
```

框架工程已落根：`GameRes/Scenes`、`GameRes/UI`、`GameRes/Atlases`、`GameRes/Config`、`GameRes/Cfg`、`GameResExcluded`。`Scene` 组已收 `Launch.unity`。UI 已加 `Start` 组（`GameRes/UI/Start`，AddressByFileName）。`Cfg` 组收 `GameRes/Cfg`（tag `cfg`），见 [DDoveCfg](/02-程序-前/DDoveCfg.md)。Common / Login / Hall 有了再加收集器。

不要一条收集器扫整个 `UI/`。阶段靠子目录 + 收集器 tag。  
图集体积大，必须按阶段分图集，不能一张总图集。

清单出包时仍收录所有 Collector。分批的是**设备上下哪些文件**，不是少打进包。

## 用资源名 Load；关卡用配表

Tag 按**阶段**（`launch` / `start` / `common` / `login` / `hall`），不要只打种类（`player` / `monster`）。种类可作目录名。

角色、怪物共用多，本关用谁写在**业务配表**（资源名 / location），不要追求 tag 覆盖每一关：

```
dungeon_01 → Mob_Slime, Mob_Bat, HeroBase
```

进本前可对这批名字 `GetAssetInfo` 再下，或直接 `LoadAssetAsync(名字)`（会带下该 bundle）。  
预下是文件到本地；不要把整关模型都 Load 进内存。框架不管关卡表，[DDoveBoot](/02-程序-前/DDoveBoot.md) 只保证进 Launch。

## 不要整包提前下

| 做法 | 小游戏 |
|------|--------|
| 包裹级「全量预下」+ 全量 Fsm | **不做** |
| Collector 分批 tag | 给大厅 UI / 图集等整批用 |
| 配表资源名 | 给本关怪、皮肤 |
| 收集器 UserData 生成策略再编译出另一种 Load | **不做**；Load 始终一条路 |

EditorSimulate / Offline 的要版本 + 清单已经在 Res Init 里 `await`，不必上 [Core Fsm](/02-程序-前/CoreFsm.md)。Fsm 只留给以后「小批预下要重试/进度」的短链，不要做成空 tag 全下。

Yoo `ClearCacheAsync`：热更后 `ClearUnusedBundleFiles` 清旧版本。没有「缓存满自动 LRU」。微信扩展主要认 All / Unused。`Release` ≠ 删磁盘。

## 对照

H5 加载页、分块流式、`sceneabdata` 见 [TL2 场景流式对照](/02-程序-前/TL2场景流式对照.md)。本库默认不做。Yoo 侧若以后做加载页，绑 `CreateDownloader(tag)` 的 `TotalDownloadBytes`（门面还没有这个 API）。名单角色 = tag 或配表名字。

## 还没有（代码）

Host / Web 的远程 Init options、Downloader（按 tag / 按 location）、`ClearCache` 门面。EditorSimulate / Offline 的要版本 + 清单已接。未实现前不要把本篇预下 API 当已有方法。
