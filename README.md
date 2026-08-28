# DDove MiniGame Framework

Unity 小游戏框架工作区。知识库与 Agent 约定在本仓库根，与客户端工程并列。

| 路径 | 作用 |
|------|------|
| `SPEC.md` | OKF 规范原文。官方更新后整文件替换，再调用 `mgf-okf-upgrade` |
| `wiki/` | OKF 知识包。机器先读 `wiki/index.md` |
| `.agents/skills/` | 跨编辑器 skill：`mgf-wiki`（检索）、`mgf-okf-upgrade`（检查/升级） |
| `AGENTS.md` | 各 Agent 的短指针 |
| `DDoveMiniGameClient/` | Unity 工程。默认不检索 `Library/` |

日常问答用 `mgf-wiki`。替换 `SPEC.md` 后主动调用 `mgf-okf-upgrade`。
