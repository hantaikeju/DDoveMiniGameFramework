---
type: Playbook
title: Unity CLI Pipeline 接入（需求稿）
description: 把 2022.3 魔改的 com.unity.pipeline 嵌进客户端，本机装 CLI，Cursor 用硬边界和 ddove-unity-pipeline。
tags: [程序-前, 索引, agent, 需求]
status: draft
generated: { by: human:cjh, at: 2026-09-27T17:16:00Z }
sources:
  - id: upm
    resource: /02-程序-前/UPM落地.md
    title: UPM 落地
  - id: editor
    resource: /02-程序-前/DDoveEditor.md
    title: DDove Editor
  - id: asm
    resource: /02-程序-前/正确路径与程序集方向.md
    title: 正确路径与程序集方向
  - id: version
    resource: ../../DDoveMiniGameClient/ProjectSettings/ProjectVersion.txt
    title: ProjectVersion.txt
  - id: pkg
    resource: C:/Users/Administrator/Desktop/Unity官方CLI Pipeline/com.unity.pipeline/package.json
    title: com.unity.pipeline package.json
---

# Unity CLI Pipeline 接入（需求稿）

Concept ID：`/05-需求/UnityCLIPipeline接入`。

把桌面上已按 Unity 2022.3 改过的 `com.unity.pipeline` 嵌进客户端，本机安装魔改 CLI `1.0.0-beta.2`，并给 Cursor 加上硬边界和技能 `ddove-unity-pipeline`。当前编辑器是 `2022.3.22f1c1`。

[要什么不要](UnityCLIPipeline接入.html)

## 要什么

包从 `C:\Users\Administrator\Desktop\Unity官方CLI Pipeline\com.unity.pipeline` 整夹拷到 `DDoveMiniGameClient/Packages/com.unity.pipeline@0.4.0-exp.1/`。`package.json` 的 `unity` 改为 `2022.3`。`Packages/manifest.json` 保持原样。包自带 `Pipeline` 菜单，不挂进 `DDove/Editor`。

打开工程后，Package Manager 按包内依赖解析 `com.unity.nuget.newtonsoft-json` 与 `com.unity.nuget.mono-cecil`，包能编译。编辑器加载后，本机 `127.0.0.1` 的 `7800–7849` 起 Pipeline 服务。

本机安装魔改 CLI（`rocwood/unity-cli-2022-mod`，`1.0.0-beta.2`）：

```powershell
$env:UNITY_CLI_CHANNEL='beta'; & ([scriptblock]::Create((irm https://raw.githubusercontent.com/rocwood/unity-cli-2022-mod/main/cli/install.ps1))) -Target 1.0.0-beta.2
```

编辑器开着该工程时，下面这条有应答：

```powershell
unity command --project-path G:\Self_WorkSpace\DDoveMiniGameFramework\DDoveMiniGameClient editor_status
```

Cursor 侧两处一起加：

- `.cursor/rules` 常驻一条规则，只写硬边界。按本仓库改写。
- 技能 `ddove-unity-pipeline`：`.agents/skills/ddove-unity-pipeline/SKILL.md` 与 `.cursor/skills/ddove-unity-pipeline/SKILL.md` 各一份。命令写在技能里。仓库根 `AGENTS.md` 加一行指针。

硬边界只约束 Cursor：

- 场景、Prefab、材质、ScriptableObject、动画、工程设置，以及移动或删除 `Assets` 里已有资源，走 Pipeline。
- `.meta` 和序列化文件不手改。
- `.cs` 等纯文本直接改，改完用 CLI 重编译确认（`set_autotick`、`recompile`、`recompile_status`）。
- 编辑器没开或 CLI 连不上时，纯文本仍可改，回复里写明还没做编译或场景验证。

其它 agent 用同一个 `unity` 命令。这刀不给它们另写规则。热重载命令写进技能；日常改代码以重编译为准。

## 不要

- `package.json` 的 `unity` 留在 `6000.0`
- 改 `manifest.json`，或改成 `file:`
- 把 Pipeline 面板挂进 `DDove/Editor`
- 原样拷贝桌面 `.cursor/rules/unity_guide.mdc` 和 `unity-pipeline` 技能
- 用包内 README 的官方 Unity 6 `install.ps1`
- 给非 Cursor 的 agent 另写一套规则
- 把热重载写成日常改代码的必经步骤
- 依赖解析失败时自行改低 `newtonsoft-json` 或 `mono-cecil` 版本

## 验收

1. `DDoveMiniGameClient/Packages/com.unity.pipeline@0.4.0-exp.1/package.json` 的 `unity` 是 `2022.3`。`DDoveMiniGameClient/Packages/manifest.json` 相对这刀没有改动。
2. `.cursor/rules` 有一条常驻规则，只写硬边界。`.agents/skills/ddove-unity-pipeline/SKILL.md` 与 `.cursor/skills/ddove-unity-pipeline/SKILL.md` 都在。仓库根 `AGENTS.md` 有一行指向 `ddove-unity-pipeline`。
3. 本机 `unity` 命令能跑，版本是魔改 CLI `1.0.0-beta.2`。
4. 编辑器开着 `DDoveMiniGameClient` 时，`unity command --project-path G:\Self_WorkSpace\DDoveMiniGameFramework\DDoveMiniGameClient editor_status` 有应答。
5. 编辑器没开，或 `com.unity.nuget.newtonsoft-json`、`com.unity.nuget.mono-cecil` 解析失败，这刀不算过。不把这两个依赖改到低于包内 `package.json` 所写版本。
