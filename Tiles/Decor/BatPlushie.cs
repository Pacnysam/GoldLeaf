using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Tiles.Decor
{
	public class BatPlushie : ModItem
	{
        public override void SetDefaults() 
		{
			Item.width = 20;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createTile = TileType<BatPlushieT>();
			Item.rare = ItemRarityID.Purple;

            Item.value = Item.buyPrice(0, 1, 0, 0);
        }
	}

	public class BatPlushieT : ModTile
	{
		public override void SetStaticDefaults()
		{
			HitSound = SoundID.Critter with { Pitch = -0.5f, MaxInstances = 0 };

			AddMapEntry(new Color(177, 91, 219));
			RegisterItemDrop(ItemType<BatPlushie>());

			Main.tileSolid[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);

            TileObjectData.newTile.CoordinateHeights =
            [
                24
			];
			TileObjectData.newTile.CoordinateWidth = 20;
			TileObjectData.newTile.DrawYOffset = -6;

			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;

			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);
		}

        public override bool RightClick(int i, int j)
        {
			SoundEngine.PlaySound(SoundID.Critter with { Pitch = -0.5f , MaxInstances = 0 });
			return true;
        }
    }
}