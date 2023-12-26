using GoldLeaf.Tiles.Grove;
using System;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core
{
	public class TileCount : ModSystem
	{
		public int groveTileCount;

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
			groveTileCount = tileCounts[TileType<GroveStoneT>()] + tileCounts[TileType<GroveGrassT>()];
		}
	}
}
