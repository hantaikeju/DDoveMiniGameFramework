---
name: ddove-grill
description: 拷问到决策对齐。对齐后同一条对话里落稿、开工、展示。只在用户调用 /ddove-grill 时用。
disable-model-invocation: true
---

# ddove-grill

权威：`DDoveMiniGameWiki/00-索引/OKF约定.md`。工作流：`DDoveMiniGameWiki/00-索引/Agent/需求稿工作流.md`。共享语言在 `DDoveMiniGameWiki/`，不写 `CONTEXT.md`。

人的必经步骤是三步：本技能对齐，`ddove-work` 开工，收口展示。不问短号。补充内容下次再调用本技能。

## 步骤

1. **选篇**：本拍只碰一篇处理中的需求（`05-需求` 且 draft 且 tags 含 `需求`）。只有一篇直接做，不必点名。多于一篇且没点名就停，只问选哪一篇，其它篇不动。点名是路径、title 或文件名；点了名就只碰那一篇。没有处理中的需求：拷问这一条新需求。完成：已选定这一篇，或已停下只问选哪一篇，或已确定是新需求。
2. **检索**：按 `ddove-wiki` grep 已有篇。完成：已 grep。
3. **拷问**：按轮次问 **frontier**（前提已齐、尚未拍板的问题）。事实自查代码/wiki，决策等用户。气泡只写检索结论和「已齐、不再问」，**不要**铺 `❓ Q1` 或 A/B 列表。本轮全部无依赖 frontier 用 Cursor **`AskQuestion` 弹窗一次问完**：每题 ≥2 选项，推荐放第一项并标「推荐」；开放发挥走 Other。有依赖的题等用户提交后再开下一轮弹窗。对齐复述前，验收里要有证明方式（编译即可、必须进 Play、或只改 wiki / skill），「要什么」里要有变更种类（功能、缺陷、或行为保持不变的结构调整）。缺了就当作本轮 frontier。完成：用户已提交本轮弹窗。
4. **对齐**：frontier 空则复述共享理解，等用户确认。完成：用户点头。
5. **接着做完**（用户已确认对齐才做）：同一条对话里接着做，不把 `/ddove-wiki-create`、`/ddove-work`、`/ddove-fix-up`、`/ddove-wiki-change-state`、`/ddove-archive` 报成用户的下一步。
   - 读并执行 `.agents/skills/ddove-wiki-create/SKILL.md`，把对齐结果写成 `05-需求` 需求稿。
   - 读并执行 `.agents/skills/ddove-work/SKILL.md`，按这篇开工。
   - 开工收口已经写上正式 / 废弃并给出产品时，本步到此为止。
   完成：用法篇已正式，需求稿已进归档夹，产品已给到用户。
6. **产品不对**：用户用自己的话指出要改的地方时，读并执行 `.agents/skills/ddove-fix-up/SKILL.md`。完成：两篇回到处理中并改完后，再次写成正式 / 废弃，产品再次给到用户。

问答和排障不走步骤 5、6。

完成：用户已确认对齐；代理已落稿、开工、展示。
