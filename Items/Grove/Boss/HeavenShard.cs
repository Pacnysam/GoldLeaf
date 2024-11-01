using Terraria;
using static Terraria.ModLoader.ModContent;
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

namespace GoldLeaf.Items.Grove.Boss
{
	public class HeavenShard : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.White;

            ItemID.Sets.ItemNoGravity[Item.type] = true;
            ItemID.Sets.ShimmerTransformToItem[Item.type] = ItemType<EveDroplet>();
        }
    }
}