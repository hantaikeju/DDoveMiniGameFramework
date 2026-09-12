---
type: Skill
title: ddove-config
description: 本库配表 Agent 流程。对照 luban/ai 改编，不整包拷官方 luban-*。入口 .agents/skills/ddove-config。
tags: [索引, agent]
status: stable
generated: { by: human:cjh, at: 2026-09-12T09:52:00Z }
verified: { by: human:cjh, at: 2026-09-12T09:55:00Z }
sources:
  - id: skill
    resource: ../../../.agents/skills/ddove-config/SKILL.md
    title: ddove-config SKILL.md
  - id: fill
    resource: /01-策划/Luban填表.md
    title: Luban 填表
  - id: cfg
    resource: /02-程序-前/DDoveCfg.md
    title: DDoveCfg
  - id: map
    resource: /00-索引/Agent/Agent总览.md
    title: Agent 总览
  - id: up
    resource: /00-索引/Agent/上游skill更新.md
    title: 上游 skill 更新
  - id: luban-ai
    resource: https://github.com/focus-creative-games/luban/tree/main/ai
    title: luban/ai
---

# ddove-config

Concept ID：`/00-索引/Agent/ddove-config`。入口：[SKILL.md](../../../.agents/skills/ddove-config/SKILL.md)。地图：[Agent 总览](/00-索引/Agent/Agent总览.md)。工程约定：[Luban 填表](/01-策划/Luban填表.md)、[DDoveCfg](/02-程序-前/DDoveCfg.md)。

对照 [luban/ai](https://github.com/focus-creative-games/luban/tree/main/ai) 的六个 `luban-*`，**合成**本库一个 skill。不整包拷进 `.agents/skills/`。不引进 `Luban.Agent` / MCP（本刀生成器只有 Release `Luban.dll`）。

## 要什么

改 / 加 `DDoveMiniGameConfig/Data/*.csv`，或排查 `gen_client` 失败。人和 Agent、Excel / WPS 都改**同一份** csv（UTF-8 BOM）。

不问框架其它代码（走 `ddove-work`）。不建 wiki 篇。不在 Unity 里 `Process` 导表。

## 源与生成

| 谁改 | 路径 |
|------|------|
| 源 | `DDoveMiniGameConfig/Data/*.csv` |
| 登记 | `__tables__.csv`；结构 `__beans__.csv` / `__enums__.csv`；向量 `Defines/builtin.xml` |
| 生成器 | `DDoveMiniGameConfig/Tools/Luban/`（v5.1.0） |
| 命令 | 双击 `gen_client.bat`（或 `.sh`），target `client`，`cs-simple-json` + `json` |
| 生成物 | `Assets/Game/Generate/Luban/`、`Assets/GameRes/Cfg/`（导表会清空这两处） |

不要用 `#` 开头文件名。不要另存 xlsx 当源。不要手改生成物。

## 分支（只走一条）

与 SKILL 一致：填表 / 加表 / schema / 校验 / 导表失败 / 运行时。运行时只对照 [DDoveCfg](/02-程序-前/DDoveCfg.md)（`DDoveCfgKit.LoadAsync` 再 `Interface`），不新造 loader。

## 不要

- 把官方 `luban-add-table` 等六个文件夹原样丢进本仓
- 为通过生成削弱校验
- 拷第二份表给服务端
- 当问答五段用（那是 `ddove-work`）

## 维护

官方 [luban/ai](https://github.com/focus-creative-games/luban/tree/main/ai) 有问法增量：`ddove-writing-for-agents` 补 `ddove-config`，同步 `.cursor/skills/`。见 [上游 skill 更新](/00-索引/Agent/上游skill更新.md)。
