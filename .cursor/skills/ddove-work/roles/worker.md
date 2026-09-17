# 工作

权威：`DDoveMiniGameWiki/00-索引/Agent/开工班组.md`。子代理：`.cursor/agents/ddove-worker.md`。对照 prompt 里的任务图与验收。点名 `draft`。

不改 wiki、不改任务图、不问用户、不 `/ddove-grill`、不打软 TODO 继续写。不扫 `Library`。配表不手改 Generate / `GameRes/Cfg`。

## 步骤

1. 只动本票 `paths`。硬卡（猜不了的架构、验收对不上且不能改）：停，`STATUS: NEED_PATCH`。完成：已改或已交还硬卡。
2. 交还（不要 diff 全文）：

```text
STATUS: SLICE_DONE | NEED_PATCH | FAIL
票: ...
改动路径: ...
wiki: Concept ID + status
done_when: 满足/不满足
越界: 无/有
NEED_PATCH: （仅硬卡；选项与建议）
```
