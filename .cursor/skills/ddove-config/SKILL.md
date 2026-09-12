---
name: ddove-config
description: >-
  改 / 加 / 查 DDoveMiniGameConfig 配表，或导表失败排查。
  Triggers: 改表、加表、填 csv、__tables__、导表报错、schema、校验器。
  Does not create wiki pages, run Unity Process, or edit Generate/Luban / GameRes/Cfg by hand.
---

# ddove-config

本库配表 playbook。上游对照：[luban/ai](https://github.com/focus-creative-games/luban/tree/main/ai)（`luban-add-table` / `excel-fill` / `schema-design` / `validator` / `generate-debug` / `runtime-load`）。改编，不整包覆盖、不引进 `Luban.Agent` / MCP。

工程约定：`DDoveMiniGameWiki/01-策划/Luban填表.md`、`DDoveMiniGameWiki/02-程序-前/DDoveCfg.md`。检索原语：`.agents/skills/ddove-wiki/SKILL.md`。问答五段 / 改客户端其它代码走 `ddove-work`。方案没对齐走 `/ddove-grill`。

源只在 `DDoveMiniGameConfig/Data/*.csv`（UTF-8 BOM）。生成器 `Tools/Luban` 钉 Release **v5.1.0**。本刀 target `client`，`cs-simple-json` + `json`。

## 步骤

1. **检索**：执行 `ddove-wiki`，关键词含 填表 / DDoveCfg / 表名。完成：命中列表或「Wiki 未收录」。
2. **只走一条分支**（完成条件写在各分支末）：
   - **填表**：只改 `Data/` 里已有 csv 的数据行。先读该文件 `##var` / `##type` / `##group`。不改 `##type`、主键语义、分组，除非用户点名。单元格有逗号要按 csv 引号。存回 UTF-8 BOM。完成：改的是源 csv，不是导出 json。
   - **加表**：先在 `Data/` 新建 `名字.csv`（不要 `#` 开头），A1 为 `##var`；再在 `__tables__.csv` 登记 `full_name` / `value_type` / `read_schema_from_file` / `input` / `index` / `mode`。`read_schema_from_file=true` 时不要在 `__beans__` 再定义同名 bean。`input` 相对 `dataDir`（即文件名）。完成：表文件 + `__tables__` 一行都在。
   - **schema**：程序改 `__beans__.csv` / `__enums__.csv` / `Defines/`。第二行是嵌套列（`*fields` / `*items`），不要压成单行表头。扁平行表优先 `read_schema_from_file`。敏感字段 `s`。完成：类型字符串与 mode/index 对得上。
   - **校验**：写在 `##type`（或 XML `type`）。常用：`int!`、`int#ref=demo.Tbitem`、`int#ref=demo.Tbitem?`、`int#range=[1,100]`、`(list#size=4),int`。不要为通过生成而削弱校验。完成：类型串已写上，准备导表。
   - **导表 / 失败**：跑 `DDoveMiniGameConfig/gen_client.bat`（或 `.sh`）。不要在 Unity 里 `Process`。失败先看日志；需要机器读时在同命令加 `--errorFormat json`，按 `schema` / `data` / `validation` / `codegen` / `cli` 修对应 csv 单元格。输出目录会被清空：只许指到 `Assets/Game/Generate/Luban` 与 `Assets/GameRes/Cfg`。完成：bat 退出 0，或已指出文件+行列。
   - **运行时**：不手写 loader。读表走 `CfgUtility` / 生成的 `cfg.Tables`；Launch 先 `DDoveCfgKit.LoadAsync` 再碰 `Interface`。Game 不引 Res / Yoo。完成：只对照 [DDoveCfg](DDoveMiniGameWiki/02-程序-前/DDoveCfg.md)，未新造加载入口。
3. **禁止**：改 `Generate/Luban`、`GameRes/Cfg` 当源；另存 xlsx 当源；拷第二份给 server；把官方 `luban-*` 技能原样丢进 `.agents/skills/`。

完成：源 csv 已改或导表已跑；wiki 未收录处已标明。
