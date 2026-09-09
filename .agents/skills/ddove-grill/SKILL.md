---
name: ddove-grill
description: 拷问到决策对齐，可选落 draft 需求稿。只在用户调用 /ddove-grill 时用。
disable-model-invocation: true
---

# ddove-grill

权威：`wiki/00-索引/OKF约定.md`。工作流：`wiki/00-索引/Agent/需求稿工作流.md`。共享语言在 `wiki/`，不写 `CONTEXT.md`。

## 步骤

1. **检索**：按 `ddove-wiki` grep 已有篇。完成：已 grep。
2. **拷问**：按轮次问 **frontier**（前提已齐、尚未拍板的问题）。一轮列出全部 frontier；事实自查代码/wiki，决策等用户。每问固定形：`❓ **Q1** - **标题**`，正文，单独一行 `➡️` 推荐。
3. **对齐**：frontier 空则复述共享理解，等用户确认。完成：用户点头。
4. **落盘**（仅当用户要落需求稿）：读并执行 `.agents/skills/ddove-wiki-create/SKILL.md`（读文件，不要再 `/` 一次）。`type: Playbook`；`status: draft`。工程稿进 `01`–`04`（职种 `tags` + `需求`）；AI 用法稿进 `00-索引/Agent`（`tags: [索引, agent, 需求]`）。用户不落盘：停，并告诉下一刀是 `/ddove-wiki-create`。

入正式库不是本 skill：用户确认整篇处理后走 `/ddove-wiki-change-state`。

完成：用户确认对齐；若落盘，则 create 的完成条件也满足。
