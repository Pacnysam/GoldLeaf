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

namespace GoldLeaf.Prefixes.Enchantments
{
    public class Vital : TooltipPrefix
    {
        //public override PrefixCategory Category => PrefixCategory.AnyWeapon;
        public override bool IsNegative => false;
        public override void Apply(Item item)
        {
            
        }
    }
}
