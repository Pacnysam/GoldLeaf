using GoldLeaf.Core;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace GoldLeaf.Effects.Dusts
{
    public class AetherDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 6, 8);
            dust.scale *= 3;
            dust.velocity *= 1.8f;
            dust.alpha = 300;
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
            dust.alpha -= 1;
            dust.position += dust.velocity;
            dust.velocity *= 0.95f;
            dust.rotation = (float)Math.Atan2(dust.velocity.Y, dust.velocity.X) + 1.57f;
            dust.scale *= 0.88f;
            if (dust.scale < 0.25f)
            {
                dust.active = false;
            }
            Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), dust.scale * 0.7f, dust.scale * 0.2f, dust.scale * 0.9f);
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
            => new Color(255, 255, 255, 0);
    }
}