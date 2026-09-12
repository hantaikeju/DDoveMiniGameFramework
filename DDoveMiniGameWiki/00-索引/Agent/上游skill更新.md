---
type: Playbook
title: 上游 skill 更新
description: 对照 mattpocock/skills 的 Release 改编本库 ddove-*。不订阅读包、不 npx update 进本仓。
tags: [索引, agent]
status: draft
generated: { by: human:cjh, at: 2026-09-09T08:52:00Z }
sources:
  - id: upstream
    resource: https://github.com/mattpocock/skills
    title: mattpocock/skills
  - id: releases
    resource: https://github.com/mattpocock/skills/releases
    title: Releases
  - id: wfa
    resource: ../../../.agents/skills/ddove-writing-for-agents/SKILL.md
    title: ddove-writing-for-agents
  - id: map
    resource: /00-索引/Agent/Agent总览.md
    title: Agent 总览
---

# 上游 skill 更新

Concept ID：`/00-索引/Agent/上游skill更新`。上游：[mattpocock/skills](https://github.com/mattpocock/skills)。改本库 skill 用 `ddove-writing-for-agents`。

本库 **不**装 Claude 插件，**不**对这个仓库跑 `npx skills add` / `npx skills update`。那些会写入 `CONTEXT.md` / `docs/agents/`，和 wiki 抢权威。

## 对照

| 上游 | 本库 | 跟不跟 |
|------|------|--------|
| `grilling` / `grill-me` | `/ddove-grill` | 跟问法（frontier 轮次）；出口改 wiki |
| `grill-with-docs` / `domain-modeling` | `DDoveMiniGameWiki/` + create | **不跟**落盘（禁止 `CONTEXT.md` / ADR） |
| `writing-for-agents` | `ddove-writing-for-agents` | 跟写法杠杆；共享语言改 wiki |
| `ask-matt` | `/ddove-help` | 跟「路由器」角色；路由本库命令 |
| `setup` / `to-spec` / `tickets` / `implement` / `wayfinder` / `triage` | — | **不引进** |
| 其余 engineering | — | 默认不跟；要引进先 `/ddove-grill` |

钉一次 commit：本机 `~\.cursor\vendor\mattpocock-skills`（或任意仓外 clone），记下 `PINNED_SHA`。不要进本 Git。

第二上游：[luban/ai](https://github.com/focus-creative-games/luban/tree/main/ai)。六个 `luban-*` **合成**本库 `ddove-config`，不整包拷进 `.agents/skills/`，不引进 `Luban.Agent` / MCP。官方 skill 有问法增量时，用 `ddove-writing-for-agents` 补 `ddove-config`。

## 步骤

1. 看 [Releases](https://github.com/mattpocock/skills/releases)（或 `git fetch` 后 `git log PINNED_SHA..origin/main -- skills/productivity/grill-me skills/productivity/grilling skills/productivity/writing-for-agents`）。
2. 分类：问法 / 完成条件 → 值得跟；写 `CONTEXT.md` / tracker → 不跟；更名 → 只改对照表，本库 `name:` 不改。
3. 值得跟：`@ddove-writing-for-agents` 改对应 `ddove-*`，同步 `.cursor/skills/`。过改写过滤器：`CONTEXT.md`→`DDoveMiniGameWiki/`；issue tracker 删；`ddove-` 前缀保留。
4. 更新本机钉 SHA。本篇对照表若变了，改本篇并记 `_log`。
5. `/ddove-help` 走一遍，确认整图仍对。

完成：本库 skill 已按选定机制补丁；无 `CONTEXT.md`；无整包覆盖。
