---
name: ddove-rename-workspace
description: >-
  fork / 拷走开新游戏时，把 DDoveMiniGame 四件套目录和仓库内指针换成自定义前缀。
  只在用户调用 /ddove-rename-workspace 或点名改 Client/Config/Wiki/Tools 目录名时用。
disable-model-invocation: true
---

# ddove-rename-workspace

权威：`DDoveMiniGameWiki/00-索引/Agent/改工作区前缀.md`。换引擎 / 换 `ddove-` 品牌走 [工作流迁移](DDoveMiniGameWiki/00-索引/Agent/工作流迁移.md)，不要走本 skill。

不写独立脚本。不问答五段（`ddove-work`）。不改表（`ddove-config`）。不建篇。

## 步骤

1. **收参**：消息里已有自定义名就用；否则问一句「新前缀？（拼成 `{名}Client` / `Config` / `Wiki` / `Tools`）」。名：字母或数字开头，只含字母数字下划线，建议 PascalCase。不要空格、不要中文、不要 `DDoveMiniGame`。完成：已有合法前缀。

2. **护栏**：四个顶层目录必须仍叫 `DDoveMiniGameClient` / `Config` / `Wiki` / `Tools`，否则停（已改过或不是这份工作区）。仓库根仍是 `DDoveMiniGameFramework` 时：用户本条或本会话必须写明 fork / 拷走 / 开新游戏；没有则停，问一句「这是 fork 开新游戏吗？本框架仓不能跑。」不要改根文件夹名、不要动 git remote。完成：四目录在，且允许跑。

3. **改目录**：`git mv`（或等价）四件套 → `{名}Client` / `{名}Config` / `{名}Wiki` / `{名}Tools`。根上若另有 `wiki/` 副本，不当第五件套。完成：新四件套在，旧四件套不在。

4. **改指针**：只替换这四对（按这个顺序，避免截断）：

   `DDoveMiniGameClient` → `{名}Client`  
   `DDoveMiniGameConfig` → `{名}Config`  
   `DDoveMiniGameWiki` → `{名}Wiki`  
   `DDoveMiniGameTools` → `{名}Tools`

   白名单：`gen_client.bat` / `.sh`、`AGENTS.md`、README、`.agents/skills/`、`.cursor/skills/`、`{名}Wiki/` 概念篇（排除 `_log` / `_tools` / `_spec`）、`.github/`、`{名}Tools/check_wiki.py`、`.gitignore`、`.cursorignore`。

   禁扫：`Packages/`、`Library/`、`PackageCache`、`.git/`、`{名}Client/Assets/Packages`。不要改 `DDoveMiniGameFramework`、`DDoveFramework`、`ddove-*` 品牌、PlayerSettings、`.sln` / `.csproj`。完成：白名单里旧四名已换成新名。

5. **核验**：在禁区外 `rg` 旧四名，概念篇应无命中（`_log` 历史可留）。用新路径跑 `{名}Tools/check_wiki.py`。完成：检查通过；回复里写：去 Unity 自己配 `productName` / 公司名 / bundle，并打开 `{名}Client` 让编辑器重生 sln。

完成：四件套已改名，白名单指针已换，检查通过。
