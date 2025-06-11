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
    public class Peppy : MinionSpeedPrefix
    {
        public override float MinionSpeed => 0.02f;
    }
    /*public class Spunky : MinionSpeedPrefix
    {
        public override float MinionSpeed => 0.02f;
    }
    public class Vivacious : MinionSpeedPrefix
    {
        public override float MinionSpeed => 0.03f;
    }*/
    public class Jubilated : MinionSpeedPrefix
    {
        public override float MinionSpeed => 0.04f;
    }

    public abstract class MinionSpeedPrefix : ModPrefix
    {
        public virtual float MinionSpeed => 0.05f;

        public override PrefixCategory Category => PrefixCategory.Accessory;

        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + (MinionSpeed * 7f);
        }

        public override void ApplyAccessoryEffects(Player player)
        {
            player.GetModPlayer<MinionSpeedPlayer>().summonSpeed += MinionSpeed;
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            yield return new TooltipLine(Mod, "PrefixAccessoryMinionSpeed", MinionSpeedTooltip.Format(MinionSpeed * 100))
            {
                IsModifier = true,
                IsModifierBad = MinionSpeed < 0,
            };
        }

        public static LocalizedText MinionSpeedTooltip { get; private set; }

        public override void SetStaticDefaults()
        {
            MinionSpeedTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(MinionSpeedTooltip)}");
        }
    }
}
