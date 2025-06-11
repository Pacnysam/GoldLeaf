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

namespace GoldLeaf.Prefixes
{
    public class Flimsy : FishingRodPrefix
    {
        public override int FishingPower => -8;
    }
    public class Shallow : FishingRodPrefix
    {
        public override int FishingPower => -5;
        public override bool Sonar => true;

    }
    public class Steady : FishingRodPrefix
    {
        public override int FishingPower => 5;
        public override int BaitSaveChance => 5;
    }
    public class Luring : FishingRodPrefix
    {
        public override int FishingPower => 10;
    }
    public class Resourceful : FishingRodPrefix
    {
        public override int BaitSaveChance => 25;
    }
    public class Riveting : FishingRodPrefix
    {
        public override int FishingPower => 5;
        public override int BaitSaveChance => 10;
    }
    public class Sonorous : FishingRodPrefix
    {
        public override int FishingPower => 2;
        public override bool Sonar => true;
    }
    public class Profishient : FishingRodPrefix
    {
        public override int FishingPower => 8;
        public override int BaitSaveChance => 15;
        public override bool Sonar => true;

        public override float RollChance(Item item) => 0.5f;
    }

    public abstract class FishingRodPrefix : ModPrefix
    {
        public static List<int> fishingRodPrefixes = [];
        //public static readonly List<FishingRodPrefix> fishingRodPrefixes = [];
        public virtual int FishingPower => 0;
        public virtual int BaitSaveChance => 0;
        public virtual bool Sonar => false;

        //public override PrefixCategory Category => PrefixCategory.Custom;

        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + (FishingPower * 0.025f) + (BaitSaveChance * 0.025f) + (Sonar? 0.2f : 0);
        }

        public override bool AllStatChangesHaveEffectOn(Item item)
        {
            return item.fishingPole + FishingPower > 0;
        }
        //public override float RollChance(Item item) => 500f;

        public override bool CanRoll(Item item)
        {
            return item.fishingPole > 0;
        }

        public override void Apply(Item item)
        {
            item.fishingPole += FishingPower;
            item.GetGlobalItem<FishingRodPrefixItem>().baitSaveChance = BaitSaveChance;
            item.GetGlobalItem<FishingRodPrefixItem>().sonar = Sonar;
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            if (Sonar)
            {
                yield return new TooltipLine(Mod, "FishingRodSonarTooltip", FishingRodSonarTooltip.Format())
                {
                    IsModifier = true,
                    IsModifierBad = false,
                };
            }
            if (FishingPower != 0)
            {
                yield return new TooltipLine(Mod, "FishingRodPowerTooltip", FishingRodPowerTooltip.Format(FishingPower))
                {
                    IsModifier = true,
                    IsModifierBad = FishingPower < 0,
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
        }

        public static LocalizedText FishingRodPowerTooltip { get; private set; }
        public static LocalizedText FishingRodBaitTooltip { get; private set; }
       public static LocalizedText FishingRodSonarTooltip { get; private set; }

        /*public override void Load() => fishingRodPrefixes.Add(this);
        public override void Unload() => fishingRodPrefixes.Clear();*/

        public override void SetStaticDefaults()
        {
            fishingRodPrefixes.Add(Type);

            FishingRodPowerTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(FishingRodPowerTooltip)}");
            FishingRodBaitTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(FishingRodBaitTooltip)}");
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

        public bool sonar = false;
        public int baitSaveChance = 0;

        public override void SetDefaults(Item entity)
        {
            sonar = false;
            List<int> list = FishingRodPrefix.fishingRodPrefixes;
            entity.Prefix(Main.rand.Next(list.Count()));
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

        public override void HoldItem(Item item, Player player)
        {
            if (item.fishingPole > 0 && sonar)
            {
                player.sonarPotion = true;
            }
        }

        public override bool? CanConsumeBait(Player player, Item bait)
        {
            return !Main.rand.NextBool(Math.Clamp(baitSaveChance, 0, 100), 100);
        }
    }
}
