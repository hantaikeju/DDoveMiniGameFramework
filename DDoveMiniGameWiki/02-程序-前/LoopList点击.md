---
type: Playbook
title: LoopList 点击
description: LoopList.AddClick 把格子根按钮的点击交到外部，下标在点击当时读。非空声音名经 ClickSound.SetSoundName 挂或改一份。
tags: [程序-前, ddoveui]
status: stable
generated: { by: human:cjh, at: 2026-09-25T19:17:00Z }
verified: { by: human:cjh, at: 2026-09-25T19:17:00Z }
sources:
  - id: list
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/Scroll/LoopList.cs
    title: LoopList.cs
  - id: sound
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/Sound/ClickSound.cs
    title: ClickSound.cs
  - id: demo
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/WndLoopDemo.cs
    title: WndLoopDemo.cs
  - id: row
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/DemoRow.cs
    title: DemoRow.cs
  - id: loop
    resource: /02-程序-前/LoopList.md
    title: LoopList
  - id: ui-biz
    resource: /02-程序-前/UI业务封装.md
    title: UI 业务封装
  - id: req
    resource: /05-需求/LoopList点击.md
    title: LoopList 点击（需求稿）
---

# LoopList 点击

Concept ID：`/02-程序-前/LoopList点击`。清单：[index_cjh](/02-程序-前/index_cjh.md)。列表其余行为看 [LoopList](/02-程序-前/LoopList.md)。点击音门槛看 [UI 业务封装](/02-程序-前/UI业务封装.md)。

[三块](LoopList点击.html)

## 干什么

`LoopList.AddClick` 把格子根上 `Button` 的点击交到外部，回调收到这次 `Bind` 写下的下标。下标在点击当时读。再调一次替掉上一次的动作和记下的声音名。

传入非空声音名时，露出的格子保证有一份 `ClickSound`，名字用 `SetSoundName` 写入。已经有一份就改名字。播放仍由 `ClickSound` 调 `PlaySound`。`LoopList` 不调用 `PlaySound`。声音名为空时不加 `ClickSound`，也不改格子上已有的那份。没有根上 `Button` 的格子不挂这次点击。横竖共用这一套。

`DemoRow` 的 `Awake` 只缓存文本，不再自己挂 `onClick`。竖样板 `WndLoopDemo` 在 `Create` 之后调用 `AddClick`，打出 `[WndLoopDemo] row ` 加下标，不传声音名。面板 `AddClick` 的签名不变。

## 有哪些接口

- `LoopList.AddClick(Action<int> onClick, string soundName = null)`
- `ClickSound.SetSoundName(string value)`

## 每个接口干什么

`LoopList.AddClick` 记下这一次的动作和声音名。格子根 `Button` 只挂一次监听。点击时读该格子当前的下标（`ShowCell` 在 `Bind` 之后写入）。再调一次只留新动作和新的声音名，同一次点击只跑一次。

`soundName` 非空时，对当前露出的格子取一份 `ClickSound`，没有才挂上，然后 `SetSoundName`。之后新露出的格子按当前声音名同样处理。`soundName` 为空则这两步都不做。

`ClickSound.SetSoundName` 写入这一份组件的声音名。左键点击时仍由 `ClickSound` 自己调用 `PlaySound`。名字为空则不播。

竖样板不传声音名：`_list.AddClick(index => Debug.Log("[WndLoopDemo] row " + index))`。
