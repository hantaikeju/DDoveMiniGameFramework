---
type: Playbook
title: LoopList
description: 已按稿落地。竖、横样板格子进视口时播入场并铺平；离开视口不播退场，滑出遮罩即可。翻页写在 Anim 上。不再做中心缩放，不再复制离场影子。Home 不挂。LoopList 不引用 PrimeTween。格子点击走 AddClick，下标在点击当时读；非空声音名经 ClickSound.SetSoundName。
tags: [程序-前, ddoveui]
status: stable
generated: { by: human:cjh, at: 2026-09-18T13:54:00Z }
verified: { by: human:cjh, at: 2026-09-25T19:11:00Z }
sources:
  - id: grill
    resource: /00-索引/Agent/需求稿工作流.md
    title: 需求稿工作流
  - id: ui
    resource: /02-程序-前/DDoveUI.md
    title: DDoveUI
  - id: ui-biz
    resource: /02-程序-前/UI业务封装.md
    title: UI 业务封装
  - id: input
    resource: /02-程序-前/DDoveUI切InputSystem.md
    title: DDoveUI 切 Input System
  - id: tween
    resource: /02-程序-前/PrimeTween.md
    title: PrimeTween
  - id: pool
    resource: /02-程序-前/DDovePool.md
    title: DDovePool
  - id: path
    resource: /02-程序-前/正确路径与程序集方向.md
    title: 正确路径与程序集方向
  - id: home
    resource: /02-程序-前/WndHomeSample入口.md
    title: WndHome Sample入口
  - id: bind
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/DDoveUINodeBind.cs
    title: DDoveUINodeBind.cs
  - id: game-asmdef
    resource: ../../DDoveMiniGameClient/Assets/Game/Game.asmdef
    title: Game.asmdef
  - id: loop-src
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/Scroll/LoopList.cs
    title: LoopList.cs
  - id: loop-layout
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/Scroll/LoopList.Layout.cs
    title: LoopList.Layout.cs
  - id: loop-pool
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/Scroll/LoopList.Pool.cs
    title: LoopList.Pool.cs
  - id: loop-preview
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/Scroll/LoopList.Preview.cs
    title: LoopList.Preview.cs
  - id: loop-item
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/Scroll/ILoopItem.cs
    title: ILoopItem.cs
  - id: motion
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/Scroll/ScrollItemMotion.cs
    title: ScrollItemMotion.cs
  - id: scroll-tween
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/Scroll/Tween/DefaultTween_V.cs
    title: DefaultTween_V.cs
  - id: tween-h
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/Scroll/Tween/DefaultTween_H.cs
    title: DefaultTween_H.cs
  - id: demo-wnd
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/WndLoopDemo.cs
    title: WndLoopDemo.cs
  - id: demo-row
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/DemoRow.cs
    title: DemoRow.cs
  - id: click-sound
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/Sound/ClickSound.cs
    title: ClickSound.cs
  - id: demo-menu
    resource: ../../DDoveMiniGameClient/Assets/Game/Editor/OpenLoopDemoMenu.cs
    title: OpenLoopDemoMenu.cs
  - id: h-wnd
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/WndLoopHDemo.cs
    title: WndLoopHDemo.cs
  - id: h-menu
    resource: ../../DDoveMiniGameClient/Assets/Game/Editor/OpenLoopHDemoMenu.cs
    title: OpenLoopHDemoMenu.cs
  - id: home-wnd
    resource: ../../DDoveMiniGameClient/Assets/Game/UI/Start/WndHome.cs
    title: WndHome.cs
  - id: exporter
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/Editor/DDoveUIPrefabExporter.cs
    title: DDoveUIPrefabExporter.cs
---

# LoopList

Concept ID：`/02-程序-前/LoopList`。清单：[index_cjh](/02-程序-前/index_cjh.md)。制作导出仍看 [DDoveUI](/02-程序-前/DDoveUI.md)。挂件目录看 [UI 业务封装](/02-程序-前/UI业务封装.md)。输入看 [DDoveUI 切 Input System](/02-程序-前/DDoveUI切InputSystem.md)。入口行数看 [WndHome Sample入口](/02-程序-前/WndHomeSample入口.md)。

对照：[PrimeTween](/02-程序-前/PrimeTween.md)、[DDovePool](/02-程序-前/DDovePool.md)、[正确路径与程序集方向](/02-程序-前/正确路径与程序集方向.md)。仍过期：[DDoveUI](/02-程序-前/DDoveUI.md)「还没有」仍写 OSA，且只点定高垂直；[UI 业务封装](/02-程序-前/UI业务封装.md) 入场仍写成面板 tween，未点名 `WndLoopDemo`；[WndHome Sample入口](/02-程序-前/WndHomeSample入口.md) 仍写两行、且不改本内核；[PrimeTween](/02-程序-前/PrimeTween.md) 未写格子进退场。

权威实现：[LoopList.cs](../../DDoveMiniGameClient/Assets/Game/Mono/Scroll/LoopList.cs)。结论与代码冲突以当前代码为准。

## 要什么

同一 `LoopList` 已有方向：`LoopDirection.Vertical`（定高，枚举 0）或 `Horizontal`（定宽，枚举 1）。默认 `Vertical`，已有竖 Prefab 不改轴。一次只开一轴。`itemSize` 竖为条高、横为条宽。

制作场景能调 `itemSize` / 间距 / 预览条数并看见排法。Play 里头出尾进，上百条不再按条 `Instantiate`。窗只 `Create<T>(IList<T>)`；模板 `ILoopItem<T>`；`GetData` / `GetShown`。`ScrollTo`：竖 `Top` / `Middle` / `Bottom`，横 `Left` / `Middle` / `Right`，立刻贴齐。传了另一轴的对齐值时按 `Middle`。

横样板是新窗 `WndLoopHDemo`。竖样板 `WndLoopDemo` 留着。`WndHome` 多一行，标题 `LoopH`，`NavigateToAsync` 打开横窗。横窗 `BtnBack` → `BackAsync`。

面板 `Hide` 不再 `DropSource`。`IList` 还在。再次启用时用这份引用把格子刷回来，主轴滚动位置停在离开前。`OnDestroy` 仍 `DropSource`。窗 `OnClose` 仍丢掉自己的 list。

格子进出视口时播样式。公共进出流程在 `Scroll/ScrollItemMotion`：只播入场：露出约两成从入场姿态播到铺平，播完就停。离开视口不播退场，格子滑出遮罩；完全离开后再回到入场起点，下次进来重新播。滑动过程中不再按露出比例改姿态。竖列表翻页和横列表下插走同一条。竖列表用 `DefaultTween_V` 翻页：绕 Y 轴从 `-50°` 转到 `0°`，并沿交叉轴向正方向挪约视口宽的 `100/420`，深度约 `-100/420`，透明度 0 到 1。横列表不用这套翻页，改挂 `DefaultTween_H`，从上方插入约条高的 `0.55` 再落到原位，不转轴。时长 `0.7` 秒，`Ease.OutCubic`。样式仍实现进场、离场、铺平；驱动只调用进场。完全离开视口时把 `Anim` 放回入场起点。`IScrollItemTween.ApplyPage(Vector3 viewCenterWorld, bool horizontal)` 在每次滚动刷新时对当前格子调用。没有这个接口的格子只滑动。`StopMotion()` 把 `Anim` 转回铺平。列表另有 `HorizontalMotion`、`MotionItemSize`、`MotionStride`。

位移和旋转都写在格子根下的 `Anim` 上。根的排版位置和缩放不动。

位移和旋转都写在格子根下名为 `Anim` 的内容节点上，节点带 `CanvasGroup`。不改格子根的 `anchoredPosition`，根的 `localScale` 保持 1。列表排版仍只写根。已经铺平的格子，格内滑动时不重播翻页。

可复用流程是 `Scroll/ScrollItemMotion`。竖样板挂 `DefaultTween_V`，横样板挂 `DefaultTween_H`。接口文件和 `LoopList` 不引用 PrimeTween。不再按视口中心写缩放，也不再复制离场影子。

[`DemoRow`](../../DDoveMiniGameClient/Assets/Game/UI/Start/DemoRow.cs) 去掉 `LateUpdate` 里按距离手算缩放。竖、横样板的格子模板把可视内容放进 `Anim`。竖列表根上挂 `DefaultTween_V`，横列表根上挂 `DefaultTween_H`。两个 Build 菜单在挂 `DemoRow` 时一并包好。`HomeEntryRow` 不挂，缩放保持 1。

同一个 `partial class LoopList` 已拆成同目录四份文件。主文件留对外入口和生命周期。排版、回收、编辑器预览各一份。`DataSource` 仍是私有嵌套类。拆分不改 `Create`、夹紧、`Hide` 留数据。`IScrollItemTween` 与 `ScrollItemMotion` 只播入场。竖挂 `DefaultTween_V`，横挂 `DefaultTween_H`。样板格子根下有 `Anim`。不另开列表类型，不另开 `LoopItem` 基类。

不另开横挂件。不搬 [DDoveUI](/02-程序-前/DDoveUI.md) 清单里的 OSA。不进 DDoveUI Base。不进 IOC。不生成每张表一份 Adapter。对外没有 `SetCount` / `BindItem`。网格、可变高、字体本刀不做。

## 落点

| | 放哪 |
|--|------|
| 列表挂件 | `Assets/Game/Mono/Scroll/`，`namespace Game.Mono`。同一个 `partial class LoopList`，组件不泛型。序列化方向默认 Vertical，字段留在主文件。公开读法 `HorizontalMotion`、`MotionItemSize`、`MotionStride` |
| 格子接口 | 同目录 [`ILoopItem.cs`](../../DDoveMiniGameClient/Assets/Game/Mono/Scroll/ILoopItem.cs)：`ILoopItem<T>`（`Bind(T, int)` / `Recycle()`）。`LoopAlign` 保留 `Top` / `Middle` / `Bottom`，加上 `Left` / `Right`。另有 `IScrollItemTween`：`ApplyPage` / `StopMotion` |
| 入场样式 | `Scroll/ScrollItemMotion` 只播入场，放在 Tween 外面。`Scroll/Tween` 里只有样式：竖 `DefaultTween_V`、横 `DefaultTween_H`。动 `Anim`。模板内部用 PrimeTween。样式里的 `ApplyLeave` 驱动不调用。不新建 `LoopItem` 基类 |
| 竖样板 | Start 阶段 `WndLoopDemo`，方向 Vertical。不改成横列表 |
| 横样板 | Start 阶段 `WndLoopHDemo`。`BtnBack` + 一条 Horizontal `LoopList`。格子可复用 `DemoRow` / `DemoRowData` |
| 入口 | [`WndHome`](../../DDoveMiniGameClient/Assets/Game/UI/Start/WndHome.cs) 在现有两行后再加一行：`Title = "LoopH"`，打开 `WndLoopHDemo`。不改 `GameLaunch` |
| 打开 / 构建 | 菜单 `Game/Open WndLoopHDemo`（仅 Play）。构建：`Game/Build WndLoopHDemo Assets`。竖菜单保留 |
| Inspector | 组件序列化字段。无自定义 Editor 窗，无 `DDove/Editor` 侧栏页 |

四份文件：

| 文件 | 放什么 |
|------|--------|
| `LoopList.cs` | `Create` / `AddClick` / `GetData` / `GetShown` / `ScrollTo` / `Refresh`、开关、`DataSource`、序列化字段 |
| `LoopList.Layout.cs` | 方向、padding、Content 尺寸、格子坐标、轴锁与夹紧 |
| `LoopList.Pool.cs` | 格子池、头出尾进、`RefreshVisible`、每次滚动调用 `ApplyPage` |
| `LoopList.Preview.cs` | 仅 `#if UNITY_EDITOR` 的占位预览 |

不挂 `[DDoveBind*]`。`Game.asmdef` **不要**加 `Unity.InputSystem`。`Game.Editor` 可引用 `DDoveUI.Editor`，用来建场景 / 导出。

## 制作场景

`ScrollRect` + Viewport + Content。Content 下**一份**失活 `itemTemplate`。不要 `VerticalLayoutGroup` / `HorizontalLayoutGroup` / `ContentSizeFitter` 排格子。

Inspector：方向、`itemSize`、`spacing`、`padding`、`previewCount`。改完 Scene 立刻按该方向排出占位条。预览节点不得写入场景、不得进导出 Prefab（`HideAndDontSave`，或导出前删掉）。

竖：Content 顶对齐，条向下排。横：Content 左对齐，条向右排。`ScrollRect` 只开对应轴。

NodeBind 只绑列表根 `RectTransform`。样板用 `GetComponentInChildren<LoopList>()`。本刀不扩 `DDoveUINodeBindType`。

导出：[DDoveUIPrefabExporter](../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveUI/Editor/DDoveUIPrefabExporter.cs) 在存 Prefab 前后剥 `DontSave` / `HideAndDontSave` 子孙，再剥 NodeBind。竖：`Game/Build WndLoopDemo Assets`。横：`Game/Build WndLoopHDemo Assets`。改布局回制作场景。`Start` 收集器 AddressByFileName 能 Load `WndLoopHDemo`。

## 运行时

保留 Unity `ScrollRect`。自己算 Content 主轴长度和每条 `anchoredPosition`。`BindHierarchy` / `Create` 按方向锁轴：竖 `horizontal=false`、`vertical=true`；横相反。另一轴位置归零。

回收：主轴头出尾进。竖是上 1 下 1；横是左 1 右 1。`start` 比首条可见再让 1 格，夹在 `[0, count - 格子数]`。滑 1 格只 Recycle/Bind 滑出那条。跳过超过 1 格则整表重绑。到头没有 -1；到尾没有 `count`。条数 < 池则按条数造。`OnDisable` 退订并回收格子，**不** `DropSource`。`OnDestroy` 仍 `DropSource`，只丢引用。再次启用且引用还在时，按当前 `content` 位置重刷可见格，不把主轴滚回 0。`ScrollRect.movementType` 为夹紧，滑到头停在最后一条，不把底拖出来。

组件不泛型。样板：

```csharp
_bags = new List<DemoRowData>(80);
_list.Create(_bags);
_list.AddClick(index => Debug.Log("[WndLoopDemo] row " + index));
```

`Create<T>(IList<T> data)` 只借引用，包在内部 `DataSource<T>`，不 `new T`，不 `Clear` / `Dispose`。Query 的 list 由窗 `OnClose` 丢掉（样板 `_bags = null`）。改了 IList 不监听，再 `Create` 或 `Refresh()`。

`GetData<T>(i)` 回 `T`（`Create` 的 T 对不上则 `default`）。`GetShown(i)` 回 `RectTransform`；`GetShown<T>(i)` 回 `ILoopItem<T>`。不在当前格子表则 `null`，不现造。

`ScrollTo(int index, LoopAlign align)` 立刻改主轴 `content.anchoredPosition`，夹在可滚区间，`StopMovement`，再刷可见格。不播滚动动画。`LoopList` **不**引用 PrimeTween。

内部：`ILoopItem<T>.Bind(data[i], i)`。没有 `BindItem` / `SetCount`。当前格子若有 `IScrollItemTween`，每次滚动刷新调用 `ApplyPage`。没有接口则跳过。不再按距离调用缩放。

`DemoRow` 在 `Awake` 只缓存 TMP。`Bind` 只写字，不再自己挂 `onClick`。距离、缩放和入场不在 `DemoRow` 里算。

不要用 [DDovePool](/02-程序-前/DDovePool.md) 回收格子。

## 输入

指针与滚轮：现有 `InputSystemUIInputModule`，列表不读 InputAction。格子上的 `Selectable.navigation = None`。

点击：格子根 `Button` 走 `LoopList.AddClick(Action<int> onClick, string soundName = null)`。下标在点击当时读这次 `Bind` 写下的值。再调一次替掉上一次的动作和记下的声音名。非空声音名对露出的格子保证一份 `ClickSound`，经 `SetSoundName` 写入；播放仍在 `ClickSound`，列表不调用 `PlaySound`。声音名为空则不加、也不改已有的那份。没有根上 `Button` 的格子不挂这次点击。竖样板在 `Create` 之后调用，不传声音名。不要改 [UI 业务封装](/02-程序-前/UI业务封装.md) 的面板 `AddClick`。不新增 `LoopItem` 基类。

## Tween

入场由 `ScrollItemMotion` 在露出约两成时，用 PrimeTween 从入场姿态播到铺平，时长 `0.7` 秒，`Ease.OutCubic`。已经铺平的格子不再重播。完全离开视口后把 `Anim` 放回入场起点，不播退场。样式仍实现 `ApplyLeave`，驱动不调用。样板 `OnClose` 仍可调无参 `Tween.StopAll()` 停掉本页其它 tween。列表在 `OnDisable` / `OnDestroy` 调用 `StopMotion`，并清掉名字以 `ScrollExitGhost` 开头的残留。`LoopList` 与 `IScrollItemTween` 不引用 PrimeTween。其它面板的 tween 仍看 [PrimeTween](/02-程序-前/PrimeTween.md)。

## 不要

- 另开横列表挂件；一个 `LoopList` 同时开横竖两轴
- 列表进 [DDoveUI](/02-程序-前/DDoveUI.md) Base，或拆掉 `ScrollRect`
- `Game` 引 `Unity.InputSystem`；手柄逐条、`EnterList`、玩法键盘 Actions
- 每表生成 Adapter / ViewsHolder；开 `Mono/UI/`；空的 `Mono/Tween/`
- 流动 / 分页 / 网格 / 可变高 / 多模板混排
- 把 `WndLoopDemo` 改成横的，或删掉竖样板
- 预览克隆进导出 Prefab
- 包 `ITween`，或列表内核播动画、播 `ScrollTo` tween
- 列表 `new T` / `Clear` / `Dispose` 外来 `IList`
- 监听 IList 自动刷
- 对外 `SetCount` / `BindItem`
- 不在缓冲的 index 现造格子给 `GetShown`
- 本刀补中文字体、拷外部字体
- `Hide` / `OnDisable` 时 `DropSource`
- 给 `HomeEntryRow` 做入场或中心缩放
- `DemoRow` 在 `LateUpdate` 里自己按距离写缩放
- `LoopList` 或 `ILoopItem.cs` 引用 PrimeTween
- 再按视口中心写 1 / 0.85 缩放
- 把入场位移写在格子根上，和排版抢 `anchoredPosition`
- 再复制离场影子，或给离开视口的格子播退场
- 已经铺平的格子在格内滑动时重播入场
- 为样式再公开一个 `LoopItem` 基类
- `Show` 时把主轴滚动位置拉回 0
- 拆文件时改回收、夹紧、`Hide` 留数据或对外签名
- 为拆分再公开一个列表类型；把 partial 放到 `Scroll/` 以外

## 还没有

网格、可变高、NodeBind 认 `ScrollRect`、手柄进列表。未实现前不要当已有方法。

## 验收

1. `LoopList` 在 `Game/Mono/Scroll/`，`namespace Game.Mono`。没有进 DDoveUI、没有进 `GameArchitecture`。没有第二个横列表类型。
2. `Game.asmdef` 不含 `Unity.InputSystem`。
3. 方向默认 Vertical。制作场景改方向 / `itemSize` / `spacing` / `previewCount`，Scene 按该轴看见占位；导出 Prefab 只有失活模板，没有预览克隆。
4. `WndLoopDemo` 仍是竖列表。另有 `WndLoopHDemo`：Horizontal、`Create<T>`、`BtnBack` → `BackAsync`。没有 `SetCount` / `BindItem`。
5. `WndHome` 在原两行后再有一行 `LoopH`，`NavigateToAsync<WndLoopHDemo>`。`GameLaunch` 不改。`Start` 组能 Load `WndLoopHDemo`。
6. 模板 `ILoopItem<T>`；`GetData` 能取 T；`GetShown` 仅对缓冲内格子有值。
7. 竖 `ScrollTo` 用 `Top` / `Middle` / `Bottom`；横用 `Left` / `Middle` / `Right`。立即生效，到头到尾夹紧。另一轴的对齐值按 `Middle`。
8. 竖滑动上 1 下 1；横滑动左 1 右 1。条数远大于可见数时不再 `Instantiate`。运行时只开对应 `ScrollRect` 轴。
9. 格子 `navigation` 为 `None`。`DemoRow` 不在 `LateUpdate` 里按距离写缩放。格子根缩放保持 1。
10. 没装这份挂件时，Core / Res / Boot / `DDove/Editor` 仍能编。
11. 从样板 `BackAsync` 回 `WndHome`，入口行还在，主轴滚动位置与离开前相同。再打开竖列表或 `WndLoopHDemo`，条和滚动位置也还在。`OnDestroy` 仍丢掉引用。
12. 竖、横样板里，格子进入视口时播入场并铺平，离开视口不播退场。已经铺平的格子在拖动过程中不再改转角。`WndHome` 入口行缩放保持 1，没有翻页。
13. 仍是一个 `LoopList`。四份 partial 都在 `Game/Mono/Scroll/`：主文件、`LoopList.Layout.cs`、`LoopList.Pool.cs`、`LoopList.Preview.cs`。没有新的公开列表类型。`Create`、夹紧、`Hide` 留数据与拆分前相同。
14. `IScrollItemTween` 与 `ScrollItemMotion` 在 `Game/Mono/Scroll/`。`Scroll/Tween` 里只有默认样式 `DefaultTween_V` 与 `DefaultTween_H`。竖样板翻页，横样板从上方插入。`HomeEntryRow` 没有该接口。`LoopList` 与接口文件不引用 PrimeTween。
