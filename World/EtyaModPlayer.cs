// ============================================
// 文件: EtyaModPlayer.cs
// 作用: 玩家侧剧情数据的单一事实来源(ModPlayer),负责随人物存档、读档
// 结构: EtyaModPlayer 类(碎片觉醒度 / 已获能力列表 / 暴力·温和路线计数 / 佩戴遗物共鸣计数)
// 依赖: 被 NPCs/、content/Items/、UI/ 各模块读写(一律通过公共接口);不依赖其他模块
// ============================================
//
// 【接入指南(B/C/D/E 模块请阅读)】
// 获取实例:
//     var etyaPlayer = player.GetModPlayer<EtyaModPlayer>();
// 常用调用:
//     etyaPlayer.AwakenShard(1);                 // 提交记忆碎片等剧情节点,提升体内碎片觉醒度
//     etyaPlayer.UnlockAbility("PhantomStep");   // 战后能力觉醒(能力用字符串键约定,如 "PhantomStep" 幻影步)
//     if (etyaPlayer.HasAbility("PhantomStep")) { ... }
//     etyaPlayer.AddViolence(); / etyaPlayer.AddMercy();   // 剧情选择处记录路线倾向
// 遗物共鸣(任务 C):遗物饰品在 UpdateAccessory 中每帧调用 RegisterEquippedRelic(),
//     当帧 EquippedRelicCount 即"当前佩戴的旧遗物件数",满 3 件触发完整之歌(任务 B/E 读取)。

using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EtyaMod.World
{
    public class EtyaModPlayer : ModPlayer
    {
        private int shardAwakening;
        private readonly List<string> abilities = new();
        private int violenceCount;
        private int mercyCount;

        // 每帧由遗物饰品重新上报,不存档(佩戴状态本身即事实来源)
        private int equippedRelicCount;

        // ---------- 只读接口 ----------

        /// <summary>体内旧时代碎片的觉醒度(只增不减,剧情节点提升)。</summary>
        public int ShardAwakening => shardAwakening;

        /// <summary>是否已解锁指定能力(键约定见文件头注释)。</summary>
        public bool HasAbility(string abilityKey) => abilities.Contains(abilityKey);

        /// <summary>已解锁能力的只读快照(调试/图鉴用,勿缓存引用)。</summary>
        public IReadOnlyList<string> Abilities => abilities;

        /// <summary>暴力路线计数(击杀可放过的目标等)。</summary>
        public int ViolenceCount => violenceCount;

        /// <summary>温和路线计数(饶恕、和平解决等)。</summary>
        public int MercyCount => mercyCount;

        /// <summary>
        /// 路线倾向:正数偏暴力,负数偏温和,0 中立。
        /// Etya 歌声变化(Bad Ending 前兆)以此判断,见《故事背景.md》第三节。
        /// </summary>
        public int RouteTendency => violenceCount - mercyCount;

        /// <summary>当前佩戴的旧 NPC 遗物件数(每帧刷新,满 3 件触发灵魂共鸣)。</summary>
        public int EquippedRelicCount => equippedRelicCount;

        // ---------- 写入口 ----------

        /// <summary>提升碎片觉醒度(负数无效)。剧情节点调用,如向 Etya 提交记忆碎片。</summary>
        public void AwakenShard(int amount)
        {
            if (amount > 0)
                shardAwakening += amount;
        }

        /// <summary>解锁一项能力,重复解锁无副作用。返回是否为首次解锁。</summary>
        public bool UnlockAbility(string abilityKey)
        {
            if (abilities.Contains(abilityKey))
                return false;
            abilities.Add(abilityKey);
            return true;
        }

        /// <summary>记录一次暴力路线选择。</summary>
        public void AddViolence() => violenceCount++;

        /// <summary>记录一次温和路线选择。</summary>
        public void AddMercy() => mercyCount++;

        /// <summary>遗物饰品在 UpdateAccessory 中每帧调用一次,上报"本帧佩戴中"。</summary>
        public void RegisterEquippedRelic() => equippedRelicCount++;

        // ---------- 帧重置 / 存档 / 读档 ----------

        public override void ResetEffects()
        {
            // 佩戴计数由饰品每帧重新上报
            equippedRelicCount = 0;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["shardAwakening"] = shardAwakening;
            tag["abilities"] = abilities;
            tag["violence"] = violenceCount;
            tag["mercy"] = mercyCount;
        }

        public override void LoadData(TagCompound tag)
        {
            shardAwakening = tag.GetInt("shardAwakening");
            abilities.Clear();
            abilities.AddRange(tag.GetList<string>("abilities"));
            violenceCount = tag.GetInt("violence");
            mercyCount = tag.GetInt("mercy");
        }
    }
}
