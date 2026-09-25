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

改正文用 `/ddove-wiki-update`。本 skill 不改正文。例外：`05-需求` 升 `stable` 时按步骤 4 写用法篇三块，需求稿留下要什么、不要、评分、用法篇链接和打开同名 HTML 的链接，并调用 `ddove-show-me` 两次。其它升 `stable` 或单独标 `deprecated` 不画 HTML。单独标 `deprecated` 可加一句替代篇链接。

## 步骤

1. **目标篇**：路径或 title。打开该 `.md`，看现有 `status` / `tags` / `generated.by` / `verified`。完成：已打开 frontmatter。
2. **目标状态**：用户没说清楚就问。`deprecated` **不删文件**。完成：目标状态已定。
3. **生成人**：本会话没有短号则问一次。改**别人的篇**：只改 `status` / `verified` / 升 `stable` 时的 `需求` tag / 清单该行；不改 `generated.by`；不挂到自己的 `index_*`。例外：`05-需求` 整份做完按步骤 4 改用法篇正文，以及需求稿的正文和 `title`。完成：短号已定。
4. **写 frontmatter**：
   - `draft`：`status: draft`。删掉 `verified`。不调用 `ddove-show-me`。
   - `stable`，且篇在 `05-需求`：这是整份需求做完。按 `tags` 里的职种生成或改写用法篇，不把需求稿本身留成用法。画 HTML 只调用 `ddove-show-me`，不在本技能里另写画法。只调用两次：这一次的用法篇，以及这一篇需求稿。不扫描仓库去给其它已有篇补 HTML。
     - `策划` → `01-策划`；`程序-前` → `02-程序-前`；`程序-后` → `03-程序-后`；`美术` → `04-美术`；只有 `索引` + `agent` → `00-索引/Agent`。
     - 用法篇：同一文件名。只写三块：干什么、有哪些接口、每个接口干什么。没有需求稿的「要什么 / 不要 / 验收」，没有 `## 评分`。`tags` 无 `需求`。`status: stable`，同步写 `verified`。目标目录已有同名篇则改正文并升 `stable`，不另开第二份。三块之外加一条 Markdown 链接，打开同目录、与该 `.md` 主文件名相同的 `.html`。然后读并执行 `.agents/skills/ddove-show-me/SKILL.md`（读文件，不要再 `/` 一次）：传入该用法篇路径，种类「三块」。
     - 需求稿改为 `deprecated`，文件不删。正文留下 `## 要什么`、`## 不要`、`## 评分` 和用法篇链接；不留 `## 验收`、`## 落点`。`title` 仍含「需求稿」。得分留在需求稿。改写后仍保留打开同目录、同主文件名 `.html` 的链接；这条链接若在要删的段里，挪到留下的正文里。用法篇挂到目标目录的 `index_<短号>.md`；需求稿那一行徽章改为 `deprecated`。正文改完后，再读并执行 `.agents/skills/ddove-show-me/SKILL.md`（读文件，不要再 `/` 一次）：传入该需求稿路径，种类「要什么不要」，按最终 `## 要什么` / `## 不要` 重画。HTML 主文件名取该 `.md` 文件名，不用带「需求稿」的 `title`。
   - `stable`，且篇不在 `05-需求`：`status: stable`。同步写 `verified: { by: human:<短号>, at: <UTC> }`。从 `tags` **去掉** `需求`。不调用 `ddove-show-me`。
   - `deprecated`：`status: deprecated`。正文可加一句替代篇链接。不调用 `ddove-show-me`。
   完成：YAML 已写。`05-需求` 整份完成则用法篇已在职种目录且为 `stable`（只写干什么 / 有哪些接口 / 每个接口干什么，无 `## 评分`，无「要什么 / 不要 / 验收」；篇内有打开同名 `.html` 的链接；旁边同名 `.html` 已由 `ddove-show-me` 按种类「三块」写好），需求稿为 `deprecated`（留下 `## 要什么`、`## 不要`、`## 评分`、用法篇链接和打开同名 `.html` 的链接，不留 `## 验收`、`## 落点`，`title` 含「需求稿」，文件还在；旁边同名 `.html` 已按最终 `## 要什么` / `## 不要` 重画）。其它篇升 `stable` 则无 `需求` tag 且已有 `verified`，且未调用 `ddove-show-me`。单独 `deprecated` 也未调用 `ddove-show-me`。
5. **清单一行**：在本篇目录的 `index_<短号>.md`（`generated.by` 的短号）、同目录 `index.md`、`00-索引/index.md` 里找链到本篇的那一行。只改该行末尾徽章：先 `type`；`status` 为 `draft` 或 `deprecated` 时再写对应词；`tags` 仍含 `需求` 则写 `需求`。升 `stable` 且已摘 `需求` 后只留 `type`。没有挂行则跳过。不重写其它行，不改根 `DDoveMiniGameWiki/index.md`，不重生成整份清单。完成：至多改了这些文件里的该行。
6. **log**：`DDoveMiniGameWiki/_log/log_YYYY-MM-DD_<短号>.md` 顶部 `**Update**`：哪篇 `status` x → y（升 `stable` 时写了 `verified`、摘了 `需求`）。完成：当日 log 已记。
7. **检查**：`py -3 DDoveMiniGameTools/check_wiki.py`（或 `python DDoveMiniGameTools/check_wiki.py`）。完成：检查通过。

新建篇用 `ddove-wiki-create`。

完成：`status` 已写、升 `stable` 则无 `需求` 且已写 `verified`、清单该行徽章已对齐（或本无挂行）、当日 `_log` 已记、`check_wiki.py` 通过。`05-需求` 整份做完时，用法篇旁边有同名 `.html`（三块）且篇内有链接；需求稿旁边的同名 `.html` 与最终「要什么 / 不要」一致，稿内仍有打开它的链接。
