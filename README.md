# DDove MiniGame Framework

Unity 小游戏框架工作区。知识库与 Agent 约定在本仓库根，与客户端工程并列。

| 路径 | 作用 |
|------|------|
| `SPEC.md` | OKF 规范原文。官方更新后整文件替换，再调用 `ddove-okf-upgrade` |
| `DDoveMiniGameWiki/` | OKF 知识包。机器检索：grep frontmatter（排除 `_log/`）。`index.md` 只服务写入 |
| `DDoveMiniGameTools/` | 仓库工具：`check_wiki.py`（frontmatter 闸门）。**不要**把导表脚本塞进来 |
| `.agents/skills/`、`.cursor/skills/` | 一律 `ddove-`：`ddove-wiki` 只检索；`ddove-work` 问答/排障/改代码；`/ddove-wiki-create` 建篇；`/ddove-wiki-change-state` 改状态；`ddove-okf-upgrade` 跟 SPEC |
| `AGENTS.md` | 各 Agent 的短指针 |
| `DDoveMiniGameClient/` | Unity 工程。默认不检索 `Library/` |
| `DDoveMiniGameConfig/` | 共用配表：`Data/`（本刀 csv）、`gen_client`、Luban 生成器 |

日常干活用 `ddove-work`（先检索）。只查篇用 `ddove-wiki`。写 wiki 用 `/ddove-wiki-create` / `/ddove-wiki-change-state`。替换 `SPEC.md` 后调用 `ddove-okf-upgrade`。

## 第三方

`DDoveMiniGameClient/Packages/com.kyrylokuzyk.primetween@1.4.11/` 嵌入 [PrimeTween](https://github.com/KyryloKuzyk/PrimeTween)（版权 Kyrylo Kuzyk），只为离线构建。本仓库不卖、不主张其所有权。条款见包内 `license.md`。用法见 [DDoveMiniGameWiki/02-程序-前/PrimeTween.md](DDoveMiniGameWiki/02-程序-前/PrimeTween.md)。

`DDoveMiniGameConfig/Tools/Luban/` 钉 [Luban](https://github.com/focus-creative-games/luban) Release **v5.1.0** 生成器；`DDoveMiniGameClient/Packages/com.code-philosophy.luban@1.2.0/` 嵌入 [luban_unity](https://github.com/focus-creative-games/luban_unity)。MIT。用法见 [DDoveMiniGameWiki/02-程序-前/DDoveCfg.md](DDoveMiniGameWiki/02-程序-前/DDoveCfg.md)。导表在外面双击 `DDoveMiniGameConfig/gen_client.bat`。
