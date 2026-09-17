---
type: Playbook
title: DDoveUI
description: 制作场景导出 Prefab，Launch 打开 WndHome。第二刀：UI EventSystem 只开 Input System Package，文本只留 TMP。
tags: [程序-前, ddoveui]
status: stable
generated: { by: human:cjh, at: 2026-09-10T08:14:00Z }
verified: { by: human:cjh, at: 2026-09-17T10:25:00Z }
sources:
  - id: grill
    resource: /00-索引/Agent/需求稿工作流.md
    title: 需求稿工作流
  - id: res
    resource: /02-程序-前/DDoveRes.md
    title: DDoveRes
  - id: ondemand
    resource: /02-程序-前/DDoveRes按需加载.md
    title: DDoveRes 按需加载
  - id: res-use
    resource: /02-程序-前/DDoveRes配置与使用.md
    title: DDoveRes 配置与使用
  - id: boot
    resource: /02-程序-前/DDoveBoot.md
    title: DDoveBoot
  - id: editor
    resource: /02-程序-前/DDoveEditor.md
    title: DDove Editor
  - id: arch
    resource: /02-程序-前/Architecture与角色.md
    title: Architecture 与角色
  - id: unitask
    resource: /02-程序-前/UniTask异步.md
    title: 异步用 UniTask
  - id: scriban
    resource: /02-程序-前/NuGet与Scriban.md
    title: NuGet 与 Scriban
  - id: core
    resource: /02-程序-前/DDoveFramework-Core.md
    title: DDoveFramework Core
  - id: primetween
    resource: /02-程序-前/PrimeTween.md
    title: PrimeTween
  - id: atlas
    resource: /02-程序-前/DDoveAtlas.md
    title: DDoveAtlas
  - id: ui-biz
    resource: /02-程序-前/UI业务封装.md
    title: UI 业务封装
  - id: input-system
    resource: /02-程序-前/DDoveUI切InputSystem.md
    title: DDoveUI 切 Input System
---

# DDoveUI

Concept ID：`/02-程序-前/DDoveUI`。清单：[index_cjh](/02-程序-前/index_cjh.md)。第一刀垂直闭环已落地（制作场景 → Prefab → Launch 开 `WndHome`）。结论与代码冲突以代码为准。

对照：[DDoveRes](/02-程序-前/DDoveRes.md)、[DDoveRes 按需加载](/02-程序-前/DDoveRes按需加载.md)、[DDoveAtlas](/02-程序-前/DDoveAtlas.md)、[DDoveBoot](/02-程序-前/DDoveBoot.md)、[DDove Editor](/02-程序-前/DDoveEditor.md)、[Architecture 与角色](/02-程序-前/Architecture与角色.md)、[UI 业务封装](/02-程序-前/UI业务封装.md)、[DDoveUI 切 Input System](/02-程序-前/DDoveUI切InputSystem.md)。

## 第一刀（已落地）

一张 `WndHome`：制作场景 → 导出 Prefab → 收集器能 Load → Play 看见面板。

运行时带齐：`Initialize`、`OpenAsync` / `Close` / `CloseAll` / `OpenExclusiveAsync`、`NavigateToAsync` / `BackAsync` / `BackToAsync`、LRU、`DDoveUIPopupPanelBase`。样板页只用 `OpenAsync<WndHome>`。

不搬：多人分屏、OSA、URP Overlay、图集进 UI 程序集（见 [DDoveAtlas](/02-程序-前/DDoveAtlas.md)）、模块/扩展模板选择、Builtin+Remote。UI EventSystem 已切 Input System，见 [DDoveUI 切 Input System](/02-程序-前/DDoveUI切InputSystem.md)。PrimeTween 不进 Base、不做 ClickScale；业务装包见 [PrimeTween](/02-程序-前/PrimeTween.md)。

## 第二刀（Input System + TMP）

只开 Input System Package（`activeInputHandler: 2`）。不要 Both。EventSystem 用 `InputSystemUIInputModule`。文本只留 TMP。运行时 Canvas 仍是 Camera。不搬 Probe。细则与验收见 [DDoveUI 切 Input System](/02-程序-前/DDoveUI切InputSystem.md)。

## 程序集

没装 UI 时，Core / Res / Boot / 总门面必须仍能编。**不要**为了在 Boot 里调 `Initialize` 让 Boot 引用 UI。

```
DDoveFramework.Core                         references 空
DDoveFramework.Editor                       references 空；includePlatforms: Editor
DDoveFramework.Extension.DDoveRes           Core, UniTask, YooAsset
DDoveFramework.Extension.DDoveRes.Editor    DDoveRes, DDoveFramework.Editor, YooAsset, YooAsset.Editor
DDoveFramework.Extension.DDoveBoot          Core, DDoveRes, UniTask, YooAsset
DDoveFramework.Extension.DDoveUI            Core, DDoveRes, UniTask, YooAsset, UnityEngine.UI, Unity.TextMeshPro, Unity.InputSystem
DDoveFramework.Extension.DDoveUI.Editor     DDoveUI, DDoveFramework.Editor, Unity.InputSystem；Scriban 只进此 Editor
Game                                        Core, DDoveUI, UniTask, UnityEngine.UI, Unity.TextMeshPro；不引 Input System；图集见 [DDoveAtlas](/02-程序-前/DDoveAtlas.md)；PrimeTween 见 [PrimeTween](/02-程序-前/PrimeTween.md)
```

| 程序集 | 根命名空间 | 放哪 |
|--------|------------|------|
| `DDoveFramework.Extension.DDoveUI` | `DDoveFramework.Extension.DDoveUI` | `Extension/DDoveUI/` |
| `DDoveFramework.Extension.DDoveUI.Editor` | `DDoveFramework.Extension.DDoveUI.Editor` | `Extension/DDoveUI/Editor/`，`includePlatforms: Editor` |
| `Game` | `Game` | `Assets/Game/Game.asmdef` |

引用规则：

- 运行时 **不要**引用任何 `*.Editor`。
- `DDoveUI` **不要**引用 `DDoveBoot`。`Initialize` 在 [业务](#业务) 的 `GameLaunch` 里调（Launch 加载之后，Res 已就绪）。
- `DDoveUI` **不要**直打 `YooAssets` / `GetPackage`。持 `AssetHandle` 可以，Load / Release 只走 [DDoveRes](/02-程序-前/DDoveRes.md)。
- `DDoveUI` 引用 Input System 只给 UI EventSystem 用，**不要**引用 URP。**不要**引用 PrimeTween、不要做 ClickScale；业务在 `Game` 用，见 [PrimeTween](/02-程序-前/PrimeTween.md)。`Game` **不要**引用 Input System。
- `DDoveUI` **不要**引用 Atlas。换图与 late-bind 见 [DDoveAtlas](/02-程序-前/DDoveAtlas.md)。
- `DDoveUI.Editor` **不要**引用 `DDoveBoot`、`DDoveRes`、`Game`。Scriban 只开 Editor，见 [NuGet 与 Scriban](/02-程序-前/NuGet与Scriban.md)。
- `DDoveFramework.Editor` **不要**引用 UI（总门面已约定）。UI 页用 `[DDoveEditorPanel]` 自挂。`[DDoveHotboxEntry]` 属性放在门面程序集，编排走总窗 [DDove Editor](/02-程序-前/DDoveEditor.md) 的 **HotBox** 页。
- `Game` **不要**引用 `DDoveBoot`、`DDoveRes`、YooAsset、Editor。业务开面板只碰 Kit + Architecture。
- 业务脚本 **不要**进 `DDoveFramework`。

`DDoveBoot.InitializeExtensionsAsync` 保持空挂钩，本刀不用它来 Init UI。

## 配置

一份 SO：`DDoveUIInitInfo`，放 `Extension/DDoveUI/Resources/`，运行时 `Resources.Load`（名字 `DDoveUIInitInfo`）。**不是** `GameRes/` 里的资产。学 [DDoveResInitInfo](/02-程序-前/DDoveRes配置与使用.md)，不要再拆 EditorConfig + KitConfig。

总窗 **`DDove/Editor` → UI 页**改。默认值：

| 字段 | 默认 |
|------|------|
| 参考分辨率 | 1080×1920 |
| `matchWidthOrHeight` | 0.5 |
| LRU 容量 | 5（0 = 不缓存） |
| 制作场景根 | `Assets/GameResExcluded/CreateUIScenes` |
| Prefab 根 | `Assets/GameRes/UI` |
| 绑定代码 | `Assets/Game/Generate/UI` |
| 业务代码 | `Assets/Game/UI`（再按阶段分子目录） |
| 导出根节点名 | `UIRoot` |

创建制作场景与运行时 Scaler **都读这份 SO**。

## 垂直闭环

```
DDove/Editor · UI 页 · 制作
    创建 UI 场景
        GameResExcluded/CreateUIScenes/Start/WndHome.unity
    UIRoot 下摆控件 + NodeBind
    导出 UI
        Game/Generate/UI/WndHome.Generated.cs
        Game/Generate/UI/WndHome.IController.Generated.cs
        Game/UI/Start/WndHome.cs          （已有则不覆盖）
        GameRes/UI/Start/WndHome.prefab   （去掉 NodeBind）
Yoo Group Start  收 GameRes/UI/Start  tag: start  AddressByFileName
Play
    Boot → DDoveRes Init → LoadScene(Launch)
    GameLaunch → DDoveCfgKit.LoadAsync → GameArchitecture.Interface → 图集 Init 见 [DDoveAtlas](/02-程序-前/DDoveAtlas.md) → DDoveUIKit.Initialize → OpenAsync<WndHome>
    DDoveResKit.LoadAssetAsync<GameObject>("WndHome")
```

只导出 `UIRoot`。场景里的 Camera / Overlay Canvas / EventSystem / `Excluded_*` / `PanelDescription` 不进 Prefab。改布局回制作场景，不要直接改导出 Prefab。

制作场景 Canvas 用 Overlay；运行时 Kit 自建 ScreenSpaceCamera（Built-in，无 URP）。EventSystem 用 `InputSystemUIInputModule`，见 [DDoveUI 切 Input System](/02-程序-前/DDoveUI切InputSystem.md)。

场景名 = 类名 = Prefab 文件名 = Yoo location。

## 目录

| | 路径 | 进收集器 |
|--|------|----------|
| 开始阶段制作场景 | `GameResExcluded/CreateUIScenes/Start/` | 否 |
| 开始阶段 Prefab | `GameRes/UI/Start/` | 是，Group `Start` |
| 公用制作场景 | `GameResExcluded/CreateUIScenes/Common/` | 否 |
| 公用 Prefab / 图集 | `GameRes/UI/Common/`、`GameRes/Atlases/Common/` | **有资产再加组**，不要先扫空目录、不要扫整个 `UI/`。图集见 [DDoveAtlas](/02-程序-前/DDoveAtlas.md) |

`Start` = 开始阶段。`Common` = 多阶段共用，**不是**开始页。Login / Hall 有了再加组，见 [DDoveRes 按需加载](/02-程序-前/DDoveRes按需加载.md)。

## 运行时

`Extension/DDoveUI/`：`DDoveUIKit`、`DDoveUIPanelBase<T>`、`DDoveUIPopupPanelBase<T>`、`DDoveUINodeBind`（编辑期）、`DDoveUIPanelDescription`（编辑期）。不进 [IOC](/02-程序-前/IOC容器.md)，不进 [Core](/02-程序-前/DDoveFramework-Core.md)。异步 [UniTask](/02-程序-前/UniTask异步.md)。日志 [DDoveDebug](/02-程序-前/DDoveDebug.md)，`title` 固定 `DDoveUI`。

加载器写死接 `DDoveResKit.LoadAssetAsync`。关页 `Release` handle。Generated **不要** `k_PackageType`、不要 Builtin 降级。

UIRoot 可 `DontDestroyOnLoad`。Boot 自己仍不 DDOL，见 [DDoveBoot](/02-程序-前/DDoveBoot.md)。

层级沿用：Background / Normal / Bar / Popup / Top / System。

## 编辑器

`DDoveUI.Editor` 只挂 `[DDoveEditorPanel("ui", "UI", 200)]`。侧栏顺序见 [DDove Editor](/02-程序-前/DDoveEditor.md) 的 `DDoveEditorNav.Ids`。页内只改 `DDoveUIInitInfo`（分辨率、路径、LRU）。没有特殊原因都用 UIToolkit，见该篇「用哪套 UI」。

创建场景 / 绑定 / 导出 / 样板挂 `[DDoveHotboxEntry]`。属性在 `DDoveFramework.Editor`。Space 饼环在总窗 **HotBox** 页编排：给环起名，条目沿一圈散开；点中心切环。

不搬独立 UI EditorWindow、模块管理、扩展模板选择。Create 资产菜单可以留 `DDove/UI/Init Info`。

导出：Scriban 渲染绑定 + 业务初稿 → 编译后把字段绑到 `UIRoot` 上的面板组件 → `SaveAsPrefabAsset`。流程拆开（生成 / 绑定 / 存 Prefab），不要揉成一个上帝类。

## 业务

```
Assets/Game/
  Game.asmdef
  Generate/Core/GameArchitecture.Generated.cs   根类，不要手改
  Generate/UI/              绑定代码，不要手改
  Mono/GameLaunch.cs        挂 Launch 场景；目录约定见 [UI 业务封装](/02-程序-前/UI业务封装.md)
  UI/Start/WndHome.cs       业务，只写一次
```

`GameArchitecture` 由 [Architecture 自动注册](/02-程序-前/Architecture自动注册.md) 生成，见 [Architecture 与角色](/02-程序-前/Architecture与角色.md)。面板 `IController`，`GetArchitecture()` 返回 `GameArchitecture.Interface`。本库没有 HybridCLR，**不要**造 `HotUpdate/`。

`GameLaunch`：先 `DDoveCfgKit.LoadAsync`，再碰 `Interface`（触发 Init）→ 图集 `Initialize` 见 [DDoveAtlas](/02-程序-前/DDoveAtlas.md) → `DDoveUIKit.Initialize` → `OpenAsync<WndHome>`。正式游戏换成自己的入口，删或改这一份占位。见 [DDoveCfg](/02-程序-前/DDoveCfg.md)。第二扇窗与读写门槛见 [UI 业务封装](/02-程序-前/UI业务封装.md)：业务继续直调 Kit，不要为开窗加只转发的 System。

## 不要

- Boot 引用 UI，或在 Boot 场景挂业务面板
- 一个 Editor asmdef 引用所有 Kit
- Builtin + Remote 双包裹、双导出路径
- 一条收集器扫整个 `GameRes/UI`
- 业务脚本进框架程序集
- 运行时引用 Scriban / Editor
- 在制作场景 Prefab 上继续改，却不回场景重导
- 为开窗加只转发 `DDoveUIKit` 的 System（`UISystem`）

## 还没有（本刀之后）

按 tag 预下、Host/Web、玩法键盘/手柄 Actions、OSA、多人。图集 Kit 已落地，见 [DDoveAtlas](/02-程序-前/DDoveAtlas.md)。未实现前不要把未落地 API 当已有方法。

## 验收

1. 没装 `DDoveUI` 时 Core / Res / Boot / `DDove/Editor` 仍能编。
2. 总窗侧栏是 HotBox / Architecture / Res / UI（见 [DDove Editor](/02-程序-前/DDoveEditor.md) `DDoveEditorNav.Ids`）。UI 页改分辨率写进 `DDoveUIInitInfo`。饼环在 HotBox 页编。
3. 能建 `Start/WndHome` 制作场景，导出 Prefab 与生成代码；业务 `.cs` 第二次导出不覆盖。
4. 收集器 Group `Start` 能 `LoadAssetAsync("WndHome")`。
5. Play：Boot → Launch → 看见 `WndHome`，只开 Input System Package 时能点。输入细则见 [DDoveUI 切 Input System](/02-程序-前/DDoveUI切InputSystem.md)。
