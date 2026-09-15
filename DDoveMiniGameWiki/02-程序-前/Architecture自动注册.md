---
type: Playbook
title: Architecture 自动注册
description: 三类特性收集并生成 GameArchitecture；总窗 Architecture 页配路径、Hotbox 创建三类。不手写游戏根 Init。
tags: [程序-前, architecture]
status: stable
generated: { by: human:cjh, at: 2026-09-11T06:48:00Z }
verified: { by: human:cjh, at: 2026-09-15T02:38:00Z }
sources:
  - id: grill
    resource: /00-索引/Agent/需求稿工作流.md
    title: 需求稿工作流
  - id: arch
    resource: /02-程序-前/Architecture与角色.md
    title: Architecture 与角色
  - id: ioc
    resource: /02-程序-前/IOC容器.md
    title: IOC 容器
  - id: ioc-use
    resource: /02-程序-前/IOC容器使用规范.md
    title: IOC 容器使用规范
  - id: core
    resource: /02-程序-前/DDoveFramework-Core.md
    title: DDoveFramework Core
  - id: editor
    resource: /02-程序-前/DDoveEditor.md
    title: DDove Editor
  - id: ui
    resource: /02-程序-前/DDoveUI.md
    title: DDoveUI
---

# Architecture 自动注册

Concept ID：`/02-程序-前/Architecture自动注册`。清单：[index_cjh](/02-程序-前/index_cjh.md)。角色与 IOC 仍看 [Architecture 与角色](/02-程序-前/Architecture与角色.md)、[IOC 容器](/02-程序-前/IOC容器.md)。几根、谁进游戏根见 [IOC 容器使用规范](/02-程序-前/IOC容器使用规范.md)。结论与代码冲突以代码为准。

对照：[DDoveFramework Core](/02-程序-前/DDoveFramework-Core.md)、[DDove Editor](/02-程序-前/DDoveEditor.md)、[DDoveUI](/02-程序-前/DDoveUI.md)、[IOC 容器使用规范](/02-程序-前/IOC容器使用规范.md)。

## 要什么

业务不再手写 `GameArchitecture` / `Init` / `Register*`。类上挂三类特性，Editor 用 `TypeCache` 收集，生成根类。进 Play 前对清单；对不上就生成、打断 Play、编译完再进。运行时仍是普通 `Init()` + IOC，**不**反射扫程序集。

总窗侧栏 **Architecture** 页只配三类存放路径。创建 Model / System / Utility 和生成根类走 [DDove Editor](/02-程序-前/DDoveEditor.md) 的 **HotBox** 页（或菜单 `DDove/Generate Architecture Bind`）。创建成功后立刻再生成根。

## 特性

放 [Core](/02-程序-前/DDoveFramework-Core.md) `Architecture/`（角色标记，业务能挂）。

| 特性 | 生成 |
|------|------|
| `[DDoveBindModel]` | `RegisterModel(new T())` |
| `[DDoveBindSystem]` | `RegisterSystem(new T())` |
| `[DDoveBindUtility]` | `RegisterUtility(new T())` |
| `[DDoveBindUtility(As = typeof(IAd))]` | `RegisterUtility<IAd>(new T())` |

规则：

- 只扫**运行时**程序集。排除 Editor。
- 凡挂了特性的都收，不限 `Game` 程序集。
- 非抽象、公有、无参构造。
- 不合格、或同一键收两次：生成**失败、不写文件**。
- Controller / 面板不挂、不进 IOC。
- Kit、Boot 不参与注册。
- 闭环内部件**不要**挂特性，否则会进 `GameArchitecture.Generated`。独立根手写 `Init()`，见 [IOC 容器使用规范](/02-程序-前/IOC容器使用规范.md)。生成器多根本篇不做。

`As` 只出现在 Utility。不写 `As` 按具体类型当键，与 [IOC 容器](/02-程序-前/IOC容器.md) 一致。

## 生成物

类名仍是 `Game.GameArchitecture`（`GameLaunch` 继续碰 `Interface`）。只是文件名带 `.Generated`。

```
Assets/Game/Generate/Core/GameArchitecture.Generated.cs
```

文件内完整 `class GameArchitecture : Architecture<GameArchitecture>` 和 `Init()`。顺序：先全部 Utility，再 Model，再 System。零条特性也生成空 `Init`，保证根类型在。

不要手写 `GameArchitecture`。不开放业务 `Init`。测试替换继续 `OnRegisterPatch`。

生成器放 `DDoveFramework.Editor`，`TypeCache` 写文件，**不要**引用 `Game`。`Architecture<T>` 本身不扫类型。

UI 绑定在 `Assets/Game/Generate/UI`，与根类同挂 `Generate/`。

## 总窗

`[DDoveEditorPanel("architecture", "Architecture", 150)]`，挂在 [DDove Editor](/02-程序-前/DDoveEditor.md) 侧栏。侧栏顺序只改 `DDoveEditorNav.Ids`（当前 HotBox / Architecture / Res / UI）。实现放 `DDoveFramework.Editor`。**不要**再开独立 `EditorWindow`。**不要**引用 `DDoveUI`。

页内只改路径 SO。创建指令不在这页重复列。

### 配置

一份 Editor-only SO（学 `DDoveHotboxConfig`）：不进 `Resources/`、不进收集器、不进运行时包。三字段：

| 字段 | 默认 |
|------|------|
| Model 路径 | `Assets/Game/Model` |
| System 路径 | `Assets/Game/System` |
| Utility 路径 | `Assets/Game/Utility` |

业务手写类和 `Generate/` 分开。

### HotBox

创建 Model / System / Utility 和「生成根类」挂 `[DDoveHotboxEntry]`，`group = Architecture`。总窗 **HotBox** 页收录并编进饼环。默认环「UI」「架构」。不要在 Architecture 页再列一遍按钮。

点创建：UITK 弹窗居中。填 `{角色}Name` 和**模块**（在配置路径下建模块文件夹）；类名自动补 `Model` / `System` / `Utility`（已带后缀不重复加）。Utility 多一个可选 `As`。写出命名空间 `Game`、带对应特性的空类。例如模块 `Login`、名称 `Player` → `Assets/Game/Model/Login/PlayerModel.cs`。文件已存在则失败、不覆盖。成功后立刻跑绑定生成；生成失败仍不覆盖旧根。Editor 工具默认 UIToolkit，见 [DDove Editor](/02-程序-前/DDoveEditor.md)。

「生成根类」（旧称「生成绑定」）：按三类特性重写 `GameArchitecture`，不是 UI NodeBind。菜单兜底 `DDove/Generate Architecture Bind`。

## 何时生成

1. **进 Play**：比特性集合（类型 + 角色 + 键）和生成清单。不是只比条数。不一致则生成、取消本次 Play，等编译结束自动再进。
2. **菜单兜底**：`DDove/Generate Architecture Bind`。生成失败修完特性后可再点，不必为了重试去进 Play。
3. **创建三类成功后**立刻生成。
4. 生成 `.cs` 会触发编译，同一帧不能带着新代码进 Play。

运行时没有 `GetTypes` / `Activator`。`TypeCache` 和集合比对只在 Editor。

## 不要

- 在 `Architecture<T>` 里运行时扫类型
- 手写第二份 `GameArchitecture`，或把玩法 `Register` 手写进游戏根
- 把 `DDoveUIKit` / Res / Yoo 注册进游戏 IOC
- Architecture 页引用 `DDoveUI`
- 造 `HotUpdate/` 程序集（本库没有 HybridCLR）

## 验收

绑定生成（已有代码）：

1. 手写 `GameArchitecture.cs` 已删；`GameLaunch` 仍能 `GameArchitecture.Interface`。
2. 挂三类特性各一个，生成文件出现对应 `Register*`；Utility 带 `As` 时生成接口键。
3. 抽象类 / 无参构造缺失 / 键重复：报错且不覆盖旧生成文件。
4. 改特性集合后直接 Play：先生成、打断、编译后再进；清单与特性一致。
5. Editor 程序集挂特性：不进清单。
6. 没装业务 Model 时仍生成空 `Init`，Play 不缺类型。

总窗 / 创建（已落地）：

7. 侧栏有 Architecture；页内只改三个路径，写进 Editor-only SO。
8. HotBox 能创建三类；类在 `配置路径/模块/` 下、类名自动带角色后缀、带特性、命名空间 `Game`。重名失败。
9. 创建成功后生成根已含新类型。
10. Space 饼环在 HotBox 页编；Architecture 页不再列创建按钮。
11. 菜单 `DDove/Generate Architecture Bind` 仍可用。
