using GoldLeaf.Tiles.Blizzard.Crafted;
using GoldLeaf.Tiles.Decor;
using GoldLeaf.Tiles.Grove;
using System;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core
{
	public class TileCount : ModSystem
	{
		public int groveTileCount;
		public int waxCandleCount;

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
			groveTileCount = tileCounts[TileType<EchoslateT>()] + tileCounts[TileType<GroveGrassT>()];
			waxCandleCount = tileCounts[TileType<WaxCandleT>()] + tileCounts[TileType<AuroraWaxCandle>()];
        }
	}
}
