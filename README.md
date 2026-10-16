# DDove MiniGame Framework

Unity 小游戏框架工作区。知识库与 Agent 约定在本仓库根，与客户端工程并列。人从这里进；Agent 读 `AGENTS.md`。

## 人的三步

细则：[需求稿工作流](DDoveMiniGameWiki/00-索引/Agent/需求稿工作流.md)。地图：[Agent 总览](DDoveMiniGameWiki/00-索引/Agent/Agent总览.md)。

1. **对齐**：`/ddove-grill` 问完落稿后停下；已有稿用 `/ddove-grill-with-demand`，问完按已有这篇开工。
2. **开工**：人另发 `/ddove-work`，只做本条对话刚落的那一篇。
3. **展示**：看用法篇、说明页、这次改了什么。

## 其它入口

| 你要做的 | 命令 |
|----------|------|
| 问答 / 排障 | `ddove-work`（先检索；不进开工班组） |
| 改表 / 加表 / 导表失败 | `ddove-config` |
| 场景、Prefab、材质、重编译 | `ddove-unity-pipeline` |
| 只查 wiki | `ddove-wiki` |
| 废弃需求稿超过 90 天 | `/ddove-archive` |
| 刚替换 `SPEC.md` | `ddove-okf-upgrade` |
| 写或改 skill | `ddove-writing-for-agents` |
| fork 后改四件套目录名 | `/ddove-rename-workspace`（本框架仓不跑） |

## 目录

| 路径 | 作用 |
|------|------|
| `SPEC.md` | OKF 规范原文。官方更新后整文件替换，再调用 `ddove-okf-upgrade` |
| `AGENTS.md` | Agent 短指针 |
| `DDoveMiniGameWiki/` | OKF 知识包。`01`–`04` 用法篇；`05-需求/` 处理中的需求稿；`00-索引/Agent/` AI 用法；`_archive/需求/` 已废弃需求稿。机器检索 grep frontmatter（排除 `_log/`）。`index.md` 只服务写入 |
| `DDoveMiniGameTools/` | `check_wiki.py`（frontmatter 闸门）。导表脚本留在配表目录 |
| `.agents/skills/`、`.cursor/skills/` | 仓库 skill，一律 `ddove-` |
| `.cursor/agents/` | 开工班组：拆图、工人、审查、评分。人的入口仍是 `ddove-work` |
| `DDoveMiniGameClient/` | Unity 工程。默认不检索 `Library/` |
| `DDoveMiniGameConfig/` | 共用配表：`Data/`（本刀 csv）、`gen_client`、Luban 生成器 |

## 第三方

`DDoveMiniGameClient/Packages/com.kyrylokuzyk.primetween@1.4.11/` 嵌入 [PrimeTween](https://github.com/KyryloKuzyk/PrimeTween)（版权 Kyrylo Kuzyk），只为离线构建。本仓库不卖、不主张其所有权。条款见包内 `license.md`。用法见 [DDoveMiniGameWiki/02-程序-前/PrimeTween.md](DDoveMiniGameWiki/02-程序-前/PrimeTween.md)。

`DDoveMiniGameConfig/Tools/Luban/` 钉 [Luban](https://github.com/focus-creative-games/luban) Release **v5.1.0** 生成器；`DDoveMiniGameClient/Packages/com.code-philosophy.luban@1.2.0/` 嵌入 [luban_unity](https://github.com/focus-creative-games/luban_unity)。MIT。用法见 [DDoveMiniGameWiki/02-程序-前/DDoveCfg.md](DDoveMiniGameWiki/02-程序-前/DDoveCfg.md)。导表在外面双击 `DDoveMiniGameConfig/gen_client.bat`。
