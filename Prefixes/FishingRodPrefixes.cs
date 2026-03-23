using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Enums;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Core;
using Terraria.Utilities;
using Terraria.ModLoader.IO;
using Terraria.Audio;
using Terraria.DataStructures;

namespace GoldLeaf.Prefixes
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
        public override string Description => this.GetLocalizedValue("Tooltip");
        public override bool Sonar => true;
    }
    public class Profishient : FishingRodPrefix
    {
        public override int FishingPower => 15;
        public override int BaitSaveChance => 25;
        public override int ExtraBobbers => 2;

        public override float RollChance(Item item) => 0.25f;
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
    public class Meticulous : FishingRodPrefix
    {
        public override float RollChance(Item item) => 0.2f;
        public override string Description => this.GetLocalizedValue("Tooltip");
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
    }
    public class Fishless : FishingRodPrefix
    {
        public override int FishingPower => -20;
        //public override int LineSnapChance => 50;
        public override float RollChance(Item item) => 0.2f;
        public override void ModifyAttempt(ref FishingAttempt attempt)
        {

        }
        public override string Description => this.GetLocalizedValue("Tooltip");
    }
    public class Consequential : FishingRodPrefix
    {
        public override void ModifyCatch(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn)
        {
            if (attempt.playerFishingConditions.BaitItemType == ItemID.TruffleWorm || !Main.rand.NextBool(3)) return;

            itemDrop = 0;
            if (attempt.common || attempt.uncommon)
                npcSpawn = NPCID.Demon;
            if (attempt.rare || attempt.veryrare)
                npcSpawn = NPCID.RedDevil;
            if (attempt.legendary)
                npcSpawn = NPCID.DungeonGuardian;
        }
        public override bool Dangerous => true;
        public override string Description => this.GetLocalizedValue("Tooltip");
        public override bool IsNegative => true;
        public override bool CanRoll(Item item) => Main.hardMode;
        public override float RollChance(Item item) => 0.025f;
    }
    public class Robust : FishingRodPrefix
    {
        public override int FishingPower => 5;
        public override bool Unbreakable => true;
    }

    public abstract class FishingRodPrefix : ModPrefix
    {
        public static List<int> fishingRodPrefixes = [];
        public virtual int FishingPower => 0;
        public virtual int BaitSaveChance => 0;
        public virtual int LineSnapChance => 0;
        public virtual int ExtraBobbers => 0;
        public virtual bool Sonar => false;
        public virtual bool Unbreakable => false;
        public virtual bool Dangerous => false;
        public virtual string Description => "";
        public virtual bool IsNegative => false;

        public virtual void ModifyCatch(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn) { }
        public virtual void ModifyAttempt(ref FishingAttempt attempt) { }

        public override PrefixCategory Category => PrefixCategory.Custom;

        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= Math.Clamp(1f + (FishingPower * 0.015f) + (BaitSaveChance * 0.0075f) - (LineSnapChance * 0.005f) + (ExtraBobbers * 0.15f) + (Sonar? 0.25f : 0) 
                + (Unbreakable ? 0.1f : 0) - (Dangerous ? 0.15f : 0), 0.25f, 2.25f);
        }

        //public override bool AllStatChangesHaveEffectOn(Item item) => item.fishingPole + FishingPower > 0;
        public override bool CanRoll(Item item) => item.fishingPole > 0;
        
        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            if (ExtraBobbers > 0)
            {
                yield return new TooltipLine(Mod, "ExtraBobbersTooltip", ExtraBobbersTooltip.Format(ExtraBobbers))
                {
                    IsModifier = true,
                    IsModifierBad = false,
                };
            }
            if (BaitSaveChance > 0)
            {
                yield return new TooltipLine(Mod, "FishingRodBaitTooltip", FishingRodBaitTooltip.Format(BaitSaveChance))
                {
                    IsModifier = true,
                    IsModifierBad = false,
                };
            }
            if (LineSnapChance > 0)
            {
                yield return new TooltipLine(Mod, "FishingRodLineSnapTooltip", FishingRodLineSnapTooltip.Format(LineSnapChance))
                {
                    IsModifier = true,
                    IsModifierBad = true,
                };
            }
            if (Unbreakable)
            {
                yield return new TooltipLine(Mod, "UnbreakableLineTooltip", UnbreakableLineTooltip.Format())
                {
                    IsModifier = true,
                    IsModifierBad = false,
                };
            }
            if (Dangerous)
            {
                yield return new TooltipLine(Mod, "DangerousCatchTooltip", DangerousCatchTooltip.Format())
                {
                    IsModifier = true,
                    IsModifierBad = true,
                    OverrideColor = Color.Crimson,
                };
            }
            if (Description != "") 
            {
                yield return new TooltipLine(Mod, "PrefixTooltip", Description)
                {
                    IsModifier = true,
                    IsModifierBad = IsNegative,
                };
            }
        }
        public static LocalizedText ExtraBobbersTooltip { get; private set; }
        public static LocalizedText FishingRodBaitTooltip { get; private set; }
        public static LocalizedText FishingRodLineSnapTooltip { get; private set; }
        public static LocalizedText UnbreakableLineTooltip { get; private set; }
        public static LocalizedText DangerousCatchTooltip { get; private set; }

        public override void SetStaticDefaults()
        {
            fishingRodPrefixes.Add(Type);

            ExtraBobbersTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(ExtraBobbersTooltip)}");
            FishingRodBaitTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(FishingRodBaitTooltip)}");
            FishingRodLineSnapTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(FishingRodLineSnapTooltip)}");
            UnbreakableLineTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(UnbreakableLineTooltip)}");
            DangerousCatchTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(DangerousCatchTooltip)}");
        }
        public sealed override void Unload()
        {
            fishingRodPrefixes = null;
        }
    }

    public class FishingRodPrefixItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.fishingPole > 0;

        public override bool? PrefixChance(Item item, int pre, UnifiedRandom rand)
        {
            if (item.fishingPole > 0)
            {
                switch (pre)
                {
                    case -3:
                        return true;
                    case -1:
                        return rand.NextBool(3);
                }
            }
            return base.PrefixChance(item, pre, rand);
        }

        public override void SetDefaults(Item entity)
        {
            base.SetDefaults(entity);

            List<int> list = FishingRodPrefix.fishingRodPrefixes;
            entity.Prefix(Main.rand.Next(list.Count()));
        }

        public override bool CanReforge(Item item)
        {
            if (item.fishingPole > 0)
                return true;

            return base.CanReforge(item);
        }

        public override int ChoosePrefix(Item item, UnifiedRandom rand)
        {
            if ((item.damage > 0 && rand.NextBool()) || item.fishingPole <= 0)
                return -1;

            List<int> list = FishingRodPrefix.fishingRodPrefixes;
            return list[rand.Next(list.Count)];
        }

        public override void Load()
        {
            On_Item.CanHavePrefixes += FishingRodCanHavePrefix;
            On_Item.CanRollPrefix += FishingRodCanRollPrefix;
        }
        public override void Unload()
        {
            On_Item.CanHavePrefixes -= FishingRodCanHavePrefix;
            On_Item.CanRollPrefix -= FishingRodCanRollPrefix;
        }
        
        private bool FishingRodCanHavePrefix(On_Item.orig_CanHavePrefixes orig, Item self)
        {
            bool result = orig(self);

            if (self.fishingPole > 0 && (self.type != ItemID.None && (self.maxStack == 1 || self.AllowReforgeForStackableItem) && self.ammo == 0))
                result = true;

            return result;
        }
        private bool FishingRodCanRollPrefix(On_Item.orig_CanRollPrefix orig, Item self, int prefix)
        {
            bool result = orig(self, prefix);

            ModPrefix actualPrefix = PrefixLoader.GetPrefix(prefix);

            if (self.fishingPole > 0 && actualPrefix is FishingRodPrefix)
                result = true;

            return result;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            ModPrefix prefix = PrefixLoader.GetPrefix(item.prefix);

            if (prefix is FishingRodPrefix fishPrefix)
            {
                TooltipLine tipLine = tooltips.Find(n => n.Name == "FishingPower");

                if (tipLine != null)
                {
                    string text = "";
                    int finalFishingPower = Math.Max(1, item.fishingPole + fishPrefix.FishingPower);//item.fishingPole + fishPrefix.FishingPower;
                    int changedFishingPower = finalFishingPower - item.fishingPole;//Math.Abs(finalFishingPower - item.fishingPole);

                    if (fishPrefix.FishingPower > 0)
                        text = Language.GetTextValue("GameUI.PrecentFishingPower", finalFishingPower + " [c/78BE78:(+" + changedFishingPower + "%)]");
                    else if (fishPrefix.FishingPower < 0)
                        text = Language.GetTextValue("GameUI.PrecentFishingPower", finalFishingPower + " [c/BE7878:(" + changedFishingPower + "%)]");

                    if (text != "")
                    {
                        text = text.Remove(text.IndexOf("%", text.IndexOf("%") + 1), 1);
                        text = text.Insert(text.IndexOf(finalFishingPower.ToString()) + finalFishingPower.ToString().Length, "%");
                        tooltips.ElementAt(tooltips.IndexOf(tipLine)).Text = text;
                    }
                }
            }
        }

        public override void HoldItem(Item item, Player player)
        {
            ModPrefix prefix = PrefixLoader.GetPrefix(item.prefix);
            if (item.fishingPole > 0 && prefix is FishingRodPrefix fishPrefix)
            {
                int finalFishingPower = Math.Max(1, item.fishingPole + fishPrefix.FishingPower);
                int changedFishingPower = finalFishingPower - item.fishingPole;

                player.sonarPotion = true;
                if (fishPrefix.Unbreakable || fishPrefix is Consequential)
                    player.accFishingLine = true;
                if (fishPrefix.FishingPower != 0)
                {
                    player.fishingSkill += changedFishingPower;
                }
            }
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            ModPrefix prefix = PrefixLoader.GetPrefix(item.prefix);
            if (prefix is FishingRodPrefix && (prefix as FishingRodPrefix).ExtraBobbers > 0 && item.fishingPole > 0)
            {
                bool reverse = false;
                for (int p = 1; p < (prefix as FishingRodPrefix).ExtraBobbers + 1; p++)
                {
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(4f * p)), type, damage, knockback, player.whoAmI);
                    reverse.Toggle();
                }
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
    }

    public class FishingRodPrefixPlayer : ModPlayer
    {
        public override void Load() => On_Player.ItemCheck_CheckFishingBobber_PickAndConsumeBait += SaveBaitChance;
        public override void Unload() => On_Player.ItemCheck_CheckFishingBobber_PickAndConsumeBait -= SaveBaitChance;

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            Item fishingPole = attempt.playerFishingConditions.Pole;
            ModPrefix modPrefix = PrefixLoader.GetPrefix(fishingPole.prefix);

            if (fishingPole.fishingPole > 0 && modPrefix is FishingRodPrefix prefix)
            {
                prefix.ModifyCatch(attempt, ref itemDrop, ref npcSpawn);
            }
        }

        public override void ModifyFishingAttempt(ref FishingAttempt attempt)
        {
            Item fishingPole = attempt.playerFishingConditions.Pole;
            ModPrefix modPrefix = PrefixLoader.GetPrefix(fishingPole.prefix);

            if (fishingPole.fishingPole > 0 && modPrefix is Fishless)
            {
                if (attempt.waterTilesCount > 75)
                    attempt.waterTilesCount = 75;
                attempt.waterNeededToFish = 10;
            }
            if (fishingPole.fishingPole > 0 && modPrefix is Shallow)
            {
                attempt.waterNeededToFish = 10;
            }

            if (fishingPole.fishingPole > 0 && modPrefix is FishingRodPrefix prefix)
            {
                prefix.ModifyAttempt(ref attempt);
            }
        }

        private void SaveBaitChance(On_Player.orig_ItemCheck_CheckFishingBobber_PickAndConsumeBait orig, Player self, Projectile bobber, out bool pullTheBobber, out int baitTypeUsed)
        {
            orig(self, bobber, out pullTheBobber, out baitTypeUsed);

            ModPrefix prefix = PrefixLoader.GetPrefix(self.HeldItem.prefix);
            if (pullTheBobber && prefix is FishingRodPrefix)
            {
                if (Main.rand.NextBool(Math.Clamp((prefix as FishingRodPrefix).LineSnapChance, 0, 100), 100))
                {
                    bobber.localAI[1] = 0; //gets rid of any catch
                    bobber.ai[0] = 2f; //instantly snaps line
                    bobber.netUpdate = true;
                }
                /*else if (Main.rand.NextBool(Math.Clamp((prefix as FishingRodPrefix).BaitSaveChance, 0, 100), 100))
                {
                    self.GetItem(self.whoAmI, new Item(baitTypeUsed), new GetItemSettings(false, true));

                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundID.Item35 with { Volume = 1.0f, Pitch = 1.0f });
                }*/
            }
        }

        public override bool? CanConsumeBait(Item bait)
        {
            ModPrefix prefix = PrefixLoader.GetPrefix(Main.LocalPlayer.HeldItem.prefix);
            int baitSaveChance = Math.Clamp((prefix as FishingRodPrefix).BaitSaveChance, 0, 100);

            if (Main.rand.NextBool(baitSaveChance, 100))
                return false;

            return null;
        }
    }
}
