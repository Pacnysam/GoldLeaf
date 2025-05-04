using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Core;
using GoldLeaf.Tiles.Decor;
using Terraria.GameContent;

namespace GoldLeaf.Tiles.Blizzard.Crafted
{
    public class AuroraWaxCandleItem : WaxCandle
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = TileType<AuroraWaxCandle>();
            Item.rare = ItemRarityID.White;

            Item.value = Item.buyPrice(0, 0, 0, 50);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(5)
            .AddIngredient(ItemType<WaxCandle>(), 5)
            .AddIngredient(ItemType<AuroraCluster>())
            .AddTile(TileID.IceMachine)
            .AddOnCraftCallback(RecipeCallbacks.AuroraMinor)
            .Register();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, ColorHelper.AuroraAccentColor(Item.timeSinceItemSpawned * 0.1f), rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, ColorHelper.AuroraAccentColor(Item.timeSinceItemSpawned * 0.1f) with { A = 0 }, rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Color color = ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 4f) with { A = 0 };

            spriteBatch.Draw(glowTex.Value, position, null, ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 4f), 0, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glowTex.Value, position, null, ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 4f) with { A = 0 }, 0, origin, scale, SpriteEffects.None, 0f);
        }

        public override void PostUpdate()
        {
            Color color = ColorHelper.AuroraAccentColor(Item.timeSinceItemSpawned * 0.1f);
            Lighting.AddLight((int)((Item.position.X + Item.width / 2) / 16f), (int)((Item.position.Y + Item.height / 2) / 16f), color.R / 255f * 0.2f, color.G / 255f * 0.2f, color.B / 255f * 0.2f);
        }
    }

    public class AuroraWaxCandle : ModTile
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(0, 225, 255));
            RegisterItemDrop(ItemType<AuroraWaxCandleItem>());

            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileWaterDeath[Type] = true;

            DustType = DustID.Torch;
            AdjTiles = [TileID.Candles];

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.RandomStyleRange = 6;
            TileObjectData.newTile.StyleMultiplier = 6;
            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.CoordinateHeights =
            [
                28
            ];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.DrawYOffset = -10;

            TileObjectData.addTile(Type);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Color color = ColorHelper.AuroraAccentColor((Main.GlobalTimeWrappedHourly * 4f) + (i * 1.4f));

            r = color.R / 255f * 0.25f;
            g = color.G / 255f * 0.25f;
            b = color.B / 255f * 0.25f;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = Main.rand.Next(1, 5);

        /*public override bool CreateDust(int i, int j, ref int type)
        {
            return base.CreateDust(i, j, ref type);
        }*/

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var tile = Main.tile[i, j];

            if (!TileDrawing.IsVisible(tile))
            {
                return;
            }

            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);

            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }

            ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i);
            int width = 16;
            int height = 28;
            int frameX = tile.TileFrameX;

            for (int k = 0; k < 4; k++)
            {
                float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
                float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

                spriteBatch.Draw(glowTex.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + xx, j * 16 - (int)Main.screenPosition.Y + -10 + yy) + zero, new Rectangle(frameX, 0, width, height), ColorHelper.AuroraAccentColor((Main.GlobalTimeWrappedHourly * 4f) + (0.5f * k) + (i * 1.4f)), 0f, default, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(glowTex.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + xx, j * 16 - (int)Main.screenPosition.Y + -10 + yy) + zero, new Rectangle(frameX, 0, width, height), ColorHelper.AuroraAccentColor((Main.GlobalTimeWrappedHourly * 4f) + (0.5f * k) + (i * 1.4f)) with { A = 0 }, 0f, default, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}