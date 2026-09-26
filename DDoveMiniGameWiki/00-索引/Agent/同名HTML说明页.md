---
type: Playbook
title: 同名HTML说明页
description: ddove-show-me 写同名静态 HTML。create 画要什么不要；整份做完画三块并重画需求稿那一份。
tags: [索引, agent]
status: stable
generated: { by: human:cjh, at: 2026-09-26T06:18:00Z }
verified: { by: human:cjh, at: 2026-09-26T06:18:00Z }
sources:
  - id: show
    resource: ../../../.agents/skills/ddove-show-me/SKILL.md
    title: ddove-show-me
  - id: create
    resource: ../../../.agents/skills/ddove-wiki-create/SKILL.md
    title: ddove-wiki-create
  - id: state
    resource: ../../../.agents/skills/ddove-wiki-change-state/SKILL.md
    title: ddove-wiki-change-state
  - id: req
    resource: /05-需求/同名HTML说明页.md
    title: 同名HTML说明页（需求稿）
---

# 同名HTML说明页

Concept ID：`/00-索引/Agent/同名HTML说明页`。地图：[Agent 总览](/00-索引/Agent/Agent总览.md)。步骤见 [需求稿工作流](/00-索引/Agent/需求稿工作流.md)。个人技能仍是 [show-me 安装](/00-索引/Agent/show-me安装.md)。

[三块](同名HTML说明页.html)

## 干什么

`ddove-show-me` 在一篇 `.md` 旁边写一张静态 HTML，主文件名与该 `.md` 相同。HTML 不是概念，`check_wiki.py` 不扫它。

`/ddove-wiki-create` 落 `05-需求` 时调用它，画「要什么 / 不要」，并在需求稿正文加链接。`/ddove-wiki-change-state` 把 `05-需求` 一篇整份做完时调用它两次：用法篇旁画三块并加链接；再按最终「要什么 / 不要」重画需求稿那一份。

`ddove-work` 过线不调用。本机 `/show-me` 不改、不进仓库。已有篇不补 HTML。技能在 `.agents/skills/ddove-show-me/` 与 `.cursor/skills/ddove-show-me/`。

## 有哪些接口

- `ddove-show-me`（种类「要什么不要」或「三块」）
- `/ddove-wiki-create`（落 `05-需求`）
- `/ddove-wiki-change-state`（`05-需求` 整份做完）

## 每个接口干什么

`ddove-show-me` 一次写一张。HTML 与目标 `.md` 同目录、同主文件名。`title` 含「需求稿」时，文件名仍用 `.md` 主文件名。种类「要什么不要」取 `## 要什么`、`## 不要`，左「要什么」、右「不要」，窄屏上下排。种类「三块」取 `## 干什么`、`## 有哪些接口`、`## 每个接口干什么`，三节顺排。页是静态 HTML，无 frontmatter，无脚本，无分步按钮。它不改目标 `.md`。

`/ddove-wiki-create` 落在 `05-需求` 时传入该需求稿，种类「要什么不要」，并在开篇加上打开这份 HTML 的链接。其它目录不画。

`/ddove-wiki-change-state` 把一篇 `05-需求` 整份做完时先对用法篇传入种类「三块」，再对该需求稿传入种类「要什么不要」重画。不扫描仓库去给其它已有篇补 HTML。
