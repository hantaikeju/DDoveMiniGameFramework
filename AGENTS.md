# MiniGameFramework — Agent 入口

知识在 `DDoveMiniGameWiki/`（OKF bundle）。**工程用法**在 `DDoveMiniGameWiki/01`–`04`；**需求稿**在 `DDoveMiniGameWiki/05-需求/`；**AI 用法**在 `DDoveMiniGameWiki/00-索引/Agent/`；约定在 `DDoveMiniGameWiki/00-索引/OKF约定.md`。规范原文是仓库根 `SPEC.md`（整文件替换后跟官方走）。

本库 skill 一律 `ddove-` 开头（旧名 `mgf-*` 已废）。不知道用哪个：`/ddove-help`。

1. 问答 / 排障 / 改代码：走 `ddove-work`（内部先跑 `ddove-wiki` 检索）。按需求稿开工见 `DDoveMiniGameWiki/00-索引/Agent/开工班组.md`。处理中的需求每一拍只碰一篇。只有一篇直接做。多于一篇只做这一拍点名的那篇；没点名就停，只问选哪一篇。其它篇不动。问答和排障不问选哪一篇。拆图 / 工人 / 审查 / 评分子代理在 `.cursor/agents/`（`ddove-plan` 只读；`ddove-worker` 可并行）。只要查篇：`ddove-wiki`。不要为了找篇去走 `index.md` → `index_<短号>`。不要默认翻 `DDoveMiniGameClient/Library`。
2. **生产 wiki（主动触发，不要在问答里顺手写）**：`/ddove-wiki-create` 新建篇；`/ddove-wiki-update` 改正文（回 `draft`）；`/ddove-wiki-change-state` 改 `draft`/`stable`/`deprecated`（升 `stable` 同步写 `verified`）。
3. 刚替换 `SPEC.md`，或要检查/升级 wiki 合规：主动调用 `ddove-okf-upgrade`（不要在日常问答里自动升级）。
4. 约定：`DDoveMiniGameWiki/00-索引/OKF约定.md`。约定与 `SPEC.md` 冲突时以 `SPEC.md` 为准，再改约定。
5. 新篇按路径、标题和正文区分。不问短号，不写 `generated.by`，不挂 `index_<短号>.md`。当日 log 写 `_log/log_日期.md`。缺 `type`/`status` 跑 `py -3 DDoveMiniGameTools/check_wiki.py`。
6. 需求未对齐：`/ddove-grill`。人的三步是对齐、开工、展示。对齐后同一条对话里代理落稿（`05-需求`，`draft` + `需求`）、按篇开工，做完把用法篇标正式、需求稿标废弃并搬入 `_archive/需求/`，再给人看产品。人不改状态。见 `DDoveMiniGameWiki/00-索引/Agent/需求稿工作流.md`。地图：`DDoveMiniGameWiki/00-索引/Agent/Agent总览.md`。
7. 写或改 skill：`ddove-writing-for-agents`。
8. 改 / 加配表、导表失败：`ddove-config`（对照 [luban/ai](https://github.com/focus-creative-games/luban/tree/main/ai)，本库改编）。
9. fork / 拷走开新游戏、改四件套目录名：`/ddove-rename-workspace`。权威：`DDoveMiniGameWiki/00-索引/Agent/改工作区前缀.md`。本框架仓不跑。
10. 场景 / Prefab / 资源 / 重编译：走 `ddove-unity-pipeline`。
11. 按需求稿做完：代理写上正式 / 废弃，再给人看产品（用法篇、说明页、这次改了什么）。产品不对时，人用自己的话指出，代理把两篇回到处理中，改完再写上。问答和排障不走这套收口。
