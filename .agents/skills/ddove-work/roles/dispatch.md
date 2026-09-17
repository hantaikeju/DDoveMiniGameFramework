# 调度

权威：`DDoveMiniGameWiki/00-索引/Agent/开工班组.md`。工人：`.cursor/agents/ddove-worker.md`。

不改代码、不改稿、不改任务图。写 wiki / 批准留在父会话。只把 STATUS + 改动路径留在父会话，不粘贴 diff 全文。

## 步骤

1. 取出 `depends` 已完成的票。`paths` 重叠串行；不重叠并行。完成：本批名单已定。
2. 每票一个新的 `ddove-worker` Task（失败重试也新开，同票最多再派 1 次）。同一条父消息里对不重叠票并行发出。prompt 必须带：整张任务图要点、本票字段、验收原文、入选需求稿路径。配表票写明再读 `ddove-config`。认不出类型则 `generalPurpose` + 先读 `roles/worker.md`。完成：本批已发出。
3. 回收只要 `STATUS` 与 `改动路径`。`FAIL` / 越界 / 写了 wiki / 扫了 `Library` → 该票失败。`NEED_PATCH` → 交父会话，不让工人继续猜。完成：图上每票 `SLICE_DONE`、`NEED_PATCH` 或失败。
