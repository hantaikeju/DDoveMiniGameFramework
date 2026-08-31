---
type: Reference
title: Architecture 与角色
description: GameArchitecture 注册 Model/System/Utility；Command 写、Query 读、Controller 发令。
tags: [程序-前, core, architecture]
status: draft
generated: { by: human:cjh, at: 2026-08-28T11:40:00Z }
sources:
  - id: arch
    resource: ../../DDoveMiniGameClient/Assets/EUFramework/Core/Architecture/Architecture.cs
    title: Architecture.cs
  - id: roles
    resource: ../../DDoveMiniGameClient/Assets/EUFramework/Core/Architecture/CoreInterface.cs
    title: CoreInterface.cs
---

# Architecture 与角色

Concept ID：`/02-程序-前/Architecture与角色`。权威实现见 `sources`。与代码冲突以代码为准。

业务写一个 `GameArchitecture : Architecture<GameArchitecture>`，在 `Init()` 里 Register。实例存在 [IOC 容器](/02-程序-前/IOC容器.md) 里。业务不要 `new IOCContainer()`。

## 角色

| 角色 | 基类 | 做什么 | 能碰什么 |
|------|------|--------|----------|
| Model | `AbstractModel` | 持数据、发事件 | Utility、SendEvent |
| System | `AbstractSystem` | 编排、缓存 Model 引用 | Model / System / Utility / 事件 |
| Utility | `IUtility` 空标记 | 无状态可替换工具 | 不经 Architecture 找别人 |
| Controller | `IController` | 面板等入口 | 发 Command、订事件、读 Model/System |

Command / Query 的契约在 [CommandQuery](/02-程序-前/CommandQuery.md)。谁能 `SendCommand` / `SendQuery` 由本页角色决定。

默认按**具体类型**注册。只有会换实现（广告、存档）才 `RegisterUtility<IAd>(impl)`。

## 生命周期

1. 首次 `GameArchitecture.Interface` → `Init()` 注册 → 先 `Model.Init` 再 `System.Init`
2. 已启动后再 Register，立即 Init
3. `Architecture<T>.Reset()`：先 System 再 Model 调 `Deinit`，清空 IOC 与事件，丢掉单例

`OnInit` 里把 `GetModel` 结果存字段，不要每帧 Get。`OnDeinit` 默认可空，有订阅再卸。

## 业务怎么挂

```csharp
public class GameArchitecture : Architecture<GameArchitecture>
{
    protected override void Init()
    {
        RegisterUtility(new GameSaveUtility());
        RegisterModel(new PlayerModel());
        RegisterSystem(new PlayerSystem());
    }
}
```

面板实现 `IController`，`GetArchitecture()` 返回 `GameArchitecture.Interface`。

## 不做

Core 不写业务 Model。不把道具列表放进 IOC。Command 第一轮不池化，见 [CommandQuery](/02-程序-前/CommandQuery.md)。
