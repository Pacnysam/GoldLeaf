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
    public abstract class TooltipPrefix : ModPrefix
    {
        public LocalizedText Tooltip => this.GetLocalization(nameof(Tooltip));
        public virtual bool IsNegative => true;

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            yield return new TooltipLine(Mod, "PrefixWeaponAwesomeDescription", Tooltip.Value)
            {
                IsModifier = true,
                IsModifierBad = IsNegative,
            };
        }

        public override void SetStaticDefaults()
        {
            _ = Tooltip;
        }
    }
}
