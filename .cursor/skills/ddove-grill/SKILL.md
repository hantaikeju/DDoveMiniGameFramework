---
name: ddove-grill
description: 拷问到决策对齐。对齐后同一条对话里落稿、改文件、写上正式和废弃，再给人看产品。只在用户调用 /ddove-grill 时用。
disable-model-invocation: true
---

# ddove-grill

权威：`DDoveMiniGameWiki/00-索引/OKF约定.md`。工作流：`DDoveMiniGameWiki/00-索引/Agent/需求稿工作流.md`。共享语言在 `DDoveMiniGameWiki/`，不写 `CONTEXT.md`。

## 步骤

1. **选篇**：本拍只碰一篇处理中的需求（`05-需求` 且 draft 且 tags 含 `需求`）。只有一篇直接做，不必点名。多于一篇且没点名就停，只问选哪一篇，其它篇不动。点名是路径、title 或文件名；点了名就只碰那一篇。没有处理中的需求：拷问这一条新需求。完成：已选定这一篇，或已停下只问选哪一篇，或已确定是新需求。
2. **检索**：按 `ddove-wiki` grep 已有篇。完成：已 grep。
3. **拷问**：按轮次问 **frontier**（前提已齐、尚未拍板的问题）。事实自查代码/wiki，决策等用户。气泡只写检索结论和「已齐、不再问」，**不要**铺 `❓ Q1` 或 A/B 列表。本轮全部无依赖 frontier 用 Cursor **`AskQuestion` 弹窗一次问完**：每题 ≥2 选项，推荐放第一项并标「推荐」；开放发挥走 Other。有依赖的题等用户提交后再开下一轮弹窗。完成：用户已提交本轮弹窗。
4. **对齐**：frontier 空则复述共享理解，等用户确认。完成：用户点头。
5. **接着做完**（用户已确认对齐才做）：同一条对话里接着做，不把 `/ddove-wiki-create`、`/ddove-fix-up`、`/ddove-wiki-change-state` 报成用户的下一步。
   - 读并执行 `.agents/skills/ddove-wiki-create/SKILL.md`，把对齐结果写成 `05-需求` 需求稿。
   - 按这篇改 skill 和 wiki。
   - 文件做完，读并执行 `.agents/skills/ddove-wiki-change-state/SKILL.md`：用法篇 `stable` 并写 `verified`，需求稿 `deprecated`。不等人先点头。
   - 给人看产品：用法篇、说明页、这次改了什么。改了客户端时写明行为是否符合对齐过的需求。
   完成：用法篇已正式，需求稿已废弃，产品已给到用户。
6. **产品不对**：用户用自己的话指出要改的地方时，读并执行 `.agents/skills/ddove-fix-up/SKILL.md`。完成：两篇回到处理中并改完后，再次写成正式 / 废弃，产品再次给到用户。

问答和排障不走步骤 5、6。

完成：用户已确认对齐；代理已落稿、改完文件、写上正式和废弃，并给出产品。
