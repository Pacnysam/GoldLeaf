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
using GoldLeaf.Items.Grove.Boss;
using GoldLeaf.Tiles.Decor;

namespace GoldLeaf.Items.Grove
{
	/*public class RemorseCandle : ModItem
	{
        public override void SetDefaults()
		{
			Item.width = 10;
			Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.White;
            //Item.createTile = TileType<EchobarkT>();
		}

        public override void PostUpdate()
        {
            Lighting.AddLight((int)((Item.position.X + (Item.width / 2)) / 16f), (int)((Item.position.Y + (Item.height / 2)) / 16f), (230f / 255) * 0.4f, (74 / 255) * 0.4f, (255f / 255) * 0.4f);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D glowTex = Request<Texture2D>(Texture + "Glow").Value;
            spriteBatch.Draw
            (
                glowTex,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - glowTex.Height * 0.5f
                ),
                new Rectangle(0, 0, glowTex.Width, glowTex.Height),
                Color.White,
                rotation,
                glowTex.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
        }

        /public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<WaxCandle>(), 1);
            recipe.AddIngredient(ItemType<EveDroplet>(), 3);

            //recipe.AddOnCraftCallback(RecipeCallbacks.RemorseCandleCraftEffect);
            recipe.Register();
        }
    }*/
}