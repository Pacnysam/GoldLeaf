using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace GoldLeaf.Tiles.MusicBoxes
{
    public class UndergroveMusicBox : ModItem
    {
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = TileType<UndergroveMusicBoxT>();
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.accessory = true;
            Item.hasVanityEffects = true;
        }
    }

    internal class UndergroveMusicBoxT : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileObsidianKill[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.addTile(Type);
			
            TileID.Sets.DisableSmartCursor[Type] = true;
            

            AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.MusicBox"));
			RegisterItemDrop(ItemType<UndergroveMusicBox>());
            DustType = -1;
        }

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ItemType<UndergroveMusicBox>();
		}

		public override bool CanDrop(int i, int j) => false;

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
			=> Item.NewItem(null, new Rectangle(i * 16, j * 16, 32, 32), new Item(ItemType<UndergroveMusicBox>()), false, true);
		//Spawn in the drop manually to prevent a random prefix
	}
}
