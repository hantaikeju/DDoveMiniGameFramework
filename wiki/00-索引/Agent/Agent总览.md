---
type: Reference
title: Agent 总览
description: 工程 wiki 与 AI 用法 wiki 分轨。仓库 ddove-* 与个人 Cursor skill 的地图。
tags: [索引, agent]
status: stable
generated: { by: human:cjh, at: 2026-09-09T08:16:00Z }
verified: { by: human:cjh, at: 2026-09-09T08:42:00Z }
sources:
  - id: okf
    resource: /00-索引/OKF约定.md
    title: OKF 约定与检索
  - id: agents
    resource: ../../AGENTS.md
    title: AGENTS.md
---

# Agent 总览

Concept ID：`/00-索引/Agent/Agent总览`。约定：[OKF 约定](/00-索引/OKF约定.md)。入口：仓库根 `AGENTS.md`（短指针，不入库）。

## 两层

| 问的是 | 检索 | 落篇 |
|--------|------|------|
| 本工程（DDoveRes / Boot / Editor…） | `wiki/01`–`04` | 职种目录，`tags` 跟职种 |
| 整个 AI 怎么用（skill / grill / 个人 Cursor） | `wiki/00-索引/Agent` | 本目录，`tags: [索引, agent]` |

仓库 skill：`.agents/skills/ddove-*/SKILL.md`，Cursor 同步 `.cursor/skills/`。个人 skill：`~\.cursor\skills\`，不进 Git。

## 仓库 skill

| 调用 | 干什么 | SKILL.md |
|------|--------|----------|
| `/ddove-help` | 整套工作流指路，只提示不代跑 | `.agents/skills/ddove-help/SKILL.md` |
| `ddove-wiki` | 只检索 wiki，交出命中列表 | `.agents/skills/ddove-wiki/SKILL.md` |
| `ddove-work` | 问答 / 排障 / 改代码（先跑 wiki 检索） | `.agents/skills/ddove-work/SKILL.md` |
| `/ddove-grill` | 拷问对齐；可选落 `draft`+`需求` | `.agents/skills/ddove-grill/SKILL.md` |
| `/ddove-wiki-create` | 新建概念（工程或本目录） | `.agents/skills/ddove-wiki-create/SKILL.md` |
| `/ddove-wiki-change-state` | `draft`/`stable`/`deprecated`；入正式库 | `.agents/skills/ddove-wiki-change-state/SKILL.md` |
| `ddove-okf-upgrade` | 换 SPEC 后合规 | `.agents/skills/ddove-okf-upgrade/SKILL.md` |
| `ddove-writing-for-agents` | 写或改 `ddove-*` | `.agents/skills/ddove-writing-for-agents/SKILL.md` |

需求稿步骤见 [需求稿工作流](/00-索引/Agent/需求稿工作流.md)。换引擎见 [工作流迁移](/00-索引/Agent/工作流迁移.md)。上游 Release 见 [上游 skill 更新](/00-索引/Agent/上游skill更新.md)。

## 个人 skill（不进仓库）

| 调用 | 干什么 | 篇 |
|------|--------|-----|
| `/show-me` | 把当前话题画清楚 | [show-me 安装](/00-索引/Agent/show-me安装.md) |
