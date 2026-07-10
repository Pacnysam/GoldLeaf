using GoldLeaf.Core;
using GoldLeaf.Core.Helpers;
using GoldLeaf.Items.Grove.Boss.AetherComet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static GoldLeaf.Core.ColorHelper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Effects.Dusts
{
    public class SpecialSmoke : ModDust
    {
        public override string Texture => "GoldLeaf/Effects/Dusts/SpecialSmoke";

        private static readonly Gradient defaultGradient = new([(Color.Yellow, 0.05f), (new Color(246, 68, 14), 0.4f), (new Color(30, 30, 30), 0.6f)]);
        public static readonly Gradient aetherSmokeGradient = new([(new Color(255, 119, 246), 0.1175f), (new Color(196, 43, 255), 0.425f), (new Color(54, 47, 56), 0.55f)]);

        public override void OnSpawn(Dust dust)
        {
            dust.fadeIn += 1;

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
                dust.velocity.Y -= (dust.alpha / 1750f) * ((dust.fadeIn * 2f) / 2f);

            if (dust.alpha > 100)
            {
                dust.scale *= 1f + Math.Min(dust.fadeIn / 100f, 0.15f);
                dust.alpha += Math.Max((int)(dust.fadeIn * 2f), 1);
                dust.velocity *= 0.925f;
            }
            else
            {
                if (!dust.noLight)
                    Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.15f);

                dust.scale *= 1f + dust.fadeIn / 150f;
                dust.alpha += Math.Max((int)(dust.fadeIn * 4f), 1);
                dust.velocity *= 0.975f;
            }

            dust.position += dust.velocity;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Color color = dust.color;

            if (dust.customData is Gradient gradient)
                color = gradient.GetColor(1f - dust.Opacity());

            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, color * dust.Opacity(), dust.rotation, dust.frame.Size() / 2f, dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}