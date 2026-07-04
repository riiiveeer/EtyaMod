// ============================================
// 文件: EtyaDebugCommand.cs
// 作用: 剧情进度调试指令(ModCommand),供各模块自测时查看/设置进度,不参与正式玩法
// 结构: EtyaDebugCommand 类(/etya 指令的子命令分发)
// 依赖: 读写 WorldTruthSystem、EtyaModPlayer(调试接口);仅单人/本地客户端使用
// ============================================
//
// 用法(游戏内聊天输入):
//   /etya status            查看世界进度 + 玩家数据
//   /etya phase <0-4>       设置主线阶段(0初遇 1首次共鸣 2三次共鸣完成 3终焉之路 4结局)
//   /etya dream <0-2>       标记梦境副本完成(0卡诺斯 1商人 2护士),走正式 CompleteDream 流程
//   /etya undream <0-2>     清除副本完成标记
//   /etya res <0-3>         设置共鸣度
//   /etya ability <键>      给自己解锁能力(如 /etya ability PhantomStep)
//   /etya awaken <n>        提升碎片觉醒度

using System;
using Terraria;
using Terraria.ModLoader;

namespace EtyaMod.World
{
    public class EtyaDebugCommand : ModCommand
    {
        public override string Command => "etya";

        public override CommandType Type => CommandType.Chat;

        public override string Usage => "/etya status | phase <0-4> | dream <0-2> | undream <0-2> | res <0-3> | ability <key> | awaken <n>";

        public override string Description => "EtyaMod 剧情进度调试指令";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            var truth = ModContent.GetInstance<WorldTruthSystem>();
            var etyaPlayer = caller.Player.GetModPlayer<EtyaModPlayer>();

            if (args.Length == 0)
            {
                caller.Reply(Usage);
                return;
            }

            switch (args[0].ToLowerInvariant())
            {
                case "status":
                    caller.Reply($"[世界] 阶段: {truth.CurrentPhase} ({(int)truth.CurrentPhase})  共鸣度: {truth.ResonanceLevel}/{WorldTruthSystem.MaxResonance}");
                    foreach (DreamBoss boss in Enum.GetValues<DreamBoss>())
                        caller.Reply($"[副本] {boss}: {(truth.IsDreamCompleted(boss) ? "已完成" : "未完成")}");
                    caller.Reply($"[玩家] 碎片觉醒度: {etyaPlayer.ShardAwakening}  路线倾向: {etyaPlayer.RouteTendency} (暴力{etyaPlayer.ViolenceCount}/温和{etyaPlayer.MercyCount})");
                    caller.Reply($"[玩家] 已获能力: {(etyaPlayer.Abilities.Count == 0 ? "无" : string.Join(", ", etyaPlayer.Abilities))}");
                    caller.Reply($"[玩家] 当前佩戴遗物数: {etyaPlayer.EquippedRelicCount}");
                    break;

                case "phase" when TryParseArg(caller, args, out int phase) && CheckRange(caller, phase, 0, 4):
                    truth.DebugSetPhase((StoryPhase)phase);
                    caller.Reply($"主线阶段已设为 {truth.CurrentPhase}");
                    break;

                case "dream" when TryParseArg(caller, args, out int dream) && CheckRange(caller, dream, 0, 2):
                    truth.CompleteDream((DreamBoss)dream);
                    caller.Reply($"副本 {(DreamBoss)dream} 已标记完成,当前阶段 {truth.CurrentPhase},共鸣度 {truth.ResonanceLevel}");
                    break;

                case "undream" when TryParseArg(caller, args, out int undream) && CheckRange(caller, undream, 0, 2):
                    truth.DebugResetDream((DreamBoss)undream);
                    caller.Reply($"副本 {(DreamBoss)undream} 完成标记已清除");
                    break;

                case "res" when TryParseArg(caller, args, out int res) && CheckRange(caller, res, 0, WorldTruthSystem.MaxResonance):
                    truth.DebugSetResonance(res);
                    caller.Reply($"共鸣度已设为 {truth.ResonanceLevel}");
                    break;

                case "ability" when args.Length >= 2:
                    caller.Reply(etyaPlayer.UnlockAbility(args[1])
                        ? $"已解锁能力 {args[1]}"
                        : $"能力 {args[1]} 已存在");
                    break;

                case "awaken" when TryParseArg(caller, args, out int amount):
                    etyaPlayer.AwakenShard(amount);
                    caller.Reply($"碎片觉醒度现为 {etyaPlayer.ShardAwakening}");
                    break;

                default:
                    caller.Reply(Usage);
                    break;
            }
        }

        private static bool TryParseArg(CommandCaller caller, string[] args, out int value)
        {
            value = 0;
            if (args.Length >= 2 && int.TryParse(args[1], out value))
                return true;
            caller.Reply("参数错误:需要一个整数参数");
            return false;
        }

        private static bool CheckRange(CommandCaller caller, int value, int min, int max)
        {
            if (value >= min && value <= max)
                return true;
            caller.Reply($"参数越界:应在 {min}~{max} 之间");
            return false;
        }
    }
}
