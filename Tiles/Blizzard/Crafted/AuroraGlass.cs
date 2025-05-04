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
using GoldLeaf.Items.Misc.Accessories;
using Terraria.GameContent;
using System;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Tiles.Marble;
using GoldLeaf.Core;
using ReLogic.Content;

namespace GoldLeaf.Tiles.Blizzard.Crafted
{
    public class AuroraGlassItem : ModItem
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<AuroraGlass>());
            Item.value = Item.sellPrice(0, 0, 0, 20);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(20)
            .AddIngredient(ItemType<AuroraCluster>())
            .AddTile(TileID.IceMachine)
            .AddOnCraftCallback(RecipeCallbacks.AuroraMinor)
            .Register();

            Recipe recipe2 = CreateRecipe()
            .AddIngredient(ItemType<AuroraGlassWallItem>(), 4)
            .AddTile(TileID.WorkBenches)
            .AddOnCraftCallback(RecipeCallbacks.AuroraMinor)
            .Register();
        }

        public override void UpdateInventory(Player player)
        {
            Item.color = ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 1.5f) * 0.4f;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            spriteBatch.Draw(TextureAssets.Item[Type].Value, Item.Center - Main.screenPosition, null, ColorHelper.AuroraAccentColor(Item.timeSinceItemSpawned * 0.05f) * 0.4f, rotation, TextureAssets.Item[Type].Value.Size() / 2, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite * 0.3f, rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(TextureAssets.Item[Type].Value, position, null, ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 1.5f) * 0.4f, 0, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glowTex.Value, position, null, ColorHelper.AdditiveWhite * 0.3f, 0, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class AuroraGlass : ModTile
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            LocalizedText name = CreateMapEntryName();
            RegisterItemDrop(ItemType<AuroraGlassItem>());
            AddMapEntry(new Color(0, 225, 255), name);

            MineResist = 0.1f;
            HitSound = SoundID.DD2_WitherBeastCrystalImpact;
            DustType = DustType<AuroraTwinkle>();
            
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = false;

            TileID.Sets.DrawsWalls[Type] = true;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            if (item.type == ItemType<AuroraGlassItem>())
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, new Vector2(i * 16, j * 16));
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GoldLeafTile.DrawSlopedGlowMask(i, j, TextureAssets.Tile[Type].Value, ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 6f + ((i + j)/4f)) * 0.4f, Vector2.Zero);
            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GoldLeafTile.DrawSlopedGlowMask(i, j, glowTex.Value, ColorHelper.AdditiveWhite * 0.3f, Vector2.Zero);
        }

        /*public override void FloorVisuals(Player player)
        {
            if (Main.rand.NextBool(10) && Math.Abs(player.velocity.X) > 2)
            {
                Gore.NewGore(null, player.Bottom, new Vector2(0f, Main.rand.NextFloat(-0.6f, -1f)), GoreType<EveBubble>());
                //SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/HollowKnight/JellyfishEggPop") { PitchVariance = 0.8f, MaxInstances = 2, Pitch = 1f, Volume = 0.5f }, player.MountedCenter);
            }
        }*/
    }
}