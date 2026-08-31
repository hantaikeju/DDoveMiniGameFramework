---
name: ddove-wiki
description: >-
  Wiki-first answering for DDove MiniGame Framework. Before reasoning or coding,
  grep wiki/ as an OKF bundle (index.md, type/tags/status frontmatter). Use on
  any project question, bug triage, architecture, UI, packing, or when the user
  mentions wiki, OKF, knowledge, or 问题/答案/思考先 Wiki 检索. If SPEC.md was just
  replaced or the user wants wiki compliance/upgrade, tell them to invoke
  ddove-okf-upgrade instead of upgrading here. Replaces retired name mgf-wiki.
---

# 问题 · 答案 · 思考 → 先 Wiki（OKF）

知识库：仓库内 `wiki/`。规范原文：仓库根 `SPEC.md`。本库约定：`wiki/00-索引/OKF约定.md`。

本库 skill 一律 `ddove-` 开头。旧名 `mgf-wiki` / `mgf-okf-upgrade` 已废，不要再调用。

**强制顺序（不可跳过）：**

```
用户问题 → 提取关键词
        → Grep frontmatter（type/title/tags）+ 正文（排除 _log/_tools/_spec）
        → 读命中概念；Stub 跟 sources / 文内权威链接
        → 用 Wiki 结论组织思考 →（不足时再查代码）→ 给出答案
```

问答**不要**为了找篇去走 `index.md` → `index_<短号>.md`。那两份只给**写入/归属**。约定只读 `wiki/00-索引/OKF约定.md`，不必先开检索指南 / 来源说明。

禁止一上来全库翻 `DDoveMiniGameClient/Library` / `PackageCache`。  
Wiki 无命中或过期时，再查代码，并标明「Wiki 未收录 / 以代码为准」。

日常问答**不要**通读 `SPEC.md`，**不要**新建/改状态 wiki 篇。写篇请用户调用 `/ddove-wiki-create`；改 `status`/`verified` 调用 `/ddove-wiki-change-state`。替换 `SPEC.md` 或要检查/升级调用 `ddove-okf-upgrade`。

## 何时启用

- MiniGame 框架 / 客户端 / 打包 / UI / 资源 / 排障相关提问
- 需要先想清楚再改代码、写结论时
- 用户提到 wiki、知识库、OKF、`ddove-wiki`

## 检索步骤（必须执行工具调用）

1. **拆词**：2–6 个中英文关键词（症状、类名、协议、表名）。
2. **Grep**（优先 frontmatter；**必须**排除 `_log` 等）。路径用仓库相对 `wiki`，职种已明则收窄目录：

```bash
rg -n -g "*.md" -g "!_log/**" -g "!_tools/**" -g "!_spec/**" "^type: |^title: |^tags: |关键词1|关键词2" wiki
rg -n -g "!_log/**" "关键词" wiki/02-程序-前
```

3. **读概念**：看 `type` / `status` / `sources` / `generated.by` / `verified`。`type: Stub` 或 `wiki_stub: true` → 跟 `sources[].resource` 与文内权威链接，不当全文。
4. **信任**：无 `verified` = unverified；与代码冲突以**当前代码**为准。`deprecated` / 过 `stale_after` 先核对源。
5. **再决定是否查代码**：Wiki 已够用 → 直接答；缺口 / 需行号 → 只打开相关脚本。

`index.md` / `index_<短号>.md`：**写篇、挂清单**时才打开，问答检索跳过。

## 分类速查

业务目录按职种，谁主写放谁那里。

| 目录 | 职种 |
|------|------|
| `00-索引` | 检索 / 约定 / 升级 |
| `01-策划` | 策划 |
| `02-程序-前` | 程序（前） |
| `03-程序-后` | 程序（后） |
| `04-美术` | 美术 |

约定入口：`wiki/00-索引/OKF约定.md`。职种目录见上表（按关键词选，不必先读根 index）。

## 禁区（默认不要全文 rg）

- `DDoveMiniGameClient/Library/`、`Temp/`、`Logs/`、`UserSettings/`
- `wiki/_tools/`、`wiki/_spec/`、`wiki/_log/`、`wiki/.obsidian/`
- 仓库根 `SPEC.md`：规范原文，不当概念检索；升级走 `ddove-okf-upgrade`

## 写 wiki（先要生成人）

本会话要**新建或改写** `01`–`04`（及带归属的约定篇）时：

1. 还没有生成人短号 → **先问一句**「这篇算谁的生成人（短号）？」停住，不要先写文件。
2. 同一会话只问一次，后续篇复用；用户改口再换。
3. 短号对 [整理人](wiki/00-索引/整理人.md)；表里没有先补一行。
4. `generated.by: human:<短号>`。禁止猜、禁止用 `process:agent` 顶替。
5. 新篇挂到该人的 `index_<短号>.md`，不要写到别人的清单。

只检索、不改 wiki 时不必问。

## 写 index

业务目录（`01`–`04`）：加/改名/删一篇只改**该生成人**的 `index_<短号>.md`。职种 `index.md` 只在新人加入时加一行 `index_<短号>`。不要用 `index_1`。不要改根 `wiki/index.md`。`00-索引` 的约定篇仍可直接列在该目录 `index.md`。

## 写变更史

改 wiki 或落地约定时：条目写进当天 `wiki/_log/log_YYYY-MM-DD_<短号>.md`（插在该日文件顶部）。根 `wiki/log.md` 只加/保持当日 `##` 并链到各人分册。新的一天先建自己的日文件；该日还没有 `##` 再在 `log.md` 最上面加一节。

## 答案模板

1. **结论**（1–3 句；`draft` / `deprecated` 必须点明）
2. **Wiki 依据**（Concept ID + `type` + `status`：draft 起草 / stable 正式 / deprecated 废弃 + 要点）
3. **信任**（unverified / machine-confirmed / human-reviewed；是否过期）
4. **代码核对**（若有；否则写「未再翻代码」）
5. **下一步**（验证 / 仍缺的信息；若涉及 SPEC 替换则提示调用 `ddove-okf-upgrade`）
