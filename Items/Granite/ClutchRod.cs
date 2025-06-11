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
using ReLogic.Content;
using GoldLeaf.Tiles.Granite;
using GoldLeaf.Items.Nightshade;
using Terraria.Utilities;

namespace GoldLeaf.Items.Granite
{
	public class ClutchRod : ModItem
	{
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }
        public override void SetStaticDefaults()
        {
            ItemSets.Glowmask[Type] = (glowTex, ColorHelper.AdditiveWhite(), true);
        }

        //public override bool? PrefixChance(int pre, UnifiedRandom rand) => false;
        //public override int ChoosePrefix(UnifiedRandom rand) => 0;

        public override void SetDefaults()
		{
			Item.damage = 14;
			Item.DamageType = DamageClass.Summon;

            Item.knockBack = 12f;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 30;
            Item.noMelee = true;

            Item.shoot = ProjectileID.MagicMissile;

            ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;

            Item.staff[Type] = true;

            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Green;

            Item.channel = true;

            Item.width = 38;
            Item.height = 40;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 15)
                .AddIngredient(ItemID.Granite, 10)
                .AddRecipeGroup("GoldLeaf:GoldBars", 2)
                .AddCondition(GoldLeafConditions.LearnedRecipe(Item.type))
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}