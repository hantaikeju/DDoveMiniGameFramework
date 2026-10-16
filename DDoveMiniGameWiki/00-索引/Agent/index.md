# Agent

整个 AI 用法（仓库 `ddove-*`、grill、个人 Cursor skill）。不是本工程业务。约定仍读 [OKF约定](../OKF约定.md)。

本目录改的人少，概念直接列在下面。

# 概念

* [Agent 总览](Agent总览.md) - 用法在 01–04，需求稿在 05-需求，AI 用法在本目录。聊天里敲 `/ddove-help`。 `Reference`* [需求稿工作流](需求稿工作流.md) - 人只走对齐、开工、展示。问完落稿后停下，开工等 /ddove-work。 `Playbook`* [开篇与清单徽章](开篇与清单徽章.md) - 正文不自报 status。05-需求整份做完才生成用法篇。 `Playbook`
* [开工班组](开工班组.md) - ddove-work 按 05-需求 里的需求稿改代码。得分写在需求稿上。 `Playbook`
* [show-me 安装](show-me安装.md) - 个人 skill 装到 `~\.cursor\skills\`，不进仓库 `ddove-`。 `Playbook`
* [工作流迁移](工作流迁移.md) - 换引擎只带走 AI 层与空工程壳。 `Playbook`
* [上游 skill 更新](上游skill更新.md) - 长期对照 mattpocock/skills 与 pstack。人看到更新后再 grill。 `Playbook`
* [ddove-config](ddove-config.md) - 本库配表 Agent 流程。对照 luban/ai 改编。 `Skill`
* [改工作区前缀](改工作区前缀.md) - fork 开新游戏时把四件套换成自定义前缀。入口 `/ddove-rename-workspace`。 `Playbook`
* [开工评分](开工评分.md) - 评分尺子。得分记在需求稿上，不另开文件。 `Playbook`
* [开工评分收口与编译验证](开工评分收口与编译验证.md) - 过线要审查 PASS。父会话每批回收后串行编译。离线记「未验证」。 `Playbook`
* [需求做完生成接口总结](需求做完生成接口总结.md) - 整份做完时用法篇只写三块，需求稿收成薄记录。 `Playbook`
* [需求稿留下要什么不要](需求稿留下要什么不要.md) - 做完后需求稿留下要什么、不要、评分和用法篇链接。 `Playbook`
* [说明页按show-me画](说明页按show-me画.md) - ddove-show-me 只认一种「说明页」。create / change-state / wiki-update 都传这个种类。开篇写「打开说明」。 `Playbook`
* [同名HTML说明页](同名HTML说明页.md) - 旧画法。现行见说明页按show-me画。 `Playbook` `deprecated`
* [对齐开工展示](对齐开工展示.md) - 人只走对齐、开工、展示。问完落稿后停下，开工等 /ddove-work。 `Playbook`* [对齐后接着做完](对齐后接着做完.md) - grill 对齐后落稿并停下。人另发 /ddove-work 再改文件并写上状态。 `Playbook`* [问完直接开工](问完直接开工.md) - grill 问完落稿并停下。开工等人另发 /ddove-work。 `Playbook`* [落稿后再开工](落稿后再开工.md) - /ddove-grill 问完就落需求稿并停下。人另发 /ddove-work 再按这篇开工。 `Playbook`
* [目标明确不数稿](目标明确不数稿.md) - 这句话里写了新目标就用 /ddove-grill，不数处理中的稿。问完落稿后停下。 `Playbook`* [接上前文](接上前文.md) - 只敲 /ddove-grill 时接上前文最近一条。问完落稿后停下，开工等 /ddove-work。 `Playbook`* [开工后选下一步](开工后选下一步.md) - grill 对齐后落稿并停下。人另发 /ddove-work 再写上状态。 `Playbook`* [一拍只处理一篇](一拍只处理一篇.md) - 落稿后停下。开工等 /ddove-work，每一拍只碰一篇。 `Playbook`
* [收掉skill冗余](收掉skill冗余.md) - 浅层 skill 副本与权威正文对齐。help 只留一处指路。create 落稿后停下。 `Playbook`
* [人的三步写短](人的三步写短.md) - 仓库根 README 的「人的三步」收成两个链接加三句。细则仍在需求稿工作流。 `Playbook`