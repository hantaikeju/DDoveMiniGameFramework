# MiniGameFramework — Agent 入口

知识在 `wiki/`（OKF bundle）。规范原文是仓库根 `SPEC.md`（整文件替换后跟官方走）。

1. 问答 / 排障 / 改代码：先走 skill `mgf-wiki`。读 `wiki/index.md`，再 grep `wiki/` 的 frontmatter，不要默认翻 `DDoveMiniGameClient/Library`。
2. 刚替换 `SPEC.md`，或要检查/升级 wiki 合规：主动调用 `mgf-okf-upgrade`（不要在日常问答里自动升级）。
3. 约定：`wiki/00-索引/OKF约定.md`。约定与 `SPEC.md` 冲突时以 `SPEC.md` 为准，再改约定。
4. 写/改业务 wiki 前先要一次生成人短号（会话内复用），写入 `generated.by: human:<短号>`，篇挂 `index_<短号>.md`。不要猜、不要用 `process:agent` 顶替。
