---
type: Reference
title: TL2 场景流式对照
description: 提取 TL2 分块流式、sceneabdata、进场景暂停延迟加载。本库小游戏默认不做；留给以后大世界参考。
tags: [程序-前, ddoveres, yooasset]
status: stable
generated: { by: human:cjh, at: 2026-09-09T09:20:00Z }
verified: { by: human:cjh, at: 2026-09-09T09:16:00Z }
sources:
  - id: ondemand
    resource: /02-程序-前/DDoveRes按需加载.md
    title: DDoveRes 按需加载
  - id: boot
    resource: /02-程序-前/DDoveBoot.md
    title: DDoveBoot
  - id: tl2-runtime
    resource: TL2_LLM_Wiki/02-打包构建/运行时按需加载与流畅策略.md
    title: TL2 运行时按需加载（外库）
  - id: tl2-pack
    resource: TL2_LLM_Wiki/02-打包构建/资源管理分包与出包总览.md
    title: TL2 资源管理分包（外库）
  - id: tl2-arch
    resource: TL2_LLM_Wiki/01-架构与框架/当前客户端框架架构分析.md
    title: TL2 客户端框架架构分析（外库）
---

# TL2 场景流式对照

Concept ID：`/02-程序-前/TL2场景流式对照`。外库提取，**不是**本库现行约定。小游戏默认仍走 [DDoveRes 按需加载](/02-程序-前/DDoveRes按需加载.md)：一个包裹、用资源名 Load、边玩边下。实现未齐时以那篇为准。

本库 **不要**抄 `sceneabdata`、`.sab`、`CustomSceneLoadManager`。下面只记 TL2 为什么这么做，以及以后若真有大世界可对照的点。

## 本库先不做

| TL2 | 本库现在 |
|-----|----------|
| 场景单独一套目录 + 分块流式 | `LoadSceneAsync(location)`，整场景进 Yoo 同一包裹 |
| 切场景暂停 DelayLoad | 无延迟队列，无暂停开关 |
| 按主角位置补远处块 | 框架不管关卡表；Boot 只保证进 Launch |

有大厅 / 单关 / 小场景就够。出现「一张图太大、切场景和远处抢带宽」再回头看下面三节。

## 1. sceneabdata：场景自己一份目录

TL2 资源目录和场景目录是两套：

| | 普通资源 | 场景 |
|--|----------|------|
| 管理器 | `ResourceManager` | `SceneAssetManager` |
| 目录 | `abdata.data` | `sceneabdata.data` |
| 包体 | `.rab` / `.rgab` | `.sab` + `scenebase.sab` |
| 磁盘 | `AssetBundles/` | `AssetBundlesScene/` |

ForceLoad **两份目录都要齐**，才知道 CDN 上哪个文件对应哪块场景。没有目录就没有按需下。

优化点（以后若场景体积和 UI/模型不在一个量级）：

- 目录必须先于 Load。Yoo 对等是 Host/Web **要版本 + 拉清单**，不必再造第二份 `sceneabdata`。
- TL2 把场景 AB 和普通 AB 分目录，是为了流式块和 UI 包互不扫。Yoo 用 **Collector 分组 + tag** 就能隔离，不要拆第二个 Package。
- 热更只换「最新目录名」，变过的文件才再下。本库以后 `ClearUnusedBundleFiles` 对的是旧 bundle，不是另写场景清单。

## 2. 分块流式：先能走，远处后补

`SceneBlockManager` + `CustomSceneLoadManager`：按主角位置加载场景块，不是整张图一次进内存。启动预分配 `RendererData` / `SplitBlockData`，少在运行时 new。

```
进场景
  只更第 1 层 / 第一块（能走的地面 + 主角）
  DelayLoadLayer 非 Max → 远处不更
  IsOnlyLoadBlockOne → 等 SceneAssetManager.LoadingCount <= 3 再放开
之后
  按位置补远处建筑
```

优化点（以后真有大世界）：

- **可见最小集先进内存**；预下到磁盘 ≠ Load 进内存（按需加载篇已写）。
- 并发帽：TL2 WebGL 延迟加载 **1 路**，非 WebGL **2 路**。切场景时 `ResourceManager.LoadTime` 从 `0.50s` 压到 `0.05s`，避免一帧解太多 AB。
- 限流对象是 **带宽 + 主线程时间片**，不是再开一条 Load API。本库 Load 仍走 `DDoveResKit.LoadSceneAsync` / `LoadAssetAsync`。
- 人先有逻辑、模型后到（`ObjManager` → `InitModel`）是同屏优化，和分块是两件事；小游戏同屏少时不必上。

## 3. 进场景暂停延迟加载

切场景卡爆的常见原因：远处 DelayLoad 和当前场景抢带宽。

```
进场景前
  AssetManager.IsPauseDelayLoad = true
  CustomSceneLoadManager.SetDelayLoadScene()
进场景后
  IsPauseDelayLoad = false
  SetDelayLoadSceneEnd()
```

Loading 期间把每帧预算提到 `0.100f`，进战斗再收紧。

优化点（以后有「后台预下」或延迟队列时）：

- **切场景必须先停不紧急的下**，当前 location 下完再放开。没有暂停开关就不要开后台预下。
- 暂停的是「不紧急」；当前场景块 / Launch 依赖不算 DelayLoad。
- 预下载完成 ≠ 已在内存。TL2：`IsPreDownloadComplete` ≠ `IsMemoryPreloadComplete`。

## 旁路（同一套流畅模型，不是场景专属）

| TL2 | 以后若要对照 |
|-----|----------------|
| `PreloadEnterAssets` 写死最小集 + 打包 bytes 名单 | Yoo 对等：`CreateDownloader(tag)` 或配表名字；见 [按需加载](/02-程序-前/DDoveRes按需加载.md) |
| 异步队列约 200 槽、每帧时间片 | 小游戏默认 1 路按需即可；卡帧再加时间片 |
| 引用计数 + 对象池 | Yoo handle `Release`；不要每次切 UI 卸依赖再打 CDN |
| 设备分档（阴影 / 特效 / 物理） | Launch 之后的业务，不进 [DDoveBoot](/02-程序-前/DDoveBoot.md) |

## 不要抄进本库

- 第二套场景目录格式（`sceneabdata` / `.sab`）。Yoo 清单就是目录。
- Builtin + Remote 双包裹，或按模块拆 Package。
- 把分块管理器塞进 Boot。Boot 只 Init（以后加版本/清单）再 `LoadScene(Launch)`。
- 没有延迟队列就先做 `IsPauseDelayLoad`。
- 整包 `CreateDownloader()` + 补丁 Fsm（按需加载篇已禁）。

外库与代码冲突以外库当时工程为准；本库与代码冲突以本库代码为准。
