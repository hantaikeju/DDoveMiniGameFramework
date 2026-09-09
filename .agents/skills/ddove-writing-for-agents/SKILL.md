---
name: ddove-writing-for-agents
description: >-
  写或改本库 Agent skill、AGENTS.md、wiki type Skill。
  Triggers: 新建 ddove-*、优化 SKILL.md、ddove 化 writing-for-agents。
---

# ddove-writing-for-agents

共享语言在 `wiki/`。仓库 skill 名必须 `ddove-`。权威：`wiki/00-索引/OKF约定.md`。

## 步骤

1. **落点**：必须懂 wiki / 短号 / OKF → `.agents/skills/ddove-<x>/`，并同步 `.cursor/skills/ddove-<x>/`（本库 Cursor 入口）。纯对话、全机通用 → 本机 `~\.cursor\skills\`，不进 Git。
2. **每个 SKILL.md**：
   - `description` 只写触发分支。编排类加 `disable-model-invocation: true`。
   - 正文是步骤，每步有完成条件。
   - OKF / 短号 / index / log 只指针约定篇，不抄表。
   - 正向步骤。
3. **职责不抢**：整图指路归 `/ddove-help`；只检索归 `ddove-wiki`；问答 / 排障 / 改代码归 `ddove-work`；建篇归 `ddove-wiki-create`；升正式归 `ddove-wiki-change-state`；拷问归 `ddove-grill`。
4. **wiki**：用户要落 `type: Skill` 篇时走 create，写到 `00-索引/Agent/`。本 skill 不直接建概念。`AGENTS.md` 只加一行指针。

完成：目标 `SKILL.md` 可运行；无 `CONTEXT.md`；无第二套 `status`；与现有四个生产 skill 不重复职责。
