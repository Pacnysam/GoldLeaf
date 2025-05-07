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

        public override bool? PrefixChance(int pre, UnifiedRandom rand)
        {
            return false;
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            return 0;
        }

        public override void SetDefaults()
		{
			Item.damage = 14;
			Item.DamageType = DamageClass.Summon;

            Item.knockBack = 12f;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 30;

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
                .AddIngredient(ItemID.Wood, 15)
                .AddIngredient(ItemID.Granite, 10)
                .AddRecipeGroup("GoldLeaf:GoldBars", 2)
                .AddCondition(GoldLeafConditions.LearnedRecipe(Item.type))
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useStyle = ItemUseStyleID.Swing;
                Item.useTime = 10;
                Item.useAnimation = 15;

                Item.consumable = true;

                Item.shoot = ProjectileID.None;
                Item.createTile = TileType<BuriedClutchRod>();
                Item.NetStateChanged();
            }
            else
            {
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.useTime = Item.useAnimation = 30;

                Item.consumable = false;
                Item.shoot = ProjectileID.MagicMissile;
                Item.createTile = -1;
                Item.NetStateChanged();
            }
            return true;
        }

        /*public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.useTime = Item.useAnimation = 30;

                Item.consumable = false;
                Item.shoot = ProjectileID.MagicMissile;
                Item.createTile = -1;
            }
            else
            {
                Item.useStyle = ItemUseStyleID.Swing;
                Item.useTime = 10;
                Item.useAnimation = 15;

                Item.consumable = true;

                Item.shoot = ProjectileID.None;
                Item.createTile = TileType<BuriedClutchRod>();
            }
            return true;
        }*/

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, Color.White, rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);
        }
    }
}