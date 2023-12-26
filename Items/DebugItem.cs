using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using GoldLeaf.Core;
using Terraria.Audio;
using System.Diagnostics.Metrics;

namespace GoldLeaf.Items
{
    public class DebugItem : ModItem
    {
        public override string Texture => "GoldLeaf/Items/Sets/Blizzard/ArcticFlower";
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.staff[Item.type] = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.rare = ItemRarityID.Quest;

        }

        public override bool? UseItem(Player player)
        {
            float temp1 = player.GetModPlayer<GoldLeafPlayer>().temp1;
            float temp2 = player.GetModPlayer<GoldLeafPlayer>().temp2;

            //Gore.NewGorePerfect(Terraria.Entity.GetSource_None(), Main.MouseWorld, Vector2.Zero, GoreType<RingGoreRewrite>(), Scale: 0.7f + Main.rand.NextFloat(temp1, temp2) / 30f);
            return true;
        }
    }
}
