---
type: Reference
title: Agent 总览
description: 工程 wiki 与 AI 用法 wiki 分轨。仓库 ddove-* 与个人 Cursor skill 的地图。
tags: [索引, agent]
status: draft
generated: { by: human:cjh, at: 2026-09-09T08:16:00Z }
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
| 本工程用法（DDoveRes / Boot / Editor…） | `DDoveMiniGameWiki/01`–`04` | 职种目录。整份需求做完才把用法篇生成或改写到这里：只写干什么、有哪些接口、每个接口干什么 |
| 处理中的需求稿 | `DDoveMiniGameWiki/05-需求` | 得分记在该篇 `## 评分`。整份做完改为 `deprecated`：留下 `## 要什么`、`## 不要`、`## 评分` 和用法篇链接，不留 `## 验收`、`## 落点`，`title` 仍含「需求稿」，文件不删 |
| 整个 AI 怎么用（skill / grill / 个人 Cursor） | `DDoveMiniGameWiki/00-索引/Agent` | 本目录，`tags: [索引, agent]` |

仓库 skill：`.agents/skills/ddove-*/SKILL.md`，Cursor 同步 `.cursor/skills/`。个人 skill：`~\.cursor\skills\`，不进 Git。

## 仓库 skill

| 调用 | 干什么 | SKILL.md |
|------|--------|----------|
| `/ddove-help` | 整套工作流指路，只提示不代跑 | `.agents/skills/ddove-help/SKILL.md` |
| `ddove-wiki` | 只检索 wiki，交出命中列表 | `.agents/skills/ddove-wiki/SKILL.md` |
| `ddove-work` | 问答 / 排障 / 改代码（先跑 wiki 检索） | `.agents/skills/ddove-work/SKILL.md` |
| `ddove-config` | 改 / 加配表、导表失败（对照 luban/ai，本库改编） | `.agents/skills/ddove-config/SKILL.md` |
| `/ddove-grill` | 拷问对齐；可选落 `draft`+`需求` | `.agents/skills/ddove-grill/SKILL.md` |
| `/ddove-wiki-create` | 新建概念。落在 `05-需求` 时调用 `ddove-show-me` 画要什么 / 不要，开篇链接同名 HTML | `.agents/skills/ddove-wiki-create/SKILL.md` |
| `/ddove-wiki-update` | 已有篇改正文（回 `draft`） | `.agents/skills/ddove-wiki-update/SKILL.md` |
| `/ddove-wiki-change-state` | `draft`/`stable`/`deprecated`。篇不在 `05-需求`：升 `stable` 同步写 `verified`，摘 `需求`。`05-需求` 整份做完：按职种生成或改写用法篇（同一文件名；干什么 / 有哪些接口 / 每个接口干什么；`stable` + `verified`；`tags` 无 `需求`；无 `## 评分`；无「要什么 / 不要 / 验收」；已有同名则改正文，不另开）。需求稿改 `deprecated`，文件不删，正文留下 `## 要什么`、`## 不要`、`## 评分`、用法篇链接和打开同名 HTML 的链接，不留 `## 验收`、`## 落点`，`title` 仍含「需求稿」。整份做完调用 `ddove-show-me` 两次：用法篇旁画三块并加链接，再按最终要什么 / 不要重画需求稿 HTML。职种目录：`策划`→`01-策划`，`程序-前`→`02-程序-前`，`程序-后`→`03-程序-后`，`美术`→`04-美术`，只有 `索引`+`agent`→`00-索引/Agent` | `.agents/skills/ddove-wiki-change-state/SKILL.md` |
| `ddove-okf-upgrade` | 换 SPEC 后合规 | `.agents/skills/ddove-okf-upgrade/SKILL.md` |
| `ddove-show-me` | 被 create / change-state 调用，写一张同名静态 HTML | `.agents/skills/ddove-show-me/SKILL.md` |
| `ddove-writing-for-agents` | 写或改 `ddove-*` | `.agents/skills/ddove-writing-for-agents/SKILL.md` |
| `/ddove-rename-workspace` | fork 开新游戏时改四件套前缀；本框架仓不跑 | `.agents/skills/ddove-rename-workspace/SKILL.md` |

需求稿步骤见 [需求稿工作流](/00-索引/Agent/需求稿工作流.md)。换引擎见 [工作流迁移](/00-索引/Agent/工作流迁移.md)。fork 改四件套见 [改工作区前缀](/00-索引/Agent/改工作区前缀.md)。上游 Release 见 [上游 skill 更新](/00-索引/Agent/上游skill更新.md)。

## 个人 skill（不进仓库）

| 调用 | 干什么 | 篇 |
|------|--------|-----|
| `/show-me` | 把当前话题画清楚 | [show-me 安装](/00-索引/Agent/show-me安装.md) |
