---
type: Playbook
title: LoopList 点击（需求稿）
description: 薄记录。用法篇才是接口总结。得分留在本篇。
tags: [程序-前, ddoveui, 需求]
status: deprecated
generated: { by: human:cjh, at: 2026-09-25T18:20:00Z }
sources:
  - id: use
    resource: /02-程序-前/LoopList点击.md
    title: LoopList 点击
  - id: list
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/Scroll/LoopList.cs
    title: LoopList.cs
  - id: sound
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/Sound/ClickSound.cs
    title: ClickSound.cs
---

# LoopList 点击（需求稿）

用法篇：[LoopList 点击](/02-程序-前/LoopList点击.md)。

[要什么不要](LoopList点击.html)

## 要什么

`LoopList` 增加 `AddClick(Action<int> onClick, string soundName = null)`。格子根上的 `Button` 被点击时，把这次 `Bind` 写下的下标交给 `onClick`。下标在点击当时读，不在造格子时抓住。再调一次 `AddClick` 替掉上一次的动作和声音名，不叠加。

`soundName` 非空：每个露出的格子保证有一份 `ClickSound`，并把这个名字写进去。已经有一份就改名字，不另挂第二份。播放仍走 `ClickSound` 的 `PlaySound`。`LoopList` 自己不调 `PlaySound`。`soundName` 为空：不加 `ClickSound`，也不改格子上原来就有的那份。

没有根上 `Button` 的格子不挂这次点击。`DemoRow` 去掉 `Awake` 里自己挂的 `onClick`。竖样板窗用 `AddClick` 接原来的行日志，不传声音名。

不改面板 `AddClick`。不新增 `LoopItem` 基类。

## 不要

- 新增 `abstract LoopListItem`，或让 `ILoopItem` 自己去听 `Button`
- `LoopList` 里直接 `PlaySound`
- 改 [UI 业务封装](/02-程序-前/UI业务封装.md) 的面板 `AddClick`
- 每次 `Bind` 再 `AddListener`，把同一次点击叠成多次
- 用造格子时的下标，滑走再点仍报旧行
- 声音名为空时还去加 `ClickSound`，或同一格子挂两份
- 横竖各写一套点击

## 评分

分数记在本节，不另开文件。尺子：开工评分 v11。

| 轮 | 总分 | 过线 | 记录 |
|----|------|------|------|
| 1 | 100 | 是 | 审查 PASS。需求验收 20、Wiki 同步 20、职责边界 20、范围 15、信任表述 15、规格增量 10。条文不变。 |
