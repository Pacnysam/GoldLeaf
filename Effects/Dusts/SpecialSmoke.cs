using GoldLeaf.Core;
using GoldLeaf.Core.Helpers;
using GoldLeaf.Items.Grove.Boss.AetherComet;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.ColorHelper;

namespace GoldLeaf.Effects.Dusts
{
    public class SpecialSmoke : ModDust
    {
        public override string Texture => "GoldLeaf/Effects/Dusts/SpecialSmoke";

        private static readonly Gradient defaultGradient = new([(Color.Yellow, 0.05f), (new Color(246, 68, 14), 0.4f), (new Color(30, 30, 30), 0.6f)]);
        public static readonly Gradient aetherSmokeGradient = new([(new Color(255, 119, 246), 0.1175f), (new Color(196, 43, 255), 0.425f), (new Color(54, 47, 56), 0.55f)]);

        public override void OnSpawn(Dust dust)
        {
            dust.fadeIn = 2;

            if (dust.customData is not Gradient)
                dust.customData = defaultGradient;

            dust.scale *= Main.rand.NextFloat(0.8f, 1.45f) * 0.7f;
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 36, 34, 36);
            dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);

            //dust.position -= new Vector2(17f, 18f) * dust.scale;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color color = dust.color;

            if (dust.customData is Gradient gradient)
                color = gradient.GetColor(1f - dust.Opacity());

            return color * dust.Opacity();
        }

        public override bool Update(Dust dust)
        {
            if (!dust.noGravity)
                dust.velocity.Y -= (dust.alpha / 1750f) * (dust.fadeIn / 2f);

            if (dust.alpha > 100)
            {
                dust.scale += (dust.fadeIn / 75f);
                dust.alpha += (int)dust.fadeIn;
                dust.velocity *= 0.925f;
            }
            else
            {
                if (!dust.noLight)
                    Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.15f);

                dust.scale += (dust.fadeIn / 200f);
                dust.alpha += (int)(dust.fadeIn * 2f);
                dust.velocity *= 0.975f;
            }

            dust.position += dust.velocity;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }
}