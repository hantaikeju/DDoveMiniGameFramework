---
name: ddove-wiki
description: >-
  只检索本库 DDoveMiniGameWiki/：grep frontmatter、读命中篇、列出要点。
  Triggers: 只要查 wiki、其它 skill 需要检索。
  Does not answer product questions, debug, or edit code.
---

# ddove-wiki（只检索）

约定：`DDoveMiniGameWiki/00-索引/OKF约定.md`。本 skill 不写文件、不改代码、不给五段业务答案。干活走 `ddove-work`。

## 步骤

1. **拆词**：2–6 个中英文关键词（症状、类名、协议、表名）。
2. **Grep**（排除 `_log` / `_tools` / `_spec`）：

```bash
rg -n -g "*.md" -g "!_log/**" -g "!_tools/**" -g "!_spec/**" "^type: |^title: |^tags: |关键词1|关键词2" DDoveMiniGameWiki
```

工程用法收窄 `DDoveMiniGameWiki/01`–`04`；需求稿收窄 `DDoveMiniGameWiki/05-需求`；AI / skill 收窄 `DDoveMiniGameWiki/00-索引/Agent`。只读 `OKF约定.md`，不走 `index.md` → `index_<短号>`。
3. **读命中篇**：`type` / `status` / `sources` / `generated.by` / `verified`。Stub 跟 `sources` 与文内链接。
4. **交出检索结果**：每条 Concept ID + type + status + 一句要点；无命中写「Wiki 未收录」。

完成：已执行排除 `_log` 的 grep，且结果列表齐。
