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
using System;

namespace GoldLeaf.Tiles.MusicBoxes
{
    public abstract class BaseMusicBox(int tileType) : ModItem
    {
        public int tileType = tileType;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
        }
        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(Item.createTile = tileType);
        }
    }
    public abstract class BaseMusicBoxTile(int item, string soundPath, bool tall = false) : ModTile
    {
        public int item = item;
        public bool tall = tall;
        public string soundPath = soundPath;

        public override void SetStaticDefaults()
        {
            MusicLoader.AddMusicBox(GoldLeaf.Instance, MusicLoader.GetMusicSlot(GoldLeaf.Instance, soundPath), item, Type);

            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = tall ? 0 : 2;
            TileObjectData.newTile.CoordinateHeights = tall ? [16, 18] : [16, 16];
            TileObjectData.addTile(Type);

            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;

            AddMapEntry(new Color(191, 142, 111), Language.GetText("ItemName.MusicBox"));
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

        public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
        {
            if (TileDrawing.IsVisible(tile) && tileFrameX == 36 && tileFrameY % 36 == 0 && (int)Main.timeForVisualEffects % 7 == 0 && Main.rand.NextBool(3))
            {
                int goreType = Main.rand.Next(570, 573);
                Vector2 position = new Vector2(i, j).ToWorldCoordinates(autoAddY: -8);
                Vector2 velocity = new Vector2(Main.WindForVisuals * 2f, -0.5f) * Main.rand.NextFloat(0.5f, 1.5f);

                if (goreType == 572)
                    position.X -= 8f;

                if (goreType == 571)
                    position.X -= 4f;

                Gore gore = Gore.NewGoreDirect(new EntitySource_TileUpdate(i, j), position, velocity, goreType, .8f);
                gore.position.X -= gore.Width / 2;
            }
        }
    }
}
