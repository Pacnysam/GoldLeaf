using GoldLeaf.Core;
using GoldLeaf.Effects.Gores.Base;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace GoldLeaf.Effects.Gores
{
    public class AetherRingGore : RingGore
    {
        public override string Texture => "GoldLeaf/Textures/RingGlow0";
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            growthDrag = 0.87f + (startingScale * 0.15f);
            growthVelocity = 0.25f - (startingScale * 0.3f);
            fadeSpeed = 11;
            fadeAlpha = 245;
            initialScale = 0.1f;
            gore.alpha = 5;

        }

        public override Color? GetAlpha(Gore gore, Color lightColor)
        {
            color = new Color(255, 119, 246, gore.alpha);
            return color;
        }
    }
}