---
name: ddove-wiki-create
description: >-
  Create a new OKF wiki concept. User only needs to give 生成人 short id;
  the agent fills directory, title, type, body. Use only when invoked as
  /ddove-wiki-create or the user explicitly asks to create a wiki page.
disable-model-invocation: true
---

# 新建 wiki 概念

权威：`wiki/00-索引/OKF约定.md`。短号表：`wiki/00-索引/整理人.md`。

**用户只需给生成人短号。** 职种、文件名、`type`、`title`、`description`、正文由 Agent 根据本条消息和会话上下文补齐。拿不准只再问**一句**（例如职种二选一），不要问卷。

## 步骤

1. **生成人**：消息里已有短号就用；否则本会话沿用已问过的；都没有才问「生成人短号？」。对整理人表；没有则先补表 + 职种 `index.md` 一行 + 空 `index_<短号>.md`。禁止猜、禁止 `process:agent`。
2. **补齐（不要逐项问用户）**：
   - 目录：`01-策划` / `02-程序-前` / `03-程序-后` / `04-美术`（约定才进 `00-索引`）
   - `type`：Playbook / Reference / Report / Stub / Table / Index
   - 文件名贴近 `title`，不要 `index.md` / `log.md`
   - `tags` 跟目录：`策划` / `程序-前` / `程序-后` / `美术`
3. **防重**：`rg -g "!_log/**" "^title: |关键词" wiki/<目录>`。已有同类篇则停，不要第二份。
4. **写篇**：`status: draft`，不要自动 `verified`。

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

5. 只改该短号的 `index_<短号>.md`。不改根 `wiki/index.md`。
6. `_log/log_YYYY-MM-DD_<短号>.md` 顶部 `**Add**`；根 `log.md` 缺该日 `##` 则补链接。
7. `py -3 scripts/check_wiki.py`（或 `python scripts/check_wiki.py`），失败修到过。

改状态用 `/ddove-wiki-change-state`。本 skill 新建一律 `draft`。
