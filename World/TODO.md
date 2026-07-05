# 任务 A:剧情进度系统(World/)

> 依赖:无。**本模块是所有其他模块的公共依赖,必须最先完成并合入主干。**
> 参考:`开发计划.md` 里程碑 M2,《故事背景.md》第四、五节。

## 待办

- [x] `WorldTruthSystem.cs`(ModSystem):
  - [x] 定义主线阶段枚举 `StoryPhase`(初遇 / 首次共鸣 / 三次共鸣完成 / 终焉之路 / 结局分支)
  - [x] 世界侧进度字段:当前阶段、已完成的梦境副本、共鸣度
  - [x] `SaveWorldData` / `LoadWorldData` 存档读档
  - [x] 对外只读属性 + 推进进度的公共方法(其他模块不得直接改字段)
- [x] `EtyaModPlayer.cs`(ModPlayer):
  - [x] 玩家侧数据:体内碎片觉醒度、已获能力列表、路线倾向(暴力/温和)计数
  - [x] `SaveData` / `LoadData`
- [x] 调试用聊天指令(ModCommand):查看/设置当前进度,方便各模块自测
- [x] 在本文件记录最终确定的数据结构,供 B/C/D/E 模块查阅

## 追加待办(2026-07-05 内容设计定案,详见《故事背景.md》附录)

- [ ] `EtyaModPlayer` 预留决意天赋树存档字段(天赋点、已点节点列表;树本体在 M4 实装,字段先行)
- [ ] 职业解锁系统(建议新建 `World/ClassUnlockSystem.cs` + `EtyaModPlayer` 解锁标记):
  - [ ] 开局锁魔法(魔力上限 0、魔法武器不可用),克眼后解锁 + 首次流星雨演出
  - [ ] 开局锁召唤(召唤武器不可用;鞭子可用、改吃近战加成),卡诺斯后解锁
  - [ ] 拦截陨落之星生成,克眼后放行
  - [ ] 开局屏蔽血月,树妖之血事件后放行
  - [ ] 解锁状态存档 + 多人同步(与 WorldTruthSystem 同样的受控写入口模式)

## 验收标准

- `dotnet build` 通过
- 进出存档后进度不丢失
- 提供清晰的公共接口注释(其他 agent 只看接口注释就能接入)

---

## 最终数据结构与公共接口(B/C/D/E 模块查阅)

> 命名空间统一为 `EtyaMod.World`。详细用法示例见各文件头部的【接入指南】注释。

### 枚举(显式赋值,存档稳定,只增不改)

```csharp
enum StoryPhase { FirstMeeting = 0, FirstResonance = 1, ResonanceComplete = 2, RoadToEnd = 3, Ending = 4 }
enum DreamBoss  { Kanos = 0, Merchant = 1, Nurse = 2 }
```

### WorldTruthSystem(世界侧,`ModContent.GetInstance<WorldTruthSystem>()` 获取)

| 成员 | 类型 | 说明 |
|------|------|------|
| `CurrentPhase` | `StoryPhase`(只读) | 当前主线阶段 |
| `ResonanceLevel` | `int`(只读) | 共鸣度 0~3(`MaxResonance = 3`) |
| `IsDreamCompleted(DreamBoss)` | `bool` | 指定梦境副本是否完成 |
| `CompletedDreamCount` | `int`(只读) | 已完成副本数量 |
| `AdvancePhase()` | 方法 | 主线阶段顺序推进一步(剧情节点用) |
| `CompleteDream(DreamBoss)` | 方法 | 标记副本完成,自动加共鸣度并按需推阶段(Boss 胜利判定用,重复调用无副作用) |

写入只允许走 `AdvancePhase` / `CompleteDream`,且只在服务器/单人端调用,多人同步自动完成(WorldData 包)。

### EtyaModPlayer(玩家侧,`player.GetModPlayer<EtyaModPlayer>()` 获取)

| 成员 | 类型 | 说明 |
|------|------|------|
| `ShardAwakening` | `int`(只读) | 体内碎片觉醒度 |
| `AwakenShard(int)` | 方法 | 提升觉醒度(提交碎片等剧情节点) |
| `HasAbility(string)` / `UnlockAbility(string)` | 方法 | 能力查询/解锁,能力用字符串键约定(如 `"PhantomStep"`) |
| `Abilities` | `IReadOnlyList<string>` | 已解锁能力快照 |
| `ViolenceCount` / `MercyCount` / `RouteTendency` | `int`(只读) | 路线计数;倾向 = 暴力 - 温和 |
| `AddViolence()` / `AddMercy()` | 方法 | 剧情选择处记录路线 |
| `EquippedRelicCount` | `int`(只读) | 当前佩戴遗物件数(每帧刷新,满 3 触发共鸣) |
| `RegisterEquippedRelic()` | 方法 | **任务 C**:遗物饰品在 `UpdateAccessory` 中每帧调用一次 |

### 调试指令(EtyaDebugCommand)

游戏内聊天输入 `/etya status | phase <0-4> | dream <0-2> | undream <0-2> | res <0-3> | ability <key> | awaken <n>`。
