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

namespace GoldLeaf.Items.Granite
{
	public class Bladecaster : ModItem
	{
        public override void SetDefaults()
		{
			Item.damage = 19;
			Item.DamageType = DamageClass.Melee;

            //Item.useTurn = true;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 30;
			Item.useAnimation = 30;

			Item.crit = 28;
			Item.GetGlobalItem<GoldLeafItem>().critDamageMod = 1.5f;

            Item.knockBack = 8f;

            Item.value = Item.sellPrice(0, 1, 25, 0);
            Item.rare = ItemRarityID.Blue;

            Item.width = 38;
            Item.height = 40;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            //player.itemTime += 10;
            //player.itemAnimation += 10;
        }
    }
}