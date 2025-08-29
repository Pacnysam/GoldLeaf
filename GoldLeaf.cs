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
            if (!Main.dedServ)
            {
                Filters.Scene["CrispPixelation"] = new Filter(new ScreenShaderData(Assets.Request<Effect>("Effects/CrispPixelation"), "CrispPixelationPass"), EffectPriority.VeryHigh);
                GameShaders.Misc["LightPixelation"] = new MiscShaderData(this.Assets.Request<Effect>("Effects/LightPixelation"), "LightPixelationPass");
            }
        }
    }
}