using GoldLeaf.Core;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace GoldLeaf.Effects.Dusts
{
    public class SpecialSmoke : ModDust //i did not make any of this! i nabbed it from starlight river
    {
        public override string Texture => "GoldLeaf/Effects/Dusts/SpecialSmoke";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 36, 34, 36);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color color;
            if (dust.alpha < 80)
                color = Color.Lerp(Color.Yellow, Color.Orange, dust.alpha / 80f);
            else if (dust.alpha < 140)
                color = Color.Lerp(Color.Orange, new Color(30, 30, 30), (dust.alpha - 80) / 80f);
            else
                color = new Color(30, 30, 30);

            return color * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            if (dust.velocity.Length() > 3)
                dust.velocity *= 0.85f;
            else
                dust.velocity *= 0.9f;

            if (dust.alpha > 100)
            {
                dust.scale += 0.012f;
                dust.alpha += 2;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.985f;
                dust.alpha += 4;
            }

            dust.position += dust.velocity;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }

    public class SpecialSmokeFast : ModDust
    {
        public override string Texture => "GoldLeaf/Effects/Dusts/SpecialSmoke";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 36, 34, 36);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color color;
            if (dust.alpha < 40)
                color = Color.Lerp(Color.Yellow, Color.Orange, dust.alpha / 40f);
            else if (dust.alpha < 80)
                color = Color.Lerp(Color.Orange, new Color(25, 25, 25), (dust.alpha - 40) / 40f);
            else
                color = new Color(25, 25, 25);

            return color * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            if (dust.velocity.Length() > 3)
                dust.velocity *= 0.85f;
            else
                dust.velocity *= 0.92f;

            if (dust.alpha > 60)
            {
                dust.scale += 0.01f;
                dust.alpha += 6;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.985f;
                dust.alpha += 4;
            }

            dust.position += dust.velocity;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }
}