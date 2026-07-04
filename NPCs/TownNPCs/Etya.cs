using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Localization;
using System.Data.OracleClient;

namespace EtyaMod.NPCs.TownNPCs
{
    public class Etya : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 25;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 700;
            NPCID.Sets.AttackType[NPC.type] = 2;
            NPCID.Sets.AttackTime[NPC.type] = 90;
            NPCID.Sets.AttackAverageChance[NPC.type] = 30;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPC.Happiness
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
                .SetBiomeAffection<DungeonBiome>(AffectionLevel.Like)
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike);
            // TODO: add liking npcs the other days;
            //.SetNPCAffection(NPCID.Wizard, AffectionLevel.Like)
            //.SetNPCAffection(NPCID.Cyborg, AffectionLevel.Dislike);
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);
        }
        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = NPCAIStyleID.Passive;
            if (Main.hardMode)
            {
                NPC.damage = 60;
                NPC.defense = 60;
            }
            else
            {
                NPC.damage = 20;
                NPC.defense = 20;
            }
            NPC.lifeMax = 25000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.8f;
            AnimationType = NPCID.Guide;
            NPC.homeless = false;
            int spawnTileX = Main.spawnTileX;
            int spawnTileY = Main.spawnTileY;
            NPC.Center = new Vector2(spawnTileX * 16, spawnTileY * 16);


        }


        // 图鉴
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.EtyaMod.Bestiary.Etya")
            });
        }

        // 入住条件
        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return true; // 初始生成;
        }

        // 姓名直接隐藏
        public override List<string> SetNPCNameList()
        {
            return new List<string>() // 不显示名字;
            {
            };
        }

        // 用国王雕像决定性别
        public override bool CanGoToStatue(bool toKingStatue)
        {

            return !toKingStatue;
        }



        // 当etya被传送时吐槽
        public override void OnGoToStatue(bool toKingStatue)
        {
            // 左下角弹出吐槽
            if (toKingStatue)
                Main.NewText("(⊙﹏⊙)");
            else
                Main.NewText("长得这么可爱一定不是男孩子啦~");
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();

            chat.Add(this.GetLocalizedValue("Chat.Normal1"));
            chat.Add(this.GetLocalizedValue("Chat.Normal2"));

            if (Main.bloodMoon)
            {
                chat.Add(this.GetLocalizedValue("Chat.BloodMoon1"));
                chat.Add(this.GetLocalizedValue("Chat.BloodMoon2"));
            }

            if (Main.eclipse)
            {
                chat.Add(this.GetLocalizedValue("Chat.Eclipse1"));
            }

            if (BirthdayParty.PartyIsUp)
            {
                chat.Add(this.GetLocalizedValue("Chat.Party"));
            }

            if (NPC.FindFirstNPC(NPCID.Guide) != -1)
            {
                chat.Add(this.GetLocalizedValue("Chat.GuidePresent"));
            }

            return chat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            // 引用原版的商店文本
            button = Language.GetTextValue("LegacyInterface.28"); // "Shop"
            button2 = "圣物";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                shopName = "Etya's Shop";
            }
        }

        public override void AddShops()
        {
            NPCShop shop = new(Type);
            shop.Add(ItemID.WoodenBow);
            shop.Add(ItemID.WoodenArrow);
            shop.Add(ItemID.GreenCap);
            shop.Add(ItemID.ThrowingKnife, Condition.NpcIsPresent(NPCID.Merchant));
            shop.Add(ItemID.Grenade, Condition.NpcIsPresent(NPCID.Demolitionist));
            shop.Add(ItemID.DyeTradersScimitar, Condition.NpcIsPresent(NPCID.DyeTrader));
            shop.Add(ItemID.FrostDaggerfish, Condition.NpcIsPresent(NPCID.Angler));
            shop.Add(ItemID.PainterPaintballGun, Condition.NpcIsPresent(NPCID.Painter));
            shop.Add(ItemID.FlintlockPistol, Condition.NpcIsPresent(NPCID.ArmsDealer), Condition.PreHardmode);
            shop.Add(ItemID.Minishark, Condition.NpcIsPresent(NPCID.ArmsDealer), Condition.Hardmode);
            shop.Add(ItemID.AleThrowingGlove, Condition.NpcIsPresent(NPCID.DD2Bartender));
            shop.Add(ItemID.StylistKilLaKillScissorsIWish, Condition.NpcIsPresent(NPCID.Stylist));
            shop.Add(ItemID.SpikyBall, Condition.NpcIsPresent(NPCID.GoblinTinkerer));
            shop.Add(ItemID.RedHat, Condition.NpcIsPresent(NPCID.Clothier));
            shop.Add(ItemID.CombatWrench, Condition.NpcIsPresent(NPCID.Mechanic));
            shop.Add(ItemID.PartyGirlGrenade, Condition.NpcIsPresent(NPCID.PartyGirl));
            shop.Add(ItemID.TaxCollectorsStickOfDoom, Condition.NpcIsPresent(NPCID.TaxCollector));
            shop.Add(ItemID.ClockworkAssaultRifle, Condition.NpcIsPresent(NPCID.Steampunker));
            shop.Add(ItemID.IvyGuitar, Condition.NpcIsPresent(NPCID.Steampunker));
            shop.Add(ItemID.PrincessWeapon, Condition.NpcIsPresent(NPCID.Princess));
            shop.Add(ItemID.Revolver, Condition.NpcIsPresent(NPCID.TravellingMerchant), Condition.PreHardmode);
            shop.Add(ItemID.PulseBow, Condition.NpcIsPresent(NPCID.TravellingMerchant), Condition.Hardmode);
            // 你们说，我做出来之前，red会不会突然更新，再加一个npc进去
            shop.Register();
        }

        // 设置攻击力
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = NPC.damage;
            knockback = 4f;
        }

        // 设置攻击速度
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 10;
            randExtraCooldown = 50;
        }

        // 设置弹幕类型
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ProjectileID.MagicMissile;
            attackDelay = 30; // 攻击间隔
        }

        // 设置魔法光环
        public override void TownNPCAttackMagic(ref float auraLightMultiplier)
        {
            auraLightMultiplier = 1f; 
        }
    }
}
