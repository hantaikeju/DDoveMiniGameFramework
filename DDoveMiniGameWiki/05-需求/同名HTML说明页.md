---
type: Playbook
title: 同名HTML说明页（需求稿）
description: 薄记录。用法篇才是接口总结。得分留在本篇。
tags: [索引, agent, 需求]
status: deprecated
generated: { by: human:cjh, at: 2026-09-25T06:06:00Z }
sources:
  - id: use
    resource: /00-索引/Agent/同名HTML说明页.md
    title: 同名HTML说明页
  - id: show
    resource: ../../../.agents/skills/ddove-show-me/SKILL.md
    title: ddove-show-me
  - id: create
    resource: ../../../.agents/skills/ddove-wiki-create/SKILL.md
    title: ddove-wiki-create
  - id: state
    resource: ../../../.agents/skills/ddove-wiki-change-state/SKILL.md
    title: ddove-wiki-change-state
---

# 同名HTML说明页（需求稿）

用法篇：[同名HTML说明页](/00-索引/Agent/同名HTML说明页.md)。

[要什么不要](同名HTML说明页.html)

## 要什么

新技能 `ddove-show-me`。它只写一张静态 HTML，文件名与对应 `.md` 相同（不含「需求稿」标题后缀）。HTML 不是概念，`check_wiki.py` 不扫它。

`/ddove-wiki-create` 落 `05-需求` 需求稿时调用它：在同目录生成 `<文件名>.html`，画这篇的「要什么 / 不要」。需求稿正文加链接打开这份 HTML。

`/ddove-wiki-change-state` 把 `05-需求` 一篇整份做完时调用它两次：

- 用法篇旁边生成同名 HTML，画「干什么 / 有哪些接口 / 每个接口干什么」。用法篇正文加链接打开它。
- 按需求稿最终的「要什么 / 不要」重画 `05-需求` 那一份。

`ddove-work` 过线时不生成 HTML。本机 `/show-me` 不改、不进仓库。已有篇不补 HTML。

技能两份一起放：`.agents/skills/ddove-show-me/SKILL.md` 与 `.cursor/skills/ddove-show-me/SKILL.md`。create、change-state 两份技能都改。流程篇补上这一步：

- [需求稿工作流](/00-索引/Agent/需求稿工作流.md)
- [Agent 总览](/00-索引/Agent/Agent总览.md)

## 不要

- 改本机 `~\.cursor\skills\show-me\`
- 把 `/show-me` 放进 `.agents/skills/`
- HTML 写成可点的分步流程
- `ddove-work` 过线时生成 HTML
- 给已有需求稿或用法篇补 HTML
- HTML 带上概念 frontmatter
- 文件名用带「需求稿」的标题

## 评分

分数记在本节，不另开文件。尺子：开工评分 v11。

| 轮 | 总分 | 过线 | 记录 |
|----|------|------|------|
| 1 | 100 | 是 | 审查 PASS。需求验收 20、Wiki 同步 20、职责边界 20、范围 15、信任表述 15、规格增量 10。条文不变。 |
| 2 | 100 | 是 | 对照验收，改动路径无。审查 PASS。需求验收 20、Wiki 同步 20、职责边界 20、范围 15、信任表述 15、规格增量 10。条文不变。 |
