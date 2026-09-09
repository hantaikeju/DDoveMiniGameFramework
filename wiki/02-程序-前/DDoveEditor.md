---
type: Playbook
title: DDove Editor
description: UIToolkit 总门面 DDove/Editor。面板用 [DDoveEditorPanel] 挂上去。SO Inspector 仍跟 Extension。
tags: [程序-前, editor]
status: stable
generated: { by: human:cjh, at: 2026-09-04T09:06:00Z }
sources:
  - id: core
    resource: /02-程序-前/DDoveFramework-Core.md
    title: DDoveFramework Core
  - id: res
    resource: /02-程序-前/DDoveRes.md
    title: DDoveRes
  - id: boot
    resource: /02-程序-前/DDoveBoot.md
    title: DDoveBoot
  - id: scriban
    resource: /02-程序-前/NuGet与Scriban.md
    title: NuGet 与 Scriban
  - id: hub
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Editor/DDoveEditorWindow.cs
    title: DDoveEditorWindow.cs
  - id: panel-attr
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Editor/DDoveEditorPanelAttribute.cs
    title: DDoveEditorPanelAttribute.cs
  - id: res-panel
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveRes/Editor/DDoveResEditorPanel.cs
    title: DDoveResEditorPanel.cs
---

# DDove Editor

Concept ID：`/02-程序-前/DDoveEditor`。权威实现：[DDoveEditorWindow.cs](../../DDoveMiniGameClient/Assets/DDoveFramework/Editor/DDoveEditorWindow.cs)。结论与代码冲突以代码为准。

总门面是一个 UIToolkit 窗口，菜单 **`DDove/Editor`**。以后要给使用者看的面板，**不要**再开独立 `EditorWindow`，用注册宏挂到这扇窗上。

## 挂面板（宏）

实现 `IDDoveEditorPanel`，类上挂 `[DDoveEditorPanel(id, title, order)]`。门面用 `TypeCache` 收集，**不要**改窗口源码、不要 `#if` 往里硬塞。

```csharp
[DDoveEditorPanel("res", "Res", 100)]
public sealed class DDoveResEditorPanel : IDDoveEditorPanel
{
    public void Build(VisualElement root) { }
}
```

| | 放哪 |
|--|------|
| 窗壳 / 宏 / 收集 | `DDoveFramework/Editor/`，程序集 `DDoveFramework.Editor` |
| 面板实现 | `Extension/Xxx/Editor/`，`Xxx.Editor` 引用 `DDoveFramework.Editor` + 自己的运行时 |
| SO Inspector、Create 菜单 | 仍跟 Kit，不必进总窗 |
| 业务面板 | 游戏工程 `Editor/`，同样挂宏。不进框架 |

`DDoveFramework.Editor` **不要**引用任何 Kit。没装 UI 也能编门面；有 UI 时 `DDoveUI.Editor` 自己挂。

## 落点

```
DDoveFramework/
  Core/                         无 Editor
  Editor/                       总门面（UIToolkit）
  Extension/
    DDoveRes/Editor/            宏挂 Res 页 + Init Info Inspector
    DDoveBoot/                  一般不需要 Editor
    DDoveUI/Editor/             以后同样挂宏
```

Yoo 官方收集器照旧，不要再包一层。

[DDoveBoot](/02-程序-前/DDoveBoot.md) 不拖 Init Info。Res 页只读 Yoo 收集器里的包。

## 不要

一个 asmdef 引用所有 Kit → 没装 UI 也编 UI Editor。  
门面源码里 `#if` / 手写 `Add(new XxxPanel())`。  
独立 `MenuItem` 再开一扇业务窗（Create 资产菜单除外）。  
Editor 进 [Core](/02-程序-前/DDoveFramework-Core.md)。

## 程序集

`DDoveFramework.Editor`：`includePlatforms: Editor`，`references` 空。  
`DDoveFramework.Extension.Xxx.Editor`：引用 `Xxx` + `DDoveFramework.Editor`。  
运行时 asmdef **不要**引用 Editor。`DDoveRes.Editor` **不要**引用 `DDoveBoot`。  
Scriban 等生成器只开 Editor，见 [NuGet 与 Scriban](/02-程序-前/NuGet与Scriban.md)。

菜单：总窗 `DDove/Editor`；Create 仍可 `DDove/Res/Init Info`。

## 和启动的关系

使用者：Yoo 收集器收资源；总窗 Res 页只展示收集器包裹；业务 Editor 进 Launch。  
运行时：[DDoveBoot](/02-程序-前/DDoveBoot.md) 只调 Extension Init。不要在 Editor 里生成另一套 Load，见 [DDoveRes 按需加载](/02-程序-前/DDoveRes按需加载.md)。
