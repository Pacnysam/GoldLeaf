using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Effects.Dusts;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using GoldLeaf.Core;
using Mono.Cecil;
using Terraria.DataStructures;
using System;
using System.Diagnostics.Metrics;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using GoldLeaf.Items.Grove;
using Terraria.GameContent.ItemDropRules;
using Terraria.UI;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;

namespace GoldLeaf.Items.VanillaBossDrops
{
    public class Rattlestaff : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override void SetDefaults() 
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Summon;
        }
    }
}
