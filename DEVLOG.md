# DEVLOG — 总线(主干)

> 记录跨模块调整、合并到主干、基础设施变更。各模块内部改动记在各自目录的 DEVLOG.md。

## 2026-07-04 | 主干初始化
- [新建] git 仓库初始化,首个提交(Etya NPC 骨架、NbSword、故事背景.md、开发计划.md)
- [新建] .cursor/rules/etyamod-conventions.mdc:项目总约定
- [新建] .cursor/rules/docs-and-devlog.mdc:注释与开发日志约定
- [新建] 五个模块(World/、NPCs/TownNPCs/、NPCs/Bosses/、content/Items/、UI/)各建 TODO.md + DEVLOG.md
- [新建] 本文件(总线 DEVLOG)
- 说明:任务 A(World/)为公共依赖须最先完成;B/C/D/E 在 A 合入主干后并行开工

## 2026-07-04 | 主干,创建 worktree
- [新建] 五个 worktree 于 `../EtyaMod-work/` 下,分支与目录一一对应:
  - task-a-world → 分支 task/a-world(剧情进度系统)
  - task-b-etya → 分支 task/b-etya(Etya 完善)
  - task-c-items → 分支 task/c-items(剧情物品)
  - task-d-boss → 分支 task/d-boss(梦境 Boss 卡诺斯)
  - task-e-ui → 分支 task/e-ui(歌词演出 UI)
- 说明:B/C/D/E 分支当前基于 A 完成前的主干,待 A 合入主干后需先合并主干再开工

## 2026-07-04 | 主干,GitHub 协作初始化
- [新建] GitHub 公开仓库 https://github.com/riiiveeer/EtyaMod ,推送 master 与五个 task 分支
- [调整] master 开启分支保护:改动须走 PR,禁止直接 push 与强推
- [调整] 本地五个 task 分支设置远程跟踪(origin/task/*)
- [新建] README.md:项目简介、分工分支表、协作流程、环境搭建、编码规范摘要

## 2026-07-04 | task/a-world
- [新建] 仓库外 `..\tModLoader.targets`(EtyaMod-work 目录下):工作树比常规 ModSources 布局深一层,csproj 里的 `..\tModLoader.targets` 找不到真实 targets 导致编译失败,新建转发文件指向 `ModSources\tModLoader.targets`。各 task-* 工作树共用,无需改动 csproj
- 任务 A(World/ 剧情进度系统)完成,`dotnet build` 0 错误,详见 World/DEVLOG.md

## 2026-07-05 | 主干,合并任务 A 与 DEVLOG 冲突治理
- [合并] PR #1(task/a-world → master):剧情进度系统落地,B/C/D/E 可开工
- [新建] .gitattributes:DEVLOG.md 使用 union 合并策略,追加类冲突自动保留双方
- [调整] docs-and-devlog 规则:task 分支禁止改总线 DEVLOG.md,任务总结由合并者在合并后补记
