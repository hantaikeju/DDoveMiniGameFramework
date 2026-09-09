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

* [DDoveFramework Core](DDoveFramework-Core.md) - Architecture 组合根 + Mechanism（Event / CommandQuery / Fsm）+ DDoveDebug。无 Singleton。 `Reference`
* [DDove Editor](DDoveEditor.md) - UIToolkit 总门面 DDove/Editor。面板用 [DDoveEditorPanel] 挂上去。SO Inspector 仍跟 Extension。 `Playbook`
* [DDoveDebug](DDoveDebug.md) - Core 统一日志。纯字符串，或 title + `[Title] (key, value)`。Log 受 DEBUG 条件编译。 `Reference`
* [DDoveRes](DDoveRes.md) - YooAsset 运行时门面。建包、初始化、LoadAssetAsync。失败打 DDoveDebug，不进 IOC。 `Reference`
* [DDoveRes 按需加载](DDoveRes按需加载.md) - 小游戏默认边玩边下。一个 DefaultPackage，多条 Collector + tag；用资源名 Load。不接整包下载 Fsm。 `Playbook`
* [DDoveBoot](DDoveBoot.md) - 起步场景与启动编排。只初始化资源包再 Yoo 加载真实场景。不进 IOC，不进 DDoveRes。 `Reference`
* [异步用 UniTask](UniTask异步.md) - 客户端异步统一 UniTask。Core 零引用。 `Playbook`
* [NuGet 与 Scriban](NuGet与Scriban.md) - nuget 源装 Scriban。UnityTls / 梯子代理。与 manifest（UPM）无关。 `Playbook`
* [UPM 落地](UPM落地.md) - GitHub URL 装完后拷一份到 `Packages/`，无科学上网不再 download err。 `Playbook`
* [IOC 容器](IOC容器.md) - 按注册泛型类型存一份实例；找不到抛错。不是构造注入框架。 `Reference`
* [Architecture 与角色](Architecture与角色.md) - GameArchitecture 注册 Model/System/Utility；谁能发令。 `Reference`
* [TypeEvent](TypeEvent.md) - 独立事件机制。Architecture 与业务都可用。 `Reference`
* [Command 与 Query](CommandQuery.md) - 一次写 / 一次读。业务写子类，执行走 Architecture。 `Reference`
* [Core Fsm](CoreFsm.md) - 分步流程状态机。Architecture 不依赖。 `Reference`
