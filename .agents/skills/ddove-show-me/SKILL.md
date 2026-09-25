---
name: ddove-show-me
description: >-
  Triggers: /ddove-wiki-create 落下一篇 05-需求，种类「要什么不要」；/ddove-wiki-change-state 把 05-需求 整份做完，用法篇种类「三块」，需求稿种类「要什么不要」。
disable-model-invocation: true
---

# ddove-show-me

个人画图仍是本机 `/show-me`，见 `DDoveMiniGameWiki/00-索引/Agent/show-me安装.md`。本技能只写一张静态 HTML。用法篇三块见 `DDoveMiniGameWiki/00-索引/Agent/需求做完生成接口总结.md`。需求稿留下的要什么、不要见 `DDoveMiniGameWiki/00-索引/Agent/需求稿留下要什么不要.md`。

调用方传入目标 `.md` 路径，以及种类「要什么不要」或「三块」。一次调用写一张。链接由 `/ddove-wiki-create` 或 `/ddove-wiki-change-state` 写进该 `.md`。本技能不改那份 `.md`。

谁来调：`/ddove-wiki-create` 落 `05-需求` 新稿时，种类「要什么不要」。`/ddove-wiki-change-state` 把 `05-需求` 整份做完时，对用法篇用「三块」，再对需求稿用「要什么不要」重画。`ddove-work` 过线不调用。不扫描仓库去给已有篇补 HTML。

## 步骤

1. **入参**：目标 `.md` 已在仓库里，种类是「要什么不要」或「三块」。一次一个路径。完成：路径存在，种类是这二者之一。
2. **文件名**：HTML 与该 `.md` 同目录，主文件名与该 `.md` 相同，扩展名 `.html`。主文件名只取自 `.md` 文件名。YAML `title` 含「需求稿」时，文件名仍用 `.md` 主文件名。完成：输出路径已定。
3. **取块**：读该 `.md` 正文，跳过 YAML frontmatter。以调用当时的正文为准。
   - 「要什么不要」：`## 要什么`、`## 不要`
   - 「三块」：`## 干什么`、`## 有哪些接口`、`## 每个接口干什么`
   每块收到下一个同级或更高级标题为止。缺任一要求的标题则停，不写 HTML，把缺的标题交还调用方。完成：要画的块已齐，或已停。
4. **写页**：UTF-8 单文件，样式内联，无外链资源，无脚本。文件开头是 `<!DOCTYPE html>`，不写 YAML，不写 `type` / `status` / `tags`。页内主标题用该篇第一个 `#` 标题；没有则用主文件名。块内段落、列表、行内代码与围栏代码转成 HTML，文字保持该篇原文。wiki 链接写成可见文字。
   - 「要什么不要」：同一页两区，左「要什么」、右「不要」；窄屏上下排。
   - 「三块」：同一页按干什么、有哪些接口、每个接口干什么三节顺排。
   整页可滚动阅读。同路径已有 `.html` 则整文件重写。完成：该 `.html` 已写；开头不是 `---`；页上没有分步按钮，也没有按步隐藏的内容。
5. **收尾**：不改目标 `.md`，不改其它篇，不把 HTML 写入清单或 log。不读、不改 `~\.cursor\skills\show-me\`。仓库里的技能目录是 `ddove-show-me`。完成：磁盘上只多了或更新了这一张 HTML。

完成：调用方给出的那一个 `.md` 旁边有同主文件名 `.html`；内容与种类对应；无概念 frontmatter；目标 `.md` 的链接仍由调用方写。
