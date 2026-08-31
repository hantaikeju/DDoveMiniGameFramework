---
okf_version: "0.2"
---

# MiniGame Framework Wiki

OKF bundle。检索 / 写入约定：[OKF约定](00-索引/OKF约定.md)。

根清单**只挂目录**，不列概念。**不要改本文件**（除非新职种目录或 bump `okf_version`）。

加篇：`/ddove-wiki-create`，你只需给生成人短号，其余这边补。改状态：`/ddove-wiki-change-state`。细则见 OKF约定。

# 分类

* [00-索引](00-索引/) - 检索、约定、升级（给 Agent，不是业务文档）
* [01-策划](01-策划/) - 规则、配表、数值、验收
* [02-程序-前](02-程序-前/) - 客户端：运行时、UI、SDK、出包
* [03-程序-后](03-程序-后/) - 服务端：协议、校验、存档
* [04-美术](04-美术/) - 命名、尺寸、导入、图集

# 保留

* [变更史](log.md) - 按日索引；明细在 `_log/log_YYYY-MM-DD_<短号>.md`
* [SPEC.md](../SPEC.md) - OKF 规范原文（在仓库根，不入库）。整文件替换后走 `ddove-okf-upgrade`
* [AGENTS.md](../AGENTS.md) - Agent 入口（在仓库根，不是概念）
