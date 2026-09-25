# 计划

权威：`DDoveMiniGameWiki/00-索引/Agent/开工班组.md`。拆图子代理：`.cursor/agents/ddove-plan.md`。写 wiki 只在**父会话**：读并执行 `.agents/skills/ddove-wiki-update/SKILL.md` 或 `ddove-wiki-create/SKILL.md`。短号 / `_log` / `check_wiki.py` 只按 `DDoveMiniGameWiki/00-索引/OKF约定.md`。本会话无短号则问一次。

不改代码、不改表、不 `change-state`、不 `update` 对照篇。刀中不在 `01`–`04` 生成用法篇。不另开评分文件。`开工评分` 只当尺子，不要打 `需求` tag。拆图子代理不写文件。

## 拆图（仅 ddove-plan）

1. 入选全部 `draft`+`需求` 篇。多篇按 `generated.at`（没有则文件时间）加权，冲突听较新。合成**一张**任务图。完成：入选列表 + 冲突裁决在 STATUS 里。
2. 每篇打开「验收」。没有任何可执行验收句：交还 `STATUS: NEED_GRILL`。不补造验收。完成：NEED_GRILL 或每篇至少一条验收。
3. 拆票。每票：`id`、`goal`、`skill`（脚本工人 / `ddove-config`）、`paths`、`depends`、`done_when`。重做圈只交补丁图（仍有效的范围 + 变更的票 + 已裁决卡点）。完成：图无孤儿依赖。
4. 交还：

```text
STATUS: PLAN | NEED_GRILL
入选: ...
冲突: 听较新篇 ...
票:
  - id: ...
验收原文: ...
```

## 批准与冻结规格（父会话）

1. `NEED_GRILL`：停，下一刀 `/ddove-grill`。完成：已停。
2. `PLAN`：记下任务图。已有 `DDoveMiniGameWiki/00-索引/Agent/开工评分.md` 则读出 `规格版本: vN` 并冻结。没有则 create：`type: Playbook`；`tags: [索引, agent]`（不要 `需求`）；标题 `开工评分`；正文用 `roles/score.md` 的 v1 种子。完成：冻结版本号已记下。

## 回写（父会话）

对入选需求稿逐篇 wiki-update：按**当前代码**改正文 / `description` / `sources`；点名过期对照篇。依据工人 STATUS 的改动路径，不把 diff 全文写进父回复。完成：update 完成条件满足。

## 记分（父会话）

把 `STATUS: SCORE` 的得分表追加到入选需求稿的 `## 评分`（没有这一节就补上）。同一篇接着写，不新建评分 md。当日 `_log`。`check_wiki.py`。完成：该需求稿里有本轮总分。

尺子仍读 `/00-索引/Agent/开工评分` 的 `规格版本: vN`。只有 SCORE 里的 vN+1 改了尺子条文，才 update 开工评分。完成：条文没变则开工评分不动。

## 整份完成

不在过线当轮做。人认定整份需求做完后跑 `/ddove-wiki-change-state`：按职种生成用法篇，需求稿改为 `deprecated`。
