using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Tiles.Decor
{
	public class MadcapPainting : ModItem
	{

		public override void SetDefaults() 
		{
			Item.width = 40;
			Item.height = 40;

            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Purple;

			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;

			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			
			Item.createTile = TileType<MadcapPaintingT>();
		}
	}

	public class MadcapPaintingT : ModTile
	{
		public override void SetStaticDefaults()
		{
            LocalizedText name = CreateMapEntryName();
			//name.SetDefault("Painting");
			AddMapEntry(new Color(150, 150, 150), name);

			RegisterItemDrop(ItemType<MadcapPainting>());

			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Height = 6;
			TileObjectData.newTile.Width = 6;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16,
				16,
				16
			};
			TileObjectData.newTile.AnchorBottom = default;
			TileObjectData.newTile.AnchorTop = default;
			TileObjectData.newTile.AnchorWall = true;
			TileObjectData.addTile(Type);
		}
	}
}