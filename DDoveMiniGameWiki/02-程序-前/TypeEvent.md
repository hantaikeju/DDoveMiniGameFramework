---
type: Reference
title: TypeEvent
description: 独立事件机制。Architecture 组合一份；业务也可自建总线。两条总线互不相通。
tags: [程序-前, core, event]
status: stable
generated: { by: human:cjh, at: 2026-08-28T13:10:00Z }
verified: { by: human:cjh, at: 2026-08-31T03:36:00Z }
sources:
  - id: ev
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Core/Mechanism/Event/EventSystem.cs
    title: EventSystem.cs
---

# TypeEvent

Concept ID：`/02-程序-前/TypeEvent`。目录：`Core/Mechanism/Event/`。

按事件类型订阅/发送。事件是普通类型（class/struct），不是字符串。无人订阅时 `Send` 静默。`Register` 返回 `IUnRegister`，可 `AddToUnregisterList` 后一次性卸。

## 谁用哪条总线

| 场景 | 怎么用 |
|------|--------|
| Model / System / 面板，跟玩法数据走 | `SendEvent` / `RegisterEvent`（Architecture 持有的那份） |
| 业务模块自己的局部通知，不想挂上 GameArchitecture | `new TypeEventSystem()`，自己持有、自己 `Clear` |
| 工具/Kit、确有跨模块且不经 Architecture | 可用 `TypeEventSystem.Global`；默认少用，避免变成第二套全局 |

**同一实例才互通。** `GameArchitecture.SendEvent<Foo>()` 到不了业务自己 `new` 出来的那份。

`Architecture<T>.Reset()` 只 `Clear` Architecture 持有的那份，清不掉业务自建的总线和 `Global`。
