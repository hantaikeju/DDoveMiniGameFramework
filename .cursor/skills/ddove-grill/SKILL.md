---
name: ddove-grill
description: 拷问到决策对齐，可选落 draft 需求稿。只在用户调用 /ddove-grill 时用。
disable-model-invocation: true
---

# ddove-grill

权威：`DDoveMiniGameWiki/00-索引/OKF约定.md`。工作流：`DDoveMiniGameWiki/00-索引/Agent/需求稿工作流.md`。共享语言在 `DDoveMiniGameWiki/`，不写 `CONTEXT.md`。

## 步骤

1. **检索**：按 `ddove-wiki` grep 已有篇。完成：已 grep。
2. **拷问**：按轮次问 **frontier**（前提已齐、尚未拍板的问题）。事实自查代码/wiki，决策等用户。气泡只写检索结论和「已齐、不再问」，**不要**铺 `❓ Q1` 或 A/B 列表。本轮全部无依赖 frontier 用 Cursor **`AskQuestion` 弹窗一次问完**：每题 ≥2 选项，推荐放第一项并标「推荐」；开放发挥走 Other。有依赖的题等用户提交后再开下一轮弹窗。完成：用户已提交本轮弹窗。
3. **对齐**：frontier 空则复述共享理解，等用户确认。完成：用户点头。
4. **落盘**（仅当用户要落需求稿）：读并执行 `.agents/skills/ddove-wiki-create/SKILL.md`（读文件，不要再 `/` 一次）。目录一律 `05-需求`；`type: Playbook`；`status: draft`；`tags` 含目标职种（或 AI 用法的 `索引` + `agent`）以及 `需求`。用户不落盘：停，并告诉下一刀是 `/ddove-wiki-create`。

整份做完不是本 skill：人跑 `/ddove-wiki-change-state`，按职种生成用法篇，需求稿改为 `deprecated`。

完成：用户确认对齐；若落盘，则 create 的完成条件也满足。
