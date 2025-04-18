using System;
using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Items.Grove;
using System.Diagnostics.Metrics;
using GoldLeaf.Effects.Dusts;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using GoldLeaf.Items.Misc.Accessories;
using ReLogic.Content;

namespace GoldLeaf.Items.Blizzard
{
    public class Avalanche : ModItem
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 24;

            Item.damage = 9;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 2.4f;
            Item.autoReuse = true;

            Item.GetGlobalItem<GoldLeafItem>().critDamageMod = -0.5f;

            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("GoldLeaf:SilverBars", 8);
            //recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddIngredient(ItemType<AuroraCluster>(), 15);
            recipe.AddTile(TileID.Anvils);
            recipe.AddOnCraftCallback(RecipeCallbacks.AuroraCraftEffect);
            recipe.Register();
        }

        /*public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Color color = ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 3); color.A = 0;

            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 3), rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, color * (0.3f - (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f), rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);
        }*/
    }
}
