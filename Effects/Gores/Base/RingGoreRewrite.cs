using GoldLeaf.Core;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace GoldLeaf.Effects.Gores.Base
{
    public class RingGoreRewrite : ModGore
    {
        private float growthDrag;
        private float growthRate;
        private int alphaVelocity;

        public override string Texture => "GoldLeaf/Textures/RingGlow0";
        //public override string Texture => "GoldLeaf/icon";
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            growthDrag = 0.99f;
            growthRate = 1.2f;
            alphaVelocity = 1;

            gore.alpha = 0;
            gore.velocity = Vector2.Zero;
        }

        public override bool Update(Gore gore)
        {
            gore.velocity = Vector2.Zero;

            if (growthRate > 1f)
            {
                gore.scale *= growthRate;

                growthRate *= growthDrag; 
            }

            if (alphaVelocity < 9) alphaVelocity++;

            gore.alpha += alphaVelocity;

            if (gore.alpha >= 235)
            {
                gore.active = false;
            }
            return true;
        }

        public override Color? GetAlpha(Gore gore, Color lightColor)
        {
            return new Color(255, 255, 255, gore.alpha);
        }
    }
}