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

权威：`DDoveMiniGameWiki/00-索引/OKF约定.md`。「信任与生命周期」一节。检索原语：`.agents/skills/ddove-wiki/SKILL.md`。开篇不自报 status：`DDoveMiniGameWiki/00-索引/Agent/开篇与清单徽章.md`。

不建篇（那是 `/ddove-wiki-create`）。不升 `stable`、不作废（那是 `/ddove-wiki-change-state`）。方案没对齐先停，让用户 `/ddove-grill`。

`stable` = 人审过**当前正文**。改正文后必须回 `draft` 并摘掉 `verified`，再升正式走 change-state。不要在本 skill 里再升正式。

改正文后，这篇旁边已有同名 HTML 则调用 `ddove-show-me`（种类「说明页」）重画这一张，并把开篇链接改成「打开说明」。没有同名 HTML 的不补。画法只跟 `ddove-show-me`。不新建「查看 HTML」指令。

## 步骤

1. **目标篇**：路径或 title。打开该 `.md`。没有这篇 → 停，告诉用户 `/ddove-wiki-create`。完成：已打开 frontmatter。
2. **检索**：执行 `ddove-wiki` 全文步骤。完成：已有命中列表或「Wiki 未收录」。
3. **对源**：用户说的变更 + 对得上的脚本。列出将改段落 / 不改段落。交叉点只链 sibling，不抄对方正文。默认一次一篇；用户说「对齐 X」时先列出过期候选再改。对不齐 → 停，`/ddove-grill`。完成：改动清单已定。
4. **写**：只动正文、`description`、`sources`。`status: draft`；删掉 `verified`。不改已有的 `generated.by` / `type` / 文件名，也不新写 `generated.by`。正文（开篇、结论）不写「本篇是 draft」「本文 `status: draft`」「不当发布依据」；将改段落里有则删。完成：文件已写，篇首无自报句。
5. **说明页**（仅旁边已有同名 HTML）：同目录、同主文件名已有 `.html` 时，读并执行 `.agents/skills/ddove-show-me/SKILL.md`（读文件，不要再 `/` 一次）。入参是这篇 `.md` 路径，种类「说明页」。一次一篇。不在本技能里另写画法。开篇打开同名 HTML 的链接可见文字改成「打开说明」，例如 `[打开说明](主文件名.html)`；没有这条链接则在开篇补上。没有同名 HTML：不调用 show-me，不补 HTML，不加链接。完成：已有同名 HTML 则已按种类「说明页」重画且开篇是「打开说明」；没有则正文已改、磁盘上没有新 HTML。
6. **log**：不问短号。`_log/log_YYYY-MM-DD.md` 顶部 `**Update**`：哪篇改正文、`status` → draft（及摘了 `verified`）。完成：当日 log 已记。
7. **检查**：`py -3 DDoveMiniGameTools/check_wiki.py`（或 `python DDoveMiniGameTools/check_wiki.py`）。完成：检查通过。

不要改根 `DDoveMiniGameWiki/index.md`。不要重生成各层 index。不扫 `DDoveMiniGameClient/Library`。

完成：正文已改、篇首无 status 自报、当日 `_log` 已记、`check_wiki.py` 通过。单独改正文时 `status` 为 `draft`、无 `verified`。这次改正文属于已验收需求的收口时，同一拍写成 `stable`，`verified` 只写时刻，清单去掉 `draft`，人不处理这个状态。旁边已有同名 HTML 则已按种类「说明页」重画且开篇是「打开说明」；没有则未补。收口接着给人看产品，不把下一指令报给用户。
