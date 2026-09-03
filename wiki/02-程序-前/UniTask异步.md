---
type: Playbook
title: 异步用 UniTask
description: 客户端异步统一 UniTask。Core 零引用；不要新开 Task / 协程当业务异步。
tags: [程序-前, unitask, 异步]
status: stable
generated: { by: human:cjh, at: 2026-08-31T03:09:00Z }
verified: { by: human:cjh, at: 2026-08-31T03:36:00Z }
sources:
  - id: unitask-pkg
    resource: ../../DDoveMiniGameClient/Packages/com.cysharp.unitask@2.5.11/package.json
    title: UniTask 2.5.11 package.json
  - id: core-asmdef
    resource: ../../DDoveMiniGameClient/Assets/EUFramework/Core/EUFramework.Core.asmdef
    title: EUFramework.Core.asmdef
  - id: core
    resource: /02-程序-前/EUFramework-Core.md
    title: EUFramework Core
---

# 异步用 UniTask

Concept ID：`/02-程序-前/UniTask异步`。清单：[index_cjh](/02-程序-前/index_cjh.md)。

包：`DDoveMiniGameClient/Packages/com.cysharp.unitask@2.5.11`（嵌入 UPM，**2.5.11**）。

客户端（EU Kit、启动、业务热更）异步统一 `Cysharp.Threading.Tasks.UniTask`。不要再开一套 `Task` / `IEnumerator` 业务接口。

## 用什么

`using Cysharp.Threading.Tasks;`

| 场景 | 写法 |
|------|------|
| 可等待、有返回 | `UniTask` / `UniTask<T>` |
| Unity 消息里开火即忘 | `async UniTaskVoid Foo()`，调用处 `Foo().Forget()` |
| 取消 | `CancellationToken`（面板关、Destroy、流程停） |
| 等一帧 / 延时 | `UniTask.Yield` / `NextFrame` / `UniTask.Delay` |
| 并行 | `UniTask.WhenAll` / `WhenAny` |

```csharp
public async UniTask<GameObject> OpenAsync(CancellationToken ct)
{
    var go = await LoadAsync(ct);
    await UniTask.NextFrame(ct);
    return go;
}
```

## 不要

- 新业务 API 返回 `System.Threading.Tasks.Task` / `Task<T>`
- 新流程用 `StartCoroutine` / `IEnumerator` 当异步（旧回调里已有的可暂留，改时换成 UniTask）
- `async void`（对不上 Unity 签名时才用；能写 `UniTaskVoid` 就写）
- `Task.Run` / 线程池跑游戏逻辑或碰 Unity API
- 同一条调用链里混用 `Task` 和 `UniTask`（只在边界转一次）
- 为了 `await` 去改 [EUFramework.Core](/02-程序-前/EUFramework-Core.md) 的程序集

第三方或平台 SDK 只给 `Task`：在**接入层** `.AsUniTask()`（或等价）再往上抛。业务不要直接 `await Task`。

Tween / 粒子不要包成 `ITween`。库自带 UniTask 扩展（如 DOTween）就直接 `await tween`。

## 和 Core

`EUFramework.Core`：`references` 空，`noEngineReferences: true`，**不引用** UniTask。

Command / Query / Event / Fsm 保持同步。谁异步谁在自己的 asmdef 里引用 `UniTask`：EURes、启动、面板 `OpenAsync`、业务 System。

## 包

| 项 | 约定 |
|----|------|
| 目录 | `Packages/com.cysharp.unitask@2.5.11/`（带 semver） |
| 进 Git | 整夹提交；别人 clone 不必再拉 GitHub |
| 落地 | Package Manager 装完后拷到 `Packages/`，见 [UPM 落地](/02-程序-前/UPM落地.md) |
| 升级 | 整夹替换，改文件夹名上的版本号 |
| 不要 | 再往 `Assets/` 拷一份 |

manifest 的远程条目留给不存本地夹的人自动拉，不要改成 `file:`、不要删。
