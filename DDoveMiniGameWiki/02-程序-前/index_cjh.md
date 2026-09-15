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

* [DDoveFramework Core](DDoveFramework-Core.md) - Architecture 组合根 + Mechanism（Event / CommandQuery / Fsm）+ DDoveDebug。无 Singleton。 `Reference`* [DDove Editor](DDoveEditor.md) - UIToolkit 总门面 DDove/Editor。没有特殊原因工具一律 UIToolkit。面板用 [DDoveEditorPanel] 挂上去。 `Playbook`
* [DDoveDebug](DDoveDebug.md) - Core 统一日志。纯字符串，或 title + `[Title] (key, value)`。Log 受 DEBUG 条件编译。 `Reference`
* [DDoveRes](DDoveRes.md) - YooAsset 运行时门面。建包、初始化、LoadAssetAsync。失败打 DDoveDebug，不进 IOC。 `Reference`
* [DDoveRes 配置与使用](DDoveRes配置与使用.md) - 总窗 Res 页怎么配默认包、加载模式、启动场景；Boot 怎么读；运行时怎么 Load。 `Playbook`
* [DDoveRes 按需加载](DDoveRes按需加载.md) - 小游戏默认边玩边下。一个 DefaultPackage，多条 Collector + tag；用资源名 Load。不接整包下载 Fsm。 `Playbook`
* [TL2 场景流式对照](TL2场景流式对照.md) - 提取 TL2 分块流式、sceneabdata、进场景暂停延迟加载。本库小游戏默认不做。 `Reference`
* [DDoveBoot](DDoveBoot.md) - 起步场景与启动编排。只初始化资源包再 Yoo 加载真实场景。不进 IOC，不进 DDoveRes。 `Reference`
* [Play 到 WndHome](Play到WndHome.md) - 点 Play 到看见 WndHome。GameLaunch 挂 Launch；Yoo 3.0.5 要版本+清单；收集器有资源不等于运行时已有清单。 `Playbook`
* [DDoveCfg](DDoveCfg.md) - 第一刀接入 Luban。填表工程在 DDoveMiniGameConfig；客户端 Kit 用 Res 读表，Launch 先 Load 再进 Architecture。 `Playbook`
* [DDovePool](DDovePool.md) - 静态对象池门面。C# 池与 GameObject 池；默认容量 30。不进 IOC，不进 Core。 `Reference`
* [DDoveAudio](DDoveAudio.md) - 独立音频根。对外只碰 Kit；通道 / 设置 / 播放池不进 GameArchitecture。按名走 Res，播放器走 Pool。 `Reference`
* [DDoveUI](DDoveUI.md) - 第一刀垂直闭环已落地。制作场景导出 Prefab，DDoveRes 按文件名加载，Launch 打开 WndHome。 `Playbook`* [UI 业务封装](UI业务封装.md) - 做窗到写业务的步骤，以及开窗 / 读写 / Tween 门槛。业务直调 DDoveUIKit。 `Playbook`* [DDoveAtlas](DDoveAtlas.md) - 散图在 Excluded，Yoo 只收图集产物。Sprite Atlas V2 - Enabled。独立 Kit late-bind 与按名取图。 `Playbook`
* [Sprite Atlas V1 与 V2](SpriteAtlasV1与V2.md) - 一代/二代运行时相同，只换 Pack 管线。Mode、Include in Build、第三方图集与代数正交。落地见 DDoveAtlas。 `Reference`
* [PrimeTween](PrimeTween.md) - 接入免费 PrimeTween 1.4.x。只装包；Game 业务引用。DDoveUI Base 本刀不引用。 `Playbook`
* [异步用 UniTask](UniTask异步.md) - 客户端异步统一 UniTask。Core 零引用。 `Playbook`* [NuGet 与 Scriban](NuGet与Scriban.md) - nuget 源装 Scriban。UnityTls / 梯子代理。与 manifest（UPM）无关。 `Playbook`
* [UPM 落地](UPM落地.md) - GitHub URL 装完后拷一份到 `Packages/`，无科学上网不再 download err。 `Playbook`
* [IOC 容器](IOC容器.md) - 按注册泛型类型存一份实例；找不到抛错。不是构造注入框架。 `Reference`
* [IOC 容器使用规范](IOC容器使用规范.md) - 一条闭环链路配一套 Architecture（一份 IOC）。不要 new 容器当全局定位器。 `Playbook`* [Architecture 与角色](Architecture与角色.md) - GameArchitecture 注册 Model/System/Utility。读写与开窗门槛见 UI 业务封装。 `Reference`* [Architecture 自动注册](Architecture自动注册.md) - 三类特性收集并生成 GameArchitecture；总窗 Architecture 页配路径、Hotbox 创建三类。 `Playbook`
* [TypeEvent](TypeEvent.md) - 独立事件机制。Architecture 与业务都可用。 `Reference`
* [Command 与 Query](CommandQuery.md) - Command 是业务操作入口，Query 是可复用只读推导。执行走 Architecture。 `Reference`* [Core Fsm](CoreFsm.md) - 分步流程状态机。Architecture 不依赖。 `Reference`
