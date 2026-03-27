using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using Terraria.ID;
using GoldLeaf.Items.Blizzard.Armor;
using GoldLeaf.Tiles.Grove.Ancient;

namespace GoldLeaf.Tiles.Grove.ChalcedonyCave
{
    public class Basanite : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<BasaniteT>());
            Item.value = Item.sellPrice(0, 0, 0, 50);

            ItemID.Sets.IsLavaImmuneRegardlessOfRarity[Item.type] = true;

            ItemID.Sets.ShimmerTransformToItem[Item.type] = ItemType<Echoslate>();
        }
    }

    public class BasaniteT : ModTile
	{
		public override void SetStaticDefaults()
		{
            RegisterItemDrop(ItemType<Basanite>());
            AddMapEntry(new Color(40, 38, 46));

            HitSound = SoundID.Tink with { Pitch = -0.75f };
            MinPick = 110;
            MineResist = 3f;
            DustType = DustID.Stone;

            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][TileID.Dirt] = true;
            Main.tileMerge[Type][TileType<EchoslateT>()] = true;
            Main.tileMerge[Type][TileType<AncientEchoslateT>()] = true;
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
			Dust dust = Main.dust[Dust.NewDust(position, 16, 16, DustID.Stone, 0f, 0f, 0, Color.DarkGray, 1f)];
            return false;
        }

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            WorldGen.TileMergeAttempt(-2, TileID.Mud, ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }
    }
}