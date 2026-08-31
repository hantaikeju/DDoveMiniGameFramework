---
type: Reference
title: Core Fsm
description: 分步流程用的状态机。Architecture 不依赖它。玩法状态优先用 Model 字段。
tags: [程序-前, core, fsm]
status: stable
generated: { by: human:cjh, at: 2026-08-28T12:32:00Z }
verified: { by: human:cjh, at: 2026-08-31T03:36:00Z }
sources:
  - id: fsm
    resource: ../../DDoveMiniGameClient/Assets/EUFramework/Core/Mechanism/Fsm/StateMachine.cs
    title: StateMachine.cs
---

# Core Fsm

Concept ID：`/02-程序-前/CoreFsm`。目录：`Core/Mechanism/Fsm/`。

`IStateNode`：`OnCreate` / `OnEnter` / `OnUpdate` / `OnExit`。黑板是 `string → object`。节点键为类型全名。入口不存在抛错；`ChangeState` 找不到节点则忽略。

给 EURes 补丁这类分步流程用。玩法状态用 Model 字段或枚举，不必上这台机器。

Core **不提供** `Singleton<T>`。全局入口只有 `Architecture<T>.Interface`。Kit 用静态门面，不继承单例基类。
