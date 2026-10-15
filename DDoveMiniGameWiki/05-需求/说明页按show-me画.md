---
type: Playbook
title: 说明页按show-me画（需求稿）
description: 薄记录。用法篇才是接口总结。得分留在本篇。
tags: [索引, agent, 需求]
status: deprecated
generated: { by: human:cjh, at: 2026-09-30T18:55:00Z }
sources:
  - id: use
    resource: /00-索引/Agent/说明页按show-me画.md
    title: 说明页按show-me画
  - id: old
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
  - id: update
    resource: ../../../.agents/skills/ddove-wiki-update/SKILL.md
    title: ddove-wiki-update
  - id: personal
    resource: /00-索引/Agent/show-me安装.md
    title: show-me 安装
---

# 说明页按show-me画（需求稿）

用法篇：[说明页按show-me画](/00-索引/Agent/说明页按show-me画.md)。

[打开说明](说明页按show-me画.html)

## 要什么

`ddove-show-me` 只保留一种「说明页」。一次仍只写一张静态 HTML，与目标 `.md` 同目录、同主文件名。`title` 含「需求稿」时，文件名仍用 `.md` 主文件名。HTML 不是概念，`check_wiki.py` 不扫它。无 frontmatter，无脚本，无分步按钮。用 HTML / CSS / SVG 画结构，像本机 `/show-me` 那样只留当前篇要看懂的边界，不逐字贴正文。

取块按篇类型：路径在 `05-需求/` 或 YAML `title` 含「需求稿」是需求稿，否则是用法篇。输出不再分「要什么不要」和「三块」：

- 需求稿：取 `## 要什么`、`## 不要`
- 用法篇：取 `## 干什么`、`## 有哪些接口`、`## 每个接口干什么`

开篇打开同名 HTML 的链接，可见文字一律是「打开说明」，例如 `[打开说明](主文件名.html)`。不再写「三块」或「要什么不要」。

谁来调：

- `/ddove-wiki-create` 落 `05-需求`：传入该需求稿，种类「说明页」，开篇加「打开说明」
- `/ddove-wiki-change-state` 把 `05-需求` 一篇整份做完：对用法篇、再对该需求稿各调一次，种类都是「说明页」
- `/ddove-wiki-update` 改正文：这篇旁边已有同名 HTML 则重画这一张，并把开篇链接改成「打开说明」。没有同名 HTML 的不补

这一刀已扫仓库里已经存在的同名 HTML，按新画法整文件重画，并把对应 `.md` 开篇链接改成「打开说明」。之后不再做全库扫描；只在 create / change-state / wiki-update 重画当前这篇。

`ddove-work` 过线不调用。本机 `/show-me` 不改、不进仓库。不新建「查看 HTML」指令。

技能两份一起改：`.agents/skills/` 与 `.cursor/skills/` 里的 `ddove-show-me`、`ddove-wiki-create`、`ddove-wiki-change-state`、`ddove-wiki-update`。流程篇改成同一条：

- [需求稿工作流](/00-索引/Agent/需求稿工作流.md)
- [Agent 总览](/00-索引/Agent/Agent总览.md)

整份做完时新开用法篇 `/00-索引/Agent/说明页按show-me画`。旧用法篇 [同名HTML说明页](/00-索引/Agent/同名HTML说明页.md) 改 `deprecated`，加替代链接，不把旧篇改写成新用法。

## 不要

- 改本机 `~\.cursor\skills\show-me\`
- 把 `/show-me` 放进 `.agents/skills/`
- 新建查看 HTML 的指令
- 开篇可见文字继续写「三块」或「要什么不要」
- 仍保留「要什么不要」和「三块」两种种类
- 逐字贴正文换皮
- 内联脚本（含 Mermaid 脚本）
- HTML 写成可点的分步流程
- `ddove-work` 过线时生成 HTML
- 没有同名 HTML 的已有篇补一张
- HTML 带上概念 frontmatter
- 文件名用带「需求稿」的标题
- 做完时改写旧用法篇 [同名HTML说明页](/00-索引/Agent/同名HTML说明页.md) 的正文当新用法
- 自动升 `stable`

## 评分

分数记在本节，不另开文件。尺子：开工评分 v11。

| 轮 | 总分 | 过线 | 记录 |
|----|------|------|------|
| 1 | 100 | 是 | 审查 PASS。需求验收 20、Wiki 同步 20、职责边界 20、范围 15、信任表述 15、规格增量 10。票 T-showme、T-callers、T-flow、T-redraw。验收 1–8 满足；9 留给 change-state。对照篇只点名过期。v11 条文不变。 |
