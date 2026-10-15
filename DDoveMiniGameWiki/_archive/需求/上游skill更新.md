---
type: Playbook
title: 上游 skill 更新（需求稿）
description: 长期对照 mattpocock/skills 与 pstack。人看到更新后再 grill。不自动拉远程，不整包装进。
tags: [索引, agent, 需求]
status: deprecated
deprecated_at: 2026-10-15T07:50:00Z
sources:
  - id: matt
    resource: https://github.com/mattpocock/skills
    title: mattpocock/skills
  - id: pstack
    resource: https://github.com/cursor/plugins/tree/main/pstack
    title: pstack
  - id: page
    resource: /00-索引/Agent/上游skill更新.md
    title: 上游 skill 更新
---

# 上游 skill 更新（需求稿）

用法篇：[上游 skill 更新](/00-索引/Agent/上游skill更新.md)。

变更种类：功能。证明方式：只改 wiki。

[打开说明](上游skill更新.html)

## 要什么

两个远程长期对照，写在现有篇 `/00-索引/Agent/上游skill更新`：

- [mattpocock/skills](https://github.com/mattpocock/skills)
- [pstack](https://github.com/cursor/plugins/tree/main/pstack)

人看到其中一处有更新之后，再用 `/ddove-grill` 决定跟哪一条。不自动拉远程，不整包装进。luban 的现有规则不动。一次一个需求、篇内按任务图并行，仍以开工班组为准。

## 不要

- 自动 `git fetch` 或 `npx skills update` 进本仓
- 把上游 skill 整包拷进 `.agents/skills/`
- 写 `CONTEXT.md`
- 这次再引进 tdd、过夜合并、多模型擂台
- 改 luban 的合成规则
- 另开第二份用法篇

## 评分

本轮无客户端改动，未编译。

| 轮 | 记录 |
|----|------|
| 1 | 两个链接已写入用法篇。不自动拉远程，不整包装进。未另开第二份用法篇。 |
