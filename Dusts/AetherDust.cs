using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace GoldLeaf.Dusts
{
    public class AetherDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 6, 8);
            dust.scale *= 3f;
        }

        public override bool MidUpdate(Dust dust)
        {
            if (!dust.noGravity)
            {
                dust.velocity.Y = 0f;
            }

            if (dust.noLight)
            {
                return false;
            }

            Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), dust.scale * 0.7f, dust.scale * 0.2f, dust.scale * 0.9f);
            return false;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation = (float)Math.Atan2(dust.velocity.Y, dust.velocity.X) + 1.57f;
            dust.scale *= 0.9f;
            if (dust.scale < 0.2f)
            {
                dust.active = false;
            }
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
            => new Color(lightColor.R, lightColor.G, lightColor.B, 50);
    }
}