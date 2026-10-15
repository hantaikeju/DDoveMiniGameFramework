# Agent

整个 AI 用法（仓库 `ddove-*`、grill、个人 Cursor skill）。不是本工程业务。约定仍读 [OKF约定](../OKF约定.md)。

本目录改的人少，概念直接列在下面。

# 概念

* [Agent 总览](Agent总览.md) - 用法在 01–04，需求稿在 05-需求，AI 用法在本目录。聊天里敲 `/ddove-help`。 `Reference`
* [需求稿工作流](需求稿工作流.md) - 人只走对齐、开工、展示。验收通过后改过的篇写成正式。 `Playbook`
* [开篇与清单徽章](开篇与清单徽章.md) - 正文不自报 status。05-需求整份做完才生成用法篇。 `Playbook`
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
* [对齐开工展示](对齐开工展示.md) - 人只走对齐、开工、展示。废弃需求稿进归档夹，超过 90 天再删。 `Playbook`
* [对齐后接着做完](对齐后接着做完.md) - grill 对齐后代理落稿、改文件，并写上正式与废弃。人只对接需求、确认产品。 `Playbook`
* [开工后选下一步](开工后选下一步.md) - grill 对齐后代理接着做完并写上状态。人看产品。 `Playbook`
* [一拍只处理一篇](一拍只处理一篇.md) - 落稿、改文件、写状态每一拍只碰一篇。多于一篇时只做点名的那篇。 `Playbook`
