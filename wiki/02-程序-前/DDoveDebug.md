---
type: Reference
title: DDoveDebug
description: Core 统一日志。纯字符串，或 title + [Title] (key, value)。Log 受 DEBUG 条件编译。
tags: [程序-前, core, debug]
status: stable
generated: { by: human:cjh, at: 2026-09-03T09:43:00Z }
verified: { by: human:cjh, at: 2026-09-03T09:57:00Z }
sources:
  - id: eudebug
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Core/DDoveDebug.cs
    title: DDoveDebug.cs
  - id: core
    resource: /02-程序-前/DDoveFramework-Core.md
    title: DDoveFramework Core
---

# DDoveDebug

Concept ID：`/02-程序-前/DDoveDebug`。权威实现：[DDoveDebug.cs](../../DDoveMiniGameClient/Assets/DDoveFramework/Core/DDoveDebug.cs)。结论与代码冲突以代码为准。

全程序集都走这一套，不要直接 `UnityEngine.Debug.Log`。放在 `DDoveFramework.Core`，Kit / 业务引用 Core 即可。默认 Unity 引擎，没有 Sink、没有多引擎适配。

## 调用

两套重载。单字符串原样打；带元组的才拼 `[Title] (key, value)`。只有一个 `string` 时走纯字符串，不会包成 `[title]`。

```csharp
DDoveDebug.Log("package not created");
DDoveDebug.Log("DDoveRes", ("location", location), ("package", packageName));
DDoveDebug.LogError("IOC", ("type", typeof(PlayerModel).FullName));
```

控制台：

```
package not created
[DDoveRes] (location, WndHome) (package, DefaultPackage)
[IOC] (type, DDoveFramework.Core.PlayerModel)
```

带 `title` 时后面是 `(key, value)`，门面里拼，调用点不要先 `$"..."`。

| 方法 | 条件编译 | 用途 |
|------|----------|------|
| `Log` / `LogWarning` | `[Conditional("DEBUG")]` | 调试信息。Editor 与 Development Build 有调用；正式包调用点整段剥掉 |
| `LogError` | 无 | 替代「不该 throw 打断」的错误，正式包仍打 |

`DEBUG` 由 Unity 注入（Editor / Development Build），不要手写进 Player Settings 的 Scripting Define Symbols。不要把 `DEBUG` 写进 asmdef `defineConstraints`。

## 和 IOC / 角色

[IOC 容器](/02-程序-前/IOC容器.md) 的 `Get<T>()` 未注册仍抛。不想打断用 `TryGet`，再 `LogError`：

```csharp
if (!this.TryGetModel<PlayerModel>(out var player))
{
    DDoveDebug.LogError("IOC", ("type", typeof(PlayerModel).FullName));
    return;
}
```

`IArchitecture` / `ICanGet*` 已有 `TryGetModel` / `TryGetSystem` / `TryGetUtility`。`Register(null)`、空参数仍抛。

## 程序集

`DDoveFramework.Core.asmdef`：`references` 空，`noEngineReferences: false`（只为 `UnityEngine.Debug`）。不要因此把 Res / UI / UniTask 拉进 Core。
