using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace GoldLeaf.Effects.Dusts
{
    public class StarDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 10, 10);
            dust.scale *= 1.2f;
            dust.noLight = false;
        }

        public override bool MidUpdate(Dust dust)
        {
            if (!dust.noGravity)
            {
                dust.velocity.Y = 0f;
            }

            if (dust.noLight)
            {
                Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), dust.scale * dust.color.R * 0.7f, dust.scale * dust.color.G * 0.7f, dust.scale * dust.color.B * 0.7f);
            }

            return true;
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.94f;
            dust.position += dust.velocity;
            dust.rotation = (float)Math.Atan2(dust.velocity.Y, dust.velocity.X) + 1.57f;
            dust.alpha += 2;
            dust.scale *= 0.98f;
            if (dust.scale < 0.2f)
            {
                dust.active = false;
            }
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color color;
            if (dust.alpha < 70)
                color = Color.Lerp(new Color(255, 237, 187), new Color(44, 207, 244), (dust.alpha - 70) / 80f);
            else
                color = new Color(44, 207, 244);

            return color * ((255 - dust.alpha) / 255f);
        }
    }
}

