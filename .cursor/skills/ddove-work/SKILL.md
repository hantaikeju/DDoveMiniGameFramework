---
name: ddove-work
description: >-
  问答、排障、改代码。先 ddove-wiki。改代码且命中 draft+需求 走开工班组。
  Triggers: 框架问题、bug、改客户端、按需求稿实现、排障。
  Does not edit DDoveMiniGameConfig tables (ddove-config).
  问答/排障不写 wiki。
---

# ddove-work

权威：`DDoveMiniGameWiki/00-索引/OKF约定.md`。开工：`DDoveMiniGameWiki/00-索引/Agent/开工班组.md`。检索：`.agents/skills/ddove-wiki/SKILL.md`（读并执行）。角色卡：同目录 `roles/`。

不扫 `DDoveMiniGameClient/Library`。无 `/ddove-crew`。无编排者。无 `CONTEXT.md`。`status` 只 `draft` / `stable` / `deprecated`。父会话不粘贴子代理 diff 全文。

## 步骤

1. **检索**：执行 `ddove-wiki` 全文步骤。完成：已有命中列表或「Wiki 未收录」。
2. **分支**（只走一条）：
   - **问答**：五段答案（`draft` / `deprecated` 必须点名）。完成：五段齐。不写篇。
   - **排障**：以**当前代码**为准。完成：原因 + 怎么验证。不写篇。
   - **改代码**：数 `draft`+`需求`。
     - **0 篇**：停。`/ddove-grill` 或 `/ddove-wiki-create`。完成：已指路、未改脚本。
     - **≥1 篇**：走「开工班组」。完成：开工完成条件满足。
3. **问答 / 排障** 里 wiki 过期：标明。要改对照篇 → 用户 `/ddove-wiki-update`。

## 开工班组

先读 `roles/plan.md`、`roles/dispatch.md`、`roles/worker.md`、`roles/review.md`、`roles/score.md`。`rework_used` 初始 0。

1. **拆图**：Task 拉 `.cursor/agents/ddove-plan`（只读）。完成：`STATUS: PLAN` 或 `NEED_GRILL`。
2. **批准**：父会话执行 `roles/plan.md`「批准与冻结规格」。`NEED_GRILL` 则停。完成：任务图已批；vN 已冻（或已 create v1）。
3. **派工**：执行 `roles/dispatch.md`。完成：每票 `SLICE_DONE` / `NEED_PATCH` / 失败。
4. **回写**：父会话执行 `roles/plan.md`「回写」。完成：入选需求稿已 update。
5. **审查**：Task 拉 `.cursor/agents/ddove-review`。完成：`STATUS: PASS|FAIL`。
6. **评分**：Task 拉 `.cursor/agents/ddove-score`。完成：`STATUS: SCORE`。
7. **规格**：父会话执行 `roles/plan.md`「升级规格」。完成：开工评分已为 vN+1。
8. **收口**：
   - 未过线且 `rework_used == 0`：`rework_used = 1`，回到步骤 1（补丁图 + 新 Task）。完成：已重做或无须重做。
   - 未过线且已重做：停，交得分表。不摘 tag。完成：用户看得见表。
   - 过线且 `PASS`：`roles/plan.md`「摘 tag」。建议 `/ddove-wiki-change-state`。完成：入选篇无 `需求` tag、`status` 仍 `draft`；回复含得分表。

规格中途不改。不自动升 `stable`。
