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

## 步骤

1. **生成人**：消息里已有短号就用；否则本会话沿用已问过的；都没有才问「生成人短号？」。对整理人表；没有则先补表 + 职种 `index.md` 一行 + 空 `index_<短号>.md`。禁止猜、禁止 `process:agent`。完成：短号已对表。
2. **补齐（不要逐项问用户）**：
   - 目录：工程 → `01`–`04`；AI 用法 → `00-索引/Agent`；约定 / 合规 → `00-索引` 根
   - `type`：Playbook / Reference / Report / Skill / Stub / Table / Index
   - 文件名贴近 `title`，不要 `index.md` / `log.md`
   - `tags`：工程跟职种；AI 用法：`索引` + `agent`
   完成：目录 / type / 文件名 / tags 已定。
3. **防重**：`rg -g "!_log/**" "^title: |关键词" DDoveMiniGameWiki/<目录>`。已有同类篇则停，不要第二份。完成：无重复或已停。
4. **写篇**：`status: draft`，不要自动 `verified`。正文（开篇、结论）不写「本篇是 draft」「本文 `status: draft`」「不当发布依据」；问答点名 draft 不要抄进篇。阶段只看 YAML。

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

完成：文件已写，篇首无上述自报句。
5. 只改该短号的 `index_<短号>.md`。不改根 `DDoveMiniGameWiki/index.md`。完成：清单已挂。
6. `_log/log_YYYY-MM-DD_<短号>.md` 顶部 `**Add**`；根 `log.md` 缺该日 `##` 则补链接。完成：当日 log 已记。
7. `py -3 DDoveMiniGameTools/check_wiki.py`（或 `python DDoveMiniGameTools/check_wiki.py`），失败修到过。完成：检查通过。

改正文用 `/ddove-wiki-update`。改状态用 `/ddove-wiki-change-state`。本 skill 新建一律 `draft`。

完成：`check_wiki.py` 通过，篇首无 status 自报，且清单已挂（`01`–`04` 用 `index_<短号>.md`；`00-索引` / `Agent` 用该目录 `index.md`）与当日 `_log`。
