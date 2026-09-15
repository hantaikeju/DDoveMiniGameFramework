---
type: Reference
title: DDoveFramework Core
description: Architecture 组合根 + Mechanism（Event / CommandQuery / Fsm）+ DDoveDebug。无 Singleton。
tags: [程序-前, core]
status: stable
generated: { by: human:cjh, at: 2026-08-31T02:00:00Z }
verified: { by: human:cjh, at: 2026-09-15T03:20:00Z }
sources:
  - id: core-asmdef
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Core/DDoveFramework.Core.asmdef
    title: DDoveFramework.Core.asmdef
  - id: ioc
    resource: /02-程序-前/IOC容器.md
    title: IOC 容器
  - id: arch
    resource: /02-程序-前/Architecture与角色.md
    title: Architecture 与角色
  - id: event
    resource: /02-程序-前/TypeEvent.md
    title: TypeEvent
  - id: cq
    resource: /02-程序-前/CommandQuery.md
    title: Command 与 Query
  - id: debug
    resource: /02-程序-前/DDoveDebug.md
    title: DDoveDebug
---

# DDoveFramework Core

Concept ID：`/02-程序-前/DDoveFramework-Core`。目录：`DDoveMiniGameClient/Assets/DDoveFramework/Core/`。

```
DDoveFramework/Core/
  DDoveDebug.cs                 统一日志；直接 UnityEngine.Debug
  Architecture/              组合根：IOC、角色、Bind 特性；持有 Event；执行 CommandQuery
  Mechanism/                 给业务/Kit 用的机制封装（不是 GoF 清单）
    Event/
    CommandQuery/
    Fsm/
```

`Mechanism` 按**用法**收拢：事件、一次写/读、分步流程。不要改名叫 `Patterns`，也不要往里预置备忘录、单例、工厂。

## 两层

| | Architecture | Mechanism |
|---|---|---|
| 是什么 | 组合根：谁注册、谁能互找 | 可复用机制：业务写子类或 `new` |
| 入口 | `Architecture<T>.Interface` | 各机制自己的类型 |

Mechanism 内部仍是三套，规则不变：

| | Event | CommandQuery | Fsm |
|---|---|---|---|
| 干什么 | 按类型发/订 | 一次写 / 一次读 | 分步流程 |
| 和 Architecture | 组合一份 | **执行必须走 Architecture** | **不引用** |
| 业务 | 也可 `new TypeEventSystem` | 继承后 `SendCommand` / `SendQuery` | `new StateMachine` |

## 后续还许不许加

允许，但 **默认否**。先问三句：

1. 离开某一块业务 / 某一个 Kit，还有第二处要用吗？
2. 是纯 C# 规则，还是资源、UI、平台、存档？
3. 是不是又在和 Architecture 抢全局入口？

| 情况 | 放哪 |
|---|---|
| 谁能 Get / Init / 角色 | `Architecture/` |
| 统一日志 | `DDoveDebug.cs`（全程序集依赖） |
| 新的可复用机制（第二处真实要用） | `Mechanism/Xxx/`，按职责命名 |
| Res / UI / 广告 / 存档 | `Extension/` |
| Editor 总门面 | `DDoveFramework/Editor/`，UIToolkit；面板用 `[DDoveEditorPanel]` 挂，见 [DDove Editor](/02-程序-前/DDoveEditor.md) |
| 该 Kit 的 Editor 面板 | `Extension/Xxx/Editor/`，挂到总门面；SO Inspector 仍在这里 |
| 备忘录、对象池、Tween、Singleton | 不进 Core |

## 已落地

| 模块 | 概念 |
|------|------|
| IOC 容器 | [IOC容器](/02-程序-前/IOC容器.md) |
| IOC 怎么用 | [IOC容器使用规范](/02-程序-前/IOC容器使用规范.md) |
| Architecture / 角色 | [Architecture与角色](/02-程序-前/Architecture与角色.md) |
| Architecture 自动注册 | [Architecture自动注册](/02-程序-前/Architecture自动注册.md) |
| 事件 | [TypeEvent](/02-程序-前/TypeEvent.md) |
| 命令 / 查询 | [CommandQuery](/02-程序-前/CommandQuery.md) |
| 状态机 | [CoreFsm](/02-程序-前/CoreFsm.md) |
| 日志 | [DDoveDebug](/02-程序-前/DDoveDebug.md) |

程序集：`DDoveFramework.Core`，`references` 空，`noEngineReferences: false`（只为 `UnityEngine.Debug`）。仍不引用 UniTask / Yoo / Kit。

`.meta` **不要手写**。挪已有资源时带着 Unity 生成的 `.meta` 一起挪。

## 边界

进 Core：规则与机制。例外：[DDoveDebug](/02-程序-前/DDoveDebug.md) 直接调 `UnityEngine.Debug`，不为换引擎做绑定。  
不进 Core：Kit、平台 SDK、业务 Model/System、效果库、`Singleton<T>`、UniTask。业务集合用 Model 内 `Dictionary`，不进 IOC。

客户端异步约定见 [异步用 UniTask](/02-程序-前/UniTask异步.md)。Core 本身保持同步。
