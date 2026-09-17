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

权威：`DDoveMiniGameWiki/00-索引/OKF约定.md`。「信任与生命周期」一节。清单行：`DDoveMiniGameWiki/00-索引/Agent/开篇与清单徽章.md`。

`status` 只允许：`draft`（起草）/ `stable`（正式）/ `deprecated`（废弃）。不要另造字段。`stable` = 人审过当前正文；升 `stable` 时**同步写** `verified`，不要再问一句「确认」。

改正文用 `/ddove-wiki-update`。本 skill 不改正文（`deprecated` 可加一句替代篇链接）。

## 步骤

1. **目标篇**：路径或 title。打开该 `.md`，看现有 `status` / `tags` / `generated.by` / `verified`。完成：已打开 frontmatter。
2. **目标状态**：用户没说清楚就问。`deprecated` **不删文件**。完成：目标状态已定。
3. **生成人**：本会话没有短号则问一次。改**别人的篇**：只改 `status` / `verified` / 升 `stable` 时的 `需求` tag / 清单该行；不改 `generated.by`；不挂到自己的 `index_*`。完成：短号已定。
4. **写 frontmatter**：
   - `draft`：`status: draft`。删掉 `verified`。
   - `stable`：`status: stable`。同步写 `verified: { by: human:<短号>, at: <UTC> }`。从 `tags` **去掉** `需求`。
   - `deprecated`：`status: deprecated`。正文可加一句替代篇链接。
   完成：YAML 已写；升 `stable` 则无 `需求` tag 且已有 `verified`。
5. **清单一行**：在本篇目录的 `index_<短号>.md`（`generated.by` 的短号）、同目录 `index.md`、`00-索引/index.md` 里找链到本篇的那一行。只改该行末尾徽章：先 `type`；`status` 为 `draft` 或 `deprecated` 时再写对应词；`tags` 仍含 `需求` 则写 `需求`。升 `stable` 且已摘 `需求` 后只留 `type`。没有挂行则跳过。不重写其它行，不改根 `DDoveMiniGameWiki/index.md`，不重生成整份清单。完成：至多改了这些文件里的该行。
6. **log**：`DDoveMiniGameWiki/_log/log_YYYY-MM-DD_<短号>.md` 顶部 `**Update**`：哪篇 `status` x → y（升 `stable` 时写了 `verified`、摘了 `需求`）。完成：当日 log 已记。
7. **检查**：`py -3 DDoveMiniGameTools/check_wiki.py`（或 `python DDoveMiniGameTools/check_wiki.py`）。完成：检查通过。

新建篇用 `ddove-wiki-create`。

完成：`status` 已写、升 `stable` 则无 `需求` 且已写 `verified`、清单该行徽章已对齐（或本无挂行）、当日 `_log` 已记、`check_wiki.py` 通过。
