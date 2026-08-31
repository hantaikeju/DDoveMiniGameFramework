# 02-程序-前

客户端：运行时、UI、表现、平台 SDK、出包。

# 概念

* [EUFramework Core](EUFramework-Core.md) - Architecture + Mechanism（Event / CommandQuery / Fsm）。 `Reference`
* [IOC 容器](IOC容器.md) - 按注册泛型类型存一份实例；找不到抛错。不是构造注入框架。 `Reference`
* [Architecture 与角色](Architecture与角色.md) - GameArchitecture 注册 Model/System/Utility；谁能发令。 `Reference`
* [TypeEvent](TypeEvent.md) - 独立事件机制。Architecture 与业务都可用。 `Reference`
* [Command 与 Query](CommandQuery.md) - 一次写 / 一次读。业务写子类，执行走 Architecture。 `Reference`
* [Core Fsm](Core工具.md) - 分步流程状态机。Architecture 不依赖。 `Reference`
