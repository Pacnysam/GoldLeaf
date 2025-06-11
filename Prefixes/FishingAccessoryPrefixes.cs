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

namespace GoldLeaf.Prefixes
{
    /*public class Luring : FishingAccessoryPrefix
    {
        public override int FishingPower => 2;
    }
    public class Profishient : FishingAccessoryPrefix
    {
        public override int FishingPower => 4;
    }

    public abstract class FishingAccessoryPrefix : ModPrefix
    {
        public virtual int FishingPower => 5;

        public override PrefixCategory Category => PrefixCategory.Accessory;

        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + (FishingPower * 0.05f);
        }

        public override void ApplyAccessoryEffects(Player player)
        {
            player.fishingSkill += FishingPower;
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            yield return new TooltipLine(Mod, "PrefixAccessoryFishingPower", FishingPowerAccessoryTooltip.Format(FishingPower))
            {
                IsModifier = true,
                IsModifierBad = FishingPower < 0,
            };
        }

        public static LocalizedText FishingPowerAccessoryTooltip { get; private set; }

        public override void SetStaticDefaults()
        {
            FishingPowerAccessoryTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(FishingPowerAccessoryTooltip)}");
        }
    }*/
}
