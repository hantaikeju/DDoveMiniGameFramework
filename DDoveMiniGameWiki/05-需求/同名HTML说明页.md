---
type: Playbook
title: 同名HTML说明页
description: 已落地。ddove-show-me 写同名静态 HTML。create 画要什么不要；整份做完画三块并重画需求稿那一份。
tags: [索引, agent, 需求]
status: draft
generated: { by: human:cjh, at: 2026-09-25T06:06:00Z }
sources:
  - id: show
    resource: /00-索引/Agent/show-me安装.md
    title: show-me 安装
  - id: flow
    resource: /00-索引/Agent/需求稿工作流.md
    title: 需求稿工作流
  - id: map
    resource: /00-索引/Agent/Agent总览.md
    title: Agent 总览
  - id: keep
    resource: /00-索引/Agent/需求稿留下要什么不要.md
    title: 需求稿留下要什么不要
  - id: create
    resource: ../../../.agents/skills/ddove-wiki-create/SKILL.md
    title: ddove-wiki-create
  - id: state
    resource: ../../../.agents/skills/ddove-wiki-change-state/SKILL.md
    title: ddove-wiki-change-state
---

# 同名HTML说明页

Concept ID：`/05-需求/同名HTML说明页`。清单：[index_cjh](/05-需求/index_cjh.md)。地图：[Agent 总览](/00-索引/Agent/Agent总览.md)。个人技能仍是 [show-me 安装](/00-索引/Agent/show-me安装.md)。

需求稿和用法篇各有一张跟 `.md` 同名的静态 HTML。篇里有链接打开它。三票技能与两篇流程已按当前技能落地。需求稿工作流与 Agent 总览已回 draft。

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

## 落点

| 路径 | 本刀 |
|------|------|
| `.agents/skills/ddove-show-me/` 与 `.cursor/skills/` 同步份 | 写同名静态 HTML |
| `.agents/skills/ddove-wiki-create/` 与同步份 | 落需求稿时画要什么 / 不要，并在稿内加链接 |
| `.agents/skills/ddove-wiki-change-state/` 与同步份 | 用法篇旁画三块；重画需求稿 HTML |
| [需求稿工作流](/00-索引/Agent/需求稿工作流.md)、[Agent 总览](/00-索引/Agent/Agent总览.md) | 写上这两步 |

## 评分

分数记在本节，不另开文件。尺子：开工评分 v11。

| 轮 | 总分 | 过线 | 记录 |
|----|------|------|------|
| 1 | 100 | 是 | 审查 PASS。需求验收 20、Wiki 同步 20、职责边界 20、范围 15、信任表述 15、规格增量 10。条文不变。 |

## 验收

1. 之后新建的 `05-需求` 篇旁边有同名 `.html`，内容是该篇「要什么 / 不要」，稿内有链接。HTML 无 frontmatter。
2. 之后整份做完时，用法篇旁边有同名 `.html`，内容是三块，篇内有链接；`05-需求` 那份 HTML 与最终「要什么 / 不要」一致。
3. `ddove-work` 过线不产生 HTML。本机 `show-me` 目录未被这刀修改。已有篇没有被补上 HTML。
4. 两份 `ddove-show-me`、两份 create、两份 change-state 说法一致。需求稿工作流与 Agent 总览写了这两步。
