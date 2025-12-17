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
    public class Barren : FishingRodPrefix
    {
        public override int FishingPower => -10;
    }
    public class Flimsy : FishingRodPrefix
    {
        public override int FishingPower => -5;
        public override int LineSnapChance => 20;
    }
    public class Wiry : FishingRodPrefix
    {
        public override int LineSnapChance => 30;
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
        public override int BaitSaveChance => 12;
    }
    public class Luring : FishingRodPrefix
    {
        public override int FishingPower => 20;
    }
    public class Resourceful : FishingRodPrefix
    {
        public override int BaitSaveChance => 30;
    }
    public class Riveting : FishingRodPrefix
    {
        public override int FishingPower => 5;
        public override int BaitSaveChance => 10;
    }
    public class Sonorous : FishingRodPrefix
    {
        public override bool Sonar => true;
    }
    public class Profishient : FishingRodPrefix
    {
        public override int FishingPower => 15;
        public override int BaitSaveChance => 20;
        public override int ExtraBobbers => 2;

        public override float RollChance(Item item) => 0.15f;
    }
    public class Twin : FishingRodPrefix
    {
        public override int ExtraBobbers => 1;
    }
    public class Indiscriminate : FishingRodPrefix
    {
        public override int FishingPower => -15;
        public override int LineSnapChance => 20;
        public override int ExtraBobbers => 3;
    }
    public class Fishless : FishingRodPrefix
    {
        //public override int FishingPower => -15;
        //public override int LineSnapChance => 50;
        public override float RollChance(Item item) => 0.2f;
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 0.25f;
        }
    }
    public class Consequential : FishingRodPrefix
    {
        //public override bool IsLoadingEnabled(Mod mod) => false;
        public override void ModifyCatch(ref FishingAttempt attempt)
        {
            attempt.rolledItemDrop = 0;
            attempt.rolledEnemySpawn = NPCID.Demon;
        }
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

        public virtual void ModifyCatch(ref FishingAttempt attempt) { }

        public override PrefixCategory Category => PrefixCategory.Custom;

        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + Math.Clamp((FishingPower * 0.0285f) + (BaitSaveChance * 0.0065f) + (Sonar? 0.1f : 0) - (LineSnapChance * 0.0085f), 0.15f, 2.25f);
        }

        public override bool AllStatChangesHaveEffectOn(Item item) => item.fishingPole + FishingPower > 0;
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
            if (Sonar)
            {
                yield return new TooltipLine(Mod, "FishingRodSonarTooltip", FishingRodSonarTooltip.Format())
                {
                    IsModifier = true,
                    IsModifierBad = false,
                };
            }
        }

        public static LocalizedText ExtraBobbersTooltip { get; private set; }
        public static LocalizedText FishingRodBaitTooltip { get; private set; }
        public static LocalizedText FishingRodLineSnapTooltip { get; private set; }
        public static LocalizedText UnbreakableLineTooltip { get; private set; }
        public static LocalizedText FishingRodSonarTooltip { get; private set; }

        public override void SetStaticDefaults()
        {
            fishingRodPrefixes.Add(Type);

            ExtraBobbersTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(ExtraBobbersTooltip)}");
            FishingRodBaitTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(FishingRodBaitTooltip)}");
            FishingRodLineSnapTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(FishingRodLineSnapTooltip)}");
            UnbreakableLineTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(UnbreakableLineTooltip)}");
            FishingRodSonarTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(FishingRodSonarTooltip)}");
        }
        public sealed override void Unload()
        {
            fishingRodPrefixes = null;
        }
    }

    public class FishingRodPrefixItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.fishingPole > 0;
        }

        public override void SetDefaults(Item entity)
        {
            base.SetDefaults(entity);
            
            List<int> list = FishingRodPrefix.fishingRodPrefixes;
            entity.Prefix(Main.rand.Next(list.Count()));
        }

        public override bool? PrefixChance(Item item, int pre, UnifiedRandom rand)
        {
            if (item.fishingPole > 0)
            {
                switch (pre)
                {
                    case -3:
                        return true;
                    case -1:
                        return rand.NextBool(4);
                }
            }
            return base.PrefixChance(item, pre, rand);
        }

        public override bool CanReforge(Item item)
        {
            if (item.fishingPole > 0)
                return true;

            return base.CanReforge(item);
        }

        public override void PreReforge(Item item)
        {
            if (item.fishingPole > 0)
                item.accessory = true;
        }
        public override void PostReforge(Item item)
        {
            if (item.fishingPole > 0)
                item.accessory = false;
        }

        public override int ChoosePrefix(Item item, UnifiedRandom rand)
        {
            if ((item.damage > 0 && rand.NextBool()) || item.fishingPole <= 0)
                return -1;

            List<int> list = FishingRodPrefix.fishingRodPrefixes;
            return list[rand.Next(list.Count())];
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

        /*public override int ChoosePrefix(Item item, UnifiedRandom rand)
        {
            if (item.fishingPole > 0)
            {
                List<int> list = FishingRodPrefix.fishingRodPrefixes;
                return list[rand.Next(list.Count())];
            }

            return -1;
        }*/

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            ModPrefix prefix = PrefixLoader.GetPrefix(item.prefix);

            if (prefix is FishingRodPrefix)
            {
                TooltipLine tipLine = tooltips.Find(n => n.Name == "FishingPower");

                if (tipLine != null)
                {
                    string text = "";
                    int finalFishingPower = item.fishingPole + (prefix as FishingRodPrefix).FishingPower;

                    if ((prefix as FishingRodPrefix).FishingPower > 0)
                        text = Language.GetTextValue("GameUI.PrecentFishingPower", finalFishingPower + " [c/78BE78:(+" + (prefix as FishingRodPrefix).FishingPower + "%)]");
                    else if ((prefix as FishingRodPrefix).FishingPower < 0)
                        text = Language.GetTextValue("GameUI.PrecentFishingPower", finalFishingPower + " [c/BE7878:(" + (prefix as FishingRodPrefix).FishingPower + "%)]");

                    if (text != "")
                    {
                        text = text.Remove(text.IndexOf("%", text.IndexOf("%") + 1), 1);
                        text = text.Insert(text.IndexOf(finalFishingPower.ToString()) + finalFishingPower.ToString().Length, "%");
                        tooltips.ElementAt(tooltips.IndexOf(tipLine)).Text = text;
                    }
                }
            }
        } //adjust fishing rod tooltip

        public override void SaveData(Item item, TagCompound tag)
        {
            if (item.fishingPole > 0 && item.prefix != 0)
            {
                tag["FishingRodPrefix"] = item.prefix;
            }
        }
        public override void LoadData(Item item, TagCompound tag)
        {
            if (item.fishingPole > 0)
            {
                item.accessory = true;
                item.Prefix(tag.Get<int>("FishingRodPrefix"));
                item.accessory = false;
            }
        }

        public override void HoldItem(Item item, Player player)
        {
            ModPrefix prefix = PrefixLoader.GetPrefix(item.prefix);
            if (item.fishingPole > 0 && prefix is FishingRodPrefix)
            {
                if ((prefix as FishingRodPrefix).Sonar)
                    player.sonarPotion = true;
                if ((prefix as FishingRodPrefix).Unbreakable)
                    player.accFishingLine = true;
                if ((prefix as FishingRodPrefix).FishingPower != 0)
                    player.fishingSkill += (prefix as FishingRodPrefix).FishingPower;
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

    public class FishingRodPrefixProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void Load()
        {
            
        }
    }

    public class FishingRodPrefixPlayer : ModPlayer
    {
        public override void Load()
        {
            On_Player.ItemCheck_CheckFishingBobber_PickAndConsumeBait += SaveBaitChance;
        }

        public override void Unload()
        {
            On_Player.ItemCheck_CheckFishingBobber_PickAndConsumeBait -= SaveBaitChance;
        }

        public override void ModifyFishingAttempt(ref FishingAttempt attempt)
        {
            Item fishingPole = attempt.playerFishingConditions.Pole;
            ModPrefix prefix = PrefixLoader.GetPrefix(fishingPole.prefix);

            if (fishingPole.fishingPole > 0 && prefix is FishingRodPrefix) 
            {
                (prefix as FishingRodPrefix).ModifyCatch(ref attempt);
            }

            if (fishingPole.fishingPole > 0 && prefix is Fishless)
            {
                if (attempt.waterTilesCount > 75)
                    attempt.waterTilesCount = 75;
                attempt.waterNeededToFish = 10;
            }
            if (fishingPole.fishingPole > 0 && prefix is Shallow)
            {
                attempt.waterNeededToFish = 10;
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
