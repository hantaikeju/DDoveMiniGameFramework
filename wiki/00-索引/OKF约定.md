---
type: Playbook
title: OKF 约定与检索
description: 本 bundle 按仓库根 SPEC.md 约束：概念 frontmatter、index.md 渐进展开、按 type/tags/status 检索。
tags: [索引, 检索, okf]
status: stable
generated: { by: process:mgf-okf-init, at: 2026-08-28T07:41:00Z }
resource: ../../SPEC.md
sources:
  - id: okf-spec
    resource: ../../SPEC.md
    title: Open Knowledge Format
---

# OKF 约定与检索

本 bundle 遵循仓库根 [SPEC.md](../../SPEC.md)。[^okf-spec] 约定与 SPEC 冲突时以 SPEC 为准，再改本页。

## 结构

| 路径 | 角色 |
|------|------|
| `/index.md` | 只挂分类目录 + 保留名（可含 `okf_version`）。**不列概念** |
| `职种目录/index.md` | 挂**子目录** + 个人清单 `index_<短号>.md`。无 frontmatter |
| `职种目录/index_<短号>.md` | 该人整理的概念清单。`type: Index` |
| `/log.md` | 变更史索引，最新日期在上（SPEC 保留名） |
| `/_log/log_YYYY-MM-DD.md` | 当日明细；非概念 |
| 其它 `*.md` | **概念**：必须有 YAML frontmatter，且含非空 `type` |
| `_tools/` `_spec/` `_log/` `.obsidian/` | 非概念，默认不检索 |
| 仓库根 `SPEC.md` | 规范原文，不是概念，不进 bundle 扫描 |

Concept ID = 相对 `wiki/` 的路径，去掉 `.md`。例如 `/00-索引/OKF约定`。

## 本库 `type`

| type | 用途 |
|------|------|
| `Playbook` | 排查、接入、修复、指南、升级步骤 |
| `Reference` | 架构、接口、总览、地图、协议 |
| `Report` | 自测 / QA / 测试清单 |
| `Skill` | 领域 Agent 流程（正文在 wiki；跨编辑器入口在 `.agents/skills/`） |
| `Stub` | 跳转 stub；正文不是权威，跟 `sources` / 文内链接 |
| `Table` | 超大配表 stub |
| `Index` | 个人清单 `index_<短号>.md`（人读）。职种入口仍是同目录 `index.md` |

未知 `type` 当普通概念，不拒绝。

## 分类：按职种，四个业务目录

`00-索引` 只给 Agent 用。业务文档只进下面四个目录，谁主写就放谁那里。

| 目录 | 放什么 |
|------|--------|
| `01-策划` | 规则、配表、数值、验收 |
| `02-程序-前` | 客户端：运行时、UI、SDK、出包 |
| `03-程序-后` | 服务端：协议、校验、存档 |
| `04-美术` | 命名、尺寸、导入、图集 |

`tags` 与目录一致：`策划` / `程序-前` / `程序-后` / `美术`。

前后都要写的，各放一篇在自己目录，互相链过去，不要在一边复制另一边的正文。

## 信任与生命周期

从 frontmatter **推导**，不另存分数：

- 无 `verified` → **unverified**
- 仅 `process:` / 工具 actor → **machine-confirmed**
- 有 `human:<id>` → **human-reviewed**
- 无 `status` → `stable`；`draft` 未审完；`deprecated` 只留历史
- `now >= stale_after` → 过期，先核对源码/原文

结论与代码冲突时以**当前代码**为准。

## 检索（消费者必须）

1. **拆词**：症状 / 类名 / 协议 / 表名，中英一起。
2. **先看** `/index.md` 认职种目录，再打开该目录 `index.md` → `index_<短号>.md` 点进概念；也可 Grep frontmatter。
3. **Grep** 限定 `wiki/`，优先 frontmatter：

```bash
rg -n -g "*.md" "^type: |^title: |^tags: |^status: |关键词" wiki
rg -n "^type: Playbook" wiki/00-索引
```

4. **读正文**：`type: Stub` 或 `wiki_stub: true` → 跟 `sources[].resource` 与文内权威链接，不要把 stub 当全文。
5. **链接**：优先 bundle 根路径 `/00-索引/OKF约定.md`。
6. **禁区**：`DDoveMiniGameClient/Library`、`PackageCache`、`_tools/`、`_spec/`、`_log/`、仓库根 `SPEC.md`（规范，不当概念检索）。

## 答案怎么写

1. 结论（1–3 句）
2. Wiki 依据：Concept ID + `type` + 要点
3. 信任：unverified / machine-confirmed / human-reviewed；是否过期
4. 代码核对（若有）
5. 下一步

## 生成人（写篇前问一次）

业务概念、整理旧篇、改 `index_<短号>.md` 时，`generated.by` 必须是 **`human:<短号>`**，不能只写 `process:agent`。否则个人清单对不上，后面生成会挂错人。

**本会话还没有生成人：先问，问到再写。同一会话只问一次，后面沿用。** 不要猜短号。用户说「还是 cjh」就继续用。

短号必须能在 [整理人](/00-索引/整理人.md) 对上。表里没有：先问职种，补一行，并在对应职种 `index.md` 挂上 `index_<短号>.md`（没有就建空清单）。

`verified.by: human:<短号>` 仍是人确认过，不要自动写。`process:` 只留给 `mgf-okf-init` / `mgf-okf-upgrade` 改合规字段。

## 新增 / 改写概念

写之前先满足上一节「生成人」。模板里的 `cjh` 换成问到的短号。

```yaml
---
type: Playbook
title: 短标题
description: 一句摘要
tags: [程序-前]
status: draft
generated: { by: human:cjh, at: 2026-08-31T00:00:00Z }
# verified: { by: human:cjh, at: 2026-08-31T00:00:00Z }
sources:
  - id: src
    resource: ../DDoveMiniGameClient/Assets/某脚本.cs
    title: 原文
---
```

不要用 `index.md` / `log.md` 当概念文件名。

加一篇：新增该 `.md`（`generated.by: human:<短号>`），并在**该短号**的 `index_<短号>.md` 补一条。不要改根 `/index.md`。职种 `index.md` 只在新人第一次出现时加一行。当日 `_log` 照常写。`00-索引` 仍可把约定篇直接列在该目录 `index.md`（改的人少）。

替换仓库根 `SPEC.md` 后，主动调用 skill `mgf-okf-upgrade` 做检查与升级。不要在日常问答里改合规字段。

## index 怎么维护（多人）

业务目录（`01`–`04`）三人关联：根 → 职种 `index.md` → `index_<短号>.md` → 篇。SPEC 保留名仍是 `index.md`；个人清单是普通概念，文件名用短号，**不要** `index_1`。

| 文件 | 关联什么 | 何时改 |
|------|----------|--------|
| 根 `/index.md` | 五个职种目录 + `log.md` | 几乎不改 |
| `02-程序-前/index.md` | 子目录 + `index_cjh` 等 | 新人加入、或新建主题子目录 |
| `02-程序-前/index_cjh.md` | cjh 的篇 | **只有 cjh** 加/改名/删自己的篇时改 |

同一职种两个人各改各的 `index_<短号>.md`，不会撞。篇文件仍按主题命名（`EUFramework-Core.md`），`generated.by: human:cjh`。短号花名册：[整理人](/00-索引/整理人.md)。

不要日常「重生成各层 index」。目录挤了再按**主题**拆子目录，不要按人拆目录。

## 变更史（按日分册）

SPEC §9 保留名仍是根 `wiki/log.md`（ISO 日期、最新在上）。本库不把全部条目堆进该文件，以免越来越难翻。

| 文件 | 写什么 |
|------|--------|
| `wiki/log.md` | 只作日期索引：`## YYYY-MM-DD` + 链到当日分册 |
| `wiki/_log/log_YYYY-MM-DD.md` | 当日条目（`**Update**` / `**Add**` 等），**当天新条目插在该日文件顶部** |

新的一天：先建 `wiki/_log/log_YYYY-MM-DD.md`，再在根 `log.md` **最上面**加一节。`_log/` 与 `_tools/` 一样不当概念、不检索。

[^okf-spec]: Open Knowledge Format（仓库根 SPEC.md）
