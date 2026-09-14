---
name: ddove-wiki-update
description: >-
  Update an existing wiki concept body (description / sources too).
  Use only when invoked as /ddove-wiki-update or the user explicitly
  asks to update or align existing wiki content. Do not run during
  ordinary Q&A.
disable-model-invocation: true
---

# 更新 wiki 正文

权威：`DDoveMiniGameWiki/00-索引/OKF约定.md`。「信任与生命周期」一节。检索原语：`.agents/skills/ddove-wiki/SKILL.md`。

不建篇（那是 `/ddove-wiki-create`）。不升 `stable`、不作废（那是 `/ddove-wiki-change-state`）。方案没对齐先停，让用户 `/ddove-grill`。

`stable` = 人审过**当前正文**。改正文后必须回 `draft` 并摘掉 `verified`，再升正式走 change-state。

## 步骤

1. **目标篇**：路径或 title。打开该 `.md`。没有这篇 → 停，告诉用户 `/ddove-wiki-create`。完成：已打开 frontmatter。
2. **检索**：执行 `ddove-wiki` 全文步骤。完成：已有命中列表或「Wiki 未收录」。
3. **对源**：用户说的变更 + 对得上的脚本。列出将改段落 / 不改段落。交叉点只链 sibling，不抄对方正文。默认一次一篇；用户说「对齐 X」时先列出过期候选再改。对不齐 → 停，`/ddove-grill`。完成：改动清单已定。
4. **写**：只动正文、`description`、`sources`。`status: draft`；删掉 `verified`。不改 `generated.by` / `type` / 文件名。改**别人的篇**：不挂自己的 `index_*`。完成：文件已写。
5. **生成人**：只为写 `_log`。本会话没有短号则问一次。`_log/log_YYYY-MM-DD_<短号>.md` 顶部 `**Update**`：哪篇改正文、`status` → draft（及摘了 `verified`）。
6. **检查**：`py -3 DDoveMiniGameTools/check_wiki.py`（或 `python DDoveMiniGameTools/check_wiki.py`）。

不要改根 `DDoveMiniGameWiki/index.md`。不要重生成各层 index。不扫 `DDoveMiniGameClient/Library`。

完成：正文已改、`status` 为 `draft`、无 `verified`、当日 `_log` 已记、`check_wiki.py` 通过。回复写下一刀是 `/ddove-wiki-change-state`。
