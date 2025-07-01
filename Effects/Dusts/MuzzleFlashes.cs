using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using GoldLeaf.Core;
using ReLogic.Content;
using Terraria.ID;

namespace GoldLeaf.Effects.Dusts
{
    public class MuzzleFlash : ModDust
    {
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 14, 18);
        }

        public override bool Update(Dust dust)
        {
            dust.rotation = dust.velocity.ToRotation() + 1.57f;

            if (dust.alpha % 20 == 15)
                dust.frame.Y += 18;

            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.7f);

            dust.alpha += 5;

            if (dust.alpha > 255)
                dust.active = false;

            dust.velocity *= 0.85f;
            dust.position += dust.velocity;

            return false;
        }
    }
}
