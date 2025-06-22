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
using GoldLeaf.Items.Accessories;
using ReLogic.Content;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;

namespace GoldLeaf.Items.Blizzard
{
    public class GlacialScissors : ModItem
    {
        private static Asset<Texture2D> glowTex;
        private static Asset<Texture2D> baseTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
            baseTex = Request<Texture2D>(Texture + "Base");
        }

        public override void SetStaticDefaults()
        {
            Item.AddElements([Element.Ice]);
            Item.SetSlash(true);
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 44;

            Item.damage = 32;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 7.8f;
            Item.autoReuse = true;
            Item.crit = 8;

            Item.value = Item.sellPrice(0, 1, 70, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<AuroraCluster>(), 18);
            recipe.AddTile(TileID.Anvils);
            recipe.AddOnCraftCallback(RecipeCallbacks.AuroraMajor);
            recipe.Register();
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Color color = ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 3); color.A = 0;

            spriteBatch.Draw(baseTex.Value, Item.Center - Main.screenPosition, null, lightColor, rotation, baseTex.Size() / 2, scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, ColorHelper.AuroraColor(Item.timeSinceItemSpawned * 0.05f) * (0.65f + (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f), rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, color * (0.75f - (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f), rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Color color = ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 3); color.A = 0;

            spriteBatch.Draw(baseTex.Value, position, null, drawColor, 0, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(glowTex.Value, position, null, ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 3f) * 0.75f * (0.65f + (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f), 0, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glowTex.Value, position, null, color * (0.75f - (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f), 0, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
