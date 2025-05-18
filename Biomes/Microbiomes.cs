using GoldLeaf.Core;
using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using System;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.Localization;
using Terraria.ModLoader;
using GoldLeaf.Tiles.Decor;

namespace GoldLeaf.Biomes
{
	public class ZoneCandle : ModBiome
	{
        public override SceneEffectPriority Priority => SceneEffectPriority.None; 

        public override bool IsBiomeActive(Player player) 
        {
			return GetInstance<TileCount>().waxCandleCount >= 5;
        }
		public override void OnEnter(Player player) => player.AddBuff(BuffType<WaxCandleBuff>(), 2);
        public override void OnLeave(Player player) => player.ClearBuff(BuffType<WaxCandleBuff>());
    }
}
