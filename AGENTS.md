# MiniGameFramework — Agent 入口

知识在 `DDoveMiniGameWiki/`（OKF bundle）。**工程**在 `DDoveMiniGameWiki/01`–`04`；**AI 用法**在 `DDoveMiniGameWiki/00-索引/Agent/`；约定在 `DDoveMiniGameWiki/00-索引/OKF约定.md`。规范原文是仓库根 `SPEC.md`（整文件替换后跟官方走）。

本库 skill 一律 `ddove-` 开头（旧名 `mgf-*` 已废）。不知道用哪个：`/ddove-help`。

1. 问答 / 排障 / 改代码：走 `ddove-work`（内部先跑 `ddove-wiki` 检索）。按需求稿开工见 `DDoveMiniGameWiki/00-索引/Agent/开工班组.md`。拆图 / 工人 / 审查 / 评分子代理在 `.cursor/agents/`（`ddove-plan` 只读；`ddove-worker` 可并行）。只要查篇：`ddove-wiki`。不要为了找篇去走 `index.md` → `index_<短号>`。不要默认翻 `DDoveMiniGameClient/Library`。
2. **生产 wiki（主动触发，不要在问答里顺手写）**：`/ddove-wiki-create` 新建篇；`/ddove-wiki-update` 改正文（回 `draft`）；`/ddove-wiki-change-state` 改 `draft`/`stable`/`deprecated`（升 `stable` 同步写 `verified`）。
3. 刚替换 `SPEC.md`，或要检查/升级 wiki 合规：主动调用 `ddove-okf-upgrade`（不要在日常问答里自动升级）。
4. 约定：`DDoveMiniGameWiki/00-索引/OKF约定.md`。约定与 `SPEC.md` 冲突时以 `SPEC.md` 为准，再改约定。
5. 写篇须有生成人短号，挂 `index_<短号>.md`，当日 log 写 `_log/log_日期_短号.md`。缺 `type`/`status` 跑 `py -3 DDoveMiniGameTools/check_wiki.py`。
6. 需求未对齐：`/ddove-grill`；落稿 `/ddove-wiki-create`（`draft` + `需求`）；已有篇过期 `/ddove-wiki-update`；入正式库 `/ddove-wiki-change-state`。见 `DDoveMiniGameWiki/00-索引/Agent/需求稿工作流.md`。地图：`DDoveMiniGameWiki/00-索引/Agent/Agent总览.md`。
7. 写或改 skill：`ddove-writing-for-agents`。
8. 改 / 加配表、导表失败：`ddove-config`（对照 [luban/ai](https://github.com/focus-creative-games/luban/tree/main/ai)，本库改编）。
9. fork / 拷走开新游戏、改四件套目录名：`/ddove-rename-workspace`。权威：`DDoveMiniGameWiki/00-索引/Agent/改工作区前缀.md`。本框架仓不跑。
