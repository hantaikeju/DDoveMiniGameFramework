---
type: Reference
title: Command 与 Query
description: Command 是业务操作入口，Query 是可复用的只读推导。业务写子类；执行仍走 Architecture，不是独立总线。
tags: [程序-前, core, command, query]
status: stable
generated: { by: human:cjh, at: 2026-08-31T01:48:00Z }
verified: { by: human:cjh, at: 2026-09-15T07:17:00Z }
sources:
  - id: cmd
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Core/Mechanism/CommandQuery/Command.cs
    title: Command.cs
  - id: query
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Core/Mechanism/CommandQuery/Query.cs
    title: Query.cs
  - id: arch
    resource: /02-程序-前/Architecture与角色.md
    title: Architecture 与角色
  - id: unitask
    resource: /02-程序-前/UniTask异步.md
    title: 异步用 UniTask
  - id: ui
    resource: /02-程序-前/DDoveUI.md
    title: DDoveUI
  - id: ui-biz
    resource: /02-程序-前/UI业务封装.md
    title: UI 业务封装
---

# Command 与 Query

Concept ID：`/02-程序-前/CommandQuery`。目录：`Core/Mechanism/CommandQuery/`。面板读写与开窗门槛看 [UI 业务封装](/02-程序-前/UI业务封装.md)。

**Command 是业务操作入口，Query 是可复用的只读推导。** 这是给业务继承的接口和基类。**不是**第二套 IOC，也**不能**当成和 Event 一样自己 `new` 就能跑通的总线。面板读字段可以直接 `GetModel`，不要每个 getter 一个 Query。

| | Command | Query |
|---|---|---|
| 契约 | `ICommand` / `AbstractCommand` | `IQuery<T>` / `AbstractQuery<T>` |
| 做什么 | 改 Model、发事件；有限制的进功能（过门后自己调 Kit） | 跨处复用的只读推导 |
| 业务 | `class BuyCommand` / `class OpenWndCommand<T>` | `class ShopPriceQuery`（第二处才抽） |
| 怎么跑 | `SendCommand<BuyCommand>()` 或 `SendCommand(cmd)`（带 `Data` 必须走实例） | `this.SendQuery(new ShopPriceQuery())` |
| 接口 | `Execute()` 同步、无返回 | `Do()` 同步、返回 `T` |

`SendCommand` / `SendQuery` 在 Architecture 上：先 `SetArchitecture`，再 `Execute` / `Do`。这样 Command 里才能 `GetModel` / `GetSystem` / `SendQuery` / `SendEvent`。

Query **不要**改 Model、不要发事件。灰态可以 `GetModel`；同一套推导出现第二处再 Query。Command 过门用 `GetModel` 或复用这条 Query。`CfgUtility` 在 Command 里 `GetUtility`，或经 System 再给 Query。

开窗走 [DDoveUIKit](/02-程序-前/DDoveUI.md)。有门槛用一个 `OpenWndCommand<T>`（`Data` 为 `IDDoveUIPanelData`），不要一窗一类，也不要为开窗加只转发的 System。无参 `SendCommand<T>()` 会 `new()`，带不上 `Data`。`Execute` 保持同步；`OpenAsync` 在 Kit，过门后 `Forget()`，见 [异步用 UniTask](/02-程序-前/UniTask异步.md)、[UI 业务封装](/02-程序-前/UI业务封装.md)。

不要：

```csharp
new BuyCommand().Execute(); // 没有 Architecture，GetModel 会空
```

角色谁能发令，仍由 [Architecture 与角色](/02-程序-前/Architecture与角色.md) 的 `ICanSendCommand` / `ICanSendQuery` 决定。第一轮不池化 Command。
