---
type: Playbook
title: Luban 填表
description: 策划在 DDoveMiniGameConfig/Data 填 csv。Agent 和 Excel / WPS 都改这一份；生成与加载看客户端篇。
tags: [策划, luban]
status: stable
generated: { by: human:cjh, at: 2026-09-12T08:20:00Z }
verified: { by: human:cjh, at: 2026-09-12T08:24:00Z }
sources:
  - id: grill
    resource: /00-索引/Agent/需求稿工作流.md
    title: 需求稿工作流
  - id: cfg
    resource: /02-程序-前/DDoveCfg.md
    title: DDoveCfg
  - id: srv
    resource: /03-程序-后/Luban服务端.md
    title: Luban 服务端
  - id: luban
    resource: https://www.datable.cn/docs/intro
    title: 什么是 Luban
  - id: cfg-skill
    resource: /00-索引/Agent/ddove-config.md
    title: ddove-config
---

# Luban 填表

Concept ID：`/01-策划/Luban填表`。清单：[index_cjh](/01-策划/index_cjh.md)。第一刀已落地。生成命令、程序集、启动顺序以 [DDoveCfg](/02-程序-前/DDoveCfg.md) 为准，不要在本篇复制。

服务端何时出表：[Luban 服务端](/03-程序-后/Luban服务端.md)。

## 要什么

策划在 `DDoveMiniGameConfig/Data/` 填表。定义和数据都在这里，前后端共用一份。本刀只维持**一张**示例表（item / `Tbitem`）能生成、能在客户端 `Get` 到。

源表用 **csv**（UTF-8 BOM，Excel / WPS 能直接开）。人和 Agent 改同一份。Agent 改表走 [ddove-config](/00-索引/Agent/ddove-config.md)。双击 `gen_client.bat` 交给 Luban，不要另存成 xlsx 当源。不要用 `#` 开头的文件名。`__beans__` / `__enums__` 的第二行是嵌套列名，不要压成单行表头。

本刀示例 `demo.item.csv`：主键 `id`，字段 `name` / `desc` / `count`；`Get(1001)` 为道具1。

### 表头

| 行 | 含义 |
|----|------|
| `##var` | 字段名。A1 必须是 `##` 开头，否则整表忽略 |
| `##type` | 类型，如 `int` / `string` |
| `##group` | `c` / `s` / `e`；空=跟随默认 |
| `##` | 中文说明，不进逻辑 |
| `#` 开头列名 | 注释列，不导出 |

## 目录

| 路径 | 谁改 |
|------|------|
| `DDoveMiniGameConfig/Data/` | `__tables__.csv` / `__beans__.csv` / `__enums__.csv` + 业务数据本刀 `demo.item.csv`。不要 `#` 开头 |
| `DDoveMiniGameConfig/Defines/` | 程序：XML schema（若用） |
| `DDoveMiniGameConfig/luban.conf` | 程序：groups / targets。不要当填表手册改 |

不要在 `DDoveMiniGameClient/Assets/` 里另放一份表。

## 怎么加表（本刀之后才铺）

1. `Data/` 里加 csv（文件名不要 `#` 开头）。Excel 请用「CSV UTF-8」存回，不要另存 xlsx 当源。
2. 在 `__tables__.csv` 登记表名、`input`、索引。`read_schema_from_file` 为 true 时，字段写在数据表头，不要再抄一份到 `__beans__`。
3. 双击 `DDoveMiniGameConfig/gen_client.bat`。生成失败先看报错，不要手改 `Generate/Luban`。
4. 客户端按表文件名当 Yoo location 加载。改表名要跟程序说，location 会变。

本刀不要求策划一次加业务数值表。示例表字段保持模板能跑即可。

## 验收（策划侧）

1. 改示例表一个字段 → 导表 → Play 里读到新值。
2. 填错类型 / 缺引用时，生成期报错，不要 silently 出空表。
3. 不要把导出 json 当策划源文件改。源永远是 `Data/`。

## 不要

- 在 Unity `GameRes/Cfg` 或 `Generate/Luban` 里改「表」
- 为客户端、服务端各维护一份表
- 用 `#xxx.xlsx` 当示例文件名（IDE 打不开）
- 本刀铺技能 / 关卡 / 经济全套表
