---
type: Playbook
title: 收掉skill冗余
description: 浅层 skill 副本与权威正文对齐。help 只留一处指路。create 落稿后停下，不再接着改文件。
tags: [索引, agent]
status: stable
verified: { at: 2026-10-16T04:27:14Z }
sources:
  - id: req
    resource: /_archive/需求/收掉skill冗余.md
    title: 收掉skill冗余（需求稿）
  - id: help
    resource: ../../../.agents/skills/ddove-help/SKILL.md
    title: ddove-help
  - id: create
    resource: ../../../.agents/skills/ddove-wiki-create/SKILL.md
    title: ddove-wiki-create
  - id: grill
    resource: ../../../.agents/skills/ddove-grill/SKILL.md
    title: ddove-grill
---

# 收掉skill冗余

Concept ID：`/00-索引/Agent/收掉skill冗余`。

[打开说明](收掉skill冗余.html)

## 干什么

浅层 `.cursor/skills` 与 `.agents/skills` 权威正文一致。`ddove-help` 的开工规则只留一处。`ddove-wiki-create` 落下需求稿后停下。

## 有哪些接口

- `.cursor/skills/ddove-*/SKILL.md`
- `ddove-help`
- `ddove-wiki-create`

## 每个接口干什么

浅层副本：`ddove-wiki-create`、`ddove-wiki-change-state`、`ddove-wiki`、`ddove-wiki-update`、`ddove-fix-up`、`ddove-writing-for-agents` 的 `.cursor/skills/ddove-*/SKILL.md` 与对应 `.agents/skills/ddove-*/SKILL.md` 全文一致。同名嵌套副本与权威正文一致。不删技能目录。

`ddove-help`：开工规则只在步骤 3。整图和选路指向 `ddove-grill`、`ddove-work` 和 [需求稿工作流](/00-索引/Agent/需求稿工作流.md)。三份 help 正文一致。

`ddove-wiki-create`：新篇在 `05-需求` 且 `tags` 含 `需求` 时，落稿后停下，不在同一条对话接着改文件。下一刀是人另发 `/ddove-work`。三份 create 正文一致，并与 `ddove-grill` 第 5 步相同。
