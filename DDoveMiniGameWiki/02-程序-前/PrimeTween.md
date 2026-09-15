---
type: Playbook
title: PrimeTween
description: 接入免费 PrimeTween 1.4.x。只装包；Game 业务引用。DDoveUI Base 本刀不引用、不做 ClickScale。
tags: [程序-前, primetween]
status: stable
generated: { by: human:cjh, at: 2026-09-11T07:33:00Z }
verified: { by: human:cjh, at: 2026-09-15T03:20:00Z }
sources:
  - id: grill
    resource: /00-索引/Agent/需求稿工作流.md
    title: 需求稿工作流
  - id: upm
    resource: /02-程序-前/UPM落地.md
    title: UPM 落地
  - id: unitask
    resource: /02-程序-前/UniTask异步.md
    title: 异步用 UniTask
  - id: core
    resource: /02-程序-前/DDoveFramework-Core.md
    title: DDoveFramework Core
  - id: ui
    resource: /02-程序-前/DDoveUI.md
    title: DDoveUI
  - id: arch-auto
    resource: /02-程序-前/Architecture自动注册.md
    title: Architecture 自动注册
  - id: github
    resource: https://github.com/KyryloKuzyk/PrimeTween
    title: KyryloKuzyk/PrimeTween
---

# PrimeTween

Concept ID：`/02-程序-前/PrimeTween`。清单：[index_cjh](/02-程序-前/index_cjh.md)。结论与代码冲突以代码为准。

对照：[UPM 落地](/02-程序-前/UPM落地.md)、[异步用 UniTask](/02-程序-前/UniTask异步.md)、[DDoveUI](/02-程序-前/DDoveUI.md)、[DDoveFramework Core](/02-程序-前/DDoveFramework-Core.md)。

库：[KyryloKuzyk/PrimeTween](https://github.com/KyryloKuzyk/PrimeTween) 免费线。不上 [PrimeTween PRO](https://assetstore.unity.com/packages/tools/animation/primetween-pro-code-free-animations-373496)。

## 要什么

本刀只做两件事：把包装进工程，让 **UI 业务层**能写 `Tween.xxx` / `await tween`。

不做 ClickScale、不开合窗位移动画、不包 `ITween`。

## 分层

| 层 | 是谁 | 本刀 |
|---|---|---|
| Base | `DDoveUI`（Kit / `DDoveUIPanelBase`） | **不**引用 `PrimeTween.Runtime` |
| UI 业务 | 现有 `Game`（`Assets/Game/UI`，如 `WndHome`） | **`Game` 引用 `PrimeTween.Runtime`** |

不新建 `Game.UI` 程序集。Core / Boot / Res / Editor **不**引用。不进 IOC，不进 [Architecture 自动注册](/02-程序-前/Architecture自动注册.md)。Tween **不进** [Core](/02-程序-前/DDoveFramework-Core.md)。

业务面板里直接调库，不要再包一层。能 `await` 就直接等，见 [异步用 UniTask](/02-程序-前/UniTask异步.md)。

## 安装

1. Package Manager：Scoped Registry `npm`，URL `https://registry.npmjs.org`，scope `com.kyrylokuzyk`。装 **当前 1.4.x**（以当时 registry 为准，写入文件夹名）。
2. 按 [UPM 落地](/02-程序-前/UPM落地.md) 从 `Library/PackageCache` 整夹拷到：

```
DDoveMiniGameClient/Packages/com.kyrylokuzyk.primetween@版本/
```

3. `Packages/manifest.json` **留下**远程条目（`com.kyrylokuzyk.primetween` + scopedRegistries）。不要改成 `file:`。
4. `Assets/Game/Game.asmdef` 的 `references` 增加 `PrimeTween.Runtime`。

`DDoveFramework.Extension.DDoveUI.asmdef` 本刀不加这条。

## 授权（公开仓）

框架开源、不当商品卖。按 `UPM落地` 嵌源码，公开仓与「衍生库只许 NPM 依赖、不许带源码」有纸面缝；知情接受。

- README 写明：PrimeTween 版权归 Kyrylo Kuzyk，本仓库只为离线构建嵌入，不卖、不主张所有权。
- 保留包内 `license.md`，不要改作者名。
- 不要把带源码的框架上 Asset Store 当资源包卖。

私有仓 / 自己出商业游戏二进制：条款允许。

## 不要

- 上 PRO、锁死 1.3.8（除非要对齐旧工程排障）
- Tween 进 Core、包 `ITween`、注册进 IOC
- 本刀改 `AddClick`、做 ClickScale、拆 `Game` 程序集
- `DDoveUI` Base 引用 `PrimeTween.Runtime`
- 只留 Git/npm URL、不拷 `Packages/`（和 [UPM落地](/02-程序-前/UPM落地.md) 冲突）

## 验收

1. `Packages/com.kyrylokuzyk.primetween@1.4.x/` 在，`package.json` 的 `version` 是 1.4.x。
2. `manifest.json` 仍有远程依赖和 `com.kyrylokuzyk` scoped registry。
3. `Game.asmdef` 含 `PrimeTween.Runtime`；`DDoveUI` / Core / Boot / Res 的 asmdef **没有**。
4. `WndHome`（或任意 `Game/UI` 面板）能 `using PrimeTween` 并编译通过。
5. 没装 `DDoveUI` 时 Core / Res / Boot / `DDove/Editor` 仍能编。
