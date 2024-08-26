using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System.IO;
using GoldLeaf.Tiles.MusicBoxes;

namespace GoldLeaf
{
    public class GoldLeaf : Mod
    {
        public override void Load()
        {
            MusicLoader.AddMusicBox(this, MusicLoader.GetMusicSlot(this, "Sounds/Music/WhisperingGrove"), ItemType<UndergroveMusicBox>(), TileType<UndergroveMusicBoxT>());
            MusicLoader.AddMusicBox(this, MusicLoader.GetMusicSlot(this, "Sounds/Music/Silence"), ItemType<SilenceMusicBox>(), TileType<SilenceMusicBoxT>());
            MusicLoader.AddMusicBox(this, MusicLoader.GetMusicSlot(this, "Sounds/Music/ToxinGrove"), ItemType<ToxinGroveMusicBox>(), TileType<ToxinGroveMusicBoxT>());
        }
    }
}