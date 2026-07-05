# 任务 D:第一个梦境 Boss——虚言者·卡诺斯(NPCs/Bosses/)

> 依赖:任务 A(World/ 剧情进度系统)合入主干后开工;副本进出机制需与 World/ 协调。
> 参考:`开发计划.md` 里程碑 M3,《故事背景.md》第三节"异变旧 NPC"。

## 待办

- [ ] `Kanos.cs`(ModNPC):虚言者·卡诺斯,向导的异变形态
  - [ ] 基础属性、图鉴条目、占位贴图
  - [ ] 两阶段 AI 设计(建议:一阶段"虚言引导"——假提示弹幕迷惑;二阶段"剧本崩坏"——狂化)
  - [ ] 技能弹幕(ModProjectile,放 `content/Projectiles/` 或本目录子文件夹,定下来后写进本文件)
  - [ ] 战败台词:"你以为是我指引了你,其实……我只是重复旧主的剧本。"(走本地化)
  - [ ] 掉落:记忆碎片 + 专属遗物(与任务 C 约定类名)
- [ ] 副本进出机制(简易版):
  - [ ] 召唤物品或 Etya 对话触发,传送到隔离区域
  - [ ] 战斗结束(胜/败)后传回,胜利时通过 WorldTruthSystem 公共接口推进进度
- [ ] Boss 血条、Boss 战 BGM 接口预留(音乐资源后补)

## 跨模块契约(2026-07-05 定稿,并行开发期间不得更改)

- **类名**:Boss 本体 `EtyaMod.NPCs.Bosses.Kanos`;召唤物 `EtyaMod.NPCs.Bosses.KanosDreamCharm`("护符残片·向导",归本任务所有,放本目录,不放 content/Items/)。
- **胜利判定**(仅服务端/单人调用):`ModContent.GetInstance<WorldTruthSystem>().CompleteDream(DreamBoss.Kanos)`;参战玩家 `GetModPlayer<EtyaModPlayer>().UnlockAbility("PhantomStep")`(幻影步的实际闪避机制本期不实现,只解锁能力键)。
- **掉落记忆碎片**:类名 `EtyaMod.content.Items.Quest.MemoryShard`(任务 C 实现)。**C 合入主干前禁止直接引用**,掉落先用原版物品 `ItemID.FallenStar` 占位,标注 `// TODO(契约): C 合入后替换为 ModContent.ItemType<MemoryShard>()`。
- **合并顺序**:C 先合入主干,D 替换占位后再合入。

## 验收标准

- `dotnet build` 通过,进游戏可完整打一场:进入 → 战斗 → 胜利 → 进度推进 → 返回
- 每次改动记入本目录 DEVLOG.md
