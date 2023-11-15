using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace GoldLeaf.Effects.Dusts
{
	public class VampireDust2 : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 18, 18);
            dust.velocity *= Main.rand.NextFloat(0.8f, 1.4f);
            dust.customData = 0;
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
            //dust.customData = dust.customData + 1; if (dust.customData >= 10) { dust.customData = 0; dust.frame.Y += 18; }

            dust.velocity *= 0.98f;
            dust.position += dust.velocity;

            //if (framecount >= 6) { dust.active = false; }
            // FIX

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
            => new Color(255, 255, 255, 0);
    }
}

