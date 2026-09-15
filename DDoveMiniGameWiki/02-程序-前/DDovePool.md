---
type: Reference
title: DDovePool
description: 静态对象池门面。C# 池与 GameObject 池；默认容量 30。不进 IOC，不进 Core。
tags: [程序-前, ddovepool]
status: stable
generated: { by: human:cjh, at: 2026-09-15T06:30:00Z }
verified: { by: human:cjh, at: 2026-09-15T06:45:00Z }
sources:
  - id: kit
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDovePool/DDovePoolKit.cs
    title: DDovePoolKit.cs
  - id: iface
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDovePool/IDDovePoolInterface.cs
    title: IDDovePoolInterface.cs
  - id: obj-pool
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDovePool/DDoveObjectPool.cs
    title: DDoveObjectPool.cs
  - id: go-pool
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDovePool/DDoveGameObjectPool.cs
    title: DDoveGameObjectPool.cs
  - id: asmdef
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDovePool/DDoveFramework.Extension.DDovePool.asmdef
    title: DDoveFramework.Extension.DDovePool.asmdef
  - id: core
    resource: /02-程序-前/DDoveFramework-Core.md
    title: DDoveFramework Core
  - id: ioc-use
    resource: /02-程序-前/IOC容器使用规范.md
    title: IOC 容器使用规范
  - id: debug
    resource: /02-程序-前/DDoveDebug.md
    title: DDoveDebug
---

# DDovePool

Concept ID：`/02-程序-前/DDovePool`。权威实现：[DDovePoolKit.cs](../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDovePool/DDovePoolKit.cs)。结论与代码冲突以代码为准。

`Extension/DDovePool/` 静态门面。不进 [IOC 容器](/02-程序-前/IOC容器.md)，不进 [DDoveFramework Core](/02-程序-前/DDoveFramework-Core.md)。日志 [DDoveDebug](/02-程序-前/DDoveDebug.md)，`title` 固定 `DDovePool`。

按类型各一份池。对象实现 `IDDovePoolObject`（`OnAcquire` / `OnRelease`）。带场景节点的再实现 `IDDoveGameObjectPoolObject.GetGameObject()`。这不是 Prefab 池：工厂自己造包装对象和 `GameObject`。

## 调用

普通对象不必 `Initialize`。GameObject 池必须先 `Initialize`。

```csharp
public sealed class Bullet : IDDovePoolObject
{
    public void OnAcquire() { }
    public void OnRelease() { }
}

var bullet = DDovePoolKit.Get<Bullet>();
DDovePoolKit.Release(bullet);
```

```csharp
DDovePoolKit.Initialize();
var view = DDovePoolKit.GetGameObject<EnemyView>();
DDovePoolKit.ReleaseGameObject(view);
```

`GetGameObject` **不会** `SetActive(true)`。激活写在 `OnAcquire`。回收时超容则 `Destroy`；未超容则失活、归到 `[Pool] 类型名` 节点下，位置 / 旋转 / 缩放归零。

| 方法 | 行为 |
|------|------|
| `Initialize()` | 没有根则建 `[DDovePool]` + `DontDestroyOnLoad`，挂退出时 `Shutdown`。已有根则 return |
| `Initialize(Transform)` | 用传入根。`null` 抛。已有根 [DDoveDebug](/02-程序-前/DDoveDebug.md) `LogWarning` 并 return |
| `Shutdown()` | 清两套池。只 Destroy **自己建的**根 |
| `Get<T>` / `Release<T>` | 按类型取还。`T : IDDovePoolObject, new()`。`Release(null)` 或重复还：忽略 |
| `Configure<T>(factory, capacity)` | 换该类型的工厂和容量。覆盖已有池 |
| `Prewarm<T>(count)` | 预造，不超过剩余容量。预造走 `OnRelease` |
| `Clear<T>` | 清并摘掉该类型池。C# 侧不 Destroy |
| `TryGetPool<T>` | 还没有建过返回 `false` |
| `GetGameObject<T>` / `ReleaseGameObject<T>` | 同上，但要已 `Initialize`。未 Init：`LogError` 后抛 |
| `ConfigureGameObject` / `PrewarmGameObject` / `ClearGameObject` / `TryGetGameObjectPool` | 对应 GO 池 |
| `GetPoolGameObjectRoot<T>` | 该类型的 `[Pool] T` 节点。还没有该池返回 `null` |
| `IsInitialized` | 是否已有全局根 |
| `DefaultCapacity` | `30`。超容：C# 丢给 GC；GO `Destroy` |

[DDoveBoot](/02-程序-前/DDoveBoot.md) **没有**自动 `Initialize`。只用 C# 池不必在 Boot 里调。

## 程序集

`DDoveFramework.Extension.DDovePool`：只引用 `DDoveFramework.Core`。`autoReferenced: true`。**没有** Editor 面板。

`.meta` **不要手写**。挪已有资源时带着 Unity 生成的 `.meta` 一起挪。

## 不要

- 把池注册进 [游戏 IOC](/02-程序-前/Architecture自动注册.md) 或任何 `Architecture`
- 把对象池塞进 [Core](/02-程序-前/DDoveFramework-Core.md)
- 当 Prefab 池（没有按 location Instantiate）
- 指望 `GetGameObject` 替你 `SetActive(true)`
- 未 `Initialize` 就走 GO 池 API

## 还没有

按资源名 / Prefab 的场景对象池、Editor 面板、Boot 自动 Init。`Shutdown` 只在自建根的 `OnApplicationQuit` 上挂；传入根由调用方自己管。
