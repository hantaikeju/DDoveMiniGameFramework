# DDove MiniGame Framework

Unity 小游戏框架工作区。知识库与 Agent 约定在本仓库根，与客户端工程并列。

| 路径 | 作用 |
|------|------|
| `SPEC.md` | OKF 规范原文。官方更新后整文件替换，再调用 `ddove-okf-upgrade` |
| `wiki/` | OKF 知识包。机器检索：grep frontmatter（排除 `_log/`）。`index.md` 只服务写入 |
| `.agents/skills/`、`.cursor/skills/` | 一律 `ddove-`：`ddove-wiki` 只检索；`ddove-work` 问答/排障/改代码；`/ddove-wiki-create` 建篇；`/ddove-wiki-change-state` 改状态；`ddove-okf-upgrade` 跟 SPEC |
| `AGENTS.md` | 各 Agent 的短指针 |
| `DDoveMiniGameClient/` | Unity 工程。默认不检索 `Library/` |

日常干活用 `ddove-work`（先检索）。只查篇用 `ddove-wiki`。写 wiki 用 `/ddove-wiki-create` / `/ddove-wiki-change-state`。替换 `SPEC.md` 后调用 `ddove-okf-upgrade`。
