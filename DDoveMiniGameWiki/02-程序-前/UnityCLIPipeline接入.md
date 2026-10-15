---
type: Playbook
title: Unity CLI Pipeline 接入
description: 把 2022.3 魔改的 com.unity.pipeline 嵌进客户端，本机装 CLI，Cursor 用硬边界和 ddove-unity-pipeline。当前编辑器是团结 2022.3.61t14。
tags: [程序-前, 索引, agent]
status: stable
generated: { by: human:cjh, at: 2026-09-30T17:42:00Z }
verified: { by: human:cjh, at: 2026-10-14T02:43:00Z }
sources:
  - id: req
    resource: /05-需求/UnityCLIPipeline接入.md
    title: Unity CLI Pipeline 接入（需求稿）
  - id: skill
    resource: ../../.agents/skills/ddove-unity-pipeline/SKILL.md
    title: ddove-unity-pipeline
  - id: rule
    resource: ../../.cursor/rules/unity-pipeline-boundary.mdc
    title: unity-pipeline-boundary
  - id: pkg
    resource: ../../DDoveMiniGameClient/Packages/com.unity.pipeline@0.4.0-exp.1/package.json
    title: com.unity.pipeline package.json
  - id: upm
    resource: /02-程序-前/UPM落地.md
    title: UPM 落地
  - id: server
    resource: ../../DDoveMiniGameClient/Packages/com.unity.pipeline@0.4.0-exp.1/Editor/EditorPipelineServer.cs
    title: EditorPipelineServer.cs
  - id: version
    resource: ../../DDoveMiniGameClient/ProjectSettings/ProjectVersion.txt
    title: ProjectVersion.txt
---

# Unity CLI Pipeline 接入

Concept ID：`/02-程序-前/UnityCLIPipeline接入`。清单：[index_cjh](/02-程序-前/index_cjh.md)。

[打开说明](UnityCLIPipeline接入.html)

## 干什么

客户端嵌入按 Unity 2022.3 改过的 `com.unity.pipeline`。本机用魔改 CLI `1.0.0-beta.2` 连开着的编辑器。Cursor 改场景、Prefab、材质、ScriptableObject、动画、工程设置，或移动删除 `Assets` 里已有资源时，走 `ddove-unity-pipeline`。`.cs` 直接改文件，再用 CLI 重编译确认。

当前编辑器是团结 `2022.3.61t14`（Tuanjie `1.6.13`）。包内 `unity` 是 `2022.3`。`manifest.json` 不写这条嵌入。包自带 `Pipeline` 菜单，不挂进 `DDove/Editor`。

## 有哪些接口

- `Packages/com.unity.pipeline@0.4.0-exp.1`
- 魔改 CLI `unity` `1.0.0-beta.2`
- `editor_status`
- Cursor 硬边界
- `ddove-unity-pipeline`
- 日常重编译

## 每个接口干什么

`Packages/com.unity.pipeline@0.4.0-exp.1` 是嵌入包。`package.json` 的 `unity` 是 `2022.3`。打开工程后 Package Manager 按包内依赖解析 `com.unity.nuget.newtonsoft-json` 与 `com.unity.nuget.mono-cecil`。编辑器加载后，本机 `127.0.0.1` 的 `7800–7849` 起 Pipeline 服务。

从 Hub 打开时命令行没有 `-automated`，也不是 batchmode。`EditorPipelineServer.ServerStarted` 打一次警告：模态框可能卡住 CLI 连续命令。包里这个参数只决定打不打这行，不关弹窗。交互式使用不必为了消掉警告改启动参数。

魔改 CLI `unity` `1.0.0-beta.2` 来自 `rocwood/unity-cli-2022-mod`。本机还没有时：

```powershell
$env:UNITY_CLI_CHANNEL='beta'; & ([scriptblock]::Create((irm https://raw.githubusercontent.com/rocwood/unity-cli-2022-mod/main/cli/install.ps1))) -Target 1.0.0-beta.2
```

`editor_status` 确认编辑器开着该工程。命令是 `unity command --project-path G:\Self_WorkSpace\DDoveMiniGameFramework\DDoveMiniGameClient editor_status`。没有应答时，纯文本仍可改，回复写明还没做编译或场景验证；场景和资源改动停在这一步。

Cursor 硬边界在 `.cursor/rules` 的 `unity-pipeline-boundary`。场景、Prefab、材质、ScriptableObject、动画、工程设置，以及移动或删除 `Assets` 里已有资源，走 Pipeline。`.meta` 和序列化文件不手改。其它 agent 用同一条 `unity` 命令，不另写规则。

`ddove-unity-pipeline` 在 `.agents/skills/ddove-unity-pipeline/SKILL.md` 与 `.cursor/skills/ddove-unity-pipeline/SKILL.md`。仓库根 `AGENTS.md` 有一行指针。命令写在技能里。热重载也写在技能里，不是日常改代码的必经步骤。

日常重编译：改完 `.cs` 后按顺序调用 `set_autotick`、`recompile`、`recompile_status`。`set_autotick` 在域重载后会关掉，下一轮编译前再开一次。
