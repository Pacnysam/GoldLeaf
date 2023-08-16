using GoldLeaf.Core;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace GoldLeaf.Core.Biomes
{
	public class WhisperingGroveSurface : ModBiome
	{
        public override int Music => MusicLoader.GetMusicSlot(Mod, "GoldLeaf/Sounds/Music/WhisperingGrovePH");
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh; 

		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

		// Use SetStaticDefaults to assign the display name

		public override bool IsBiomeActive(Player player) {
			return /*(player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight) &&*/
				ModContent.GetInstance<TileCount>().groveTileCount >= 180;
		}
		public override void OnEnter(Player player) => player.GetModPlayer<GoldLeafPlayer>().ZoneGrove = true;
        public override void OnLeave(Player player) => player.GetModPlayer<GoldLeafPlayer>().ZoneGrove = false;

    }
}
