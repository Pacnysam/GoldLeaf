using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using Terraria.ID;
using GoldLeaf.Items.Blizzard.Armor;
using GoldLeaf.Tiles.Grove.Ancient;

namespace GoldLeaf.Tiles.Grove
{
    public class Echoslate : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<EchoslateT>());
            Item.value = Item.sellPrice(0, 0, 0, 2);

            ItemID.Sets.IsLavaImmuneRegardlessOfRarity[Item.type] = true;

            ItemID.Sets.ShimmerTransformToItem[Item.type] = ItemType<AncientEchoslate>();
        }
    }

    public class EchoslateT : ModTile
	{
		public override void SetStaticDefaults()
		{
            RegisterItemDrop(ItemType<Echoslate>());
            AddMapEntry(new Color(118, 108, 98));

            HitSound = SoundID.Tink with { Pitch = -0.3f };
            MinPick = 45;
            MineResist = 1.5f;
            DustType = DustID.Stone;

            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][TileID.Dirt] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileStone[Type] = true;

            TileID.Sets.JungleSpecial[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = TileID.Mud;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Vector2 position = new(i * 16, j * 16);
			Dust dust = Main.dust[Dust.NewDust(position, 16, 16, DustID.Stone, 0f, 0f, 0, new Color(255, 255, 255), 1f)];
            return true;
        }

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            WorldGen.TileMergeAttempt(-2, TileID.Mud, ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }

        /*public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            if (j % 2 == 0)
            {
                Tile t = Main.tile[i, j];
                t.TileFrameY += 270;
            }
        }*/
    }
}