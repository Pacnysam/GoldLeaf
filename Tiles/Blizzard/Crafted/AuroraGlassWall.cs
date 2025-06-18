using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using GoldLeaf.Tiles.Grove.Ancient;
using Terraria.ObjectData;
using Terraria.Audio;
using GoldLeaf.Effects.Dusts;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using GoldLeaf.Items.Accessories;
using Terraria.GameContent;
using System;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Tiles.Marble;
using GoldLeaf.Core;
using ReLogic.Content;

namespace GoldLeaf.Tiles.Blizzard.Crafted
{
    public class AuroraGlassWallItem : ModItem
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
            ItemSets.Glowmask[Type] = (glowTex, ColorHelper.AdditiveWhite() * 0.3f, false);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(WallType<AuroraGlassWall>());
            Item.value = Item.sellPrice(0, 0, 0, 20);

            Item.width = Item.height = 32;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 1.5f) * 0.4f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(4)
            .AddIngredient(ItemType<AuroraGlassItem>())
            .AddTile(TileID.WorkBenches)
            .AddOnCraftCallback(RecipeCallbacks.AuroraMinor)
            .Register();
        }

        public override void UseAnimation(Player player)
        {
            Item.color = ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 1.5f);
        }
        public override void UpdateInventory(Player player)
        {
            Item.color = ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 1.5f);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            spriteBatch.Draw(TextureAssets.Item[Type].Value, Item.Center - Main.screenPosition, null, ColorHelper.AuroraAccentColor(Item.timeSinceItemSpawned * 0.05f) * 0.4f, rotation, TextureAssets.Item[Type].Value.Size() / 2, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite() * 0.3f, rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(TextureAssets.Item[Type].Value, position, null, ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 1.5f) * 0.4f, 0, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glowTex.Value, position, null, ColorHelper.AdditiveWhite() * 0.3f, 0, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class AuroraGlassWall : ModWall
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            RegisterItemDrop(ItemType<AuroraGlassWallItem>());
            
            AddMapEntry(new Color(10, 175, 215));

            HitSound = SoundID.DD2_WitherBeastCrystalImpact; //with { Pitch = -0.3f };
            DustType = DustType<AuroraTwinkle>();
            
            Main.wallHouse[Type] = true;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            if (item.type == ItemType<AuroraGlassWallItem>())
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact /* with { Pitch = -0.3f }*/, new Vector2(i * 16, j * 16));
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GoldLeafWall.DrawSimpleWall(i, j, TextureAssets.Wall[Type].Value, ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 6f + ((i + j) / 4f)) * 0.3f, Vector2.Zero);
            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GoldLeafWall.DrawSimpleWall(i, j, glowTex.Value, ColorHelper.AdditiveWhite() * 0.2f, Vector2.Zero);
        }
    }
}