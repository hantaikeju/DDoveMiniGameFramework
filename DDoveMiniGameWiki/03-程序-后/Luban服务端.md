---
type: Stub
title: Luban 服务端
description: server target 已在 conf 预留。服务端工程未建。正文以客户端篇为准。
tags: [程序-后, luban]
status: stable
wiki_stub: true
generated: { by: human:cjh, at: 2026-09-12T08:20:00Z }
verified: { by: human:cjh, at: 2026-09-12T08:24:00Z }
sources:
  - id: cfg
    resource: /02-程序-前/DDoveCfg.md
    title: DDoveCfg
  - id: fill
    resource: /01-策划/Luban填表.md
    title: Luban 填表
---

# Luban 服务端

Concept ID：`/03-程序-后/Luban服务端`（Stub）。约定已正式。权威篇：[DDoveCfg](/02-程序-前/DDoveCfg.md)。填表：[Luban 填表](/01-策划/Luban填表.md)。

`DDoveMiniGameConfig/luban.conf` 第一刀就留 `client` / `server` 两组。不跑 `gen_server`、不建 `DDoveMiniGameServer/` 也算本刀完成。

以后开服务端：同一份 `Data/`，单独 `gen_server` 出代码和数据；不要再拷一份表。加载与校验写新篇，不要在客户端篇里展开。
