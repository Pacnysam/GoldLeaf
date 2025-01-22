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

namespace GoldLeaf.Tiles.Decor
{
	public class WaxCandle : ModItem
	{
        private Asset<Texture2D> glowTex;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults() 
		{
            glowTex = Request<Texture2D>(Texture + "Glow");

            Item.width = 14;
			Item.height = 28;
			Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createTile = TileType<WaxCandleT>();
			Item.rare = ItemRarityID.White;

            Item.value = Item.buyPrice(0, 0, 0, 50);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D tex = Request<Texture2D>(Texture + "Glow").Value;
            spriteBatch.Draw
            (
                tex,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - tex.Height * 0.5f
                ),
                new Rectangle(0, 0, tex.Width, tex.Height),
                Color.White,
                rotation,
                tex.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
        }

        public override void PostUpdate()
        {
            Lighting.AddLight((int)((Item.position.X + (Item.width / 2)) / 16f), (int)((Item.position.Y + (Item.height / 2)) / 16f), (255f / 255) * 0.3f, (201f / 255) * 0.3f, (94f / 255) * 0.3f);
        }
    }

	public class WaxCandleT : ModTile
	{
        private Asset<Texture2D> glowTex;
        public override void SetStaticDefaults()
		{
            glowTex = Request<Texture2D>(Texture + "Glow");

            AddMapEntry(new Color(255, 198, 26), Language.GetText("Wax Candle"));

            Main.tileSolid[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;

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
            Tile tile = Main.tile[i, j];

            r = (255f / 255) * 0.4f;
            g = (201f / 255) * 0.4f;
            b = (94f / 255) * 0.4f;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = Main.rand.Next(1, 5);

        public override bool CreateDust(int i, int j, ref int type)
        {
            return base.CreateDust(i, j, ref type);
        }

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

            ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);
            Color color = new(100, 100, 100, 0);
            int width = 16;
            int height = 28;
            int frameX = tile.TileFrameX;

            for (int k = 0; k < 4; k++)
            {
                float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
                float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

                spriteBatch.Draw(glowTex.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + xx, j * 16 - (int)Main.screenPosition.Y + -10 + yy) + zero, new Rectangle(frameX, 0, width, height), color, 0f, default, 1f, SpriteEffects.None, 0f);
            }
        }
    }

    public class WaxCandleBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void SetStaticDefaults()
        {
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.aggro -= 300;
        }
    }
}