using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace GoldLeaf.Effects.Dusts
{
	public class VampireDust : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 8, 8);
            dust.scale *= 2.5f;
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

            return false;
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.94f;
            dust.position += dust.velocity;
            dust.rotation = (float)Math.Atan2(dust.velocity.Y, dust.velocity.X) + 1.57f;
            dust.scale *= 0.96f;
            if (dust.scale < 0.2f)
            {
                dust.active = false;
            }
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
            => new Color(255, 255, 255, 160);
    }
}

