using GoldLeaf.Core;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace GoldLeaf.Effects.Dusts
{
    internal class GroveDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 16, 16, 16);
            dust.customData = 1200;
            dust.scale *= 1.7f;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            if (dust.customData is int)
            {
                dust.customData = (int)dust.customData - 1;
                if ((int)dust.customData == 0) dust.active = false;
                if ((int)dust.customData >= 150)
                {
                    if (dust.color.R < 100) dust.color *= 1.53f;
                    dust.scale *= 1.0001f;
                }
                else
                {
                    dust.scale *= 0.998f;
                }
                dust.position.Y += (dust.scale * 0.3f) + (dust.velocity.Y * 0.6f);

                dust.rotation = GoldLeafWorld.rottime;
                dust.position.X += (float)Math.Sin(-dust.scale * 1.7f);
                dust.position.X += (float)Math.Sin(GoldLeafWorld.rottime + dust.fadeIn) * dust.scale * dust.velocity.X * 0.2f;

                if (dust.scale <= 0.1)
                {
                    dust.active = false;
                }
            }
            return true;
        }
    }
}