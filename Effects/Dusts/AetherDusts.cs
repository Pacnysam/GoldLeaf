using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using GoldLeaf.Core;
using ReLogic.Content;
using Terraria.ID;
using Terraria.GameContent;
using GoldLeaf.Core.Helpers;

namespace GoldLeaf.Effects.Dusts
{
    public class AetherDust : ModDust
    {
        /*public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 6, 8);
            dust.scale *= 3;
            dust.velocity *= 1.8f;
            dust.alpha = -90;
        }

        public override bool MidUpdate(Dust dust)
        {
            return false;
        }

        public override bool Update(Dust dust)
        {
            dust.alpha += 1;
            dust.position += dust.velocity;
            dust.velocity *= 0.95f;
            dust.rotation = (float)Math.Atan2(dust.velocity.Y, dust.velocity.X) + 1.57f;
            dust.scale *= 0.88f;
            if (dust.scale < 0.25f)
            {
                dust.active = false;
            }
            if (dust.noLight == false)
            {
                Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), dust.scale * 0.35f, dust.scale * 0.1f, dust.scale * 0.45f);
            }
            return false;
        }*/

        //public override string Texture => "GoldLeaf/Textures/ShineSmall";
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.noLight = false;
            dust.scale *= 3;
            dust.velocity *= 1.8f;
            dust.alpha = -90;
        }

        public override bool MidUpdate(Dust dust)
        { 
            return false; 
        }

        public override bool Update(Dust dust)
        {
            dust.alpha += 7;
            dust.position += dust.velocity;
            dust.velocity *= 0.935f;
            dust.rotation = dust.velocity.ToRotation() + MathHelper.PiOver2;
            dust.scale *= 0.88f;
            if (dust.scale < 0.25f)
            {
                dust.active = false;
            }
            if (dust.noLight == false)
            {
                Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), dust.scale * 0.35f, dust.scale * 0.1f, dust.scale * 0.45f);
            }
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
            => new Color(255, 255, 255, 0);

        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, ColorHelper.AdditiveWhite() * dust.Opacity(), dust.rotation, Texture2D.Size() / 2f, new Vector2(1f, Math.Clamp(dust.velocity.Length() * 1.25f, 0.2f, 1.15f)) * dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
    
    public class AetherSparkDust : ModDust
    {
        public override string Texture => "GoldLeaf/Textures/ShineSmall";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.noLight = false;
            dust.velocity *= 1.8f;
        }

        public override bool Update(Dust dust)
        {
            dust.alpha += 8;
            dust.position += dust.velocity;
            dust.velocity *= 0.935f;
            dust.rotation = dust.velocity.ToRotation() + MathHelper.PiOver2; //dust.rotation = (float)Math.Atan2(dust.velocity.Y, dust.velocity.X) + 1.57f;
            dust.scale *= 0.985f;
            if (dust.alpha > 230)
            {
                dust.active = false;
            }
            if (dust.noLight == false)
            {
                Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), dust.scale * 0.35f, dust.scale * 0.1f, dust.scale * 0.45f);
            }
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, new Color(255, 119, 246) { A = 0 } * dust.Opacity(), dust.rotation, Texture2D.Size() / 2f, new Vector2(1f, Math.Clamp(dust.velocity.Length() * 1.35f, 0.45f, 3.5f) * 0.75f) * dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class AetherSmoke : ModDust
    {
        public override string Texture => "GoldLeaf/Effects/Dusts/SpecialSmoke";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 1.6f);
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 36, 34, 36);
            dust.position -= new Vector2(17f, 18f) * dust.scale;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color color;
            if (dust.alpha < 30)
                color = new Color(255, 119, 246);
            else if (dust.alpha < 110)
                color = Color.Lerp(new Color(255, 119, 246), new Color(196, 43, 255), (dust.alpha - 30) / 80f);
            else if (dust.alpha < 140)
                color = Color.Lerp(new Color(196, 43, 255), new Color(54, 47, 56), (dust.alpha - 110) / 50f);
            else
                color = new Color(54, 47, 56);

            return color * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity.Y -= dust.alpha / 1750f;

            if (dust.alpha > 100)
            {
                dust.scale += 0.015f;
                dust.alpha += 2;
                dust.velocity *= 0.925f;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.99f;
                dust.alpha += 4;
                dust.velocity *= 0.975f;
            }

            dust.position += dust.velocity;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }

    public class AetherSmokeFast : ModDust
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
            if (dust.alpha < 20)
                color = new Color(179, 255, 224);
            else if (dust.alpha < 60)
                color = Color.Lerp(new Color(179, 255, 224), new Color(255, 119, 246), (dust.alpha - 20) / 40f);
            else if (dust.alpha < 100)
                color = Color.Lerp(new Color(255, 119, 246), new Color(54, 47, 56), (dust.alpha - 60) / 60f);
            else
                color = new Color(54, 47, 56);

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
