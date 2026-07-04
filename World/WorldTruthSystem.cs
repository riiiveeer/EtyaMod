// ============================================
// 文件: WorldTruthSystem.cs
// 作用: 主线剧情进度的单一事实来源(ModSystem),负责世界侧进度的存档、读档与多人同步
// 结构: StoryPhase 枚举(主线阶段)/ DreamBoss 枚举(梦境副本 Boss)/ WorldTruthSystem 类(进度字段 + 只读属性 + 受控推进方法)
// 依赖: 被 NPCs/、content/Items/、UI/ 各模块读写(一律通过公共接口);不依赖其他模块
// ============================================
//
// 【接入指南(B/C/D/E 模块请阅读)】
// 读取进度:
//     var truth = ModContent.GetInstance<WorldTruthSystem>();
//     if (truth.CurrentPhase >= StoryPhase.FirstResonance) { ... }
//     if (truth.IsDreamCompleted(DreamBoss.Kanos)) { ... }
// 推进进度(禁止直接改字段,只能调用下列方法):
//     truth.CompleteDream(DreamBoss.Kanos);  // 打赢梦境 Boss 后调用,自动累加共鸣度并按需推进阶段
//     truth.AdvancePhase();                  // 顺序推进主线阶段(初遇→首次共鸣→…→结局)
// 多人模式:上述方法只应在服务器/单人端调用(如 Boss 击杀判定处),同步会自动完成。

using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EtyaMod.World
{
    /// <summary>
    /// 主线剧情阶段。显式赋值以保证存档兼容,只增不改。
    /// 阶段设定见《故事背景.md》第四节"叙事结构"。
    /// </summary>
    public enum StoryPhase
    {
        /// <summary>初遇:玩家尚未(或刚刚)见到 Etya。</summary>
        FirstMeeting = 0,
        /// <summary>首次共鸣:完成第一个梦境副本之后。</summary>
        FirstResonance = 1,
        /// <summary>三次共鸣完成:三个梦境副本全部完成。</summary>
        ResonanceComplete = 2,
        /// <summary>终焉之路:解锁通往最终 Boss 的主线提示。</summary>
        RoadToEnd = 3,
        /// <summary>结局分支:终焉者战斗已触发/完成。</summary>
        Ending = 4,
    }

    /// <summary>
    /// 梦境副本 Boss(异变旧 NPC)。显式赋值以保证存档兼容,后续追加新值即可。
    /// 设定见《故事背景.md》第三节"异变旧 NPC"。
    /// </summary>
    public enum DreamBoss
    {
        /// <summary>向导 → 虚言者·卡诺斯(首个副本,任务 D)。</summary>
        Kanos = 0,
        /// <summary>商人 → 贪欲之灵"万物价主"。</summary>
        Merchant = 1,
        /// <summary>护士 → 血医魔母。</summary>
        Nurse = 2,
    }

    public class WorldTruthSystem : ModSystem
    {
        // 完成三次共鸣(三个梦境副本)后进入 ResonanceComplete,数值来自《故事背景.md》第五节"灵魂共鸣"
        public const int MaxResonance = 3;

        private StoryPhase currentPhase;
        private int resonanceLevel;
        // 用位掩码存已完成副本(bit N = (int)DreamBoss),存档紧凑且天然去重
        private int completedDreamMask;

        // ---------- 只读接口 ----------

        /// <summary>当前主线阶段。</summary>
        public StoryPhase CurrentPhase => currentPhase;

        /// <summary>当前共鸣度(0 ~ <see cref="MaxResonance"/>)。</summary>
        public int ResonanceLevel => resonanceLevel;

        /// <summary>指定梦境副本是否已完成。</summary>
        public bool IsDreamCompleted(DreamBoss boss) => (completedDreamMask & (1 << (int)boss)) != 0;

        /// <summary>已完成的梦境副本数量。</summary>
        public int CompletedDreamCount
        {
            get
            {
                int count = 0;
                for (int mask = completedDreamMask; mask != 0; mask >>= 1)
                    count += mask & 1;
                return count;
            }
        }

        // ---------- 推进方法(唯一合法的写入口) ----------

        /// <summary>
        /// 将主线阶段顺序推进一步(已到结局则无效)。
        /// 用于剧情节点演出(如首次见面、解锁终焉之路)。
        /// </summary>
        public void AdvancePhase()
        {
            if (currentPhase >= StoryPhase.Ending)
                return;
            currentPhase++;
            SyncIfServer();
        }

        /// <summary>
        /// 标记一个梦境副本完成:自动累加共鸣度;首次共鸣、三次共鸣时自动推进主线阶段。
        /// 由 Boss 战胜利判定处调用(任务 D)。重复调用同一 Boss 无副作用。
        /// </summary>
        public void CompleteDream(DreamBoss boss)
        {
            if (IsDreamCompleted(boss))
                return;

            completedDreamMask |= 1 << (int)boss;
            if (resonanceLevel < MaxResonance)
                resonanceLevel++;

            // 剧情联动:第一次共鸣把阶段推到 FirstResonance,共鸣满推到 ResonanceComplete
            if (resonanceLevel >= 1 && currentPhase < StoryPhase.FirstResonance)
                currentPhase = StoryPhase.FirstResonance;
            if (resonanceLevel >= MaxResonance && currentPhase < StoryPhase.ResonanceComplete)
                currentPhase = StoryPhase.ResonanceComplete;

            SyncIfServer();
        }

        // ---------- 调试接口(仅供 EtyaDebugCommand 使用,正式逻辑禁止调用) ----------

        /// <summary>[调试] 直接设置主线阶段。</summary>
        internal void DebugSetPhase(StoryPhase phase)
        {
            currentPhase = phase;
            SyncIfServer();
        }

        /// <summary>[调试] 直接设置共鸣度。</summary>
        internal void DebugSetResonance(int level)
        {
            resonanceLevel = Utils.Clamp(level, 0, MaxResonance);
            SyncIfServer();
        }

        /// <summary>[调试] 清除指定副本的完成标记。</summary>
        internal void DebugResetDream(DreamBoss boss)
        {
            completedDreamMask &= ~(1 << (int)boss);
            SyncIfServer();
        }

        // ---------- 存档 / 读档 / 同步 ----------

        public override void ClearWorld()
        {
            // 每次进入世界前重置,防止跨存档串数据
            currentPhase = StoryPhase.FirstMeeting;
            resonanceLevel = 0;
            completedDreamMask = 0;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["phase"] = (int)currentPhase;
            tag["resonance"] = resonanceLevel;
            tag["dreams"] = completedDreamMask;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            currentPhase = (StoryPhase)tag.GetInt("phase");
            resonanceLevel = tag.GetInt("resonance");
            completedDreamMask = tag.GetInt("dreams");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write((byte)currentPhase);
            writer.Write((byte)resonanceLevel);
            writer.Write(completedDreamMask);
        }

        public override void NetReceive(BinaryReader reader)
        {
            currentPhase = (StoryPhase)reader.ReadByte();
            resonanceLevel = reader.ReadByte();
            completedDreamMask = reader.ReadInt32();
        }

        // 进度变化后把 WorldData 包推给所有客户端(NetSend 随包发出)
        private static void SyncIfServer()
        {
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }
    }
}
