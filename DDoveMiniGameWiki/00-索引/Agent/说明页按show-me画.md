---
type: Playbook
title: 说明页按show-me画
description: ddove-show-me 只认一种「说明页」。create / change-state / wiki-update 都传这个种类。开篇链接写「打开说明」。
tags: [索引, agent]
status: stable
generated: { by: human:cjh, at: 2026-10-01T03:37:00Z }
verified: { by: human:cjh, at: 2026-10-01T03:37:00Z }
sources:
  - id: req
    resource: /05-需求/说明页按show-me画.md
    title: 说明页按show-me画（需求稿）
  - id: show
    resource: ../../../.agents/skills/ddove-show-me/SKILL.md
    title: ddove-show-me
  - id: create
    resource: ../../../.agents/skills/ddove-wiki-create/SKILL.md
    title: ddove-wiki-create
  - id: state
    resource: ../../../.agents/skills/ddove-wiki-change-state/SKILL.md
    title: ddove-wiki-change-state
  - id: update
    resource: ../../../.agents/skills/ddove-wiki-update/SKILL.md
    title: ddove-wiki-update
  - id: personal
    resource: /00-索引/Agent/show-me安装.md
    title: show-me 安装
---

# 说明页按show-me画

Concept ID：`/00-索引/Agent/说明页按show-me画`。地图：[Agent 总览](/00-索引/Agent/Agent总览.md)。步骤见 [需求稿工作流](/00-索引/Agent/需求稿工作流.md)。个人技能仍是 [show-me 安装](/00-索引/Agent/show-me安装.md)。旧篇：[同名HTML说明页](/00-索引/Agent/同名HTML说明页.md)。

[打开说明](说明页按show-me画.html)

## 干什么

`ddove-show-me` 在一篇 `.md` 旁边写一张静态 HTML，主文件名与该 `.md` 相同。只认种类「说明页」。HTML 不是概念，`check_wiki.py` 不扫它。用 HTML / CSS / SVG 画结构，不逐字贴正文。无 frontmatter，无脚本，无分步按钮。

路径在 `05-需求/` 或 YAML `title` 含「需求稿」按需求稿取 `## 要什么`、`## 不要`，画纳入与排除。否则按用法篇取 `## 干什么`、`## 有哪些接口`、`## 每个接口干什么`，画职责和谁调谁。

开篇链接可见文字是「打开说明」。本机 `/show-me` 不改、不进仓库。`ddove-work` 过线不调用。没有同名 HTML 的已有篇不补。

## 有哪些接口

- `ddove-show-me`（种类「说明页」）
- `/ddove-wiki-create`（落 `05-需求`）
- `/ddove-wiki-change-state`（`05-需求` 整份做完）
- `/ddove-wiki-update`（改正文且已有同名 HTML）

## 每个接口干什么

`ddove-show-me` 一次写一张。种类不是「说明页」则停。`title` 含「需求稿」时，文件名仍用 `.md` 主文件名。它不改目标 `.md`。

`/ddove-wiki-create` 落在 `05-需求` 时传入该需求稿，种类「说明页」，开篇加「打开说明」。其它目录不画。

`/ddove-wiki-change-state` 把一篇 `05-需求` 整份做完时，对用法篇、再对该需求稿各调一次，种类都是「说明页」，两边开篇都写「打开说明」。不扫描仓库去给其它已有篇补 HTML。

`/ddove-wiki-update` 改正文且旁边已有同名 HTML 时，种类「说明页」重画这一张，并把开篇链接改成「打开说明」。没有同名 HTML 的不补。
