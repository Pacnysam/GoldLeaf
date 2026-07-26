using GoldLeaf.Tiles.Blizzard.Crafted;
using GoldLeaf.Tiles.Decor;
using GoldLeaf.Tiles.Grove;
using GoldLeaf.Tiles.Grove.ChalcedonyCave;
using System;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core
{
	public class TileCount : ModSystem
	{
		public int groveTileCount;
        public int quarryTileCount;
        public int waxCandleCount;

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
			groveTileCount = tileCounts[TileType<EchoslateT>()] + tileCounts[TileType<GroveGrassT>()];
            quarryTileCount = tileCounts[TileType<BasaniteT>()];
            waxCandleCount = tileCounts[TileType<WaxCandleT>()] + tileCounts[TileType<AuroraWaxCandle>()];
        }
	}
}
