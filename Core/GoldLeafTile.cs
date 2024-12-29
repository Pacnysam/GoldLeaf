using System;
using static Terraria.ModLoader.ModContent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Items.Misc.Accessories;
using Terraria;
using GoldLeaf.Tiles.Grove;
using System.Threading.Channels;
using Terraria.ObjectData;
using Terraria.Enums;
using GoldLeaf.Items.GemSickles;

namespace GoldLeaf.Core
{
    internal class GoldLeafTile : GlobalTile
    {
        private readonly int[] oxeyeSafeTiles = [TileID.Grass, TileID.JungleGrass, TileID.HallowedGrass, TileType<GroveGrassT>()];
        private readonly int[] gemTrees = [TileID.TreeAmethyst, TileID.TreeTopaz, TileID.TreeSapphire, TileID.TreeEmerald, TileID.TreeRuby, TileID.TreeDiamond, TileID.TreeAmber];

        public override void PostSetupTileMerge()
        {
            
        }

        public override void RandomUpdate(int i, int j, int type)
        {
            Tile tile = Main.tile[i, j - 1];
            
            if (oxeyeSafeTiles.Contains(Framing.GetTileSafely(i, j).TileType) && !TileID.Sets.Platforms[type] && Main.rand.NextBool(6500) && !Main.tile[i, j].TopSlope && (Main.tileCut[tile.TileType] || TileID.Sets.BreakableWhenPlacing[tile.TileType] || !tile.HasTile) && !tile.IsActuated && Main.dayTime)
            {
                WorldGen.PlaceTile(i, j - 1, TileType<OxeyeDaisyT>(), true, false);
                NetMessage.SendObjectPlacement(-1, i, j - 1, TileType<OxeyeDaisyT>(), 0, 0, -1, -1);
            }
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (gemTrees.Contains(type) && Main.tile[i, j - 1].TileType != type && Main.rand.NextBool(10)) 
            {
                Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, ItemType<Sediment>());
            }
        }
    }
}
