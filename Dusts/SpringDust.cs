using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace GoldLeaf.Dusts
{
    public class SpringDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0.3f;
            dust.noGravity = true;
            dust.noLight = false;
            dust.scale *= 0.5f;
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 8, 8, 8);
            dust.color.R = 134;
            dust.color.G = 244;
            dust.color.B = 255;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += 0.15f;
            dust.velocity *= 0.96f;
            dust.color *= 0.99f;

            dust.scale *= 0.98f;
            if (dust.scale < 0.1f)
            {
                dust.active = false;
            }
            return false;
        }
    }
}