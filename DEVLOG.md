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

## 2026-07-05 | 主干,README 更新
- [修改] README.md:任务 A 状态改为已完成、B/C/D/E 改为可认领;新增进度系统接入指引(接口速查、/etya 指令);协作流程补充 DEVLOG 分支约定;环境搭建补充 worktree 的 tModLoader.targets 转发文件说明

## 2026-07-05 | 主干,跨模块契约定稿
- [修改] B/C/D/E 四份模块 TODO.md:补充"跨模块契约"章节——物品/Boss/UI 的类名与命名空间、进度系统调用点、未合入依赖的占位规则(ItemID.FallenStar + TODO 注释)、合并顺序 C → B/D
- 说明:B/C/D 即日起并行开发,E 暂缓

## 2026-07-05 | 主干,内容设计定案
- [修改] 故事背景.md:新增"幻象的运作与崩坏"(两段式 Boss 框架世界观)、职业解锁系统、进度节奏总表、树妖角色、Etya 深层动机(记录者+越出指令的纵容)、卡诺斯↔肉山强关联、内容设计定案附录;万物价主/血医魔母改为肉后现实 Boss;决意系统定为四职业天赋树
- [修改] 开发计划.md:当前进度反映任务 A 完成;M2 增职业解锁系统与决意树字段预留;M3 改为卡诺斯唯一梦境副本+傀儡掉落+召唤解锁;M4 增树妖之血事件、对质演出、现实 Boss、决意树实装;M5 增月总剧情包装;技术要点补职业锁定/陨星血月拦截/天赋树条目
- [修改] World/TODO.md:追加 M2 新增待办(决意树存档字段、职业解锁系统)
- 说明:路线倾向玩法后置,字段已预留;定案详情见故事背景.md 附录"内容设计定案(2026-07-05)"
