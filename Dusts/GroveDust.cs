using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace GoldLeaf.Dusts
{
    internal class GroveDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 8, 8, 8);
            dust.customData = 1500;
            dust.scale = 2f;
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
                if ((int)dust.customData >= 100)
                {
                    if (dust.color.R < 100) dust.color *= 1.53f;
                    dust.scale *= 1.025f;
                }
                else
                {
                    dust.scale *= 0.99f;
                }
                dust.position.Y += dust.scale * 0.3f;

                dust.rotation = GoldleafWorld.rottime;
                dust.position.X += (float)Math.Sin(-dust.scale * 3);
            }
            return true;
        }
    }

    internal class GroveDust2 : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 8, 8, 8);
            dust.customData = 1500;
            dust.scale = 2f;
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
                if ((int)dust.customData >= 100)
                {
                    if (dust.color.R < 100) dust.color *= 1.53f;
                    dust.scale *= 1.025f;
                }
                else
                {
                    dust.scale *= 0.99f;
                }
                dust.position.Y += dust.scale * 0.3f;

                dust.rotation = GoldleafWorld.rottime * -1;
                dust.position.X -= (float)Math.Sin(-dust.scale * 3);
            }
            return true;
        }
    }
}