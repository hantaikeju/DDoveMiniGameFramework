---
name: ddove-wiki-create
description: >-
  Create a new OKF wiki concept. User only needs to give 生成人 short id;
  the agent fills directory, title, type, body. Use only when invoked as
  /ddove-wiki-create or the user explicitly asks to create a wiki page.
disable-model-invocation: true
---

# 新建 wiki 概念

权威：`DDoveMiniGameWiki/00-索引/OKF约定.md`。短号表：`DDoveMiniGameWiki/00-索引/整理人.md`。开篇不自报 status：`DDoveMiniGameWiki/00-索引/Agent/开篇与清单徽章.md`。

**用户只需给生成人短号。** 职种、文件名、`type`、`title`、`description`、正文由 Agent 根据本条消息和会话上下文补齐。拿不准只再问**一句**（例如职种二选一），不要问卷。

落在 `05-需求` 的新稿，写篇后调用 `ddove-show-me`（种类「要什么不要」），并在开篇加同目录相对链接。其它目录不画 HTML。画法只跟 `ddove-show-me`，不在本技能里另写一套。

## 步骤

1. **生成人**：消息里已有短号就用；否则本会话沿用已问过的；都没有才问「生成人短号？」。对整理人表；没有则先补表 + 职种 `index.md` 一行 + 空 `index_<短号>.md`。禁止猜、禁止 `process:agent`。完成：短号已对表。
2. **补齐（不要逐项问用户）**：
   - 目录：`tags` 含 `需求` → `05-需求`（tags 再带目标职种，或 AI 用法的 `索引` + `agent`）。用法篇：工程 → `01`–`04`；AI 用法 → `00-索引/Agent`；约定 / 合规 → `00-索引` 根
   - `type`：Playbook / Reference / Report / Skill / Stub / Table / Index
   - 文件名贴近 `title`，不要 `index.md` / `log.md`
   - `tags`：工程跟职种；AI 用法：`索引` + `agent`
   完成：目录 / type / 文件名 / tags 已定。
3. **防重**：`rg -g "!_log/**" "^title: |关键词" DDoveMiniGameWiki/<目录>`。已有同类篇则停，不要第二份。完成：无重复或已停。
4. **写篇**：`status: draft`，不要自动 `verified`。正文（开篇、结论）不写「本篇是 draft」「本文 `status: draft`」「不当发布依据」；问答点名 draft 不要抄进篇。阶段只看 YAML。`05-需求` 正文要有 `## 要什么` 与 `## 不要`。同名 HTML 的链接留到下一步，本步不写。

```yaml
---
type: Playbook
title: 短标题
description: 一句摘要
tags: [程序-前]
status: draft
generated: { by: human:<短号>, at: <UTC ISO8601> }
sources: []
---
```

完成：文件已写，篇首无上述自报句。`05-需求` 已有 `## 要什么` 与 `## 不要`，开篇尚无 HTML 链接。
5. **说明页**（仅 `05-需求`）：读并执行 `.agents/skills/ddove-show-me/SKILL.md`（与 `.cursor/skills/ddove-show-me/SKILL.md` 同一说法；读文件，不要再 `/` 一次）。入参是刚写的这篇 `.md` 路径，种类「要什么不要」。一次一篇。不在本技能里另写文件名、版式或取块。
   - show-me 因缺 `## 要什么` 或 `## 不要` 而停：本步停，交还所缺标题，不加链接，不挂清单。
   - 同目录已有同主文件名 `.html`：在该 `.md` 开篇、紧挨 `## 要什么` 之前，加同目录相对链接，指向这份 `.html`。主文件名只取自 `.md` 文件名，不用 YAML `title`。可见文字为「要什么不要」，例如 `[要什么不要](主文件名.html)`。
   - 链接不放进 `## 验收` 或 `## 落点`。整份做完删掉这两节后，链接仍在开篇。
   - 目录不是 `05-需求`：跳过。不调用 show-me，不写 HTML，不加这条链接。
   HTML 不进清单、不进 log。完成：`05-需求` 新稿开篇有同目录相对链接，旁边的 `.html` 由 show-me 按「要什么不要」写成；其它目录的新建篇没有 HTML。
6. 只改该短号的 `index_<短号>.md`。不改根 `DDoveMiniGameWiki/index.md`。不挂 `.html`。完成：清单已挂。
7. `_log/log_YYYY-MM-DD_<短号>.md` 顶部 `**Add**`；根 `log.md` 缺该日 `##` 则补链接。不记 `.html`。完成：当日 log 已记。
8. `py -3 DDoveMiniGameTools/check_wiki.py`（或 `python DDoveMiniGameTools/check_wiki.py`），失败修到过。完成：检查通过。

改正文用 `/ddove-wiki-update`。改状态用 `/ddove-wiki-change-state`。本 skill 新建一律 `draft`。

完成：`check_wiki.py` 通过，篇首无 status 自报，且清单已挂（`01`–`04` 与 `05-需求` 用 `index_<短号>.md`；`00-索引` / `Agent` 用该目录 `index.md`）与当日 `_log`。新建的 `05-需求` 篇已按 `ddove-show-me` 画出「要什么 / 不要」，开篇有指向同名 `.html` 的同目录相对链接；其它目录没有这张 HTML。
