---
type: Playbook
title: OKF 约定与检索
description: 本 bundle 按仓库根 SPEC.md 约束。AI 检索：grep frontmatter（排除 _log）。index 只服务写入。流程篇不绑仓外工程。
tags: [索引, 检索, okf]
status: stable
generated: { by: process:mgf-okf-init, at: 2026-08-28T07:41:00Z }
verified: { by: human:cjh, at: 2026-09-17T07:33:00Z }
resource: ../../SPEC.md
sources:
  - id: okf-spec
    resource: ../../SPEC.md
    title: Open Knowledge Format
  - id: ioc-playbook
    resource: /02-程序-前/IOC容器使用规范.md
    title: IOC 容器使用规范
  - id: tl2-ref
    resource: /02-程序-前/TL2场景流式对照.md
    title: TL2 场景流式对照
  - id: opening
    resource: /00-索引/Agent/开篇与清单徽章.md
    title: 开篇与清单徽章
---

# OKF 约定与检索

本 bundle 遵循仓库根 [SPEC.md](../../SPEC.md)。[^okf-spec] 约定与 SPEC 冲突时以 SPEC 为准，再改本页。

## 结构

| 路径 | 角色 |
|------|------|
| `/index.md` | 只挂分类目录 + 保留名（可含 `okf_version`）。**不列概念** |
| `职种目录/index.md` | 挂**子目录** + 个人清单 `index_<短号>.md`。无 frontmatter |
| `职种目录/index_<短号>.md` | 该人整理的概念清单。`type: Index` |
| `/log.md` | 变更史日期索引，最新日期在上（SPEC 保留名） |
| `/_log/log_YYYY-MM-DD_<短号>.md` | 该人当日明细；非概念。不要写共享的 `log_YYYY-MM-DD.md` |
| 其它 `*.md` | **概念**：必须有 YAML frontmatter，且含非空 `type` |
| `_tools/` `_spec/` `_log/` `.obsidian/` | 非概念，默认不检索 |
| 仓库根 `SPEC.md` | 规范原文，不是概念，不进 bundle 扫描。**不要拷进 `DDoveMiniGameWiki/`** |
| 仓库根 `AGENTS.md` | 工具入口（短指针）。**不要挪进 `DDoveMiniGameWiki/`** |

Concept ID = 相对 `DDoveMiniGameWiki/` 的路径，去掉 `.md`。例如 `/00-索引/OKF约定`。

## 本库 `type`

| type | 用途 |
|------|------|
| `Playbook` | 排查、接入、修复、指南、升级步骤 |
| `Reference` | 架构、接口、总览、地图、协议 |
| `Report` | 自测 / QA / 测试清单 |
| `Skill` | 领域 Agent 流程（正文在 wiki；跨编辑器入口在 `.agents/skills/`） |
| `Stub` | 跳转 stub；正文不是权威，跟 `sources` / 文内链接 |
| `Table` | 超大配表 stub |
| `Index` | 个人清单 `index_<短号>.md`（人读）。职种入口仍是同目录 `index.md` |

未知 `type` 当普通概念，不拒绝。

## 两层 wiki

同一 bundle，两套主题，不要混写进一篇。

| 层 | 路径 | 写什么 | `tags` |
|----|------|--------|--------|
| **工程** | `01`–`04` | 本游戏 / 框架：运行时、协议、配表、美术 | 职种：`策划` / `程序-前` / `程序-后` / `美术` |
| **AI 用法** | `00-索引/Agent/` | 仓库 `ddove-*`、grill / 需求稿流程、个人 Cursor skill（如 show-me） | `索引` + `agent` |
| **约定 / 合规** | `00-索引` 根（不含 `Agent/`） | OKF、检索、升级、整理人 | `索引` + `okf` |

问答：工程收窄 `DDoveMiniGameWiki/02-程序-前` 等；skill / grill / Agent 收窄 `DDoveMiniGameWiki/00-索引/Agent`。约定只读本页。地图：[Agent 总览](Agent/Agent总览.md)。

## 分类：按职种，四个业务目录

`00-索引` 只给约定 / AI 用法。**工程文档**只进下面四个目录，谁主写就放谁那里。

| 目录 | 放什么 |
|------|--------|
| `01-策划` | 规则、配表、数值、验收 |
| `02-程序-前` | 客户端：运行时、UI、SDK、出包 |
| `03-程序-后` | 服务端：协议、校验、存档 |
| `04-美术` | 命名、尺寸、导入、图集 |

前后都要写的，各放一篇在自己目录，互相链过去，不要在一边复制另一边的正文。

## 信任与生命周期

从 frontmatter **推导**，不另开一套「正式位」。文档阶段用 SPEC 的 `status`（每篇 YAML 里已有）：

| 中文 | `status` | 含义 |
|------|----------|------|
| 起草 | `draft` | 未审完，可引用但须标明 draft，不当发布依据 |
| 正式 | `stable` | 现行约定 = 人审过**当前正文**。缺省（无 `status` 键）也按正式 |
| 废弃 | `deprecated` | 只留历史和旧链接，检索命中时先找替代篇 |

工作流只记这三态。`verified` 是 SPEC 出处字段，升 `stable` 时由 `/ddove-wiki-change-state` **同步写入**，不另当一道「再确认」闸门，不另造第四种 `status`。`now >= stale_after` → 过期，先核对源码。

生产上「能当依据」= `status: stable`。只有 `generated.by: human:cjh` 仍是起草人口径。

结论与代码冲突时以**当前代码**为准。改阶段：`/ddove-wiki-change-state` 把 `draft` 改成 `stable` 并写 `verified`；改正文走 `/ddove-wiki-update`（回 `draft`，摘掉 `verified`）；作废则改 `deprecated`，不要删文件抢链。

## 检索（AI 优先；必须）

消费者主要是 Agent，**不要**为了找篇去走 `/index.md` → 职种 `index.md` → `index_<短号>.md`。那条链只给人点、给写入挂归属。

1. **拆词**：症状 / 类名 / 协议 / 表名，中英一起。
2. **Grep** 限定 `DDoveMiniGameWiki/`，优先 frontmatter，**排除** `_log/` `_tools/` `_spec/`：

```bash
rg -n -g "*.md" -g "!_log/**" -g "!_tools/**" -g "!_spec/**" "^type: |^title: |^tags: |^status: |关键词" DDoveMiniGameWiki
rg -n -g "!_log/**" "关键词" DDoveMiniGameWiki/02-程序-前
rg -n -g "!_log/**" "关键词" DDoveMiniGameWiki/00-索引/Agent
```

工程收窄 `01`–`04`；AI 用法收窄 `DDoveMiniGameWiki/00-索引/Agent`。

3. **读正文**：`type: Stub` 或 `wiki_stub: true` → 跟 `sources[].resource` 与文内权威链接，不要把 stub 当全文。
4. **链接**：优先 bundle 根路径 `/00-索引/OKF约定.md`。约定只读本页；`检索指南` / `来源说明` 是摘抄，问答不必先开。
5. **禁区**：`DDoveMiniGameClient/Library`、`PackageCache`、`_tools/`、`_spec/`、`_log/`、仓库根 `SPEC.md`（规范，不当概念检索）。

文件名贴近 `title`（如 `CoreFsm.md` ↔ Core Fsm），方便路径和标题一起命中。

## 答案怎么写

1. 结论（1–3 句）
2. Wiki 依据：Concept ID + `type` + **`status`（起草/正式/废弃）** + 要点
3. 信任：`status`（起草/正式/废弃）；`now >= stale_after` 则过期
4. 代码核对（若有）
5. 下一步

`status: draft` 或 `deprecated` 必须在结论里点明，不要写成既定事实。

## 生成人（写篇前问一次）

业务概念、整理旧篇、改 `index_<短号>.md` 时，`generated.by` 必须是 **`human:<短号>`**，不能只写 `process:agent`。否则个人清单对不上，后面生成会挂错人。

**用户写篇只需给生成人短号。** 职种、标题、`type`、正文由 Agent 补齐。本会话还没有短号：先问一次，后面沿用。不要猜短号，不要问卷式追问。

短号必须能在 [整理人](/00-索引/整理人.md) 对上。表里没有：先问职种，补一行，并在对应职种 `index.md` 挂上 `index_<短号>.md`（没有就建空清单）。

升 `stable` 时由 change-state 写 `verified.by: human:<短号>`，不要在 create / update 里写。`process:` 只留给合规升级（现行 `ddove-okf-upgrade`）。历史页上的 `process:mgf-okf-init` 不要改。

## 新增 / 改写概念

写之前先满足上一节「生成人」。模板里的 `cjh` 换成问到的短号。

```yaml
---
type: Playbook
title: 短标题
description: 一句摘要
tags: [程序-前]
status: draft
generated: { by: human:cjh, at: 2026-08-31T00:00:00Z }
# verified: 升 stable 时由 change-state 写
sources:
  - id: src
    resource: ../DDoveMiniGameClient/Assets/某脚本.cs
    title: 原文
---
```

不要用 `index.md` / `log.md` 当概念文件名。文件名贴近 `title`，不要「文件叫 Core工具、title 叫 Core Fsm」。

**生产走主动 skill，不要在问答里顺手建篇。** 本库 skill 一律 `ddove-`。不知道用哪个：`/ddove-help`。只检索：`ddove-wiki`。问答 / 排障 / 改代码：`ddove-work`。改 / 加配表：`ddove-config`。需求拷问：`/ddove-grill`。新建：`/ddove-wiki-create`。改正文：`/ddove-wiki-update`。改起草/正式/废弃：`/ddove-wiki-change-state`。写/改 Agent：`ddove-writing-for-agents`。需求稿生命周期见 [需求稿工作流](Agent/需求稿工作流.md)。工程篇进 `01`–`04`；AI 用法进 `00-索引/Agent/`。

加一篇（`ddove-wiki-create` 已按此做）：先对该目录 grep `title` / 文件名，避免重复篇。再新增该 `.md`（`generated.by: human:<短号>`），并在**该短号**的 `index_<短号>.md` 补一条。不要改根 `/index.md`。职种 `index.md` 只在新人第一次出现时加一行。当日 `_log` 照常写。`00-索引` 仍可把约定篇直接列在该目录 `index.md`（改的人少）。

改**别人已有篇**：走 `/ddove-wiki-update`，只改正文；**不要**改 `generated.by`；**不要**再挂到自己的 `index_<短号>.md`（移交另说）。当日 log 写自己的 `_log/log_YYYY-MM-DD_<短号>.md`，不要写共享日文件。

`SPEC.md` / `AGENTS.md` 留在仓库根。wiki 根 `index.md`「保留」只外链，不拷贝正文。

替换仓库根 `SPEC.md` 后，主动调用 skill `ddove-okf-upgrade` 做检查与升级。不要在日常问答里改合规字段。

写 `01`–`04` 工程篇还要满足下一节。

## 流程篇与外库对照

写 `01`–`04` 的 Playbook / 工程 Reference 时先问：把本仓拷到一台没有那份工程的机器上，这句话还能独立执行吗？不要维护外库名单。

| 能（留下） | 不能（抽走） |
|------------|--------------|
| 本 bundle 互链（「对照：[DDoveRes]…」） | 另一仓的项目名、外库类名、外库 wiki 路径、本机盘符 |
| `sources` / 正文链到本仓 `DDoveMiniGameClient/`、`DDoveMiniGameConfig/` | |
| 已嵌入本仓的 `Packages/` | |
| 公开上游 URL（Unity 手册、GitHub） | |

不能独立执行的句子：改成**本库步骤**写进流程篇。还要留「为什么 / 不要抄哪个符号」时，另开对照 `Reference`（样板：[TL2 场景流式对照](/02-程序-前/TL2场景流式对照.md)）。开篇写清「外库提取，不是本库现行约定」；权威仍是流程篇。对照按**主题**开，不要按来源库开总目录。

本库改名前缀（如旧 `EU` → `DDove`）可留一句史，不要写成「去某盘找旧工程」。

## index 怎么维护（多人）

业务目录（`01`–`04`）三人关联：根 → 职种 `index.md` → `index_<短号>.md` → 篇。SPEC 保留名仍是 `index.md`；个人清单是普通概念，文件名用短号，**不要** `index_1`。

| 文件 | 关联什么 | 何时改 |
|------|----------|--------|
| 根 `/index.md` | 五个职种目录 + `log.md` | 几乎不改 |
| `02-程序-前/index.md` | 子目录 + `index_cjh` 等 | 新人加入、或新建主题子目录 |
| `02-程序-前/index_cjh.md` | cjh 的篇 | **只有 cjh** 加/改名/删自己的篇时改 |

同一职种两个人各改各的 `index_<短号>.md`，不会撞。篇文件仍按主题命名（`DDoveFramework-Core.md`），`generated.by: human:cjh`。短号花名册：[整理人](/00-索引/整理人.md)。

不要日常「重生成各层 index」。例外：`/ddove-wiki-change-state` 只改**本篇所在清单那一行**徽章，不重写整份。细则：[开篇与清单徽章](Agent/开篇与清单徽章.md)。目录挤了再按**主题**拆子目录，不要按人拆目录。

## 变更史（按日 + 短号）

SPEC §9 保留名仍是根 `DDoveMiniGameWiki/log.md`。明细按人拆，避免两人改同一份当天文件。

| 文件 | 写什么 |
|------|--------|
| `DDoveMiniGameWiki/log.md` | 日期索引：`## YYYY-MM-DD` + 链到各人当日分册 |
| `DDoveMiniGameWiki/_log/log_YYYY-MM-DD_<短号>.md` | 该人当日条目，**新条目插在该日文件顶部** |

新的一天：先建自己的 `log_YYYY-MM-DD_<短号>.md`；根 `log.md` 若还没有该日 `##` 再加一节（两人同日只可能撞这一行）。`_log/` 不当概念、不检索。

## 作业闸门（CI / 认主）

1. **缺 `type` / `status`**：本地 `python DDoveMiniGameTools/check_wiki.py`；GitHub Actions `wiki-check` 同样跑。概念必须有 `type`，`status` 只能是 `draft`|`stable`|`deprecated`。分类 `index.md` / `log.md` / `_log/` 不查。
2. **当天 log**：只写 `log_YYYY-MM-DD_<短号>.md`，见上一节。
3. **改别人的篇**：`generated.by` 不是自己则不要改归属、不要挂进自己的 `index_*`。GitHub 认主用仓库根 `.github/CODEOWNERS`（把 `@CHANGE_ME` 换成 [整理人](/00-索引/整理人.md) 里的 GitHub 名并取消注释）。未填用户名时 PR 不会自动指定审的人，仍靠本条。

[^okf-spec]: Open Knowledge Format（仓库根 SPEC.md）
