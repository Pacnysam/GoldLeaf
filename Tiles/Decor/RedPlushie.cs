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
	public class RedPlushie : ModItem
	{
        public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(TileType<RedPlushieT>());

			Item.width = 24;
			Item.height = 38;

			Item.rare = ItemRarityID.Red;

            Item.value = Item.buyPrice(0, 1, 0, 0);
        }
	}

	public class RedPlushieT : ModTile
	{
		public override void SetStaticDefaults()
		{
            HitSound = SoundID.Critter with { Pitch = -0.5f, MaxInstances = 0 };

            AddMapEntry(new Color(213, 49, 49));
			RegisterItemDrop(ItemType<RedPlushie>());

			Main.tileSolid[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileNoAttach[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.CoordinatePadding = 2;

			TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);

            TileObjectData.newTile.CoordinateHeights =
            [
                16, 22
			];
			TileObjectData.newTile.CoordinateWidth = 24;
			TileObjectData.newTile.DrawYOffset = -4;

			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;

			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);
		}

        public override bool RightClick(int i, int j)
        {
            SoundEngine.PlaySound(SoundID.Critter with { Pitch = -0.5f, MaxInstances = 0 });
            return true;
        }
    }
}