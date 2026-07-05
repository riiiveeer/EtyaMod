# 任务 C:剧情物品(content/Items/)

> 依赖:任务 A(World/ 剧情进度系统)合入主干后开工。
> 参考:`开发计划.md` 里程碑 M2,《故事背景.md》第五节"Etya 的商店"。

## 待办

- [ ] 目录规划:`content/Items/Quest/`(碎片类)、`content/Items/Relics/`(旧 NPC 遗物)
- [ ] `MemoryShard`(记忆碎片):基础收集品,提交给 Etya 用
- [ ] 三件旧 NPC 遗物(各带微弱异能 + Lore 文本):
  - [ ] 护士的手套(如:回血小加成)
  - [ ] 向导的徽章(如:低血量减伤)
  - [ ] 渔夫的鱼饵盒(如:渔力小加成)
- [ ] 碎片卷轴占位(解锁能力用,能力系统在后续里程碑)
- [ ] 佩戴遗物时写入 EtyaModPlayer 的共鸣计数(为灵魂共鸣系统预留)
- [ ] 每件物品:占位贴图(复制 NbSword.png 改名 + `// TODO: 占位贴图`)、本地化条目、图鉴描述

## 跨模块契约(2026-07-05 定稿,并行开发期间不得更改)

- **类名与命名空间**(B/D 将按此引用,一个字母都不能差):
  - `EtyaMod.content.Items.Quest.MemoryShard` — 记忆碎片(目录 `content/Items/Quest/MemoryShard/`)
  - `EtyaMod.content.Items.Relics.NurseGlove` — 护士的手套(生命回复小加成)
  - `EtyaMod.content.Items.Relics.GuideEmblem` — 向导的徽章(低血量减伤)
  - `EtyaMod.content.Items.Relics.AnglerBaitBox` — 渔夫的鱼饵盒(渔力加成)
- **遗物共鸣**:三件遗物是饰品(accessory),在 `UpdateAccessory` 中每帧调 `player.GetModPlayer<EtyaModPlayer>().RegisterEquippedRelic()`(见 `World/EtyaModPlayer.cs` 文件头接入指南)。
- **本任务无外部依赖,是 B/D 的被依赖方,应最先完成合入主干**,解锁 B/D 的占位替换。

## 验收标准

- `dotnet build` 通过,进游戏物品可正常获取、佩戴、显示文本
- 每次改动记入本目录 DEVLOG.md
