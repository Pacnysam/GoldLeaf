using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using GoldLeaf.Core;
using Terraria.Utilities;
using Terraria.DataStructures;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using Terraria.Audio;

namespace GoldLeaf.Prefixes.Fishing
{
    public class Repulsive : FishingRodPrefix
    {
        public override int FishingPower => -15;
    }
    public class Flimsy : FishingRodPrefix
    {
        public override int FishingPower => -5;
        public override int LineSnapChance => 20;
    }
    public class Wiry : FishingRodPrefix
    {
        public override int LineSnapChance => 45;
    }
    public class Shallow : FishingRodPrefix
    {
        public override int FishingPower => -5;
        public override int BaitSaveChance => 5;
    }
    public class Sensitive : FishingRodPrefix
    {
        public override int FishingPower => 5;
        public override int LineSnapChance => 20;
    }
    public class Steady : FishingRodPrefix
    {
        public override int FishingPower => 10;
        public override int BaitSaveChance => 15;
    }
    public class Luring : FishingRodPrefix
    {
        public override int FishingPower => 20;
    }
    public class Resourceful : FishingRodPrefix
    {
        public override int BaitSaveChance => 50;
    }
    public class Riveting : FishingRodPrefix
    {
        public override int FishingPower => 5;
        public override int BaitSaveChance => 10;
    }
    public class Sonorous : FishingRodPrefix
    {
        public override bool HasTooltip => true;
        public override void ModifyValue(ref float valueMult) => valueMult *= 1.25f;
        public override void HeldEffects(Item item, Player player) => player.sonarPotion = true;
    }
    public class Profishient : FishingRodPrefix
    {
        public override int FishingPower => 15;
        public override int BaitSaveChance => 25;
        public override int ExtraBobbers => 2;
        public override float RollChance(Item item) => 0.15f;
        public override void ModifyValue(ref float valueMult) => valueMult *= 1.5f;
    }
    public class Twin : FishingRodPrefix
    {
        public override int ExtraBobbers => 1;
    }
    public class Indiscriminate : FishingRodPrefix
    {
        public override int FishingPower => -20;
        public override int LineSnapChance => 35;
        public override int ExtraBobbers => 3;
    }
    public class Robust : FishingRodPrefix
    {
        public override int FishingPower => 5;
        public override bool HasTooltip => true;
        public override void ModifyValue(ref float valueMult) => valueMult *= 1.1f; 
        public override void HeldEffects(Item item, Player player) => player.accFishingLine = true; 
    }
    public class Meticulous : FishingRodPrefix
    {
        public override bool HasTooltip => true;
        public override void ModifyAttempt(ref FishingAttempt attempt)
        {
            if (Main.rand.NextBool(3) && attempt.questFish != -1 && !Main.LocalPlayer.HasItem(attempt.questFish) && NPC.AnyNPCs(NPCID.Angler) && !Main.anglerQuestFinished)
            {
                attempt.rolledItemDrop = attempt.questFish;
                if (!attempt.rare && !attempt.veryrare && !attempt.legendary)
                {
                    attempt.uncommon = true;
                    attempt.common = false;
                }
            }
        }
        public override float RollChance(Item item) => 0.5f;
        public override void ModifyValue(ref float valueMult) => valueMult *= 1.3f;
    }
    public class Prosperous : FishingRodPrefix
    {
        public override bool HasTooltip => true;
        public override void ModifyAttempt(ref FishingAttempt attempt)
        {
            if (attempt.playerFishingConditions.BaitItemType == ItemID.TruffleWorm) 
                return;

            if (Main.rand.NextBool(7))
            {
                if (!attempt.veryrare && !attempt.legendary)
                {
                    attempt.crate = true;

                    if (Main.rand.NextBool(3) && Main.LocalPlayer.cratePotion)
                    {
                        attempt.rare = true;
                        attempt.uncommon = false;
                        attempt.common = false;
                    }
                }
            }
        }
        public override float RollChance(Item item) => 0.35f;
        public override void ModifyValue(ref float valueMult) => valueMult *= 1.5f;
    }
    public class Ambitious : FishingRodPrefix
    {
        public override bool HasTooltip => true;
        public override void ModifyAttempt(ref FishingAttempt attempt)
        {
            if (attempt.playerFishingConditions.BaitItemType == ItemID.TruffleWorm)
                return;

            int seed = Main.rand.Next(100) + 1;

            if (seed <= 20)
            {
                if (Main.rand.NextBool(Main.LocalPlayer.cratePotion ? 10 : 5))
                    attempt.crate = false;

                ImproveRarity(ref attempt);
            }
            if (seed <= 5)
                ImproveRarity(ref attempt);
        }
        public override void ModifyValue(ref float valueMult) => valueMult *= 1.25f;
    }
    public class Callous : FishingRodPrefix
    {
        public override bool IsNegative => true;
        public override bool HasTooltip => true;
        public override void ModifyAttempt(ref FishingAttempt attempt)
        {
            if (attempt.playerFishingConditions.BaitItemType == ItemID.TruffleWorm)
                return;

            int seed = Main.rand.Next(100) + 1;

            if (seed <= 20)
            {
                attempt.crate = false;
                DowngradeRarity(ref attempt);
            }
            if (seed <= 5)
                DowngradeRarity(ref attempt);
        }
        
        public override void ModifyCatch(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn)
        {
            if (!attempt.legendary && !attempt.veryrare && !attempt.rare && !attempt.uncommon && !attempt.common) 
            {
                itemDrop = Main.rand.Next(ItemID.OldShoe, ItemID.TinCan + 1);

                if (Main.rand.NextBool(8))
                    itemDrop = ItemID.JojaCola;
            }
        }
        public override void ModifyValue(ref float valueMult) => valueMult *= 0.75f;
    }
    public class Fishless : FishingRodPrefix
    {
        public override int FishingPower => -20;
        public override int LineSnapChance => 50;
        public override bool HasTooltip => true;
        public override Color? TooltipColorOverride => ColorHelper.RarityColor(ItemRarityID.Gray);
        public override void ModifyCatch(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn)
        {
            itemDrop = Main.rand.Next(ItemID.OldShoe, ItemID.TinCan + 1);

            if (Main.rand.NextBool(8))
                itemDrop = ItemID.JojaCola;
        }
    }
    public class Adaptable : FishingRodPrefix
    {
        public override bool HasTooltip => true;
        public override void ModifyAttempt(ref FishingAttempt attempt)
        {
            if (attempt.playerFishingConditions.BaitItemType == ItemID.TruffleWorm)
                return;

            if (attempt.inLava && attempt.CanFishInLava || attempt.inHoney)
            {
                attempt.fishingLevel += 20;

                if (Main.rand.NextBool(5))
                    ImproveRarity(ref attempt);
            }
        }
        public override void ModifyValue(ref float valueMult) => valueMult *= 1.15f;
    }
    public class Consequential : FishingRodPrefix
    {
        public override DangerLevel FishingSafety => DangerLevel.Dangerous;
        public override bool HasTooltip => true;
        public override bool IsNegative => true;
        public override bool CanRoll(Item item) => Main.hardMode;
        public override float RollChance(Item item) => 0.05f;
        public override void HeldEffects(Item item, Player player)
        {
            player.sonarPotion = false;
            player.accFishingLine = true;
        }
        public override void ModifyCatch(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn)
        {
            if (attempt.playerFishingConditions.BaitItemType == ItemID.TruffleWorm && attempt.waterTilesCount < attempt.waterNeededToFish)
                npcSpawn = NPCID.MoonLordCore;

            if (!Main.rand.NextBool(3) || attempt.playerFishingConditions.BaitItemType == ItemID.TruffleWorm) return;

            itemDrop = 0;
            if (attempt.common || attempt.uncommon)
                npcSpawn = NPCID.Demon;
            if (attempt.rare || attempt.veryrare)
                npcSpawn = NPCID.RedDevil;
            if (attempt.legendary)
                npcSpawn = NPCID.DungeonGuardian;
        }
        public override void ModifyValue(ref float valueMult) => valueMult *= 0.5f;
    }
    public class Miraculous : FishingRodPrefix
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override DangerLevel FishingSafety => DangerLevel.Wonderful;
        public override bool HasTooltip => true;
        public override bool CanRoll(Item item) => Main.hardMode;
        public override float RollChance(Item item) => 0.05f;
        public override void HeldEffects(Item item, Player player)
        {
            player.sonarPotion = true;
            player.accFishingLine = true;
        }
        public override void PostRollFish(Projectile projectile, ref FishingAttempt attempt)
        {
            if (attempt.playerFishingConditions.BaitItemType == ItemID.TruffleWorm) return;

            if (true)//(attempt.rare && Main.rand.NextBool(20) || attempt.veryrare && Main.rand.NextBool(5) || attempt.legendary)
            {
                attempt.rolledEnemySpawn = 0;
                Projectile.NewProjectileDirect(projectile.GetSource_Misc("PostRollFish"), projectile.Center, new Vector2(0f, -4f), ProjectileID.CoinPortal, 0, 0, projectile.owner);
                
                if (!Main.dedServ)
                {
                    SoundEngine.PlaySound(SoundID.ResearchComplete with { Pitch = 0.5f });
                }
            }
        }
        public override void ModifyValue(ref float valueMult) => valueMult *= 1.75f;
    }
}
