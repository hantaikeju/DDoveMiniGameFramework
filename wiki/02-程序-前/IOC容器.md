---
type: Reference
title: IOC 容器
description: EU Core 按注册泛型类型存一份实例；找不到抛错。不是构造注入框架。
tags: [程序-前, core, ioc]
status: stable
generated: { by: human:cjh, at: 2026-08-28T11:13:00Z }
verified: { by: human:cjh, at: 2026-08-31T03:36:00Z }
sources:
  - id: ioc-src
    resource: ../../DDoveMiniGameClient/Assets/EUFramework/Core/Architecture/IOCContainer.cs
    title: IOCContainer.cs
---

# IOC 容器

Concept ID：`/02-程序-前/IOC容器`。权威实现：[IOCContainer.cs](../../DDoveMiniGameClient/Assets/EUFramework/Core/Architecture/IOCContainer.cs)。结论与代码冲突以代码为准。

`EUFramework.Core` 的实例仓库。Architecture / GetModel / GetSystem 只会调它。业务不要直接 new 一份当全局服务定位器。

## 做什么

| 能力 | 行为 |
|------|------|
| `Register<T>(instance)` | 键是 `typeof(T)`。同类型再注册则覆盖。`instance == null` 抛 `ArgumentNullException` |
| `Get<T>()` | 取已注册实例。未注册或类型对不上抛 `InvalidOperationException` |
| `TryGet<T>(out T)` | 可选读取，未注册返回 `false` |
| `Contains<T>()` | 是否已注册该键 |
| `Unregister<T>()` | 只删该键 |
| `Clear()` | 清空，留给 Architecture 重置 |

键是**注册时写的泛型 T**，不是实例的运行时类型。

```csharp
container.Register<Foo>(new Foo());
container.Get<Foo>();          // 成功
container.Get<IFoo>();         // 抛：没按 IFoo 注册

container.Register<IFoo>(new Foo());
container.Get<IFoo>();         // 成功
container.Get<Foo>();          // 抛：没按 Foo 注册
```

当前约定：按**具体类型**注册（与旧 EU Architecture 的 `RegisterSystem<TSystem>` 一致）。不要先抽一层业务接口再当键。

## 不做什么

不要加构造注入、`Register<T>(Func<T>)`、瞬态/作用域 lifetime、按字符串取实例、线程安全。全是 Architecture 级一份实例，主线程用。

效果库、YooAsset、广告不要进这个容器当「服务」。那些走 Kit 或业务 Utility。

## 程序集

路径：`DDoveMiniGameClient/Assets/EUFramework/Core/Architecture/`。

- `asmdef`：`EUFramework.Core`，`rootNamespace: EUFramework.Core`
- `references` 空
- `noEngineReferences: true`（纯 C#，不引用 UnityEngine）

## 验收

1. `Register<A>(a)` 后 `Get<A>()` 为同一引用
2. 再 `Register<A>(a2)` 后 `Get<A>()` 为 `a2`
3. 未注册 `Get<B>()` 抛
4. `Register<A>(null)` 抛
5. `Clear()` 后 `Get<A>()` 抛
6. `TryGet` 未注册为 `false`，不抛

Architecture 已落地，见 [Architecture与角色](/02-程序-前/Architecture与角色.md)。业务通过 `RegisterModel` / `GetModel` 用这份容器，不要自己 `new IOCContainer()`。
