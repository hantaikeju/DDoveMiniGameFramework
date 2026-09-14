---
name: ddove-wiki-change-state
description: >-
  Change a wiki concept status (draft / stable / deprecated).
  Use only when the user invokes /ddove-wiki-change-state or
  explicitly asks to mark a wiki page official, draft, or deprecated.
  Do not run during ordinary Q&A.
disable-model-invocation: true
---

# 改 wiki 状态

权威：`DDoveMiniGameWiki/00-索引/OKF约定.md`。「信任与生命周期」一节。

`status` 只允许：`draft`（起草）/ `stable`（正式）/ `deprecated`（废弃）。不要另造字段。`stable` = 人审过当前正文；升 `stable` 时**同步写** `verified`，不要再问一句「确认」。

改正文用 `/ddove-wiki-update`。

## 步骤

1. **目标篇**：路径或 title。打开该 `.md`，看现有 `status` / `generated.by` / `verified`。
2. **目标状态**：用户没说清楚就问。`deprecated` **不删文件**。
3. **生成人**：本会话没有短号则问一次。改**别人的篇**：只改 `status` / `verified`；不改 `generated.by`；不挂到自己的 `index_*`。
4. **写 frontmatter**：
   - `draft`：`status: draft`。删掉 `verified`。
   - `stable`：`status: stable`。同步写 `verified: { by: human:<短号>, at: <UTC> }`。
   - `deprecated`：`status: deprecated`。正文可加一句替代篇链接。
5. **log**：`DDoveMiniGameWiki/_log/log_YYYY-MM-DD_<短号>.md` 顶部 `**Update**`：哪篇 `status` x → y（升 `stable` 时写了 `verified`）。
6. **检查**：`py -3 DDoveMiniGameTools/check_wiki.py`（或 `python DDoveMiniGameTools/check_wiki.py`）。

不要改根 `DDoveMiniGameWiki/index.md`。不要重生成各层 index。新建篇用 `ddove-wiki-create`。

完成：`status` 已写、当日 `_log` 已记、`check_wiki.py` 通过。
