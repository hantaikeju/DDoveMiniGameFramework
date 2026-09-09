---
name: ddove-work
description: >-
  问答、排障、改代码。先按 ddove-wiki 检索，再用结论回答或改代码。
  Triggers: 框架问题、bug、改客户端、实现功能、排障。
  Does not create wiki pages or upgrade SPEC.
---

# ddove-work

权威：`wiki/00-索引/OKF约定.md`。检索原语：`.agents/skills/ddove-wiki/SKILL.md`（读并执行，不要只口头说「先 wiki」）。

不扫 `DDoveMiniGameClient/Library`。不建篇、不改 `status`（那些是 create / change-state）。

## 步骤

1. **检索**：执行 `ddove-wiki` 全文步骤。完成：已有命中列表或「Wiki 未收录」。
2. **分支**（按用户意图只走一条）：
   - **问答**：用检索结果组织思考；要行号再开对得上的脚本。答五段：结论（`draft` / `deprecated` 必须点名）→ Wiki 依据 → 信任 → 代码核对（或「未再翻代码」）→ 下一步。完成：五段齐。
   - **排障**：wiki 与代码冲突以**当前代码**为准。先收紧到一条能复现的现象，再改。完成：原因 + 你怎么验证。
   - **改代码**：只开相关脚本；对照 wiki 或 `draft`+`需求` 篇。方案没对齐先停，让用户 `/ddove-grill`。完成：改完且对照过检索结果。
3. Wiki 过期或未收录：标明，并以代码为准。
