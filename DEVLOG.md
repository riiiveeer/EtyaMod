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
