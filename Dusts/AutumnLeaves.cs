﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace GoldLeaf.Dusts
{
    public class AutumnLeaves : ModDust
    {
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return lightColor;
        }

        public override void OnSpawn(Dust dust)
        {
            dust.fadeIn = Main.rand.NextFloat(6.28f);
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 8, 8, 8);
            dust.noLight = true;
        }

        public override bool Update(Dust dust)
        {
            dust.position.Y += dust.velocity.Y;
            dust.velocity.Y += 0.01f;
            dust.position.X += (float)Math.Sin(GoldleafWorld.rottime + dust.fadeIn) * dust.scale * dust.velocity.X * 0.4f;
            dust.rotation = (float)Math.Sin(GoldleafWorld.rottime + dust.fadeIn) * 0.5f;
            dust.scale *= 0.99f;
            dust.color *= 0.92f;
            if (dust.scale <= 0.2)
            {
                dust.active = false;
            }
            return false;
        }
    }
}