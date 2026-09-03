---
type: Index
title: 程序-前 · cjh
description: cjh 整理的客户端概念清单。
tags: [程序-前]
status: stable
generated: { by: human:cjh, at: 2026-08-31T03:06:00Z }
verified: { by: human:cjh, at: 2026-08-31T03:36:00Z }
---

# 程序-前 · cjh

Concept ID：`/02-程序-前/index_cjh`。本目录里 cjh 整理的篇。别人加篇写自己的 `index_<短号>.md`，不要改本文件。

# 概念

* [EUFramework Core](EUFramework-Core.md) - Architecture 组合根 + Mechanism（Event / CommandQuery / Fsm）。无 Singleton。 `Reference`
* [异步用 UniTask](UniTask异步.md) - 客户端异步统一 UniTask。Core 零引用。 `Playbook`
* [NuGet 与 Scriban](NuGet与Scriban.md) - nuget 源装 Scriban。UnityTls / 梯子代理。与 manifest（UPM）无关。 `Playbook`
* [UPM 落地](UPM落地.md) - GitHub URL 装完后拷一份到 `Packages/`，无科学上网不再 download err。 `Playbook`
* [IOC 容器](IOC容器.md) - 按注册泛型类型存一份实例；找不到抛错。不是构造注入框架。 `Reference`
* [Architecture 与角色](Architecture与角色.md) - GameArchitecture 注册 Model/System/Utility；谁能发令。 `Reference`
* [TypeEvent](TypeEvent.md) - 独立事件机制。Architecture 与业务都可用。 `Reference`
* [Command 与 Query](CommandQuery.md) - 一次写 / 一次读。业务写子类，执行走 Architecture。 `Reference`
* [Core Fsm](CoreFsm.md) - 分步流程状态机。Architecture 不依赖。 `Reference`
