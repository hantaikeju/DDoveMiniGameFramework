---
type: Reference
title: Agent 总览
description: 工程 wiki 与 AI 用法 wiki 分轨。仓库 ddove-* 与个人 Cursor skill 的地图。
tags: [索引, agent]
status: stable
verified: { at: 2026-10-15T06:56:00Z }
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
| 处理中的需求稿 | `DDoveMiniGameWiki/05-需求` | 得分记在该篇 `## 评分`。整份做完改为 `deprecated` 并搬入 `_archive/需求/`：留下 `## 要什么`、`## 不要`、`## 评分` 和用法篇链接，不留 `## 验收`、`## 落点`，`title` 仍含「需求稿」 |
| 整个 AI 怎么用（skill / grill / 个人 Cursor） | `DDoveMiniGameWiki/00-索引/Agent` | 本目录，`tags: [索引, agent]` |

仓库 skill：`.agents/skills/ddove-*/SKILL.md`，Cursor 同步 `.cursor/skills/`。个人 skill：`~\.cursor\skills\`，不进 Git。

## 仓库 skill

| 调用 | 干什么 | SKILL.md |
|------|--------|----------|
| `/ddove-help` | 整套工作流指路，只提示不代跑。每一拍只碰一篇处理中的需求。只有一篇直接做。多于一篇只做点名的那篇；没点名就停，只问选哪一篇。问答和排障不问选哪一篇 | `.agents/skills/ddove-help/SKILL.md` |
| `ddove-wiki` | 只检索 wiki，交出命中列表 | `.agents/skills/ddove-wiki/SKILL.md` |
| `ddove-work` | 问答 / 排障 / 改代码（先跑 wiki 检索）。每一拍只碰一篇处理中的需求。只有一篇直接做。多于一篇只做点名的那篇；没点名就停，只问选哪一篇。问答和排障不问选哪一篇。按一篇 `draft`+`需求` 做完后，代理写上正式 / 废弃，再给人看产品。问答和排障不走这套收口 | `.agents/skills/ddove-work/SKILL.md` |
| `ddove-fix-up` | 产品不对时由代理执行。用法篇和需求稿回到处理中，改完再写成正式 / 废弃。人不敲这条命令 | `.agents/skills/ddove-fix-up/SKILL.md` |
| `ddove-config` | 改 / 加配表、导表失败（对照 luban/ai，本库改编） | `.agents/skills/ddove-config/SKILL.md` |
| `/ddove-grill` | 拷问对齐。每一拍只碰一篇处理中的需求。只有一篇直接做。多于一篇只做点名的那篇；没点名就停，只问选哪一篇。确认后同一条对话里代理落稿、开工、展示 | `.agents/skills/ddove-grill/SKILL.md` |
| `/ddove-archive` | 人调用时清扫废弃需求稿。超过 90 天从仓库删除。不是每篇需求的一步 | `.agents/skills/ddove-archive/SKILL.md` |
| `ddove-wiki-create` | 代理用来落稿的手续。落在 `05-需求` 时调用 `ddove-show-me`（种类「说明页」）。对齐后不把这条报成用户的下一步 | `.agents/skills/ddove-wiki-create/SKILL.md` |
| `/ddove-wiki-update` | 已有篇改正文（回 `draft`）。旁边已有同名 HTML 则调用 `ddove-show-me`（种类「说明页」）重画这一张，开篇链接改成「打开说明」；没有同名 HTML 的不补 | `.agents/skills/ddove-wiki-update/SKILL.md` |
| `/ddove-wiki-change-state` | 每一拍只碰一篇处理中的需求。只有一篇直接做。多于一篇只做点名的那篇；没点名就停，只问选哪一篇。问答和排障不问选哪一篇。`draft`/`stable`/`deprecated`。篇不在 `05-需求`：升 `stable`，`verified` 只写时刻，摘 `需求`。`05-需求` 整份做完：按职种生成或改写用法篇（同一文件名；干什么 / 有哪些接口 / 每个接口干什么；`stable`；`verified` 只写时刻；`tags` 无 `需求`；无 `## 评分`；无「要什么 / 不要 / 验收」；已有同名则改正文，不另开）。需求稿改 `deprecated`，写入 `deprecated_at`，连同名 html 搬入 `_archive/需求/`，正文留下 `## 要什么`、`## 不要`、`## 评分`、用法篇链接和「打开说明」，不留 `## 验收`、`## 落点`，`title` 仍含「需求稿」。整份做完调用 `ddove-show-me` 两次，种类都是「说明页」：用法篇旁画说明页并加「打开说明」，再按最终要什么 / 不要重画需求稿 HTML，开篇也是「打开说明」。职种目录：`策划`→`01-策划`，`程序-前`→`02-程序-前`，`程序-后`→`03-程序-后`，`美术`→`04-美术`，只有 `索引`+`agent`→`00-索引/Agent`。`05-需求` 整份做完后人看产品，不把落稿或改状态报成下一步 | `.agents/skills/ddove-wiki-change-state/SKILL.md` |
| `ddove-okf-upgrade` | 换 SPEC 后合规 | `.agents/skills/ddove-okf-upgrade/SKILL.md` |
| `ddove-show-me` | 被 create / change-state / wiki-update 调用，写一张同名静态 HTML。种类只认「说明页」。update 只重画已有同名 HTML。不新建查看指令，不改本机 `/show-me` | `.agents/skills/ddove-show-me/SKILL.md` |
| `ddove-writing-for-agents` | 写或改 `ddove-*` | `.agents/skills/ddove-writing-for-agents/SKILL.md` |
| `/ddove-rename-workspace` | fork 开新游戏时改四件套前缀；本框架仓不跑 | `.agents/skills/ddove-rename-workspace/SKILL.md` |

每一拍只碰一篇处理中的需求。只有一篇直接做。多于一篇只做点名的那篇；没点名就停，只问选哪一篇。问答和排障不问选哪一篇。需求稿收口：grill 对齐后代理落稿、改文件、写上正式和废弃，再给人看产品。产品不对则两篇回到处理中再做完。步骤见 [需求稿工作流](/00-索引/Agent/需求稿工作流.md)。换引擎见 [工作流迁移](/00-索引/Agent/工作流迁移.md)。fork 改四件套见 [改工作区前缀](/00-索引/Agent/改工作区前缀.md)。上游 Release 见 [上游 skill 更新](/00-索引/Agent/上游skill更新.md)。

## 个人 skill（不进仓库）

| 调用 | 干什么 | 篇 |
|------|--------|-----|
| `/show-me` | 把当前话题画清楚 | [show-me 安装](/00-索引/Agent/show-me安装.md) |
