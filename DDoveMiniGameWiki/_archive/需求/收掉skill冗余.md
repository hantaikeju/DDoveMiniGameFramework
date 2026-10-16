---
type: Playbook
title: 收掉skill冗余（需求稿）
description: 浅层 skill 副本与权威正文对齐。help 只留一处指路。create 落稿后停下，不再接着改文件。
tags: [索引, agent, 需求]
status: deprecated
deprecated_at: 2026-10-16T04:27:14Z
sources:
  - id: land
    resource: /00-索引/Agent/落稿后再开工.md
    title: 落稿后再开工
  - id: map
    resource: /00-索引/Agent/Agent总览.md
    title: Agent 总览
  - id: grill
    resource: ../../../.agents/skills/ddove-grill/SKILL.md
    title: ddove-grill
  - id: create
    resource: ../../../.agents/skills/ddove-wiki-create/SKILL.md
    title: ddove-wiki-create
  - id: help
    resource: ../../../.agents/skills/ddove-help/SKILL.md
    title: ddove-help
  - id: usage
    resource: /00-索引/Agent/收掉skill冗余.md
    title: 收掉skill冗余
---

# 收掉skill冗余（需求稿）

用法篇：[收掉skill冗余](/00-索引/Agent/收掉skill冗余.md)。

[打开说明](收掉skill冗余.html)

## 要什么

变更种类：缺陷。

浅层 `.cursor/skills/ddove-*/SKILL.md` 与 `.agents/skills/ddove-*/SKILL.md` 正文一致。已分叉的六份是 `ddove-wiki-create`、`ddove-wiki-change-state`、`ddove-wiki`、`ddove-wiki-update`、`ddove-fix-up`、`ddove-writing-for-agents`。嵌套目录 `.cursor/skills/ddove-*/ddove-*/SKILL.md` 与改后的权威正文一致。不删技能目录。

`ddove-help` 只留一处指路。步骤 3、整图、选路不再各抄一遍开工规则。细则指向 `ddove-grill`、`ddove-work` 和 [需求稿工作流](/00-索引/Agent/需求稿工作流.md)。三份 help 正文一致。

`ddove-wiki-create` 末句改成：新篇在 `05-需求` 且 `tags` 含 `需求` 时，落稿后停下，不在同一条对话接着改文件。下一刀是人另发 `/ddove-work`。三份 create 正文一致，并与 `ddove-grill` 第 5 步相同。

证明方式：只改 wiki / skill，对照条文。

## 不要

- 改 `ddove-grill` 的落稿后停下
- 改 `ddove-work` 的不数稿
- 改 `ddove-grill-with-demand` 的问完按已有这篇开工
- 删掉 `.cursor/skills/` 里的技能目录
- 改开工评分条文
- 改客户端

## 评分

分数记在本节，不另开文件。尺子：开工评分 v12（这篇是 draft）。

| 轮 | 总分 | 过线 | 记录 |
|----|------|------|------|
| 1 | 100 | 是 | 审查 PASS。本轮无客户端改动，未编译。需求验收 20 / Wiki 同步 20 / 职责边界 20 / 范围 15 / 信任表述 15 / 规格增量 10。 |
