---
type: Reference
title: Command 与 Query
description: 一次写 / 一次读的契约。业务写子类；执行仍走 Architecture，不是独立总线。
tags: [程序-前, core, command, query]
status: stable
generated: { by: human:cjh, at: 2026-08-31T01:48:00Z }
verified: { by: human:cjh, at: 2026-08-31T03:36:00Z }
sources:
  - id: cmd
    resource: ../../DDoveMiniGameClient/Assets/EUFramework/Core/Mechanism/CommandQuery/Command.cs
    title: Command.cs
  - id: query
    resource: ../../DDoveMiniGameClient/Assets/EUFramework/Core/Mechanism/CommandQuery/Query.cs
    title: Query.cs
---

# Command 与 Query

Concept ID：`/02-程序-前/CommandQuery`。目录：`Core/Mechanism/CommandQuery/`。

这是「一次写 / 一次读」的接口和基类，给业务继承。**不是**第二套 IOC，也**不能**当成和 Event 一样自己 `new` 就能跑通的总线。

| | Command | Query |
|---|---|---|
| 契约 | `ICommand` / `AbstractCommand` | `IQuery<T>` / `AbstractQuery<T>` |
| 做什么 | 一次写（改 Model、再发事件） | 一次读（不改数据） |
| 业务 | `class BuyCommand : AbstractCommand` | `class GoldQuery : AbstractQuery<int>` |
| 怎么跑 | `this.SendCommand<BuyCommand>()` 或 `SendCommand(cmd)` | `this.SendQuery(new GoldQuery())` |

`SendCommand` / `SendQuery` 在 Architecture 上：先 `SetArchitecture`，再 `Execute` / `Do`。这样 Command 里才能 `GetModel` / `SendEvent`。

不要：

```csharp
new BuyCommand().Execute(); // 没有 Architecture，GetModel 会空
```

角色谁能发令，仍由 [Architecture 与角色](/02-程序-前/Architecture与角色.md) 的 `ICanSendCommand` / `ICanSendQuery` 决定。第一轮不池化 Command。
