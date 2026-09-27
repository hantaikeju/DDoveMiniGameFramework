---
name: ddove-unity-pipeline
description: >-
  改场景、Prefab、材质、ScriptableObject、动画或工程设置，移动或删除 Assets 里已有资源，或用 unity CLI 重编译、查 editor_status。
  Triggers: 场景、Prefab、材质、ScriptableObject、动画、工程设置、移动删除 Assets、recompile、editor_status、unity CLI。
---

# ddove-unity-pipeline

本客户端工程路径：`G:\Self_WorkSpace\DDoveMiniGameFramework\DDoveMiniGameClient`。编辑器是 2022.3。Cursor 与其它 agent 用同一条 `unity` 命令。

调用形式（`--project-path` 写在 `command` 和命令名之间）：

```powershell
unity command --project-path G:\Self_WorkSpace\DDoveMiniGameFramework\DDoveMiniGameClient <命令名> [--参数 值]
```

## 步骤

1. **安装 CLI**（本机还没有 `unity` 时执行这一条）：

```powershell
$env:UNITY_CLI_CHANNEL='beta'; & ([scriptblock]::Create((irm https://raw.githubusercontent.com/rocwood/unity-cli-2022-mod/main/cli/install.ps1))) -Target 1.0.0-beta.2
```

完成：本机能执行 `unity`，版本是魔改 CLI `1.0.0-beta.2`。

2. **确认编辑器在线**。编辑器开着该工程时执行：

```powershell
unity command --project-path G:\Self_WorkSpace\DDoveMiniGameFramework\DDoveMiniGameClient editor_status
```

`editor_status` 无参数。Editor 服务在本机 `127.0.0.1` 的 `7800–7849`。

编辑器没开，或这条没有应答：`.cs` 等纯文本仍直接改文件。回复里写明还没做编译或场景验证。场景、Prefab、材质、ScriptableObject、动画、工程设置，以及移动或删除已有资源，停在这一步。

完成：`editor_status` 有应答；或回复已写明还没做编译或场景验证。

3. **日常改 `.cs`**。在磁盘上改源文件，然后按这个顺序调用。`set_autotick` 在域重载后会关掉，下一轮编译前再开一次。

```powershell
unity command --project-path G:\Self_WorkSpace\DDoveMiniGameFramework\DDoveMiniGameClient set_autotick --enable true
unity command --project-path G:\Self_WorkSpace\DDoveMiniGameFramework\DDoveMiniGameClient recompile
unity command --project-path G:\Self_WorkSpace\DDoveMiniGameFramework\DDoveMiniGameClient recompile_status
```

- `set_autotick`：`--enable` 默认 `true`（关掉用 `--enable false`）。`--interval_ms` 默认 `16`。失焦时靠它继续 tick。
- `recompile`：无必填参数，异步。可选 `--focus true` 先把编辑器拉到前台，默认 `false`。成功编译会域重载，这次调用不会等到结束。域重载期间连接中断后重试 `recompile_status`。
- `recompile_status`：无参数。轮询到 `status` 为 `completed` 或 `up_to_date`。其余取值是 `idle`、`triggered`、`compiling`。返回里带 `failed` 与 `errors`。`status` 已是 `completed` 且 `failed` 为 true 时，按 `errors` 改脚本，再从 `set_autotick` 做一轮。

热重载不在这一步。日常改代码的收尾就是上面三条。

完成：`recompile_status` 为 `up_to_date`，或 `completed` 且 `failed` 为 false。编辑器没开时，回复已写明还没做编译验证。

4. **改场景、Prefab、材质、ScriptableObject、动画、工程设置，或移动、删除 Assets 里已有资源**。用 Pipeline 命令写。`.meta` 和序列化文件不手改，包括 `.unity`、`.prefab`、`.mat`、`.asset`、`.anim`、`.controller`，以及 `ProjectSettings` 下的 yaml。导入设置用 `set_import_settings`，不改 `.meta` 文本。

Play Mode 里改场景会失败。先执行 `editor_stop`（无参数）。

下面每条都加上步骤 2 的 `--project-path`。资产路径相对 `Assets`（`Assets/` 前缀可省）。覆盖已有文件或删除资源时加 `--confirm true`。只预览时加 `--dry_run true`。

场景：

- `open_scene --path <场景路径>`
- `save_scene`（省略 `--path` 保存当前场景）
- `save_all`
- `get_scene_hierarchy`
- `create_scene --path Scenes/<名>`
- `create_gameobject --name <名>`
- `set_transform --target <handle> --position '[x,y,z]'`
- `add_component --target <handle> --type <类型名>`
- `set_component_properties --target <handle> --type <类型名> --properties '<json>'`
- `attach_script --target <handle> --script <已编译的 .cs 路径>`（类型还没编译时先做步骤 3，再重试）

Prefab：

- `create_prefab --source <handle> --path <路径>`
- `instantiate_prefab --prefab <路径>`
- `apply_prefab_overrides --instance <handle>`
- `save_prefab_contents --prefab <路径>`

材质：

- `get_material_properties --material <路径.mat>`
- `set_material_properties --material <路径.mat> --properties '<json>'`（属性名带前导下划线，颜色用 `[r,g,b,a]`）
- 新建：`create_asset --path <路径.mat> --type UnityEngine.Material`

ScriptableObject：

- `create_asset --path <路径.asset> --type <类型名>`
- `get_serialized_fields --target <路径.asset>`
- `set_serialized_field --target <路径.asset> --field <字段> --value <json>`

动画：

- `create_animation_clip --path <路径.anim>`
- `set_animation_curve --clip <路径.anim> --type Transform --property m_LocalPosition.x --keys '[{"time":0,"value":0},{"time":1,"value":1}]'`
- `create_animator_controller --path <路径.controller>`

工程设置。读用 `get_*`，写用同名 `set_*`。`set_*` 带 `--confirm true`，`--settings` 是 JSON，省略的字段保持原值。本编辑器用这些命令：`get_audio_settings`、`get_graphics_settings`、`get_input_settings`、`get_physics_settings`、`get_player_settings`、`get_quality_settings`、`get_tags_layers`、`get_time_settings`，以及对应的 `set_audio_settings`、`set_graphics_settings`、`set_input_settings`、`set_physics_settings`、`set_player_settings`、`set_quality_settings`、`set_tags_layers`、`set_time_settings`。示例：

```powershell
unity command --project-path G:\Self_WorkSpace\DDoveMiniGameFramework\DDoveMiniGameClient set_player_settings --confirm true --settings '{"productName":"DDove"}'
```

移动或删除已有资源：

- `move_asset --asset <路径> --destination <路径>`
- `rename_asset --asset <路径> --new_name <文件名>`
- `delete_asset --asset <路径> --confirm true`

完成：对应命令返回成功。编辑器没开时，回复已写明还没做场景验证。`.meta` 和序列化文件没有被文本改过。

5. **热重载（可选，不是日常改代码的必经步骤）**。日常改 `.cs` 停在步骤 3。只有游戏已经在跑，并且要换方法体、不做域重载时，才用本节。

先 `editor_play`（无参数）进入 Play Mode，或已经有 Mono 开发版 Player。IL2CPP 不可用。

就地改：目标是 `public` 的 `void` 实例方法，标 `[HotReload]`，改方法体后：

```powershell
unity command --project-path G:\Self_WorkSpace\DDoveMiniGameFramework\DDoveMiniGameClient reload_file --filename Assets/<脚本>.cs
```

`--filename` 必填。`--timeout` 默认 `30000`。`--pdb` 默认 false，需要断点时加上。

改动放在另一个文件时：`reload_file_override --filename <覆盖脚本路径>`（`--timeout` 默认 `30000`）。查看登记：`hotreload_status`（无参数）。

完成：日常 `.cs` 修改没有走到本节。只有明确要热重载时才执行了上面的命令。

6. **同一套命令**。其它 agent 也用步骤 2–5 的 `unity command --project-path G:\Self_WorkSpace\DDoveMiniGameFramework\DDoveMiniGameClient ...`。

完成：没有第二套连接方式或规则。
