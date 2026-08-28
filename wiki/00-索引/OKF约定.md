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
| `/index.md` | 根目录清单（可含 `okf_version`，版本从当前 `SPEC.md` 读取） |
| `/*/index.md` | 分类渐进展开，**无** frontmatter |
| `/log.md` | 变更史，最新在上 |
| 其它 `*.md` | **概念**：必须有 YAML frontmatter，且含非空 `type` |
| `_tools/` `_spec/` `.obsidian/` | 非概念，默认不检索 |
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
| `Index` | 分类 README 入口（人读）；机器优先读同目录 `index.md` |

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
2. **先看** `/index.md` 或分类 `index.md`，再打开概念。
3. **Grep** 限定 `wiki/`，优先 frontmatter：

```bash
rg -n -g "*.md" "^type: |^title: |^tags: |^status: |关键词" wiki
rg -n "^type: Playbook" wiki/00-索引
```

4. **读正文**：`type: Stub` 或 `wiki_stub: true` → 跟 `sources[].resource` 与文内权威链接，不要把 stub 当全文。
5. **链接**：优先 bundle 根路径 `/00-索引/OKF约定.md`。
6. **禁区**：`DDoveMiniGameClient/Library`、`PackageCache`、`_tools/`、`_spec/`、仓库根 `SPEC.md`（规范，不当概念检索）。

## 答案怎么写

1. 结论（1–3 句）
2. Wiki 依据：Concept ID + `type` + 要点
3. 信任：unverified / machine-confirmed / human-reviewed；是否过期
4. 代码核对（若有）
5. 下一步

## 新增 / 改写概念

```yaml
---
type: Playbook
title: 短标题
description: 一句摘要
tags: [策划]
status: stable
generated: { by: process:mgf-okf-init, at: 2026-08-28T00:00:00Z }
# verified: { by: human:名字, at: 2026-08-28T00:00:00Z }
sources:
  - id: src
    resource: ../DDoveMiniGameClient/Assets/某脚本.cs
    title: 原文
---
```

人确认后写 `verified.by: human:<id>`。不要用 `index.md` / `log.md` 当概念文件名。

替换仓库根 `SPEC.md` 后，主动调用 skill `mgf-okf-upgrade` 做检查与升级。不要在日常问答里改合规字段。

[^okf-spec]: Open Knowledge Format（仓库根 SPEC.md）
