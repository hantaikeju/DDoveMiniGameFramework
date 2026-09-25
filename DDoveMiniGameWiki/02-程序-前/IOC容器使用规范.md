---
type: Playbook
title: IOC 容器使用规范
description: 一条闭环链路配一套 Architecture（一份 IOC）。不要 new 容器当全局定位器。本篇只写流程，不绑外库。API 仍看 IOC 容器。
tags: [程序-前, core, ioc]
status: stable
generated: { by: human:cjh, at: 2026-09-15T02:20:00Z }
verified: { by: human:cjh, at: 2026-09-24T02:46:00Z }
sources:
  - id: arch
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Core/Architecture/Architecture.cs
    title: Architecture.cs
  - id: ioc-src
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Core/Architecture/IOCContainer.cs
    title: IOCContainer.cs
  - id: ioc
    resource: /02-程序-前/IOC容器.md
    title: IOC 容器
  - id: roles
    resource: /02-程序-前/Architecture与角色.md
    title: Architecture 与角色
  - id: bind
    resource: /02-程序-前/Architecture自动注册.md
    title: Architecture 自动注册
  - id: event
    resource: /02-程序-前/TypeEvent.md
    title: TypeEvent
---

# IOC 容器使用规范

Concept ID：`/02-程序-前/IOC容器使用规范`。清单：[index_cjh](/02-程序-前/index_cjh.md)。`Register` / `Get` / 键看 [IOC 容器](/02-程序-前/IOC容器.md)。角色看 [Architecture 与角色](/02-程序-前/Architecture与角色.md)。结论与代码冲突以代码为准。

容器跟着 **Architecture 根**走，不是全游戏一份仓库、也不是谁都能 `new` 的定位器。

## 一条链路一套根

`Architecture<T>` 每个 `T` 自带一份 [IOC](/02-程序-前/IOC容器.md) 和一份事件。玩法根是 `GameArchitecture`。一条能自洽的闭环（自己的 Model / System / Command，对外只出 Kit）再开 `XxxArchitecture`，**不要**把内部件注册进 `GameArchitecture`。

分根门槛：有没有自己的 Model + System + 对外 Kit，生命周期能不能离开 `GameArchitecture` 单独 `Reset`。

| 放哪 | 例子 |
|------|------|
| `GameArchitecture` | 对局、玩家、天赋、广告收益、跨面板 Command |
| 独立 `XxxArchitecture` | 通道 / 设置 / 播放池这类能单独 `Reset` 的闭环 |
| 不进任何 IOC | [DDoveRes](/02-程序-前/DDoveRes.md) / [DDoveUI](/02-程序-前/DDoveUI.md) / [DDoveAtlas](/02-程序-前/DDoveAtlas.md) / [DDovePool](/02-程序-前/DDovePool.md) / [DDoveCfg](/02-程序-前/DDoveCfg.md) / Yoo / 效果库；业务集合放 Model 内 `Dictionary` |

对外只碰 Kit。Kit 内部 `GetModel` / `GetSystem` 走**自己那份** `XxxArchitecture.Interface`。跨链路不互相 `Get*`，只走 Kit 或事件。

## 怎么拆独立根

1. 用上一节门槛判断：缺 Model、缺 System、或缺对外 Kit，就还留在 `GameArchitecture`。
2. 手写 `XxxArchitecture : Architecture<XxxArchitecture>`，在 `Init()` 里 `Register` 自己的 Model / System（及需要的 Utility）。
3. Kit 只通过 `XxxArchitecture.Interface.GetModel` / `GetSystem` 取内部件。
4. 内部件**不要**挂 `[DDoveBind*]`，**不要**写进 `GameArchitecture.Generated`。
5. 拆掉这条链路时调 `XxxArchitecture.Reset()`，不要动游戏根。

音频根已落地，见 [DDoveAudio](/02-程序-前/DDoveAudio.md)。再拆一条新根时仍按上面 1–5，不要先塞进游戏根。外库项目名、类名不进本篇。要对齐外工程时另开对照 `Reference`（样板：[TL2 场景流式对照](/02-程序-前/TL2场景流式对照.md)），开篇写清「外库提取，不是本库现行约定」。

## 不要

- `new IOCContainer()` 当第二套全局仓库（给 `Tables`、Yoo、广告 SDK 互 `Get`）
- 把 Kit / Res / Yoo 注册进 [游戏 IOC](/02-程序-前/Architecture自动注册.md)
- Utility 互 `Get`；[Cfg](/02-程序-前/DDoveCfg.md) 的 `Tables` 挂在 `CfgUtility` 上，不要再注册一份
- 手写第二份 `GameArchitecture`，或把闭环内部件挂进游戏根
- Command 当第二套 IOC，见 [CommandQuery](/02-程序-前/CommandQuery.md)

模块局部通知可以自己 `new TypeEventSystem()`，见 [TypeEvent](/02-程序-前/TypeEvent.md)。那是事件总线，不是容器。

## 和自动注册

[Architecture 自动注册](/02-程序-前/Architecture自动注册.md) 现在只生成 `GameArchitecture`。玩法三类继续挂 `[DDoveBind*]`。独立根手写 `Init()`，**不要**进这份生成文件。生成器要不要支持多个根，本篇不定。

## 验收

1. 玩法 Model / System 只出现在 `GameArchitecture`。
2. 闭环 Kit 的内部件只出现在自己的 `XxxArchitecture`。
3. 业务代码没有 `new IOCContainer()`。
4. `GetUtility<CfgUtility>().Tables` 能读到表；没有第二份 `Tables` 注册。
