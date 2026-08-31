# Directory Update Log

## 2026-08-31
* **Update**: Event / CommandQuery / Fsm 收入 `Core/Mechanism/`。Architecture 仍单独。父目录不叫 Patterns。
* **Add**: `/02-程序-前/CommandQuery`。`ICommand` / `IQuery` 与基类挪到 `Core/CommandQuery/`。

## 2026-08-28
* **Update**: Event 从 Architecture 抽出为 `Core/Event/`。Wiki 改为三套并列：Architecture / Event / Fsm。
* **Update**: `/02-程序-前/EUFramework-Core` 写明 Architecture 与 Fsm 并列，以及 Core 再扩展的门槛。
* **Update**: Core 目录改为 `Architecture/` + `Fsm/`，移除 `MVC`/`Patterns`/`Singleton`。Wiki 路径与边界对齐。
* **Update**: `/02-程序-前/EUFramework-Core`：`.meta` 由 Unity 生成，不手写。
* **Add**: `/02-程序-前/Architecture与角色`、`/02-程序-前/TypeEvent`、`/02-程序-前/Core工具`。Core 落地 Architecture、角色基类、TypeEvent、StateMachine、Singleton。
* **Update**: `/02-程序-前/EUFramework-Core`、`/02-程序-前/IOC容器` 与 Architecture 对齐。
* **Add**: `/02-程序-前/EUFramework-Core`、`/02-程序-前/IOC容器`。客户端落地 `EUFramework.Core` + `IOCContainer`。
* **Update**: Business dirs only: `01-策划` / `02-程序-前` / `03-程序-后` / `04-美术`. Removed topic folders and 职种入口.
* **Initialization**: Created OKF v0.2 bundle. Spec source is repo-root `SPEC.md`. Entry skills: `mgf-wiki`, `mgf-okf-upgrade`.
