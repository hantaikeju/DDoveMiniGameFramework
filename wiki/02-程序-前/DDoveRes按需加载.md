---
type: Playbook
title: DDoveRes 按需加载
description: 小游戏默认边玩边下。一个 DefaultPackage，多条 Collector + tag；用资源名 Load。不接整包下载 Fsm。
tags: [程序-前, ddoveres, yooasset]
status: stable
generated: { by: human:cjh, at: 2026-09-04T08:39:00Z }
verified: { by: human:cjh, at: 2026-09-04T08:43:00Z }
sources:
  - id: res
    resource: /02-程序-前/DDoveRes.md
    title: DDoveRes
  - id: boot
    resource: /02-程序-前/DDoveBoot.md
    title: DDoveBoot
  - id: fsm
    resource: /02-程序-前/CoreFsm.md
    title: Core Fsm
  - id: kit
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveRes/DDoveResKit.cs
    title: DDoveResKit.cs
  - id: yoo-dl
    resource: ../../DDoveMiniGameClient/Packages/com.tuyoogame.yooasset@3.0.5/Runtime/ResourcePackage/Operations/DownloaderOptions.cs
    title: DownloaderOptions.cs
  - id: h5
    resource: TL2_LLM_Wiki/02-打包构建/运行时按需加载与流畅策略.md
    title: TL2 运行时按需加载（外库对照）
---

# DDoveRes 按需加载

Concept ID：`/02-程序-前/DDoveRes按需加载`。约定与 [DDoveRes](/02-程序-前/DDoveRes.md) 门面、[DDoveBoot](/02-程序-前/DDoveBoot.md) 配合。实现未齐时以本约定为准；与代码冲突以当前代码为准。

门面上 **还没有** `CreateResourceDownloader` / 按 location 预下。Host 要版本 + 拉清单也未接。下面是现行设计，不是已落地 API。

## 结论

小游戏（及默认 App）**边玩边下**：`LoadAssetAsync` / `LoadSceneAsync` 缺文件再拉对应 bundle。  
**不接** MeowPantry 那种启动全量 `CreateDownloader()` + 补丁 Fsm。  
**仍要** 包裹 Init；Host / Web 还要 **要版本 + 更新清单**，否则 Load 没有目录。

```
DDoveBoot：Init（+ 以后：版本、清单）→ LoadScene(Launch)
之后：Load(资源名)；没有就下这个资源的包 + 依赖
可选：业务进大厅 / 进本前，按 tag 或配表名字预下一批
```

释放 `Release` 卸的是内存。磁盘缓存还在则下次 Load 走本地；换清单或清缓存才再打 CDN。不是每次 Load 都对远端。

## 一个包裹，多条收集器

只要一个 Yoo `DefaultPackage`（远程）。不要 Builtin + Remote 双包裹，也不要按模块拆 Package。  
[DDoveBoot](/02-程序-前/DDoveBoot.md) **不进**任何收集器。

| | 职责 |
|--|------|
| Collector | 收哪些目录、打成哪些 bundle |
| Tag | `CreateDownloader(tag)` 分批预下 |
| location | 日常 `LoadAssetAsync(名字)` |

```
DefaultPackage
  Group Common     CollectPath: UI/Prefabs/Common、Atlases/Common     tag: common
  Group Login      CollectPath: UI/Prefabs/Login、Atlases/Login      tag: login
  Group Hall       CollectPath: UI/Prefabs/Hall、Atlases/Hall        tag: hall
```

不要一条收集器扫整个 `UI/`。EUUI 仍可只有一个 Prefabs 根；**阶段靠子目录 + 收集器 tag**。  
图集体积大，必须按阶段分图集，不能一张总图集。

清单出包时仍收录所有 Collector。分批的是**设备上下哪些文件**，不是少打进包。

## 用资源名 Load；关卡用配表

Tag 按**阶段**（`common` / `login` / `hall`），不要只打种类（`player` / `monster`）。种类可作目录名。

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

版本 + 清单可以是 Boot 里两步 `await`，不必上 [Core Fsm](/02-程序-前/CoreFsm.md)。Fsm 只留给以后「小批预下要重试/进度」的短链，不要做成空 tag 全下。

Yoo `ClearCacheAsync`：热更后 `ClearUnusedBundleFiles` 清旧版本。没有「缓存满自动 LRU」。微信扩展主要认 All / Unused。`Release` ≠ 删磁盘。

## 对照 H5 App 加载页

H5 **没有** Yoo tag。加载页等的是 `PreloadEnterAssets`：**写死最小集 + 打包生成的 bytes 名单**，用 `abdata` 展开 AB 体积加总。第一次启动卡住进度条；以后后台预下。不是按 tag 统计，也不是整包。

Yoo 对等：加载页绑 `CreateDownloader("login","common")`（或 Launch 依赖）的 `TotalDownloadBytes`。名单角色 = tag 或配表名字。

## 还没有（代码）

Host / Web 的 Init options、要版本、拉清单、Downloader（按 tag / 按 location）、`ClearCache` 门面。未实现前不要把本篇 API 表当已有方法。
