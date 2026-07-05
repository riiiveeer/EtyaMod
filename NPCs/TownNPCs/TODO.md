# 任务 B:Etya 完善(NPCs/TownNPCs/)

> 依赖:任务 A(World/ 剧情进度系统)合入主干后开工,进度读写一律走其公共接口。
> 参考:`开发计划.md` 里程碑 M1,《故事背景.md》第三节"Etya"。

## 待办

- [ ] 清理 `Etya.cs`:
  - [ ] 删除误引入的 `using System.Data.OracleClient`
  - [ ] 确认血量(当前 25000,常规城镇 NPC 为 400)
  - [ ] 补全 NPC 好感度设定(SetNPCAffection)
- [ ] 补全文件头注释(作用/结构/依赖)
- [ ] 对话系统按剧情阶段切换对话池(初遇 / 获得旧遗物后 / 共鸣阶段 / 终章前),阶段从 WorldTruthSystem 读取
- [ ] "圣物"按钮(button2)功能:记忆碎片提交入口(碎片物品由任务 C 提供,先以物品类名约定对接)
- [ ] 商店替换为剧情商店:旧 NPC 遗物、追忆物品、碎片卷轴(物品由任务 C 提供,可先用占位条目)
- [ ] 首次见面剧情演出(简易版:进度推进 + 特殊对话)
- [ ] 所有新增文本写入 Localization,禁止硬编码

## 跨模块契约(2026-07-05 定稿,并行开发期间不得更改)

- **进度读取**:剧情阶段读 `WorldTruthSystem.CurrentPhase`,按阶段切换对话池。**B 不推进主线阶段**(阶段推进由 `CompleteDream` 等剧情节点驱动,归任务 D / 后续系统)。
- **碎片提交**:玩家提交记忆碎片每件调 `etyaPlayer.AwakenShard(1)`。
- **记忆碎片**:类名 `EtyaMod.content.Items.Quest.MemoryShard`(任务 C 实现)。**C 合入主干前禁止直接引用**(编译不过),提交入口先用原版物品 `ItemID.FallenStar` 占位,并标注 `// TODO(契约): C 合入后替换为 ModContent.ItemType<MemoryShard>()`。
- **商店遗物**:类名 `EtyaMod.content.Items.Relics.NurseGlove / GuideEmblem / AnglerBaitBox`(任务 C 实现),占位规则同上。
- **商店追忆物品**:"护符残片·向导" = `EtyaMod.NPCs.Bosses.KanosDreamCharm`(任务 D 实现),占位规则同上。
- **唱歌演出**:调 `EtyaMod.UI.SongUI.Play(string songKey)`(任务 E 实现)。E 合入前用 `Main.NewText` 输出歌词占位,标同样的 TODO(契约)注释。
- **职业解锁相关对话**(克眼后 Etya 传授魔法等,见《故事背景.md》五-0)属后续"职业解锁系统"任务,本期 B 不做。
- **合并顺序**:C 先合入主干,B 替换占位后再合入。

## 验收标准

- `dotnet build` 通过
- 不同剧情阶段进游戏实测对话池正确切换
- 每次改动记入本目录 DEVLOG.md
