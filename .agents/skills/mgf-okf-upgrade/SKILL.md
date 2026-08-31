---
name: mgf-okf-upgrade
description: >-
  Check and upgrade the wiki/ OKF bundle against the current repo-root SPEC.md.
  Use only when the user replaced SPEC.md, asks to audit OKF compliance, migrate
  frontmatter, regenerate index.md, or bump okf_version. Do not run during
  ordinary Q&A or coding.
disable-model-invocation: true
---

# Wiki 检查与升级（跟 SPEC.md 走）

权威规范 = 仓库根 `SPEC.md`（用户整文件替换）。知识包 = `wiki/`。本库剖面 = `wiki/00-索引/OKF约定.md`。

**先报告、再改。** 未得到用户确认前，只输出清单，不改文件。

## 步骤

### 1. 读当前规范

打开仓库根 `SPEC.md`，抽出：

- 版本（标题 `Version x.y` 或 Versioning 节）
- 合规条款（Conformance：frontmatter、`type`、保留名 `index.md` / `log.md`）
- Breaking / Additive 变更（若有 Changes 节）
- `index.md` / `log.md` 的结构要求

对照 `wiki/index.md` 的 `okf_version`。版本不一致 = 需要升级。

### 2. 扫描 bundle（跳过非概念）

跳过：`wiki/_tools/`、`wiki/_spec/`、`wiki/_log/`、`wiki/.obsidian/`、仓库根 `SPEC.md`、`DDoveMiniGameClient/`。

对 `wiki/` 下每个 `.md`：

| 文件 | 检查 |
|------|------|
| `wiki/index.md` | 仅允许 `okf_version`；只挂分类目录 + 保留名，**不列概念** |
| `wiki/**/index.md`（非根） | **无** frontmatter；业务目录只挂子目录 + `index_<短号>.md` |
| `wiki/**/index_*.md` | 个人清单：可解析 YAML + `type: Index` |
| `wiki/**/log.md` | 最新日期在上；本库根 `log.md` 只作日期索引，明细在 `_log/log_YYYY-MM-DD.md` |
| 其它 `*.md` | 可解析 YAML + 非空 `type` |

记录：缺 frontmatter、缺 `type`、分类 index 误带 frontmatter、把 `index.md`/`log.md` 当概念、约定文中复述已废弃字段（如旧 `timestamp`、正文 `# Citations`）。

### 3. 输出报告（必须先给用户）

```
SPEC 版本: <从 SPEC.md 读取>
wiki okf_version: <当前>
结论: 已对齐 / 需升级 / 仅有合规缺口

违规:
- path — 问题

建议迁移:
- 字段 / 约定句子 — 依据 SPEC 哪一节

将改动的文件:
- ...
```

问用户：只修合规，还是连 `okf_version` + 写当日 `_log` 一起做。不要把业务概念摊进根或职种 `index.md`（篇目在 `index_<短号>.md`）。

### 4. 确认后才改

1. 给缺 `type` 的概念补最小 frontmatter（不要重写正文）。
2. 按 SPEC Breaking 做字段迁移；Additive 字段保持可选，不强行填满。
3. 更新 `OKF约定.md` 里与新 SPEC 冲突的句子；`resource` / `sources` 继续指向 `../../SPEC.md`。
4. 将 `wiki/index.md` 的 `okf_version` 改为 SPEC 当前版本。
5. 根 `index.md` 只改 `okf_version`（及新增职种目录）。不要把业务篇摊进职种 `index.md`（篇目在 `index_<短号>.md`）。
6. 在 `wiki/_log/log_YYYY-MM-DD.md` **顶部**追加当天条目（无则新建）；根 `wiki/log.md` 若还没有该日 `##` 节则加在最上面。写：SPEC x → y，改了哪些文件。
7. 本次升级动到的约定页：`generated.by` 可用 `process:mgf-okf-upgrade`。**不要**把业务篇的 `human:<短号>` 改成 process。扩写领域文档不在本 skill。

不要：把 `SPEC.md` 拷进 `wiki/`；不要改 `DDoveMiniGameClient/`；不要在这次任务里扩写领域文档。

## 完成后

用 3–6 行说明：对齐到哪一版、修了几处、用户还需要人工 `verified` 的概念（若有）。
