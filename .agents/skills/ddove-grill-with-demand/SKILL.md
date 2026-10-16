---
name: ddove-grill-with-demand
description: 只在用户调用 /ddove-grill-with-demand 时用，拷问已有处理中的需求稿。
disable-model-invocation: true
---

# ddove-grill-with-demand

权威：`DDoveMiniGameWiki/00-索引/OKF约定.md`。工作流：`DDoveMiniGameWiki/00-索引/Agent/需求稿工作流.md`。共享语言在 `DDoveMiniGameWiki/`，不写 `CONTEXT.md`。

拷问已有处理中的需求稿。问完按这一篇开工。不问短号。补充内容下次再调用本技能。

## 步骤

1. **选篇**：处理中 = `05-需求` 且 `draft` 且 `tags` 含 `需求`。只有一篇直接做。多于一篇且没点名就停，只问选哪一篇，其它篇不动。点名是路径、title 或文件名，点了名只碰那一篇。没有处理中的需求：停，指向 `/ddove-grill`，不新建稿。完成：已选定这一篇，或已停下只问选哪一篇，或已停下并指向 `/ddove-grill`。
2. **检索**：按 `ddove-wiki` grep 已有篇。完成：已 grep。
3. **拷问**：按轮次问 **frontier**（前提已齐、尚未拍板的问题）。事实自查代码/wiki，决策由用户拍板。气泡只写检索结论和「已齐、不再问」。无依赖的 frontier 用 AskQuestion 一次问完，等人提交：每题 ≥2 选项，推荐放第一项并标「推荐」；开放发挥走 Other。有依赖的题在用户提交后再开下一轮弹窗。对齐复述前，验收里要有证明方式（编译即可、必须进 Play、或只改 wiki / skill），「要什么」里要有变更种类（功能、缺陷、或行为保持不变的结构调整）。缺了就当作本轮 frontier。完成：用户已提交本轮弹窗。
4. **对齐**：frontier 空则复述共享理解，并在同一条对话里进入开工。完成：共享理解已复述，且本条对话已进入开工。
5. **接着做完**：同一条对话里按这一篇读并执行 `.agents/skills/ddove-work/SKILL.md`。不调用 `ddove-wiki-create` 另建第二份。对齐结果若要改正文，读并执行 `.agents/skills/ddove-wiki-update/SKILL.md`，只改这一篇。不把 create、work、fix-up、change-state、archive 报成用户的下一步。开工收口已经写上正式 / 废弃并给出产品时，本步到此为止。完成：已按这一篇开工；要改正文时只改了这一篇；开工收口已写上正式 / 废弃并给出产品。
6. **产品不对**：用户用自己的话指出要改的地方时，读并执行 `.agents/skills/ddove-fix-up/SKILL.md`。完成：两篇回到处理中并改完后，再次写成正式 / 废弃，产品再次给到用户。

问答和排障在复述共享理解后结束。

完成：共享理解已在本条对话复述；已按这一篇开工并展示。没有处理中的需求时已停并指向 `/ddove-grill`。
