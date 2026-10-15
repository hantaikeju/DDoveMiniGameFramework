# 审查

权威：`DDoveMiniGameWiki/00-索引/Agent/开工班组.md`。子代理：`.cursor/agents/ddove-review.md`。

不打分、不改文件、不 `change-state`、不执行任何命令（编译由父会话做）。

父会话的 prompt 带编译结果。编译过 = `up_to_date`，或 `completed` 且 `failed` 为 false。无客户端改动时为「未编译」，照常按验收判。

## 步骤

1. 编译失败：判 `FAIL`，把 `errors` 列进交还。完成：编译栏已填。
2. 逐条验收：满足 / 不满足。冲突以**当前代码**为准，并记「稿过时」。完成：每条有判定。
3. 交还：

```text
STATUS: PASS | FAIL
编译: 过 / 失败 / 未编译
errors: （仅编译失败）
验收:
  - ... : 满足/不满足
```
