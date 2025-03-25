using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.GameContent.ObjectInteractions;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.Drawing;
using Terraria.Utilities;

namespace GoldLeaf.Tiles.MusicBoxes
{
    public abstract class BaseMusicBox(int item) : ModTile
	{
        public int item = item;

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileObsidianKill[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.addTile(Type);
			
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;

            AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.MusicBox"));
			RegisterItemDrop(item);
            DustType = -1;
        }

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = item;
		}

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool CanDrop(int i, int j) => false;

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
			=> Item.NewItem(null, new Rectangle(i * 16, j * 16, 32, 32), new Item(item), false, true);

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) //grabbed from spirit reforged
        {
            if (Lighting.UpdateEveryFrame && new FastRandom(Main.TileFrameSeed).WithModifier(i, j).Next(4) != 0 || !((int)Main.timeForVisualEffects % 7 == 0 && Main.rand.NextBool(3)))
                return;

            var tile = Framing.GetTileSafely(i, j);
            if (!TileDrawing.IsVisible(tile) || tile.TileFrameX != 36 || tile.TileFrameY % 36 != 0)
                return;

            int goreType = Main.rand.Next(570, 573);
            var position = new Vector2(i, j) * 16 + new Vector2(8, -8);
            var velocity = new Vector2(Main.WindForVisuals * 2f, -0.5f) * new Vector2(Main.rand.NextFloat(.5f, 1.5f), Main.rand.NextFloat(.5f, 1.5f));
            var gore = Gore.NewGoreDirect(new EntitySource_TileUpdate(i, j), position, velocity, goreType, .8f);
            gore.position.X -= gore.Width / 2;

            return;
        }
    }
}
