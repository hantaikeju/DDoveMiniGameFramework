---
type: Playbook
title: show-me 安装
description: 把 HumanLayer show-me 装到本机 ~/.cursor/skills/，所有 Cursor 工程可用。不进仓库 .agents/skills/。
tags: [索引, agent, cursor, skill]
status: stable
generated: { by: human:cjh, at: 2026-09-07T05:54:00Z }
verified: { by: human:cjh, at: 2026-09-15T03:20:00Z }
sources:
  - id: humanlayer
    resource: https://github.com/humanlayer/skills
    title: humanlayer/skills
  - id: skill-md
    resource: https://github.com/humanlayer/skills/blob/main/plugins/show-me/skills/show-me/SKILL.md
    title: show-me SKILL.md
  - id: cursor-skills
    resource: https://cursor.com/docs/skills
    title: Cursor Agent Skills
  - id: okf
    resource: /00-索引/OKF约定.md
    title: OKF 约定与检索
  - id: map
    resource: /00-索引/Agent/Agent总览.md
    title: Agent 总览
---

# show-me 安装

Concept ID：`/00-索引/Agent/show-me安装`。权威源：[humanlayer/skills](https://github.com/humanlayer/skills)、[Cursor Skills](https://cursor.com/docs/skills)。旧路径 `/02-程序-前/show-me安装` 是 Stub。

`show-me` 是 Cursor **个人 skill**：用伪代码、调用树、文件树、Mermaid、diff、必要时写一个小 HTML，把当前话题画清楚。本库仓库 skill 一律 `ddove-` 开头，见 [OKF 约定](/00-索引/OKF约定.md)。**不要**把 `show-me` 放进 `.agents/skills/`。

## 落点

全机所有 Cursor 工程共用这一份：

```
%USERPROFILE%\.cursor\skills\show-me\SKILL.md
```

结构必须是「文件夹 + 里面的 `SKILL.md`」。不要只丢孤立文件，不要多套一层 `show-me/show-me/`。不要写进 `~\.cursor\skills-cursor\`（Cursor 内置目录）。

| 范围 | 路径 | 本库 |
|------|------|------|
| 个人 / 全局 | `~\.cursor\skills\show-me\` | **用这个** |
| 项目 | `.cursor/skills/show-me/` | 不要。会进 Git，且和 `ddove-` 约定打架 |

## 安装（Windows）

GitHub `git` / `npx skills add` 经常 443 失败。优先 raw 落盘：

```powershell
New-Item -ItemType Directory -Force "$env:USERPROFILE\.cursor\skills\show-me" | Out-Null
Invoke-WebRequest `
  -Uri "https://raw.githubusercontent.com/humanlayer/skills/main/plugins/show-me/skills/show-me/SKILL.md" `
  -OutFile "$env:USERPROFILE\.cursor\skills\show-me\SKILL.md" `
  -UseBasicParsing
```

能访问 GitHub git 时也可以：

```powershell
npx skills add humanlayer/skills --skill show-me --agent cursor -g -y
```

`-g` 才是用户级。不要填 blob 链接 `.../SKILL.md`。Customize 里 **Rules** 装不了这个；看 **Skills**。

## 怎么用

1. **Customize → Skills** 应能看到 `show-me`。没有则 Reload Window，或新开 Agent 对话。
2. 聊天输入 `/show-me` 或 `@show-me`。

正向目标：只装到本机 `~\.cursor\skills\show-me\`；用 Customize → **Skills**；安装地址用 raw 或 `npx skills add` 的仓库路径。
