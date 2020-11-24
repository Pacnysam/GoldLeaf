using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using Terraria.ID;

namespace GoldLeaf.Items.Placeable
{
	public class GroveStoneT : ModTile
	{
		public override void SetDefaults()
		{
            soundType = SoundID.Tink;
            Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][TileType<GroveGrassT>()] = true;
            Main.tileMerge[Type][TileType<GroveBrickT>()] = true;
            drop = ItemType<GroveStone>();
            AddMapEntry(new Color(93, 81, 80));
            Main.tileBlockLight[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust dust;
            Vector2 position = new Vector2(i * 16, j * 16);
            dust = Main.dust[Terraria.Dust.NewDust(position, 16, 16, DustID.Stone, 0f, 0f, 0, new Color(255, 255, 255), 1f)];
            return true;
        }
    }

    public class GroveStone : ModItem
    {
        public override void SetDefaults()
        {
            item.consumable = true;
            item.width = 16;
            item.height = 16;
            item.value = 100;
            item.useStyle = 1;
            item.rare = 1;
            item.useTime = 10;
            item.useAnimation = 15;
            item.autoReuse = true;
            item.createTile = TileType<GroveStoneT>();
            item.maxStack = 999;
        }
    }
}