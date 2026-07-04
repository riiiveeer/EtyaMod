# EtyaMod —《余烬之梦(Ashes of a Dream)》

泰拉瑞亚 tModLoader 剧情模组:月球领主被击败多年后的"和平世界"里,一个普通人跟随歌者 Etya 的歌声,逐步揭开世界是幻象的真相,最终面对手持天顶剑的前代主角"终焉者"。

- **世界观与角色设定**:[故事背景.md](故事背景.md)(所有设定以此为准,改设定请先开 Issue 讨论)
- **总体路线图**:[开发计划.md](开发计划.md)(里程碑 M1–M6)
- **变更日志**:根目录 [DEVLOG.md](DEVLOG.md) 为总线日志,各模块目录内有各自的 DEVLOG.md

## 分工与分支

按模块垂直分工,一个任务一个分支,互不重叠:

| 任务 | 分支 | 目录 | 内容 | 状态 |
|------|------|------|------|------|
| A | `task/a-world` | `World/` | 剧情进度系统(ModSystem/ModPlayer) | **未开工,公共依赖,最先做** |
| B | `task/b-etya` | `NPCs/TownNPCs/` | Etya 引导者完善 | 等 A 合入 |
| C | `task/c-items` | `content/Items/` | 记忆碎片、旧 NPC 遗物 | 等 A 合入 |
| D | `task/d-boss` | `NPCs/Bosses/` | 梦境 Boss 虚言者·卡诺斯 | 等 A 合入 |
| E | `task/e-ui` | `UI/` | 歌词与演出 UI | 等 A 合入 |

每个模块目录里有 `TODO.md`(待办与验收标准)和 `DEVLOG.md`(改动日志)。认领任务前先看对应 TODO。

## 协作流程

1. **认领**:在群里或 Issue 里说一声你要做哪个任务,避免撞车。
2. **开发**:协作者直接在对应 `task/*` 分支上开发;非协作者 fork 后从 `task/*` 分支切出去做。
3. **B/C/D/E 开工前**:先 `git merge master` 拿到任务 A 的进度系统接口(A 合入主干后)。
4. **合并**:完成一小块就向 `master` 发 Pull Request,不要攒大改动。`master` 受保护,任何人(包括仓库主)都不应直接 push。
5. **PR 要求**:`dotnet build` 通过 0 错误;改动已记入所在模块 DEVLOG.md;新文本走 Localization 不硬编码。

## 本地开发环境

1. 安装 [tModLoader](https://store.steampowered.com/app/1281930/tModLoader/)(1.4.4+)与 .NET 8 SDK。
2. 把仓库 clone 到 `文档/My Games/Terraria/tModLoader/ModSources/EtyaMod`(目录名必须是 EtyaMod)。
3. 用 Visual Studio / Rider / VS Code 打开 `EtyaMod.sln`,或直接 `dotnet build` 验证编译。
4. 游戏内 Mods → Develop Mods 可构建并热加载测试。

## 编码规范(摘要)

完整规范见 `.cursor/rules/`(用 Cursor 开发会自动加载;人肉开发请照着遵守):

- 每个 `.cs` 文件开头写注释块:文件作用 / 内部结构 / 模块依赖。
- 剧情进度只存在 `World/` 的 ModSystem/ModPlayer 里,其他模块通过公共接口读写。
- 所有玩家可见文本走 `Localization/` 的 hjson 键值,键名 `Mods.EtyaMod.<类别>.<名称>.<条目>`。
- 每个 ModItem/ModNPC 必须有 png 贴图,缺美术时用占位贴图并标 `// TODO: 占位贴图`。
- 每次新建/修改/删除文件,在所在模块 DEVLOG.md 追加一条记录(只追加,不改历史)。

## 设计讨论

世界观、剧情、玩法的讨论用 GitHub Issues(或群里聊完把结论沉淀到 Issue),涉及设定变更的最终以 `故事背景.md` 的更新为准。
