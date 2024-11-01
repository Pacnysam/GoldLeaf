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

namespace GoldLeaf.Core
{
    internal class GoldLeafTile : GlobalTile
    {
        private readonly int[] oxeyeSafeTiles = [TileID.Grass, TileID.JungleGrass, TileID.HallowedGrass, TileType<GroveGrassT>()];

        public override void RandomUpdate(int i, int j, int type)
        {
            Tile tile = Main.tile[i, j - 1];
            
            if (oxeyeSafeTiles.Contains(Framing.GetTileSafely(i, j).TileType) && !TileID.Sets.Platforms[type] && Main.rand.NextBool(6500) && !Main.tile[i, j].TopSlope && (Main.tileCut[tile.TileType] || TileID.Sets.BreakableWhenPlacing[tile.TileType] || !tile.HasTile) && !tile.IsActuated && Main.dayTime)
            {
                WorldGen.PlaceTile(i, j - 1, TileType<OxeyeDaisyT>(), true, false);
                NetMessage.SendObjectPlacement(-1, i, j - 1, TileType<OxeyeDaisyT>(), 0, 0, -1, -1);
            }
        }
    }
}
