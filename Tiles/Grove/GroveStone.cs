using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using Terraria.ID;

namespace GoldLeaf.Tiles.Grove
{
    public class GroveStone : ModItem
    {
        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.width = 16;
            Item.height = 16;
            Item.value = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Blue;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.autoReuse = true;
            Item.createTile = TileType<GroveStoneT>();
            Item.maxStack = 999;
        }
    }

    public class GroveStoneT : ModTile
	{
		public override void SetStaticDefaults()
		{
            //SoundType = SoundID.Tink;
            //SoundStyle = 1;
            Main.tileSolid[Type] = true;
			//Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][TileType<GroveGrassT>()] = true;
            Main.tileMerge[Type][TileID.Mud] = true;
            RegisterItemDrop(ItemType<GroveStone>());
            AddMapEntry(new Color(118, 108, 98));
            Main.tileBlockLight[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust dust;
			Vector2 position = new(i * 16, j * 16);
			dust = Main.dust[Dust.NewDust(position, 16, 16, DustID.Stone, 0f, 0f, 0, new Color(255, 255, 255), 1f)];
            return true;
        }
    }
}