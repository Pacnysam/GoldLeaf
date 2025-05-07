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
    public class ArcticDust : ModDust
    {
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }

        public override void OnSpawn(Dust dust)
        {
            dust.fadeIn = 0;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 10, 10);
            dust.velocity *= 1.75f;

            dust.position += new Vector2(5, 5);
        }

        public override bool Update(Dust dust)
        {
            if (dust.customData is null)
            {
                dust.position -= new Vector2(9, 9) * dust.scale;
                dust.customData = 1;
            }

            if (dust.alpha % 40 == 35)
                dust.frame.Y += 10;

            if (!dust.noLight)
                Lighting.AddLight(dust.position, Color.Cyan.ToVector3() * 0.02f);

            dust.alpha += 5;

            if (dust.alpha > 255)
                dust.active = false;

            dust.velocity *= 0.95f;
            dust.position += dust.velocity;

            return false;
        }
    }
    
    public class AuroraTwinkle : ModDust
    {
        public override void SetStaticDefaults()
        {
            //UpdateType = DustID.PortalBolt;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White * ((160 - dust.alpha) / 255f);
        }

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 18, 18);
            dust.color.R += 10; dust.color.G += 25; dust.color.B += 40; dust.color.A = 0;
            dust.alpha -= 10;

            if (dust.alpha < 0)
                dust.scale *= 0.7f;
        }

        public override bool Update(Dust dust)
        {
            if (dust.customData == null)
            {
                dust.color = ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 3);
                dust.color.R += 10; dust.color.G += 25; dust.color.B += 40; dust.color.A = 0;
            }

            if (!dust.noLight)
                Lighting.AddLight(dust.position, new Vector3(dust.color.R / 255f, dust.color.G / 255f, dust.color.B / 255f) * 0.3f);

            if (dust.alpha < 0)
            {
                dust.alpha++;
                dust.scale *= 1.04f;
            }
            else
            {
                dust.alpha += 8;
                dust.rotation *= 0.96f;
                dust.scale *= 0.98f;
                //dust.velocity *= 0.96f;
            }

            if (dust.customData != null && dust.customData is Player player && dust.alpha >= 0)
            {
                //dust.position = item.Center + Vector2.UnitX.RotatedBy(dust.rotation, Vector2.Zero);

                dust.velocity += Vector2.Normalize(player.MountedCenter - dust.position) * (dust.alpha * 0.1f);
                if (dust.velocity.Length() > 4.6f) dust.velocity = Vector2.Normalize(dust.velocity) * 4.6f;

            }
            else if (dust.customData != null && dust.customData is Entity entity && dust.alpha >= 0)
            {
                //dust.position = item.Center + Vector2.UnitX.RotatedBy(dust.rotation, Vector2.Zero);

                dust.velocity += Vector2.Normalize(entity.Center - dust.position) * 0.05f;
                if (dust.velocity.Length() > 0.05f) dust.velocity = Vector2.Normalize(dust.velocity) * 0.05f;
            }

            if (dust.customData != null && dust.customData is Player)
                dust.position += dust.velocity;
            else
                dust.position += dust.velocity + (Vector2.UnitX.RotatedBy(dust.rotation / 3, Vector2.Zero));

            if (dust.alpha > 220) dust.active = false;

            return false;
        }
    }
    
    public class BlizzardBrassDust : ModDust
    {
        public override void SetStaticDefaults()
        {
            UpdateType = DustID.Silver;
        }

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 8, 8, 8);
        }
    }
}
