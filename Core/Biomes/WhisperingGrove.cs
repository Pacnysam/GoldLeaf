using GoldLeaf.Core;
using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
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

        //public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => Find<ModSurfaceBackgroundStyle>("GoldLeaf/WhisperingGroveBackgroundStyle");
        public override string MapBackground => BackgroundPath;

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/WhisperingGrove");
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh; 

		public override string BestiaryIcon => "GoldLeaf/Core/Biomes/WhisperingGroveSurface_Icon";
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player) 
        {
			return (player.ZoneSkyHeight || player.ZoneOverworldHeight) && GetInstance<TileCount>().groveTileCount >= 150;
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
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player)
        {
            return (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight) && GetInstance<TileCount>().groveTileCount >= 150;
        }
        public override void OnEnter(Player player) => player.GetModPlayer<GoldLeafPlayer>().ZoneGrove = true;
        public override void OnLeave(Player player) => player.GetModPlayer<GoldLeafPlayer>().ZoneGrove = false;

    }

    public class WhisperingGroveBackgroundStyle : ModSurfaceBackgroundStyle
    {
        public override int ChooseFarTexture() => BackgroundTextureLoader.GetBackgroundSlot(Mod, "Goldleaf/Core/Biomes/GroveBackgroundFar");
        public override int ChooseMiddleTexture() => BackgroundTextureLoader.GetBackgroundSlot(Mod, "Goldleaf/Core/Biomes/GroveBackgroundMid");
        //public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) => BackgroundTextureLoader.GetBackgroundSlot(Mod, "Goldleaf/Core/Biomes/GroveBackgroundNear");

        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                        fades[i] = 1f;
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                        fades[i] = 0f;
                }
            }
        }
    }
}
