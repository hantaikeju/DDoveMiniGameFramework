---
name: ddove-work
description: >-
  问答、排障、改代码。
  Triggers: 框架问题、bug、改客户端、按需求稿实现、排障。
---

# ddove-work

权威：`DDoveMiniGameWiki/00-索引/OKF约定.md`。开工：`DDoveMiniGameWiki/00-索引/Agent/开工班组.md`。检索：`.agents/skills/ddove-wiki/SKILL.md`（读并执行）。角色卡：同目录 `roles/`。配表走 `ddove-config`。

不扫 `DDoveMiniGameClient/Library`。无 `/ddove-crew`。无编排者。无 `CONTEXT.md`。`status` 只 `draft` / `stable` / `deprecated`。父会话不粘贴子代理 diff 全文。

## 步骤

1. **检索**：执行 `ddove-wiki` 全文步骤。完成：已有命中列表或「Wiki 未收录」。
2. **分支**（只走一条）：
   - **问答**：五段答案（`draft` / `deprecated` 必须点名）。完成：五段齐；未写篇；未进开工班组；未问选哪一篇需求稿。
   - **排障**：以**当前代码**为准。完成：原因 + 怎么验证；未写篇；未进开工班组；未问选哪一篇需求稿。
   - **改代码**：数处理中的篇。处理中 = `05-需求` 且 status `draft` 且 tags 含 `需求`。
     - **0 篇**：停。指 `/ddove-grill`。完成：已指路、未改脚本。
     - **1 篇**：直接做这一篇，再进「开工班组」。完成：已进开工班组做这一篇；未再问点名。
     - **多于一篇，且这一拍点了一篇**（路径、title 或文件名）：只做该篇，再进「开工班组」。完成：已进开工班组；任务图、回写、记分只落在这一篇；其它篇未改。
     - **多于一篇，且这一拍没点名**：停，只问选哪一篇。完成：已只问选哪一篇；未进开工班组；其它稿未改。
3. **问答 / 排障** 里 wiki 过期：标明。要改对照篇 → 用户 `/ddove-wiki-update`。完成：已标明过期；改正文留给用户 `/ddove-wiki-update`。

## 开工班组

先读 `roles/plan.md`、`roles/dispatch.md`、`roles/worker.md`、`roles/review.md`、`roles/score.md`。`rework_used` 初始 0。完成：五张角色卡已读；`rework_used` 为 0。

入选稿 `## 评分` 最后一行是「未验证」：跳过拆图、批准、派工、回写，直接编译 → 审查 → 评分 → 记分 → 收口。编译用该行列出的改动路径。「未验证」行不算一轮，`rework_used` 从 0 开始。完成：已直接进步骤 4；`rework_used` 为 0。

1. **拆图**：Task 拉 `.cursor/agents/ddove-plan`（只读）。完成：`STATUS: PLAN` 或 `NEED_GRILL`；任务图只落在这一篇。
2. **批准**：父会话执行 `roles/plan.md`「批准与冻结规格」。`NEED_GRILL` 则停。完成：任务图已批；vN 已冻（或已 create v1）。
3. **派工**：执行 `roles/dispatch.md`。完成：每票 `SLICE_DONE` / `NEED_PATCH` / 失败。
4. **编译**：每批工人回收后，本批 `改动路径` 含 `DDoveMiniGameClient/` 下的文件（含导表生成的代码），父会话串行跑 `ddove-unity-pipeline` 步骤 3：`set_autotick --enable true` → `recompile` → 轮询 `recompile_status`。编译过 = `status` 为 `up_to_date`，或 `completed` 且 `failed` 为 false。不含客户端改动：跳过，回复与评分行写明「本轮无客户端改动，未编译」。工人不跑 recompile。完成：已拿到编译结果（失败时含 `errors`），或已写明「本轮无客户端改动，未编译」。
   - 编译前 `editor_status` 无应答：停。本轮不过，不弹二选一。在入选需求稿 `## 评分` 记一行「未验证」，列出本轮改动路径。回复提示人开编辑器后重跑 `/ddove-work`。完成：「未验证」行已记在这一篇；未审查、未评分；未弹二选一。
5. **回写**：父会话执行 `roles/plan.md`「回写」。完成：回写只落在这一篇。
6. **审查**：Task 拉 `.cursor/agents/ddove-review`。prompt 写进编译结果（失败时含 `errors`）或「本轮无客户端改动，未编译」。完成：`STATUS: PASS|FAIL`。
7. **评分**：Task 拉 `.cursor/agents/ddove-score`。完成：`STATUS: SCORE`。
8. **记分**：父会话执行 `roles/plan.md`「记分」。完成：记分只落在这一篇；没有新的评分文件。
9. **收口**：过线 = 总分 ≥ 70 且审查 `PASS`。审查 `FAIL` 一律不过线，照常记分。只走一条：
   - 未过线且 `rework_used == 0`：`rework_used = 1`，回到步骤 1（补丁图带上编译 `errors` + 新 Task）。完成：已回到拆图。
   - 未过线且已重做：停，交得分表。需求稿仍带 `需求`。完成：用户看得见表，分数在这一篇上。
   - 过线：得分表留在回复里。完成：回复含得分表。
   按 `draft`+`需求` 开工结束，且落在「未过线且已重做」或「过线」时，接着读并执行 `.agents/skills/ddove-wiki-change-state/SKILL.md`：用法篇 `stable` 并写 `verified`，需求稿 `deprecated`。然后给人看产品：用法篇、说明页、这次改了什么。改了客户端时写明行为是否符合对齐过的需求。不弹二选一，不把 `/ddove-fix-up` 或 `/ddove-wiki-change-state` 报成用户的下一步。问答和排障不走这段。完成：用法篇已正式，需求稿已废弃，产品已给到用户，回复仍含得分表。

规格中途不改。不自动升 `stable`。完成：冻结的 vN 条文未改；未自动升 `stable`。
