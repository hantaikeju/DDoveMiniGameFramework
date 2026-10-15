---
name: ddove-archive
description: 清扫废弃需求稿。只在用户调用 /ddove-archive 时用。
disable-model-invocation: true
---

# ddove-archive

权威：`DDoveMiniGameWiki/00-索引/OKF约定.md`。归档目录：`DDoveMiniGameWiki/_archive/需求/`。

不是每篇需求的一步。人不调用本技能时不跑。不问短号。不碰用法篇。

超过 90 天：当前 UTC 时刻减去 `deprecated_at` 大于 90 个 24 小时。

## 步骤

1. **补搬**：在 `DDoveMiniGameWiki/05-需求/` 找 `status: deprecated` 且 `title` 含「需求稿」的 `.md`。每个连同目录里的同名 `.html` 搬到 `_archive/需求/`。没有 `deprecated_at` 就写成当前 UTC 时刻。原路径不留文件。清单里链到该文件名的那一行改到新路径。完成：`05-需求` 里不再有这些废弃需求稿。
2. **删除**：读 `_archive/需求/` 里每篇需求稿的 `deprecated_at`。超过 90 天的，删掉该 `.md` 和同名 `.html`，并删掉清单上链到该文件名的那一行。没有 `deprecated_at` 的写成当前时刻，本步不删。完成：超期文件已删；未超期的仍在归档夹。
3. **回复**：同一回里列出搬走的篇和删掉的篇。不再二次确认。完成：回复里有这两份名单；没有删用法篇。
4. **日志**：`DDoveMiniGameWiki/_log/log_YYYY-MM-DD.md` 顶部记搬走和删除。根 `log.md` 缺该日 `##` 则补链接。完成：当日 log 已记。
5. **检查**：`py -3 DDoveMiniGameTools/check_wiki.py`。完成：检查通过。

完成：废弃需求稿在归档夹，或已因超过 90 天从仓库删除。用法篇还在原处。
