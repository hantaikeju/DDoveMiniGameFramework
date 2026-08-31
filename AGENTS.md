# MiniGameFramework — Agent 入口

知识在 `wiki/`（OKF bundle）。规范原文是仓库根 `SPEC.md`（整文件替换后跟官方走）。

本库 skill 一律 `ddove-` 开头（旧名 `mgf-*` 已废）。

1. 问答 / 排障 / 改代码：先走 skill `ddove-wiki`。**直接** grep `wiki/` 的 frontmatter（排除 `_log/` `_tools/` `_spec/`），不要为了找篇去走 `index.md` → `index_<短号>`。不要默认翻 `DDoveMiniGameClient/Library`。
2. **生产 wiki（主动触发，不要在问答里顺手写）**：`/ddove-wiki-create` 新建篇；`/ddove-wiki-change-state` 改 `draft`/`stable`/`deprecated`（及确认时的 `verified`）。
3. 刚替换 `SPEC.md`，或要检查/升级 wiki 合规：主动调用 `ddove-okf-upgrade`（不要在日常问答里自动升级）。
4. 约定：`wiki/00-索引/OKF约定.md`。约定与 `SPEC.md` 冲突时以 `SPEC.md` 为准，再改约定。
5. 写篇须有生成人短号，挂 `index_<短号>.md`，当日 log 写 `_log/log_日期_短号.md`。缺 `type`/`status` 跑 `py -3 scripts/check_wiki.py`。
