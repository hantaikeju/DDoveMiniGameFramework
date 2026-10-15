---
type: Playbook
title: 上游 skill 更新
description: 长期对照 mattpocock/skills 与 pstack。人看到更新后再 grill。不自动拉远程，不整包装进。
tags: [索引, agent]
status: stable
generated: { by: human:cjh, at: 2026-09-09T08:52:00Z }
verified: { at: 2026-10-15T07:50:00Z }
sources:
  - id: matt
    resource: https://github.com/mattpocock/skills
    title: mattpocock/skills
  - id: pstack
    resource: https://github.com/cursor/plugins/tree/main/pstack
    title: pstack
  - id: luban
    resource: https://github.com/focus-creative-games/luban/tree/main/ai
    title: luban/ai
  - id: req
    resource: /_archive/需求/上游skill更新.md
    title: 上游 skill 更新（需求稿）
  - id: wfa
    resource: ../../../.agents/skills/ddove-writing-for-agents/SKILL.md
    title: ddove-writing-for-agents
  - id: map
    resource: /00-索引/Agent/Agent总览.md
    title: Agent 总览
---

# 上游 skill 更新

Concept ID：`/00-索引/Agent/上游skill更新`。改本库 skill 用 `ddove-writing-for-agents`。

[打开说明](上游skill更新.html)

## 干什么

长期对照两个远程。人看到更新之后，再用 `/ddove-grill` 决定跟哪一条。不自动拉远程，不整包装进，不写 `CONTEXT.md`。

- [mattpocock/skills](https://github.com/mattpocock/skills)
- [pstack](https://github.com/cursor/plugins/tree/main/pstack)

[luban/ai](https://github.com/focus-creative-games/luban/tree/main/ai) 仍是配表上游：六个 `luban-*` 合成 `ddove-config`，不整包拷进 `.agents/skills/`，不引进 `Luban.Agent` / MCP。官方 skill 有问法增量时，用 `ddove-writing-for-agents` 补 `ddove-config`。

一次一个需求、篇内按任务图并行，仍以 [开工班组](/00-索引/Agent/开工班组.md) 为准。

## 有哪些接口

- 对照 mattpocock/skills
- 对照 pstack
- 对照 luban/ai
- 引进一条

## 每个接口干什么

对照 mattpocock/skills：人看到 [Release](https://github.com/mattpocock/skills/releases) 或本机已有下载里的更新。不跑 `npx skills add` / `npx skills update`，那些会写入 `CONTEXT.md` / `docs/agents/`。当前跟不跟：

| 上游 | 本库 | 跟不跟 |
|------|------|--------|
| `grilling` / `grill-me` | `/ddove-grill` | 跟问法（frontier 轮次）；出口改 wiki |
| `grill-with-docs` / `domain-modeling` | `DDoveMiniGameWiki/` + create | 不跟落盘（禁止 `CONTEXT.md` / ADR） |
| `writing-for-agents` | `ddove-writing-for-agents` | 跟写法杠杆；共享语言改 wiki |
| `ask-matt` | `/ddove-help` | 跟路由器角色；路由本库命令 |
| `to-tickets` / `implement-spec` | 开工班组 | 目标相同：一次一个需求，需求内按任务图并行。不引进 tracker |
| `setup` / `to-spec` / `tickets` / `implement` / `wayfinder` / `triage` | — | 不引进 |
| 其余 engineering | — | 默认不跟；要引进先 `/ddove-grill` |

对照 pstack：人看到 [pstack](https://github.com/cursor/plugins/tree/main/pstack) 有更新。整包不装进本仓。要跟着一条，先 `/ddove-grill`。

对照 luban/ai：只补 `ddove-config` 的问法。不改合成方式。

引进一条：问法或完成条件值得跟，才用 `ddove-writing-for-agents` 改对应 `ddove-*`，并同步 `.cursor/skills/`。`CONTEXT.md` 改写到 `DDoveMiniGameWiki/`，issue tracker 删掉，`ddove-` 前缀保留。更名只改本篇对照表，本库 `name:` 不改。对照表变了就改本篇并记 `_log`。然后走一遍 `/ddove-help`，确认整图仍对。本机若在仓外留了 clone，可记下 `PINNED_SHA`，不要进本 Git。
