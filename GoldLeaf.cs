using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System.IO;
using GoldLeaf.Tiles.MusicBoxes;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.GameInput;
using GoldLeaf.Core;
using Terraria.Audio;
using GoldLeaf.Items.Dyes;

namespace GoldLeaf
{
    public partial class GoldLeaf : Mod
    {
        public static GoldLeaf Instance;
        public GoldLeaf() 
        {
            MusicSkipsVolumeRemap = true;
            Instance = this;
        }

        public override void Load()
        {
            MusicLoader.AddMusicBox(this, MusicLoader.GetMusicSlot(this, "Sounds/Music/WhisperingGrove"), ItemType<UndergroveMusicBox>(), TileType<UndergroveMusicBoxT>());
            MusicLoader.AddMusicBox(this, MusicLoader.GetMusicSlot(this, "Sounds/Music/Silence"), ItemType<SilenceMusicBox>(), TileType<SilenceMusicBoxT>());
            MusicLoader.AddMusicBox(this, MusicLoader.GetMusicSlot(this, "Sounds/Music/ToxinGrove"), ItemType<ToxinGroveMusicBox>(), TileType<ToxinGroveMusicBoxT>());
            MusicLoader.AddMusicBox(this, MusicLoader.GetMusicSlot(this, "Sounds/Music/GroveBoss"), ItemType<GroveBossMusicBox>(), TileType<GroveBossMusicBoxT>());
            MusicLoader.AddMusicBox(this, MusicLoader.GetMusicSlot(this, "Sounds/Music/ToxinBoss"), ItemType<ToxinBossMusicBox>(), TileType<ToxinBossMusicBoxT>());
        }
    }
}