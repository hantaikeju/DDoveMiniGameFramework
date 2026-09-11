---
type: Playbook
title: UPM 落地
description: Package Manager 用 URL 装完后，GitHub 来源的包复制一份到工程根 Packages/。无科学上网也能开工程，不会再 download err。
tags: [程序-前, upm, packages]
status: stable
generated: { by: human:cjh, at: 2026-09-03T02:10:00Z }
verified: { by: human:cjh, at: 2026-09-03T02:15:00Z }
sources:
  - id: manifest
    resource: ../../DDoveMiniGameClient/Packages/manifest.json
    title: Packages/manifest.json
  - id: unitask
    resource: /02-程序-前/UniTask异步.md
    title: 异步用 UniTask
  - id: nuget-scriban
    resource: /02-程序-前/NuGet与Scriban.md
    title: NuGet 与 Scriban
---

# UPM 落地

Concept ID：`/02-程序-前/UPM落地`。清单：[index_cjh](/02-程序-前/index_cjh.md)。

`Packages/manifest.json` 是 **Unity Package Manager** 的清单：Git URL、OpenUPM、官方注册表。装的是 UPM 包。和 [NuGet 与 Scriban](/02-程序-前/NuGet与Scriban.md) 不是一条线（那条进 `Assets/Packages/`）。

## 约定

GitHub / Git URL 装完后，**统一复制一份**到工程根 `Packages/`，整夹进 Git。别人在非科学上网环境打开工程，Unity 用本地这份，不再去 GitHub 拉，避免 `download err`。

`manifest.json` 里的 URL **留下**。给不想存本地夹、能上网的人继续自动拉。不要改成 `file:`，也不要删远程条目。

同名包：`Packages/` 里有带 `package.json` 的文件夹时，**嵌入优先**，不会再按 URL 下载。

## 步骤

1. Package Manager 用 Git URL（或注册表）装好，工程能编。
2. 从 `Library/PackageCache/com.xxx@…`（或本次解析结果）**整夹复制**到：

```
DDoveMiniGameClient/Packages/com.xxx@版本或hash/
```

保留 `package.json`。目录名带 semver 或 commit 短号，和现有 UniTask / NuGetForUnity 一样。
3. 提交该文件夹。**不改** `manifest.json`。
4. 升级：再装新版本 → 整夹替换 `Packages/com.xxx@…`（可改文件夹名上的版本）→ 提交。manifest 的 URL 仍可指向新 tag，给只靠自动拉的人用。

`com.unity.*` 走官方源即可，不必拷进 `Packages/`。

## 现有例子

| 包 | 本地夹 |
|---|---|
| UniTask | `Packages/com.cysharp.unitask@2.5.11/` |
| NuGetForUnity | `Packages/com.github-glitchenzo.nugetforunity@acc1c7bc9e/` |
| PrimeTween | `Packages/com.kyrylokuzyk.primetween@1.4.11/`，见 [PrimeTween](/02-程序-前/PrimeTween.md) |

用法见 [异步用 UniTask](/02-程序-前/UniTask异步.md)。NuGetForUnity 只是工具；Scriban 仍走 nuget，见 [NuGet 与 Scriban](/02-程序-前/NuGet与Scriban.md)。

## 不要

- 装完只留 Git URL、不拷 `Packages/`（无科学上网会 download err）
- 为落地去改 / 清空 manifest
- 把 nuget 的 `Assets/Packages/Scriban.*` 拷进工程根 `Packages/`
- 同一包既 Git URL 又嵌两份不同内容长期并存（升级时整夹替换，只留一份嵌入）
