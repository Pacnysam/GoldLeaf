using GoldLeaf.Core;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace GoldLeaf.Effects.Dusts
{
	public class SparkDust : ModDust
	{
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }

        public override void OnSpawn(Dust dust)
        {
            dust.fadeIn = 0;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 18, 18);
            dust.velocity *= 1.75f;
        }

        public override bool Update(Dust dust)
        {
            if (dust.customData is null)
            {
                dust.position -= new Vector2(9, 9) * dust.scale;
                dust.customData = 1;
            }

            if (dust.alpha % 40 == 35)
                dust.frame.Y += 18;

            Lighting.AddLight(dust.position, Color.Cyan.ToVector3() * 0.02f);

            dust.alpha += 5;

            if (dust.alpha > 255)
                dust.active = false;

            dust.velocity *= 0.95f;
            dust.position += dust.velocity;

            return false;
        }
    }

    public class SparkDustTiny : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 10, 10);
            dust.scale *= 2f;
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

        public override Color? GetAlpha(Dust dust, Color lightColor) => new Color(255, 255, 255, 160);
    }
}

