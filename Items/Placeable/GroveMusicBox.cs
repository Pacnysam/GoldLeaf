using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Placeable
{
	public class GroveMusicBoxT : ModTile
	{
		public override void SetDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileObsidianKill[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
			disableSmartCursor = true;
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Music Box");
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			Item.NewItem(i * 16, j * 16, 16, 48, ItemType<GroveMusicBox>());
		}

		public override void MouseOver(int i, int j) {
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.showItemIcon = true;
			player.showItemIcon2 = ItemType<GroveMusicBox>();
		}
	}

    public class GroveMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Music Box (Grove)");
        }

        public override void SetDefaults()
        {
            item.consumable = true;
            item.width = 32;
            item.height = 32;
            item.value = 100000;
            item.useStyle = 1;
            item.rare = 4;
            item.useTime = 10;
            item.useAnimation = 15;
            item.autoReuse = true;
            item.createTile = TileType<GroveMusicBoxT>();
            item.maxStack = 1;
            item.accessory = true;
            item.useTurn = true;
        }
    }
}
