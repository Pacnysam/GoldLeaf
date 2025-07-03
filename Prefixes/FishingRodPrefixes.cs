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
        public override int LineSnapChance => 25;
    }
    public class Shallow : FishingRodPrefix
    {
        public override int FishingPower => -2;
        public override int BaitSaveChance => 3;
    }
    public class Sensitive : FishingRodPrefix
    {
        public override int FishingPower => 3;
        public override int LineSnapChance => 15;
        public override bool Sonar => true;
    }
    public class Steady : FishingRodPrefix
    {
        public override int FishingPower => 8;
        public override int BaitSaveChance => 9;
    }
    public class Luring : FishingRodPrefix
    {
        public override int FishingPower => 10;
    }
    public class Resourceful : FishingRodPrefix
    {
        public override int BaitSaveChance => 16;
    }
    public class Riveting : FishingRodPrefix
    {
        public override int FishingPower => 5;
        public override int BaitSaveChance => 6;
    }
    public class Sonorous : FishingRodPrefix
    {
        public override bool Sonar => true;
    }
    public class Profishient : FishingRodPrefix
    {
        public override int FishingPower => 10;
        public override int BaitSaveChance => 12;
        public override bool Sonar => true;

        public override float RollChance(Item item) => 0.3f;
    }
    public class Fishless : FishingRodPrefix
    {
        public override int FishingPower => -15;
        public override int LineSnapChance => 45;
        public override bool Fearful => true;
        public override float RollChance(Item item) => 0.2f;
    }
    public class Consequential : FishingRodPrefix
    {
        public override int FishingPower => 40;
        public override int LineSnapChance => 95;
        public override bool OldMan => true;
        public override float RollChance(Item item) => 0.025f;
    }

    public abstract class FishingRodPrefix : ModPrefix
    {
        public static List<int> fishingRodPrefixes = [];
        //public static readonly List<FishingRodPrefix> fishingRodPrefixes = [];
        public virtual int FishingPower => 0;
        public virtual int BaitSaveChance => 0;
        public virtual int LineSnapChance => 0;
        public virtual bool Sonar => false;
        public virtual bool Fearful => false;
        public virtual bool OldMan => false;

        public override PrefixCategory Category => PrefixCategory.Custom;

        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + (((FishingPower * 0.026875f) + (BaitSaveChance * 0.00685f) + (Sonar? 0.1f : 0) + (Fearful ? -0.5f : 0) - (LineSnapChance * 0.0065f)) * (OldMan ? 0 : 1));
        }

        public override bool AllStatChangesHaveEffectOn(Item item)
        {
            return item.fishingPole + FishingPower > 0;
        }

        public override bool CanRoll(Item item)
        {
            return item.fishingPole > 0;
        }

        public override void Apply(Item item)
        {
            //item.fishingPole += FishingPower;
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            /*if (FishingPower != 0)
            {
                yield return new TooltipLine(Mod, "FishingRodPowerTooltip", FishingRodPowerTooltip.Format(FishingPower))
                {
                    IsModifier = true,
                    IsModifierBad = FishingPower < 0,
                };
            }*/
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
            if (Sonar)
            {
                yield return new TooltipLine(Mod, "FishingRodSonarTooltip", FishingRodSonarTooltip.Format())
                {
                    IsModifier = true,
                    IsModifierBad = false,
                };
            }
            if (Fearful)
            {
                yield return new TooltipLine(Mod, "FishFearMeTooltip", FishFearMeTooltip.Format())
                {
                    IsModifier = true,
                    IsModifierBad = true,
                };
            }
            if (OldMan)
            {
                yield return new TooltipLine(Mod, "ConsequentialTooltip", ConsequentialTooltip.Format())
                {
                    IsModifier = true,
                    IsModifierBad = true,
                };
            }
        }

        //public static LocalizedText FishingRodPowerTooltip { get; private set; }
        public static LocalizedText FishingRodBaitTooltip { get; private set; }
        public static LocalizedText FishingRodLineSnapTooltip { get; private set; }
        public static LocalizedText FishingRodSonarTooltip { get; private set; }
        public static LocalizedText FishFearMeTooltip { get; private set; }
        public static LocalizedText ConsequentialTooltip { get; private set; }

        public override void SetStaticDefaults()
        {
            fishingRodPrefixes.Add(Type);

            //FishingRodPowerTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(FishingRodPowerTooltip)}");
            FishingRodBaitTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(FishingRodBaitTooltip)}");
            FishingRodLineSnapTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(FishingRodLineSnapTooltip)}");
            FishingRodSonarTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(FishingRodSonarTooltip)}");
            FishFearMeTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(FishFearMeTooltip)}");
            ConsequentialTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(ConsequentialTooltip)}");
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
        }

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

                if ((prefix as FishingRodPrefix).FishingPower != 0)
                    player.fishingSkill += (prefix as FishingRodPrefix).FishingPower;
            }
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
                else if (Main.rand.NextBool(Math.Clamp((prefix as FishingRodPrefix).BaitSaveChance, 0, 100), 100))
                {
                    self.GetItem(self.whoAmI, new Item(baitTypeUsed), new GetItemSettings(false, true));

                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundID.Item35 with { Volume = 1.0f, Pitch = 1.0f });
                }
            }
        }
    }
}
