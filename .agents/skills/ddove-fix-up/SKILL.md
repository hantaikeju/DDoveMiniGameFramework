---
name: ddove-fix-up
description: 产品不对时，把用法篇和需求稿回到处理中，改完再写成正式和废弃。用户用自己的话指出要改的地方时执行，不要求用户敲这条命令。
disable-model-invocation: true
---

# ddove-fix-up

权威：`DDoveMiniGameWiki/00-索引/OKF约定.md`。工作流：`DDoveMiniGameWiki/00-索引/Agent/需求稿工作流.md`。

## 步骤

1. **定这一对**：用法篇在职种目录。需求稿优先在 `05-需求`，否则在 `_archive/需求/`，同一文件名。两处都没有：停，告诉人这篇已经删除，要改就重新 `/ddove-grill`。多于一对且没点名就停，只问是哪一份产品。完成：已定这一对，或已停下。
2. **回到处理中**：用法篇 `status: draft`，删掉 `verified`。需求稿若在归档夹，连同名 `.html` 搬回 `05-需求/`，删掉 `deprecated_at`。需求稿 `status: draft`，`tags` 仍含 `需求`。完成：两篇都是处理中可改的稿；需求稿再次满足 `05-需求` 且 draft 且 tags 含 `需求`。
3. **改**：按用户刚才的话改这一对和对应 skill。完成：指出的地方已改。
4. **再写上**：读并执行 `.agents/skills/ddove-wiki-change-state/SKILL.md`。用法篇 `stable`，需求稿 `deprecated` 并搬入归档夹。完成：两篇状态已写回。
5. **再给产品**：用法篇、说明页、这次改了什么。改了客户端时写明行为是否符合对齐过的需求。完成：产品已再次给到用户。不把下一指令报给用户。

完成：这一对曾回到处理中，改完后用法篇为正式、需求稿为废弃，产品已再次给出。
