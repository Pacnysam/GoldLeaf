using GoldLeaf.Core;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GoldLeaf.Core.Biomes
{
	public class WhisperingGroveSurface : ModBiome
	{
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Whispering Grove");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/WhisperingGrove");
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh; 

		public override string BestiaryIcon => "GoldLeaf/Core/Biomes/WhisperingGroveSurface_Icon";
        //public override string BackgroundPath => base.BackgroundPath;
        //public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player) 
        {
			return (player.ZoneSkyHeight || player.ZoneOverworldHeight) && ModContent.GetInstance<TileCount>().groveTileCount >= 150;
        }
		public override void OnEnter(Player player) => player.GetModPlayer<GoldLeafPlayer>().ZoneGrove = true;
        public override void OnLeave(Player player) => player.GetModPlayer<GoldLeafPlayer>().ZoneGrove = false;

    }

    public class UndergroundWhisperingGrove : ModBiome
    {
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Whispering Undergrove");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/WhisperingGrove");
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override string BestiaryIcon => "GoldLeaf/Core/Biomes/WhisperingGroveSurface_Icon";
        //public override string BackgroundPath => base.BackgroundPath;
        //public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player)
        {
            return (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight) && ModContent.GetInstance<TileCount>().groveTileCount >= 150;
        }
        public override void OnEnter(Player player) => player.GetModPlayer<GoldLeafPlayer>().ZoneGrove = true;
        public override void OnLeave(Player player) => player.GetModPlayer<GoldLeafPlayer>().ZoneGrove = false;

    }
}
