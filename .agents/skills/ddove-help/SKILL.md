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

完成：用户能说出下一刀的 `/` 命令。

## 整图

```text
不知道用哪个          → /ddove-help          （本 skill）
问答 / 排障 / 改代码   → ddove-work           （内部先 ddove-wiki）
按需求稿开工          → ddove-work           （draft+需求；见开工班组）
改表 / 加表 / 导表失败 → ddove-config
只查 wiki            → ddove-wiki
方案没对齐            → /ddove-grill         → 落稿 /ddove-wiki-create（draft+需求）
已有篇改正文          → /ddove-wiki-update   （回 draft；再升正式走 change-state）
整篇能当约定          → /ddove-wiki-change-state  （stable，同步写 verified）
只建 / 改正文 / 改状态 → /ddove-wiki-create 或 /ddove-wiki-update 或 /ddove-wiki-change-state
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
| 处理中的需求稿 | `DDoveMiniGameWiki/05-需求` |
| AI 怎么用 | `DDoveMiniGameWiki/00-索引/Agent` |
| 约定 / 合规 | `DDoveMiniGameWiki/00-索引` 根，只读 `OKF约定.md` |

## 选路

- fork / 拷走开新游戏、改 Client/Config/Wiki/Tools 目录名 → `/ddove-rename-workspace`
- 改 / 加 `DDoveMiniGameConfig` 表、导表失败 → `ddove-config`
- 已有篇、要答/排障/改代码 → `ddove-work`
- 已有 `draft`+`需求`、按稿实现 → `ddove-work`（开工班组）
- 只要检索、不要结论 → `ddove-wiki`
- 新功能还没想清 → `/ddove-grill`（不要先 create）
- 已有篇正文过期 → `/ddove-wiki-update`
- grill 完要落盘 → `/ddove-wiki-create`（需求稿进 `05-需求`；整份做完才生成到 `01`–`04` 或 `Agent/`）
- 上句没听懂 → 让用户把那句标出来重讲（无仓库 skill；用 wiki 词）
- 窗口要换人 / 换目录 → 口头交班即可；无仓库 `/handoff`
