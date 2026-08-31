---
name: mgf-wiki
description: >-
  Wiki-first answering for DDove MiniGame Framework. Before reasoning or coding,
  grep wiki/ as an OKF bundle (index.md, type/tags/status frontmatter). Use on
  any project question, bug triage, architecture, UI, packing, or when the user
  mentions wiki, OKF, knowledge, or 问题/答案/思考先 Wiki 检索. If SPEC.md was just
  replaced or the user wants wiki compliance/upgrade, tell them to invoke
  mgf-okf-upgrade instead of upgrading here.
---

# 问题 · 答案 · 思考 → 先 Wiki（OKF）

知识库：仓库内 `wiki/`。规范原文：仓库根 `SPEC.md`。本库约定：`wiki/00-索引/OKF约定.md`。

**强制顺序（不可跳过）：**

```
用户问题 → 提取关键词 → 读 wiki/index.md 或分类 index.md
        → Grep frontmatter（type/title/tags）+ 正文
        → 读概念；Stub 跟 sources / 文内权威链接
        → 用 Wiki 结论组织思考 →（不足时再查代码）→ 给出答案
```

禁止一上来全库翻 `DDoveMiniGameClient/Library` / `PackageCache`。  
Wiki 无命中或过期时，再查代码，并标明「Wiki 未收录 / 以代码为准」。

日常问答**不要**通读 `SPEC.md`，**不要**改合规字段或重生成 index。替换 `SPEC.md` 或要检查/升级 wiki 时，请用户主动调用 `mgf-okf-upgrade`。

## 何时启用

- MiniGame 框架 / 客户端 / 打包 / UI / 资源 / 排障相关提问
- 需要先想清楚再改代码、写结论时
- 用户提到 wiki、知识库、OKF、`mgf-wiki`

## 检索步骤（必须执行工具调用）

1. **拆词**：2–6 个中英文关键词（症状、类名、协议、表名）。
2. **渐进展开**：先读 `wiki/index.md` 或分类 `index.md`。
3. **Grep**（优先 frontmatter，路径用仓库相对 `wiki`）：

```bash
rg -n -g "*.md" "^type: |^title: |^tags: |关键词1|关键词2" wiki
rg -n "关键词" wiki/00-索引
```

4. **读概念**：看 `type` / `status` / `sources` / `verified`。`type: Stub` 或 `wiki_stub: true` → 跟 `sources[].resource` 与文内权威链接，不当全文。
5. **信任**：无 `verified` = unverified；与代码冲突以**当前代码**为准。`deprecated` / 过 `stale_after` 先核对源。
6. **再决定是否查代码**：Wiki 已够用 → 直接答；缺口 / 需行号 → 只打开相关脚本。

## 分类速查

业务目录按职种，谁主写放谁那里。

| 目录 | 职种 |
|------|------|
| `00-索引` | 检索 / 约定 / 升级 |
| `01-策划` | 策划 |
| `02-程序-前` | 程序（前） |
| `03-程序-后` | 程序（后） |
| `04-美术` | 美术 |

入口：`wiki/index.md`、`wiki/00-索引/OKF约定.md`。

## 禁区（默认不要全文 rg）

- `DDoveMiniGameClient/Library/`、`Temp/`、`Logs/`、`UserSettings/`
- `wiki/_tools/`、`wiki/_spec/`、`wiki/_log/`、`wiki/.obsidian/`
- 仓库根 `SPEC.md`：规范原文，不当概念检索；升级走 `mgf-okf-upgrade`

## 写变更史

改 wiki 或落地约定时：条目写进当天 `wiki/_log/log_YYYY-MM-DD.md`（插在该日文件顶部）。根 `wiki/log.md` 只加/保持当日 `##` 索引，不要把明细堆回去。新的一天先建日文件，再在 `log.md` 最上面加一节。

## 答案模板

1. **结论**（1–3 句）
2. **Wiki 依据**（Concept ID 或路径 + `type` + 要点）
3. **信任**（unverified / machine-confirmed / human-reviewed；是否过期）
4. **代码核对**（若有；否则写「未再翻代码」）
5. **下一步**（验证 / 仍缺的信息；若涉及 SPEC 替换则提示调用 `mgf-okf-upgrade`）
