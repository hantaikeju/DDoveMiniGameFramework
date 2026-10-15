---
name: ddove-help
description: 提示本库整套工作流该敲哪个命令。只在用户调用 /ddove-help 时用。
disable-model-invocation: true
---

# ddove-help

权威地图：`DDoveMiniGameWiki/00-索引/Agent/Agent总览.md`。约定：`DDoveMiniGameWiki/00-索引/OKF约定.md`。本 skill **只指路**，不代跑其它 `/ddove-*`，不写文件。

## 步骤

1. 用户已经说卡在哪：只回 **一个** 下一刀（命令 + 一句为什么）。未说：先输出下面整图，再问「你现在卡在哪一步？」
2. 干活走 `ddove-work`；只查篇走 `ddove-wiki`。
3. 处理中 = `05-需求` 且 `status: draft` 且 `tags` 含 `需求`。人的下一步是 `/ddove-grill` 时，每一拍只碰一篇。只有一篇直接做，不必点名。多于一篇只做点名的那篇（路径、`title` 或文件名）；没点名就停，只问选哪一篇。问答和排障不问选哪一篇，也不进开工班组。对齐之后的落稿、改文件、写状态由代理做，不报成用户的下一步。完成：下一刀是 `/ddove-grill`，或只问选哪一篇，或说明代理会接着做完。

完成：用户能说出下一刀的 `/` 命令。

## 整图

```text
不知道用哪个          → /ddove-help          （本 skill）
问答 / 排障 / 改代码   → ddove-work           （内部先 ddove-wiki；问答和排障不进开工班组，也不问选哪一篇）
按需求稿开工          → 对齐后的同一条对话里代理做 ddove-work
开工做完              → 代理写上正式 / 废弃，再给人看产品
产品不对              → 人用自己的话指出；代理把两篇回到处理中再做完
改表 / 加表 / 导表失败 → ddove-config
只查 wiki            → ddove-wiki
方案没对齐            → /ddove-grill         （一拍只碰一篇；对齐后代理落稿、改文件、写状态）
已有篇改正文          → 代理按 ddove-wiki-update 改正文；收口时由代理标正式
换了 SPEC.md          → /ddove-okf-upgrade
写或改 skill          → ddove-writing-for-agents
fork 改四件套前缀     → /ddove-rename-workspace（本框架仓不跑）
上游 mattpocock 更新   → 读 DDoveMiniGameWiki/00-索引/Agent/上游skill更新.md
要把话题画清楚        → /show-me             （本机，不进仓库）
换引擎迁工作流        → 读 DDoveMiniGameWiki/00-索引/Agent/工作流迁移.md
```

需求稿细则：`DDoveMiniGameWiki/00-索引/Agent/需求稿工作流.md`。开工班组：`DDoveMiniGameWiki/00-索引/Agent/开工班组.md`。

## 两层 wiki

| 内容 | 检索 / 落篇 |
|------|-------------|
| 本工程用法（Res / Boot / Editor…） | `DDoveMiniGameWiki/01`–`04` |
| 处理中的需求稿 | `DDoveMiniGameWiki/05-需求`（`draft` 且 `tags` 含 `需求`） |
| AI 怎么用 | `DDoveMiniGameWiki/00-索引/Agent` |
| 约定 / 合规 | `DDoveMiniGameWiki/00-索引` 根，只读 `OKF约定.md` |

## 选路

处理中 = `05-需求` 且 `status: draft` 且 `tags` 含 `需求`。每一拍只碰一篇。只有一篇直接做，不必点名。多于一篇只做点名的那篇（路径、`title` 或文件名）；没点名就停，只问选哪一篇。问答和排障的指路不问选哪一篇，也不进开工班组。

- fork / 拷走开新游戏、改 Client/Config/Wiki/Tools 目录名 → `/ddove-rename-workspace`
- 改 / 加 `DDoveMiniGameConfig` 表、导表失败 → `ddove-config`
- 已有篇、要答/排障/改代码 → `ddove-work`（问答和排障不进开工班组，也不问选哪一篇；改代码按处理中每一拍只碰一篇）
- 已有 `draft`+`需求`、按稿实现 → 代理做 `ddove-work`（开工班组；每一拍只碰一篇；做完写上正式 / 废弃，再给人看产品）
- 只要检索、不要结论 → `ddove-wiki`
- 新功能还没想清 → `/ddove-grill`（每一拍只碰一篇；对齐后代理落稿、改文件、写状态，再给人看产品）
- 已有篇正文过期 → 代理按 `/ddove-wiki-update` 改正文
- 产品不对 → 人用自己的话指出；代理把用法篇和需求稿回到处理中，改完再写成正式 / 废弃
- 上句没听懂 → 让用户把那句标出来重讲（无仓库 skill；用 wiki 词）
- 窗口要换人 / 换目录 → 口头交班即可；无仓库 `/handoff`
