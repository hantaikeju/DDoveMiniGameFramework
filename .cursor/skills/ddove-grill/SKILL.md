---
name: ddove-grill
description: 拷问这句话里的新目标。对齐后落稿并停下，开工等人另发 /ddove-work。只在用户调用 /ddove-grill 时用。
disable-model-invocation: true
---

# ddove-grill

权威：`DDoveMiniGameWiki/00-索引/OKF约定.md`。工作流：`DDoveMiniGameWiki/00-索引/Agent/需求稿工作流.md`。共享语言在 `DDoveMiniGameWiki/`，不写 `CONTEXT.md`。

人的必经步骤是三步：本技能对齐，落稿后停下，开工是人另发 `/ddove-work`，收口展示。不问短号。补充内容下次再调用本技能。补充的是新目标，仍调用 `/ddove-grill`。

## 步骤

1. **认这条新需求**：这句话写了目标就拷问这句；句中已有目标不用前文替换。只有命令时，接上发出这条命令之前用户已经说清的最近一条，再进入拷问；不接助手「下一步」，不一次接上所有未做目标。前文也没有已经说清的目标，才停下问要问什么。不数稿，不改已有稿。完成：已按这句进入拷问，或已接上前文用户已经说清的最近一条并进入拷问，或已停下问要问什么。
2. **检索**：按 `ddove-wiki` grep 已有篇。完成：已 grep。
3. **拷问**：按轮次问 **frontier**（前提已齐、尚未拍板的问题）。事实自查代码/wiki，决策由用户拍板。气泡只写检索结论和「已齐、不再问」。无依赖的 frontier 用 AskQuestion 一次问完，等人提交：每题 ≥2 选项，推荐放第一项并标「推荐」；开放发挥走 Other。有依赖的题在用户提交后再开下一轮弹窗。对齐复述前，验收里要有证明方式（编译即可、必须进 Play、或只改 wiki / skill），「要什么」里要有变更种类（功能、缺陷、或行为保持不变的结构调整）。缺了就当作本轮 frontier。完成：用户已提交本轮弹窗。
4. **对齐**：frontier 空则复述共享理解，并在同一条对话里落稿（执行 `ddove-wiki-create`）。读并执行 `.agents/skills/ddove-wiki-create/SKILL.md`，把对齐结果写成 `05-需求` 需求稿。完成：共享理解已复述，且本条对话已执行 `ddove-wiki-create` 落成需求稿。
5. **落稿后停下**：落稿之后停下，不执行 `ddove-work`。完成：已停下，不执行 `ddove-work`；下一刀是人另发 `/ddove-work`。
6. **产品不对**：用户用自己的话指出要改的地方时，读并执行 `.agents/skills/ddove-fix-up/SKILL.md`。完成：两篇回到处理中并改完后，再次写成正式 / 废弃，产品再次给到用户。

问答和排障在复述共享理解后结束。

完成：共享理解已在本条对话复述；落稿后停下，开工是人另发 `/ddove-work`。
