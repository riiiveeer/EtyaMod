# DEVLOG — 总线(主干)

> 记录跨模块调整、合并到主干、基础设施变更。各模块内部改动记在各自目录的 DEVLOG.md。

## 2026-07-04 | 主干初始化
- [新建] git 仓库初始化,首个提交(Etya NPC 骨架、NbSword、故事背景.md、开发计划.md)
- [新建] .cursor/rules/etyamod-conventions.mdc:项目总约定
- [新建] .cursor/rules/docs-and-devlog.mdc:注释与开发日志约定
- [新建] 五个模块(World/、NPCs/TownNPCs/、NPCs/Bosses/、content/Items/、UI/)各建 TODO.md + DEVLOG.md
- [新建] 本文件(总线 DEVLOG)
- 说明:任务 A(World/)为公共依赖须最先完成;B/C/D/E 在 A 合入主干后并行开工

## 2026-07-04 | task/a-world
- [新建] 仓库外 `..\tModLoader.targets`(EtyaMod-work 目录下):工作树比常规 ModSources 布局深一层,csproj 里的 `..\tModLoader.targets` 找不到真实 targets 导致编译失败,新建转发文件指向 `ModSources\tModLoader.targets`。各 task-* 工作树共用,无需改动 csproj
- 任务 A(World/ 剧情进度系统)完成,`dotnet build` 0 错误,详见 World/DEVLOG.md
