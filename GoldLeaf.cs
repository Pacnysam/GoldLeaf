using GoldLeaf.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace GoldLeaf
{
	public class GoldLeaf : Mod
	{
        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if (Main.myPlayer != -1 && !Main.gameMenu && Main.LocalPlayer.active)
            {
                Player player = Main.LocalPlayer;

                if (player.GetModPlayer<BiomeManager>().ZoneGrove)
                {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/WhisperingGrove");
                    priority = MusicPriority.BiomeHigh;
                }

                if (player.GetModPlayer<BiomeManager>().ZoneToxin)
                {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/ToxinGrove");
                    priority = MusicPriority.BiomeHigh;
                }
            }
            return;
        }
        public override void Load()
        {
        AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/WhisperingGrove"), ItemType("GroveMusicBox"), TileType("GroveMusicBoxT"));
            AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/ToxinGrove"), ItemType("ToxinMusicBox"), TileType("ToxinMusicBoxT"));
        }
    }
}