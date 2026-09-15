---
type: Playbook
title: DDoveSave
description: 第一刀本地档已落地。Kit 只做 KV 文件；档名业务传入；按接口挂 Utility。Launch 先 LoadFile 再碰 Architecture。
tags: [程序-前, ddovesave]
status: stable
generated: { by: human:cjh, at: 2026-09-15T10:22:00Z }
verified: { by: human:cjh, at: 2026-09-15T10:29:00Z }
sources:
  - id: grill
    resource: /00-索引/Agent/需求稿工作流.md
    title: 需求稿工作流
  - id: core
    resource: /02-程序-前/DDoveFramework-Core.md
    title: DDoveFramework Core
  - id: arch
    resource: /02-程序-前/Architecture与角色.md
    title: Architecture 与角色
  - id: arch-auto
    resource: /02-程序-前/Architecture自动注册.md
    title: Architecture 自动注册
  - id: ioc
    resource: /02-程序-前/IOC容器.md
    title: IOC 容器
  - id: ioc-use
    resource: /02-程序-前/IOC容器使用规范.md
    title: IOC 容器使用规范
  - id: cfg
    resource: /02-程序-前/DDoveCfg.md
    title: DDoveCfg
  - id: nuget
    resource: /02-程序-前/NuGet与Scriban.md
    title: NuGet 与 Scriban
  - id: kit
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveSave/DDoveSaveKit.cs
    title: DDoveSaveKit.cs
  - id: save-asmdef
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveSave/DDoveFramework.Extension.DDoveSave.asmdef
    title: DDoveSave asmdef
  - id: isave
    resource: ../../DDoveMiniGameClient/Assets/Game/Utility/IGameSave.cs
    title: IGameSave.cs
  - id: save-util
    resource: ../../DDoveMiniGameClient/Assets/Game/Utility/GameSaveUtility.cs
    title: GameSaveUtility.cs
  - id: launch
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/GameLaunch.cs
    title: GameLaunch.cs
  - id: generated
    resource: ../../DDoveMiniGameClient/Assets/Game/Generate/Core/GameArchitecture.Generated.cs
    title: GameArchitecture.Generated.cs
  - id: packages
    resource: ../../DDoveMiniGameClient/Assets/packages.config
    title: Assets/packages.config
  - id: msgpack
    resource: https://github.com/MessagePack-CSharp/MessagePack-CSharp
    title: MessagePack-CSharp
---

# DDoveSave

Concept ID：`/02-程序-前/DDoveSave`。清单：[index_cjh](/02-程序-前/index_cjh.md)。第一刀已落地。结论与代码冲突以代码为准。

对照：[DDoveFramework Core](/02-程序-前/DDoveFramework-Core.md)、[Architecture 与角色](/02-程序-前/Architecture与角色.md)、[Architecture 自动注册](/02-程序-前/Architecture自动注册.md)、[IOC 容器](/02-程序-前/IOC容器.md)、[IOC 容器使用规范](/02-程序-前/IOC容器使用规范.md)、[DDoveCfg](/02-程序-前/DDoveCfg.md)。序列化库已在客户端 NuGet：MessagePack **3.1.8**，见 [NuGet 与 Scriban](/02-程序-前/NuGet与Scriban.md)。

## 要什么

第一刀已跑通一条本地档：

- `Extension/DDoveSave` 静态 Kit：内存 KV + MessagePack + `persistentDataPath` 整文件
- `Game` 里 `IGameSave` + `GameSaveUtility`，按接口挂进 `GameArchitecture`
- 档名**只**由业务传入（openid 到了换传入的全名即可）。Kit **不**自编默认档名
- 每次 `SaveData` 立刻整文件落盘
- [GameLaunch](../../DDoveMiniGameClient/Assets/Game/Mono/GameLaunch.cs) 先传占位全名（如 `local_save.bin`）`LoadFile`，再碰 `GameArchitecture.Interface`

本刀**不**改 `PlayerModel`（demo 次数仍可 `PlayerPrefs`）。音频开关仍走设置侧 `PlayerPrefs`，不进这份档。

## 仓库落点

| 路径 | 放什么 |
|------|--------|
| `Assets/DDoveFramework/Extension/DDoveSave/` | Kit。**没有**独立 Architecture，**没有** Editor 面板 |
| `Assets/Game/Utility/` | `IGameSave`、`GameSaveUtility`（`[DDoveBindUtility(As = typeof(IGameSave))]`） |
| `Assets/Packages/MessagePack.*` | 已装，不要再拷一份进 Extension |

缺 Model + System + 对外 Kit 闭环，就留在 [游戏根](/02-程序-前/IOC容器使用规范.md)，不要开 `DDoveSaveArchitecture`。

## 程序集

[Core](/02-程序-前/DDoveFramework-Core.md) `references` 空，**零引用** MessagePack / 存档 Kit。

```
DDoveFramework.Extension.DDoveSave           只引用 Core
Game                                         现有引用 + DDoveSave
```

MessagePack **3.1.8** 在 `Assets/Packages/`（NuGet dll）。Save 的 asmdef `overrideReferences: false`，**不要**把 `MessagePack` 写进 `references`（那是给其它 asmdef 的）。

Kit 不进 [IOC](/02-程序-前/IOC容器.md)。日志 [DDoveDebug](/02-程序-前/DDoveDebug.md)，`title` 固定 `DDoveSave`。

## 接口

Kit（不进 IOC，不知道账号）：

```csharp
public static class DDoveSaveKit
{
    public const string LogTitle = "DDoveSave";
    public static string CurrentFileName { get; }

    public static void LoadFile(string fileName);
    public static void SaveFile(string fileName);
    public static void Save<T>(string key, T data);
    public static bool TryLoad<T>(string key, out T data);
    public static bool HasKey(string key);
    public static void DeleteKey(string key);
    public static void Clear();
}
```

内部：内存 `Dictionary<string, byte[]>`；`ContractlessStandardResolverAllowPrivate`；写盘走 tmp 校验再替换，失败读 `.bak`。文件不存在当空档，**不要**先 `File.Create` 空文件。空档名打 `DDoveSave` 错误并 return。`LoadFile` 成功会记下 `CurrentFileName`（含空档）。域重载时静态 KV 清空。

Utility（游戏根，接口键）：

```csharp
public interface IGameSave : IUtility
{
    void LoadFile(string fileName);
    T TryGetData<T>();
    void SaveData<T>(T data);
}
```

`LoadFile` 只记业务传入的全名并转给 Kit。`SaveData`：`Save(typeof(T).FullName, data)` 后立刻 `SaveFile`。档名优先 Utility 自己记的；为空则用 `DDoveSaveKit.CurrentFileName`（Launch 只调 Kit 时走这条）。两边都空：打 `DDoveSave` `(reason, save before LoadFile)` 并 return，不要写默认档。

业务以后换号只换传入的 `fileName`，接口不变。本刀不做换号迁移。

## 启动顺序

`LoadFile` **必须**在第一次 `GameArchitecture.Interface` 之前（`Model.OnInit` 才会读到已装入的 KV）。走 Kit 静态方法，不要先 `GetUtility` 再装档。

```
GameLaunch
    await DDoveCfgKit.LoadAsync()     失败则 return
    DDoveSaveKit.LoadFile("local_save.bin")
    GameArchitecture.Interface
    Atlas / Audio / UI
```

不要手写 `GameArchitecture.Init`。真 openid 到了只改传入的全名。

## 业务怎么存（本刀不接 PlayerModel）

Model 不要直接序列化自己。另写 `[MessagePackObject]` DTO，`OnInit` 里 `TryGetData`，变更时 `SaveData`。本刀只把接口挂上，示例 Model 留给下一刀。

## 不要

- 开独立 `DDoveSaveArchitecture`，或把 Kit 注册进游戏 IOC
- Kit 内置默认档名 / 拼接 openid
- 先碰 `Interface` 再 `LoadFile`
- 把玩法档写进 `PlayerPrefs`（demo / 音频开关除外）
- 把 MessagePack dll 再拷进 Extension
- Core 引用存档或 MessagePack
- 本刀改 `PlayerModel`、做换号合并、接微信登录

## 还没有（本刀之后）

`PlayerModel` 迁出 `PlayerPrefs`、真实 openid、换号迁档、IL2CPP AOT 生成 resolver、节流落盘。未实现前不要把这些当已有方法。

## 验收

1. 没装 `DDoveSave` 时 Core / Res / Boot / Editor 仍能编。
2. `GameArchitecture` 生成清单出现 `RegisterUtility<IGameSave>(new GameSaveUtility())`。
3. Play：Launch 在碰 `Interface` 之前 `LoadFile` 占位全名；`persistentDataPath` 下能出现该文件（第一次 `SaveData` 之后）。
4. 未 `LoadFile` 就 `SaveData` 打 `DDoveSave` 错误，不写盘。
5. Core asmdef 没有 MessagePack / DDoveSave。
6. `PlayerModel` 行为与本刀之前一致。
