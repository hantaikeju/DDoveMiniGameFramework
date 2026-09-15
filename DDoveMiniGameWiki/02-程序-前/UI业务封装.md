---
type: Playbook
title: UI 业务封装
description: 做窗到写业务的步骤，以及开窗 / 读写 / Tween / 点击音门槛。业务直调 Kit。可复用挂件放 Game/Mono 并按种类拆，不进 DDoveUI。不按窗口加 Command，不写只转发 Kit 的 System。
tags: [程序-前, ddoveui, command]
status: stable
generated: { by: human:cjh, at: 2026-09-15T06:08:00Z }
verified: { by: human:cjh, at: 2026-09-15T09:22:00Z }
sources:
  - id: grill
    resource: /00-索引/Agent/需求稿工作流.md
    title: 需求稿工作流
  - id: cq
    resource: /02-程序-前/CommandQuery.md
    title: Command 与 Query
  - id: ui
    resource: /02-程序-前/DDoveUI.md
    title: DDoveUI
  - id: play
    resource: /02-程序-前/Play到WndHome.md
    title: Play 到 WndHome
  - id: ondemand
    resource: /02-程序-前/DDoveRes按需加载.md
    title: DDoveRes 按需加载
  - id: atlas
    resource: /02-程序-前/DDoveAtlas.md
    title: DDoveAtlas
  - id: pool
    resource: /02-程序-前/DDovePool.md
    title: DDovePool
  - id: tween
    resource: /02-程序-前/PrimeTween.md
    title: PrimeTween
  - id: cfg
    resource: /02-程序-前/DDoveCfg.md
    title: DDoveCfg
  - id: roles
    resource: /02-程序-前/Architecture与角色.md
    title: Architecture 与角色
  - id: bind
    resource: /02-程序-前/Architecture自动注册.md
    title: Architecture 自动注册
  - id: unitask
    resource: /02-程序-前/UniTask异步.md
    title: 异步用 UniTask
  - id: ioc-use
    resource: /02-程序-前/IOC容器使用规范.md
    title: IOC 容器使用规范
  - id: event
    resource: /02-程序-前/TypeEvent.md
    title: TypeEvent
  - id: kit
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/DDoveUIKit.cs
    title: DDoveUIKit.cs
  - id: uidata
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/DDoveUIInterface.cs
    title: DDoveUIInterface.cs
  - id: arch-src
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Core/Architecture/Architecture.cs
    title: Architecture.cs
  - id: launch
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/GameLaunch.cs
    title: GameLaunch.cs
  - id: audio
    resource: /02-程序-前/DDoveAudio.md
    title: DDoveAudio
  - id: clicksound
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/Sound/ClickSound.cs
    title: ClickSound.cs
  - id: addclick
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/DDoveUIPanelBase.cs
    title: DDoveUIPanelBase.cs
---

# UI 业务封装

Concept ID：`/02-程序-前/UI业务封装`。清单：[index_cjh](/02-程序-前/index_cjh.md)。制作导出仍看 [DDoveUI](/02-程序-前/DDoveUI.md)。Command / Query 契约仍看 [CommandQuery](/02-程序-前/CommandQuery.md)。装包与程序集看 [PrimeTween](/02-程序-前/PrimeTween.md)。结论与代码冲突以当前代码为准。

对照：[Play 到 WndHome](/02-程序-前/Play到WndHome.md)、[DDoveRes 按需加载](/02-程序-前/DDoveRes按需加载.md)、[DDoveAtlas](/02-程序-前/DDoveAtlas.md)、[DDovePool](/02-程序-前/DDovePool.md)、[DDoveAudio](/02-程序-前/DDoveAudio.md)、[Architecture 自动注册](/02-程序-前/Architecture自动注册.md)、[Architecture 与角色](/02-程序-前/Architecture与角色.md)、[异步用 UniTask](/02-程序-前/UniTask异步.md)、[IOC 容器使用规范](/02-程序-前/IOC容器使用规范.md)、[TypeEvent](/02-程序-前/TypeEvent.md)。

本篇收**按人做事的顺序**和门槛。不抄 Kit 签名、不抄导出树。不按窗口或字段加类。不写只转发 Kit 的 System。

## 总则

改数据走 Command。读：先 `GetModel` / 订事件；同一套推导出现第二处再抽 Query。开窗：无门槛走 `DDoveUIKit`；有门槛或以后可能有门槛走**一个** `OpenWndCommand<T>`，过门后再打 Kit。本页 Tween / 换图留在面板。不要按窗口加 Command 类。

可挂物体的 Game 脚本放 `Game/Mono`（不限于 UI）：[GameLaunch](../../DDoveMiniGameClient/Assets/Game/Mono/GameLaunch.cs)、[`ClickSound`](../../DDoveMiniGameClient/Assets/Game/Mono/Sound/ClickSound.cs)。不进 [DDoveUI](/02-程序-前/DDoveUI.md)，不进 IOC，不挂 `[DDoveBind*]`。不是窗。`Model` / `System` / 面板 Controller 各看各的目录，不要在 `Game` 根再开 `Launch/`。点了干什么、换哪一声仍在面板 `AddClick`。

`Game/Mono` **按种类拆**子目录。命名空间继续 `Game.Mono`，不为子目录加一层。不要按「是不是 UI」分，不要一个 `Mono/UI/` 收所有挂件。只有一份的入口留在 `Mono/` 根：`GameLaunch`，不开 `Launch/`。点击音进 `Sound/`。粒子 / Rect Mask 落地时开 `Particle/`（或 `Mask/`），不要塞进 `Sound/`。Tween 挂件够一组再开 `Tween/`，不要先建空目录。第一份某类也可以直接开种类目录（`ClickSound` 已这样）。

Kit 不进 IOC，和 [DDoveAtlas](/02-程序-前/DDoveAtlas.md) / [DDovePool](/02-程序-前/DDovePool.md) / [DDoveCfg](/02-程序-前/DDoveCfg.md) / [DDoveAudio](/02-程序-前/DDoveAudio.md) 一样。System 该 `GetModel` 并缓存；空 System 只转发赋值或只 `Forget()` 掉 `OpenAsync`，不要写。

## 开发流程

1. **做窗**（已落地）：HotBox「UI」建制作场景 → `UIRoot` 摆控件 + NodeBind → 导出。场景名 = 类名 = Prefab 名 = Yoo location。业务稿 `Game/UI/{阶段}/WndXxx.cs` 已有不覆盖。细则、路径、不要改导出 Prefab：见 [DDoveUI](/02-程序-前/DDoveUI.md)。
2. **收进包**：该阶段收集器 `AddressByFileName`。`Start` 已有。Login / Hall / Common **有资产再加组**，不要扫整个 `UI/`，见 [DDoveRes 按需加载](/02-程序-前/DDoveRes按需加载.md)。
3. **图 / 动 / 表 / 音**（按需，已落地）：换图 [DDoveAtlas](/02-程序-前/DDoveAtlas.md)；动效该面板 `Tween.xxx`，关页停掉，见 [PrimeTween](/02-程序-前/PrimeTween.md)；表经 `CfgUtility`，Launch 已先 Load，见 [DDoveCfg](/02-程序-前/DDoveCfg.md)；默认点击音挂 `ClickSound`，播法见 [DDoveAudio](/02-程序-前/DDoveAudio.md)。
4. **写面板**：只改 `WndXxx.cs` 的 `OnOpen` / `OnShow` / `OnHide` / `OnClose`。生成的 `IController` 不要手改。需要 Model / System 用 HotBox「架构」创建并挂 `[DDoveBind*]`，再生成根，见 [Architecture 自动注册](/02-程序-前/Architecture自动注册.md)。面板不挂 Bind、不进 IOC。
5. **谁打开**：一律 `DDoveUIKit`。`WndHome` 由 `GameLaunch` 打开，见 [Play 到 WndHome](/02-程序-前/Play到WndHome.md)。第二扇窗面板自己 `OpenAsync` / `Navigate` / `Back`。

## 分层

| 层 | 做什么 |
|---|---|
| 面板 `IController` | 展示、绑点击、订事件、读字段、本页 `Tween` / 换图。开窗 / 关 / 导航调 `DDoveUIKit` |
| `Game/Mono` | 可挂物体的脚本。[`GameLaunch`](../../DDoveMiniGameClient/Assets/Game/Mono/GameLaunch.cs) 在根；默认点击音 [`ClickSound`](../../DDoveMiniGameClient/Assets/Game/Mono/Sound/ClickSound.cs) 在 `Sound/`。按种类拆。不挂 Bind、不进 IOC |
| Command | 业务动词：改 Model、发事件。有门槛的开窗：一个 `OpenWndCommand<T>`，过门后 `OpenAsync.Forget()` |
| Query | 跨处复用的只读推导。不负责「能不能开窗」 |
| `DDoveUIKit` | 加载、层、栈、LRU、Open / Close / Navigate / Back。不进 [IOC](/02-程序-前/IOC容器.md) |

不要再加 `UISystem`。缺独立 Model + 对外 Kit，不要另开 Architecture 根，见 [IOC 容器使用规范](/02-程序-前/IOC容器使用规范.md)。

## 谁许开窗

| 谁 | 开窗 |
|---|---|
| `GameLaunch` | `DDoveUIKit.OpenAsync<WndHome>` |
| 面板 | `DDoveUIKit`：`OpenAsync` / `Close` / `NavigateToAsync` / `BackAsync` |
| Command | `SendCommand(new OpenWndCommand<T> { Data })`，过门后 `OpenAsync.Forget()`。不要无参 `SendCommand<T>()`（`new()` 带不上 `Data`） |
| 对局结束 / 协议回来 | 有副作用先 Command，再在 Command 里打 Kit；不要为开窗单写 System |
| Model / Boot | 不开窗 |

## 门槛

| 场景 | 走 | 不要 |
|---|---|---|
| 改金币 / 背包 / 进度 | `SendCommand` | 面板给 Model 字段赋值 |
| 刷新「当前金币」文本 | `GetModel` 或订事件 | 为此新建 `GoldQuery` |
| 售价、是否够买、多面板同一套算法 | Query | 每个面板复制 `if (gold < price)` |
| Command 里过门 | `OpenWndCommand<T>.TryPass`，默认放行；有规则再加 `case` | `CanOpenXxxQuery`；每个 Wnd 占一行空 `case` |
| 返回 / 帮助 / 设置 | 面板 `DDoveUIKit` | `OpenHelpCommand`；无门槛还走 `OpenWndCommand` |
| 未解锁 / 按进度开门 | `OpenWndCommand<WndShop> { Data }` | 一窗一个 `OpenShopCommand`；面板不判直接开 |
| 打开后页签、关卡 id | `IDDoveUIPanelData` 放在 Command.`Data`，进 `OpenAsync` | 无参 `SendCommand<T>()`；改 Kit 签名 |
| Launch / 确定无门槛的第二扇窗 | `DDoveUIKit.OpenAsync` | `OpenHomeCommand`、`UISystem` |
| 按钮脉冲、数字跳动、本页入场 | 该面板 `Tween.xxx`，关页停掉 | `PunchCommand` / `TweenSystem` |
| 默认点击音 | Prefab 挂 `ClickSound`（`IPointerClickHandler` → Kit）。Yoo location 与 `soundName` 一致 | 改 Base `AddClick`；`DDoveUI` 引用 Audio；走 `Button.onClick`（会被 `AddClick` 清掉） |
| 这个钮换音 / 不出声 | 改组件名字，或面板 `AddClick` 里 `PlaySound` | 再挂一个只服务这一钮的 Mono |
| 五扇窗同一套 punch | `Game/Mono` 或静态小帮手（仍不进 IOC） | 为对称进 Architecture |
| 合窗位移、ClickScale | **还没有**，见 [PrimeTween](/02-程序-前/PrimeTween.md)。落地进 `Game/Mono`，够一组开 `Tween/` | 先改 Base `AddClick` / `OpenAsync` 或 Kit；为空开 `Tween/`；塞进 `Sound/` |
| UI 粒子 / Rect Mask | **还没有**。落地开 `Mono/Particle/`（或 `Mask/`） | 写进 [DDoveUI](/02-程序-前/DDoveUI.md) Base；塞进 `Sound/` 或 `Mono/UI/` |

Command 里可以直接 `GetModel` 改完 `SendEvent`，不必再经一层空 System。开窗用法以本篇为准，不要再写「一律 SendCommand → GetSystem」。

## OpenWndCommand

有门槛、或以后可能有门槛：只写**一个** `OpenWndCommand<T>`。`TryPass` 默认 `true`；某扇窗第一次出现规则时再加 `case`，不要预先给每个 Wnd 占空行。规则涨到扣券 + 引导，再把那一支抽成专用 Command。

`Data` 是 `IDDoveUIPanelData`，只服务打开之后的展示。必须 `SendCommand(new OpenWndCommand<WndShop> { Data = … })`。无参 `SendCommand<T>()` 会 `new()`，带不上 `Data`，见 [Architecture.cs](../../DDoveMiniGameClient/Assets/DDoveFramework/Core/Architecture/Architecture.cs)。面板 `OnOpen` 读 `PanelData`。

确定永远无门槛（帮助、返回、Launch）继续直调 Kit。`TryPass` 恒为 true 还对这种窗发 Command，就是空转发，不要写。

## 不要

- 一窗一个 `OpenXxxCommand`（`OpenWndCommand<T>` 除外）
- 一个 getter 一个 Query
- `CanOpenXxxQuery` 专为开窗门槛
- 空 System 只转发 `GetModel().Xxx =` 或只转发 `DDoveUIKit`
- 在 Model↔System 中间再加一层
- 把 `DDoveUIKit` / Tween / `ClickSound` 注册进 IOC
- 包 `ITween`，或 `DDoveUI` Base 引用 PrimeTween / Audio
- 改 `AddClick` 写死默认音；把窗放进 `Game/Mono`；在 `Game` 根再开 `Launch/`；开空的 `Mono/Tween/` / `Mono/Launch/` / `Mono/UI/`；把粒子 Mask 丢进 `Sound/`

## 还没有

合窗位移、ClickScale 仍按 [PrimeTween](/02-程序-前/PrimeTween.md) 本刀不做（以后进 `Game/Mono`，够一组开 `Tween/`，不进 Base）。UI 粒子 Mask 同：进 `Mono/Particle/`，不进 `Sound/` / `DDoveUI`。`OpenWndCommand<T>` 有第一笔「按进度开门」再写，不要先造空壳类。`TryPass` 未落地前不要当已有方法。

## 验收

1. 没有 `UISystem`，没有只 `Forget()` 掉 `OpenAsync` 的 System。
2. 面板没有给 Model 字段赋值。
3. 没有 `CanOpen*Query`，没有一窗一个 `OpenXxxCommand`，没有 `TweenCommand` / `TweenSystem`。有门槛开窗共用 `OpenWndCommand<T>`。
4. 没装 `DDoveUI` 时 Core / Res / Boot / `DDove/Editor` 仍能编。`DDoveUI` 不引用 Audio。
5. [DDoveUI](/02-程序-前/DDoveUI.md) 制作导出、Launch 看见 `WndHome` 的验收仍成立。
6. `GameLaunch` 在 `Game/Mono` 根；`ClickSound` 在 `Game/Mono/Sound/`，`namespace Game.Mono`。没有写进 `AddClick`，没有进 `GameArchitecture`，没有 `Game/Launch/`，没有空的 `Mono/Launch/` / `Tween/` / `UI/`。
