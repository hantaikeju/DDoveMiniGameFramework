---
type: Playbook
title: DDoveCfg
description: 第一刀接入 Luban。填表工程与生成器在 DDoveMiniGameConfig；客户端 Kit 用 Res 读表，Launch 先 Load 再进 Architecture。
tags: [程序-前, ddovecfg, luban]
status: stable
generated: { by: human:cjh, at: 2026-09-12T08:20:00Z }
verified: { by: human:cjh, at: 2026-09-15T03:20:00Z }
sources:
  - id: grill
    resource: /00-索引/Agent/需求稿工作流.md
    title: 需求稿工作流
  - id: res
    resource: /02-程序-前/DDoveRes.md
    title: DDoveRes
  - id: res-use
    resource: /02-程序-前/DDoveRes配置与使用.md
    title: DDoveRes 配置与使用
  - id: boot
    resource: /02-程序-前/DDoveBoot.md
    title: DDoveBoot
  - id: ui
    resource: /02-程序-前/DDoveUI.md
    title: DDoveUI
  - id: arch
    resource: /02-程序-前/Architecture与角色.md
    title: Architecture 与角色
  - id: arch-auto
    resource: /02-程序-前/Architecture自动注册.md
    title: Architecture 自动注册
  - id: editor
    resource: /02-程序-前/DDoveEditor.md
    title: DDove Editor
  - id: core
    resource: /02-程序-前/DDoveFramework-Core.md
    title: DDoveFramework Core
  - id: upm
    resource: /02-程序-前/UPM落地.md
    title: UPM 落地
  - id: fill
    resource: /01-策划/Luban填表.md
    title: Luban 填表
  - id: srv
    resource: /03-程序-后/Luban服务端.md
    title: Luban 服务端
  - id: luban
    resource: https://github.com/focus-creative-games/luban
    title: focus-creative-games/luban
  - id: luban-unity
    resource: https://github.com/focus-creative-games/luban_unity
    title: luban_unity
  - id: luban-ai
    resource: https://github.com/focus-creative-games/luban/tree/main/ai
    title: luban/ai
  - id: cfg-skill
    resource: /00-索引/Agent/ddove-config.md
    title: ddove-config
---

# DDoveCfg

Concept ID：`/02-程序-前/DDoveCfg`。清单：[index_cjh](/02-程序-前/index_cjh.md)。第一刀已落地。结论与代码冲突以代码为准。

填表看 [Luban 填表](/01-策划/Luban填表.md)。Agent 改表走 [ddove-config](/00-索引/Agent/ddove-config.md)。服务端 target 看 [Luban 服务端](/03-程序-后/Luban服务端.md)。对照：[DDoveRes](/02-程序-前/DDoveRes.md)、[DDoveUI](/02-程序-前/DDoveUI.md)、[Architecture 自动注册](/02-程序-前/Architecture自动注册.md)、[UPM 落地](/02-程序-前/UPM落地.md)。

官方：[luban](https://github.com/focus-creative-games/luban) 是生成器源码仓，**不要**整树嵌进 Unity。[luban_unity](https://github.com/focus-creative-games/luban_unity) 才是运行时 UPM。

## 要什么

第一刀跑通：`DDoveMiniGameConfig`（表 + 生成器）→ 客户端生成代码/数据 → Launch 异步装表 → 业务从 Utility 读一张表示例。

不做 Click 业务表、不做发布 bin、不建 server 工程。

## 仓库落点

| 路径 | 放什么 |
|------|--------|
| `DDoveMiniGameConfig/` | 共用配表工程：`Data/`（本刀 csv，UTF-8 BOM）、`Defines/builtin.xml`（vec2/3/4）、`luban.conf`、`gen_client.bat` / `.sh`、`Tools/Luban/`（Release **v5.1.0**）。本机若只有 .NET 10，脚本里 `DOTNET_ROLL_FORWARD=LatestMajor` |
| `DDoveMiniGameTools/` | wiki 检查 / CI。**不要**把导表脚本塞进来 |
| `DDoveMiniGameClient/Packages/com.code-philosophy.luban@1.2.0/` | Runtime（asmdef `Luban.Runtime`），按 [UPM 落地](/02-程序-前/UPM落地.md)：manifest 留 Git URL，再拷本地夹 |
| `Assets/DDoveFramework/Extension/DDoveCfg/` | Kit。**没有** Editor 面板 |
| `Assets/Game/Generate/Luban/` | **只**生成代码。导表会清空此目录 |
| `Assets/GameRes/Cfg/` | **只**生成数据（本刀 json）。进 Yoo 收集器 |

`luban.conf` 一开始就留 `client` / `server` 两组。本刀只跑 `gen_client`。

## 程序集

[DDoveFramework Core](/02-程序-前/DDoveFramework-Core.md) `references` 空，**零引用** Luban / 生成表类型 / json 库。

```
DDoveFramework.Extension.DDoveCfg           Core, DDoveRes, UniTask, YooAsset, Luban.Runtime
Game                                        现有引用 + DDoveCfg + Luban.Runtime
```

| 规则 | |
|------|--|
| Kit | 只用 [DDoveRes](/02-程序-前/DDoveRes.md) 按表名 location 读文件，组 `Tables` 的 loader。不进 [IOC](/02-程序-前/IOC容器.md) |
| 生成代码 | `Assets/Game/Generate/Luban/`，跟 UI 绑定一样挂在 `Game` |
| 业务入口 | `Game` 里 `[DDoveBindUtility]` 转发表（如 `CfgUtility`）。业务 `GetUtility` 读表 |
| Boot | **不**引用 Cfg。`InitializeExtensionsAsync` 保持空挂钩，同学 [DDoveUI](/02-程序-前/DDoveUI.md) |
| Game | 仍 **不**引用 `DDoveBoot`、`DDoveRes`、YooAsset |

日志 [DDoveDebug](/02-程序-前/DDoveDebug.md)，`title` 固定 `DDoveCfg`。异步 [UniTask](/02-程序-前/UniTask异步.md)。

## 启动顺序

[Architecture.Init](/02-程序-前/Architecture与角色.md) 是同步的；Yoo 读资产是异步。不能在 Utility 构造或 `Model.OnInit` 里第一次 Load。

```
Boot → DDoveResKit.InitializeAsync → LoadScene(Launch)
GameLaunch
    await DDoveCfgKit.LoadAsync()     失败则 return，不碰 Interface
    GameArchitecture.Interface        此时 Model.OnInit 能读表
    DDoveUIKit.Initialize
    OpenAsync<WndHome>
```

改 [GameLaunch](../../DDoveMiniGameClient/Assets/Game/Launch/GameLaunch.cs)：Load 必须在碰 `Interface` **之前**。不要手写 `GameArchitecture.Init`。

## 数据与格式

生成数据进 `Assets/GameRes/Cfg/`，Yoo **一个** `DefaultPackage`，单独 Collector / tag（如 `cfg`），`AddressByFileName`。location = 表文件名（与 Luban 导出文件名对齐）。

本刀 `cs-simple-json` + `json`。loader 只认表名 location，不写死扩展名以外的目录约定。

发布改 `cs-bin` + `bin`。json **不当**小游戏包体方案：示例表撑得住，正式大表体积和解析 GC 会先炸。换格式只换生成参数。

Res：已有 `LoadAssetAsync<T>` 能读收集器里的 `TextAsset` 就用，不必为本刀再造专用方法。`bytes` / RawFile 留给 bin 那一刀。未实现前不要把 [按需加载](/02-程序-前/DDoveRes按需加载.md) 里的预下 API 当已有方法。

## 导表

在资源管理器双击 `DDoveMiniGameConfig/gen_client.bat`（或跑 `gen_client.sh`）。路径按脚本自己的位置算，不靠当前工作目录。切回 Unity 会 Refresh 生成物。

**不要**在总窗、菜单、`Process` 里调 bat。总窗不设 Cfg 页。

官方会清空 `outputCodeDir` / `outputDataDir`。必须单独目录，不能指到 `Assets/Game` 或 `Assets/GameRes` 根。

## 不要

- 把 [luban](https://github.com/focus-creative-games/luban) 源码树 submodule / 拷进 `Assets/` 或 `Packages/`
- 把导表脚本塞进 `DDoveMiniGameTools/`（wiki 检查目录）
- 生成代码进 Core / Extension；每次导表改框架程序集
- Game 直接 `DDoveResKit` / `YooAssets` 读表
- Boot 引用 Cfg，或在 Boot 场景装表
- 先碰 `Interface` 再 Load
- `Resources` / 直接读 `StreamingAssets` 当本刀方案
- 把 json 写成正式包格式
- 本刀建 `DDoveMiniGameServer` 或跑 `gen_server` 当完成条件
- 在 Unity 里 `Process` 跑 `gen_client`，或再开总窗 Cfg 页
- 本刀引进 `Luban.Agent` / MCP，或把官方 `luban-*` 技能原样拷进仓库

## 还没有（本刀之后）

`cs-bin`、Host / Web 下的表预下、多语言、服务端工程。未实现前不要把官方全目标当已有方法。

## 验收

1. 没装 `DDoveCfg` 时 Core / Res / Boot / `DDove/Editor` 仍能编。
2. 双击 `DDoveMiniGameConfig/gen_client.bat` 能生成；`Generate/Luban` 与 `GameRes/Cfg` 出现文件；两目录外的脚本不被清空。
3. Yoo 收集器能打到导出 json；Play：Launch 里 Load 成功后再进 Architecture。
4. 示例表 `Get` 有值。Load 失败打 `DDoveCfg` 错误，不碰 `Interface`、不开 Home。
5. Core / Boot asmdef 没有 Luban / Cfg。`Game` 没有 `DDoveRes` / YooAsset。
6. 总窗没有 Cfg 页，没有导表按钮。
