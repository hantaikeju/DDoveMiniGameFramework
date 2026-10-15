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

`status` 只允许：`draft`（起草）/ `stable`（正式）/ `deprecated`（废弃）。`stable` = 人审过当前正文。升 `stable` 时写 `verified: { at: <UTC> }`，不写 `by`，不问短号。

改正文用 `/ddove-wiki-update`。本 skill 不改正文。例外：`05-需求` 整份做完时按步骤 3 写用法篇三块，需求稿留下要什么、不要、评分、用法篇链接和打开同名 HTML 的链接，并调用 `ddove-show-me` 两次。其它升 `stable` 或单独标 `deprecated` 不画 HTML。单独标 `deprecated` 可加一句替代篇链接。

本拍只碰一篇。处理中指 `05-需求` 且 `draft` 且 `tags` 含 `需求`。选篇在步骤 1。选定篇在 `05-需求` 且整份做完时，仍按步骤 3 写用法篇，并把该需求稿标 `deprecated` 后搬入归档夹。

## 步骤

1. **目标篇**：本拍只碰一篇。处理中指 `05-需求` 且 `draft` 且 `tags` 含 `需求`。
   - 只有一篇处理中，且用户没点别的篇：直接做这一篇。目标状态仍按步骤 2。
   - 处理中多于一篇，且本拍没点名（路径、`title` 或文件名）：停，只问选哪一篇，其它篇不动。
   - 点了名：只改那一篇。用户点名的不是处理中需求稿时，仍只做点名的那一篇，不把其它处理中稿拉进来。
   选定后打开该 `.md`，看现有 `status` / `tags` / `verified`。完成：已打开这一篇 frontmatter。多于一篇且没点名时已停，只问了选哪一篇，其它篇未动。
2. **目标状态**：用户没说清楚就问。`05-需求` 需求稿标 `deprecated` 时搬进归档夹，这一拍不删除。完成：目标状态已定。
3. **写 frontmatter**：
   - `draft`：`status: draft`。删掉 `verified`。若该需求稿在 `_archive/需求/`，连同名 `.html` 搬回 `05-需求/`，并删掉 `deprecated_at`。不调用 `ddove-show-me`。
   - `stable`，且篇在 `05-需求`：这是整份需求做完。按 `tags` 里的职种生成或改写用法篇，不把需求稿本身留成用法。画 HTML 只调用 `ddove-show-me`，不在本技能里另写画法。只调用两次：这一次的用法篇，以及搬入归档夹之后的这篇需求稿。不扫描仓库去给其它已有篇补 HTML。
     - `策划` → `01-策划`；`程序-前` → `02-程序-前`；`程序-后` → `03-程序-后`；`美术` → `04-美术`；只有 `索引` + `agent` → `00-索引/Agent`。
     - 用法篇：同一文件名。只写三块：干什么、有哪些接口、每个接口干什么。没有需求稿的「要什么 / 不要 / 验收」，没有 `## 评分`。不写 `generated.by`。`tags` 无 `需求`。`status: stable`，同步写 `verified: { at: <UTC> }`。目标目录已有同名篇则改正文并升 `stable`，不另开第二份。三块之外加一条 Markdown 链接，可见文字「打开说明」，打开同目录、与该 `.md` 主文件名相同的 `.html`，例如 `[打开说明](主文件名.html)`。然后读并执行 `.agents/skills/ddove-show-me/SKILL.md`（读文件，不要再 `/` 一次）：传入该用法篇路径，种类「说明页」。
     - 需求稿改为 `deprecated`，写入 `deprecated_at: <UTC>`。正文留下 `## 要什么`、`## 不要`、`## 评分` 和用法篇链接；不留 `## 验收`、`## 落点`。`title` 仍含「需求稿」。得分留在需求稿。开篇保留「打开说明」，指向同主文件名 `.html`。然后把该 `.md` 与同名 `.html` 一起搬到 `DDoveMiniGameWiki/_archive/需求/`。原路径不留文件。清单里链到该文件名的那一行改到归档路径；没有挂行则跳过。正文改完、搬走之后，再读并执行 `.agents/skills/ddove-show-me/SKILL.md`：传入归档夹里的需求稿路径，种类「说明页」，按最终 `## 要什么` / `## 不要` 重画。HTML 仍与 `.md` 同目录、同主文件名。
   - `stable`，且篇不在 `05-需求`：`status: stable`。同步写 `verified: { at: <UTC> }`，不写 `by`。从 `tags` **去掉** `需求`。不调用 `ddove-show-me`。
   - `deprecated`，且不是 `05-需求` 需求稿：`status: deprecated`。正文可加一句替代篇链接。不搬文件，不调用 `ddove-show-me`。
   完成：YAML 已写。`05-需求` 整份完成则用法篇已在职种目录且为 `stable`（只写干什么 / 有哪些接口 / 每个接口干什么，无 `## 评分`，无「要什么 / 不要 / 验收」；开篇有「打开说明」；旁边同名 `.html` 已由 `ddove-show-me` 写成），需求稿为 `deprecated` 且已在 `_archive/需求/`（留下 `## 要什么`、`## 不要`、`## 评分`、用法篇链接和「打开说明」，不留 `## 验收`、`## 落点`，`title` 含「需求稿」，带 `deprecated_at`）。其它篇升 `stable` 则无 `需求` tag 且 `verified` 只有 `at`。
4. **清单一行**：在 `05-需求/index_cjh.md`、同目录 `index.md`、`00-索引/index.md`、`00-索引/Agent/index.md` 里找链到本篇文件名的那一行。需求稿已搬家的，把链接改到 `_archive/需求/`。只改该行路径和末尾徽章：先 `type`；`status` 为 `draft` 或 `deprecated` 时再写对应词；`tags` 仍含 `需求` 则写 `需求`。升 `stable` 且已摘 `需求` 后只留 `type`。没有挂行则跳过。不重写其它行，不改根 `DDoveMiniGameWiki/index.md`。完成：至多改了这些文件里的该行。
5. **log**：`DDoveMiniGameWiki/_log/log_YYYY-MM-DD.md` 顶部 `**Update**`：哪篇 `status` x → y，需求稿是否已搬入归档夹。完成：当日 log 已记。
6. **检查**：`py -3 DDoveMiniGameTools/check_wiki.py`（或 `python DDoveMiniGameTools/check_wiki.py`）。完成：检查通过。
7. **收口**：`05-需求` 整份做完时，本轮改过正文的篇写成 `stable`，`verified` 只写时刻，清单去掉 `draft`。人不处理这些状态。然后给人看产品：用法篇、说明页、这次改了什么。改了客户端时写明行为是否符合对齐过的需求。不把落稿或改状态报成用户的下一步。其它 `status` 变更不要写成产品收口。

新建篇用 `ddove-wiki-create`。到期删除用 `/ddove-archive`，不在本技能里删文件。

完成：`status` 已写。升 `stable` 则无 `需求` 且 `verified` 只有时刻。`05-需求` 需求稿标废弃后人在 `_archive/需求/` 找得到，原路径没有留下文件。当日 `_log` 已记，`check_wiki.py` 通过。整份做完时给人看产品。
