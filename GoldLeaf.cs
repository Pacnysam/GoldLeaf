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
    public class GoldLeaf : Mod
    {
        public GoldLeaf() 
        {
            MusicSkipsVolumeRemap = true;
        }

        public override void Load()
        {
            MusicLoader.AddMusicBox(this, MusicLoader.GetMusicSlot(this, "Sounds/Music/WhisperingGrove"), ItemType<UndergroveMusicBox>(), TileType<UndergroveMusicBoxT>());
            MusicLoader.AddMusicBox(this, MusicLoader.GetMusicSlot(this, "Sounds/Music/Silence"), ItemType<SilenceMusicBox>(), TileType<SilenceMusicBoxT>());
            MusicLoader.AddMusicBox(this, MusicLoader.GetMusicSlot(this, "Sounds/Music/ToxinGrove"), ItemType<ToxinGroveMusicBox>(), TileType<ToxinGroveMusicBoxT>());
            MusicLoader.AddMusicBox(this, MusicLoader.GetMusicSlot(this, "Sounds/Music/GroveBoss"), ItemType<GroveBossMusicBox>(), TileType<GroveBossMusicBoxT>());
            MusicLoader.AddMusicBox(this, MusicLoader.GetMusicSlot(this, "Sounds/Music/ToxinBoss"), ItemType<ToxinBossMusicBox>(), TileType<ToxinBossMusicBoxT>());

            if (Main.netMode != NetmodeID.Server)
            {
                Asset<Effect> gameboyDyeShader = Assets.Request<Effect>("Effects/GameboyDye");
                Asset<Effect> gameboyFilterShader = Assets.Request<Effect>("Effects/Gameboy");

                GameShaders.Armor.BindShader(ItemType<RetroDye>(), new ArmorShaderData(gameboyDyeShader, "GameboyDyePass"));

                Filters.Scene["Gameboy"] = new Filter(new ScreenShaderData(gameboyFilterShader, "GameboyPass"), EffectPriority.VeryHigh);
            }

            On_Player.KeyDoubleTap += DoubleTapKey;
        }

        public override void Unload()
        {
            On_Player.KeyDoubleTap -= DoubleTapKey;
        }

        private static void DoubleTapKey(On_Player.orig_KeyDoubleTap orig, Player self, int keyDir)
        {
            orig(self, keyDir);

            self.GetModPlayer<GoldLeafPlayer>().DoubleTap(self, keyDir);
        }
    }
}