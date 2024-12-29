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
using GoldLeaf.Items.Misc.Vanity.Dyes;

namespace GoldLeaf
{
    public class GoldLeaf : Mod
    {
        public override void Load()
        {
            MusicLoader.AddMusicBox(this, MusicLoader.GetMusicSlot(this, "Sounds/Music/WhisperingGrove"), ItemType<UndergroveMusicBox>(), TileType<UndergroveMusicBoxT>());
            MusicLoader.AddMusicBox(this, MusicLoader.GetMusicSlot(this, "Sounds/Music/Silence"), ItemType<SilenceMusicBox>(), TileType<SilenceMusicBoxT>());
            MusicLoader.AddMusicBox(this, MusicLoader.GetMusicSlot(this, "Sounds/Music/ToxinGrove"), ItemType<ToxinGroveMusicBox>(), TileType<ToxinGroveMusicBoxT>());

            if (Main.netMode != NetmodeID.Server)
            {
                Asset<Effect> gameboyDyeShader = Assets.Request<Effect>("Effects/GameboyDye");
                Asset<Effect> gameboyFilterShader = Assets.Request<Effect>("Effects/Gameboy");

                GameShaders.Armor.BindShader(ItemType<RetroDye>(), new ArmorShaderData(gameboyDyeShader, "GameboyDyePass"));

                Filters.Scene["Gameboy"] = new Filter(new ScreenShaderData(gameboyFilterShader, "GameboyPass"), EffectPriority.VeryHigh);
            }
        }
    }
}