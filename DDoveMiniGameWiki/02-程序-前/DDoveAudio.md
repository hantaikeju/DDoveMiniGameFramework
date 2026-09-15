---
type: Reference
title: DDoveAudio
description: 独立音频根。对外只碰 Kit；通道 / 设置 / 播放池不进 GameArchitecture。按名走 Res，播放器走 Pool。
tags: [程序-前, ddoveaudio]
status: stable
generated: { by: human:cjh, at: 2026-09-15T07:48:00Z }
verified: { by: human:cjh, at: 2026-09-15T14:49:00Z }
sources:
  - id: kit
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveAudio/DDoveAudioKit.cs
    title: DDoveAudioKit.cs
  - id: arch
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveAudio/DDoveAudioArchitecture.cs
    title: DDoveAudioArchitecture.cs
  - id: settings
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveAudio/Model/DDoveAudioKitSettingsModel.cs
    title: DDoveAudioKitSettingsModel.cs
  - id: loader
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveAudio/Loader/DDoveDefaultAudioLoader.cs
    title: DDoveDefaultAudioLoader.cs
  - id: player
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveAudio/Player/DDoveAudioPlayer.cs
    title: DDoveAudioPlayer.cs
  - id: asmdef
    resource: ../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveAudio/DDoveFramework.Extension.DDoveAudio.asmdef
    title: DDoveFramework.Extension.DDoveAudio.asmdef
  - id: launch
    resource: ../../DDoveMiniGameClient/Assets/Game/Mono/GameLaunch.cs
    title: GameLaunch.cs
  - id: ioc-use
    resource: /02-程序-前/IOC容器使用规范.md
    title: IOC 容器使用规范
  - id: roles
    resource: /02-程序-前/Architecture与角色.md
    title: Architecture 与角色
  - id: bind
    resource: /02-程序-前/Architecture自动注册.md
    title: Architecture 自动注册
  - id: res
    resource: /02-程序-前/DDoveRes.md
    title: DDoveRes
  - id: pool
    resource: /02-程序-前/DDovePool.md
    title: DDovePool
  - id: event
    resource: /02-程序-前/TypeEvent.md
    title: TypeEvent
  - id: cq
    resource: /02-程序-前/CommandQuery.md
    title: Command 与 Query
  - id: debug
    resource: /02-程序-前/DDoveDebug.md
    title: DDoveDebug
  - id: core
    resource: /02-程序-前/DDoveFramework-Core.md
    title: DDoveFramework Core
---

# DDoveAudio

Concept ID：`/02-程序-前/DDoveAudio`。权威实现：[DDoveAudioKit.cs](../../DDoveMiniGameClient/Assets/DDoveFramework/Extension/DDoveAudio/DDoveAudioKit.cs)。结论与代码冲突以代码为准。本文 `status: draft`，不当发布依据。

`Extension/DDoveAudio/` 静态门面。业务只调 Kit。自己的 [Architecture](/02-程序-前/Architecture与角色.md) 根是 `DDoveAudioArchitecture`，按 [IOC 容器使用规范](/02-程序-前/IOC容器使用规范.md) 手写 `Init()`。通道 System、设置 Model、正在播放池 Model **不要**挂 `[DDoveBind*]`，**不要**进 `GameArchitecture`。Kit 本身不进任何 IOC，也不进 [Core](/02-程序-前/DDoveFramework-Core.md)。

日志 [DDoveDebug](/02-程序-前/DDoveDebug.md)，`title` 固定 `DDoveAudio`。

## 调用

[GameLaunch](../../DDoveMiniGameClient/Assets/Game/Mono/GameLaunch.cs) 在 Atlas 之后、UI 之前 `Initialize()`。无参则自建 `[DDoveAudio]` + `DontDestroyOnLoad`。传入根则用传入根，不 Destroy。已有根再调直接 return。未 Init 就 `Play*`：[DDoveDebug](/02-程序-前/DDoveDebug.md) `LogError` 并 return。

按名加载走 [DDoveRes](/02-程序-前/DDoveRes.md) 当前包；Yoo location 必须和名字一致。音效播放器、Loader 走 [DDovePool](/02-程序-前/DDovePool.md) C# 池（不必先 `DDovePoolKit.Initialize`）。

```csharp
DDoveAudioKit.Initialize();
DDoveAudioKit.PlayMusic("bgm_home");
DDoveAudioKit.PlaySound("click");
DDoveAudioKit.Settings.SetIsSoundOn(false);
```

| 方法 | 行为 |
|------|------|
| `Initialize()` / `Initialize(Transform)` | 建或接 AudioRoot；拉起 `DDoveAudioArchitecture`；注册 Loader 与设置事件。已有根 return |
| `Shutdown()` | 停音乐 / 语音 / 音效；`DDoveAudioArchitecture.Reset()`。只 Destroy **自己建的**根 |
| `PlayMusic(string)` / `PlayMusic(AudioClip)` | 懒建 `MusicPlayer`（默认循环）。停 / 暂停 / 恢复：`StopMusic` / `PauseMusic` / `ResumeMusic`。未播过则 no-op |
| `PlayVoice(string)` / `PlayVoice(AudioClip)` | 懒建 `VoicePlayer`（默认不循环）。`IsVoiceOn == false` 不播。另有 `StopVoice` / `PauseVoice` / `ResumeVoice` |
| `PlayVoiceOnce(string)` | 用音效池播一段，不占 VoicePlayer，可重叠 |
| `PlaySound(string)` / `PlaySound(AudioClip)` | 返回 `DDoveAudioPlayer`。可传 `playSoundMode`，否则 `DefaultPlaySoundMode` |
| `StopAllSound()` | 停正在播的音效并清空池记录 |
| `Settings` | `DDoveAudioKitSettingsModel`：开关、音量、`PlayerPrefs` |
| `DefaultPlaySoundMode` | 默认 `EveryOne` |
| `SoundFrameCountForIgnoreSameSound` / `GlobalFrameCountForIgnoreSameSound` | 两种忽略模式的帧窗，默认 `10` |
| `AudioRoot` / `MusicPlayer` / `VoicePlayer` | 根节点与两个常驻播放器 |

`DDovePlaySoundModes`：`EveryOne` 不限重复；`IgnoreSameSoundInGlobalFrames` 全局帧窗内同名只一次；`IgnoreSameSoundInSoundFrames` 每个名字自己的帧窗。

`CanPlayAudio` 为 false（例如音效关）会 `Stop` 并还池，不占着播放器。

## 根和事件

`DDoveAudioArchitecture.Init()` 只 `Register`：`DDovePlaySoundChannelSystem`、`DDoveAudioKitSettingsModel`、`DDovePlayingSoundPoolModel`。Kit 用 `Interface.GetModel` / `GetSystem`。拆这条链路只 `DDoveAudioArchitecture.Reset()`，不要动 `GameArchitecture`。

开关 / 音量变化走**这一根**的 [TypeEvent](/02-程序-前/TypeEvent.md)，不走 `TypeEventSystem.Global`。`Reset()` 清该根事件。

Kit 目录里的 `DDove*Command` 是内部静态 `Execute`，**不是** [CommandQuery](/02-程序-前/CommandQuery.md) 的 `AbstractCommand`。业务不要 `new` 它们。玩法侧要「点按钮播一段」再写真正的 Command，里面调 Kit。

## 程序集

`DDoveFramework.Extension.DDoveAudio`：`DDoveFramework.Core`、`DDoveFramework.Extension.DDovePool`、`DDoveFramework.Extension.DDoveRes`、`UniTask`、`YooAsset`。`autoReferenced: true`。**没有** Editor 面板，没有代码生成。

`.meta` **不要手写**。挪已有资源时带着 Unity 生成的 `.meta` 一起挪。

## 不要

- 把音频 Model / System 挂进 [游戏 IOC](/02-程序-前/Architecture自动注册.md)
- 业务 `SendCommand` 音频内部的静态 Command，或 `new XxxCommand().Execute()`
- 未 `Initialize` 就 `Play*`
- 指望 Kit 替你把 AudioClip 收进 Yoo 收集器
- 把音频塞进 [Core](/02-程序-前/DDoveFramework-Core.md)

## 还没有

3D / 空间化、按 tag 批量预载、Editor 面板、Boot 自动 Init。`Shutdown` 只在自建根的 `OnApplicationQuit` 上挂；传入根由调用方自己管。Host / Web 远程加载仍看 [DDoveRes](/02-程序-前/DDoveRes.md)。
