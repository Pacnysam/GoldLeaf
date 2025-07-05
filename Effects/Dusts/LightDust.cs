using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using GoldLeaf.Core;
using ReLogic.Content;
using Terraria.GameContent;
using System.Diagnostics.CodeAnalysis;

namespace GoldLeaf.Effects.Dusts
{
    public class LightDust : ModDust
    {
        public struct LightDustData(float drag = 0f, float rotationVelocity = 0f, Player owner = null)
        {
            public int counter;
            public float drag = drag;
            public float rotationVelocity = rotationVelocity;
            public Player owner = owner;

            /*public override readonly bool Equals([NotNullWhen(true)] object obj)
            {
                return (drag != 0f !& rotationVelocity != 0f || owner != null);
            }
            public static bool operator ==(LightDustData left, LightDustData right) 
                => (left.drag == right.drag && left.rotationVelocity == right.rotationVelocity && left.owner == right.owner);
            public static bool operator !=(LightDustData left, LightDustData right) 
                => !(left.drag == right.drag && left.rotationVelocity == right.rotationVelocity && left.owner == right.owner);
            
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }*/
        }
        public static Dust Spawn(LightDustData data, Vector2 position, Vector2 spawnBox, Vector2 velocity, int alpha = 0, Color color = default, float scale = 1f)
        {
            Dust dust = Dust.NewDustDirect(position, (int)spawnBox.X, (int)spawnBox.Y, DustType<LightDust>(), velocity.X, velocity.Y, alpha, color, scale);
            dust.customData = data;
            return dust;
        }
        public static void SpawnPerfect(LightDustData data, Vector2 position, Vector2 velocity, int alpha = 0, Color color = default, float scale = 1f)
        {
            Dust dust = Dust.NewDustPerfect(position, DustType<LightDust>(), velocity, alpha, color, scale);
            dust.customData = data;
        }

        public virtual float MaxFadeIn => 3f;
        //public virtual LightDustData DefaultData => new(0.95f, 0.5f);

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 14, 14);
            dust.scale *= 0.225f;
            dust.alpha -= 20;
            dust.fadeIn += 1;

            dust.customData ??= new LightDustData(0.95f, 0f);

            /*if (dust.customData is not LightDustData)
                dust.customData = new LightDustData(0.95f);*/
        }

        public virtual void SafeUpdate(Dust dust)
        {

        }

        public override bool Update(Dust dust)
        {
            if (dust.customData is LightDustData data)
            {
                data.counter++;

                dust.position += dust.velocity;
                dust.velocity *= data.drag;

                if (dust.alpha > 1) dust.alpha = (int)(dust.alpha * (1 + (0.03f * (MaxFadeIn - Math.Clamp(dust.fadeIn, 0, MaxFadeIn)))));

                if (dust.alpha < 1)
                {
                    dust.alpha += 1;
                    dust.scale *= 1.05f;
                    //dust.scale *= 1 + (0.05f * (MaxFadeIn - Math.Clamp(dust.fadeIn, 0, MaxFadeIn)));

                    //dust.scale += 0.045f;
                    /*if (data.counter > dust.fadeIn)
                    {
                        dust.alpha += 1;
                        dust.scale *= 1 + (0.05f * (MaxFadeIn - Math.Clamp(dust.fadeIn, 0, MaxFadeIn)));
                    }
                    else
                        data.counter++;*/
                }
                else
                {
                    //dust.scale -= 0.025f;
                    //dust.scale *= 0.94f;

                    dust.scale *= 1 - (0.06f * (MaxFadeIn - Math.Clamp(dust.fadeIn, 0, MaxFadeIn)));
                    dust.alpha += 7 * (int)(MaxFadeIn - Math.Clamp(dust.fadeIn, 0, MaxFadeIn));
                }

                dust.rotation += data.rotationVelocity;

                if (!dust.noLight)
                    Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.3f * dust.scale);

                if (dust.alpha > 250)
                {
                    dust.active = false;
                }

                SafeUpdate(dust);
            }
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White * (1f - (Math.Clamp(dust.alpha, 0, 255) / 255f));
        }

        public override bool PreDraw(Dust dust)
        {
            Color color = dust.color;
            int brightness = (color.R + color.G + color.B) / 3;
            Color coreColor = new(color.R + (brightness * 1.4f), color.G + (brightness * 1.4f), color.B + (brightness * 1.4f));
            color.A = 0; coreColor.A = 0;

            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.Black * ((175 - dust.alpha) / 255f) * 0.2f, dust.rotation, dust.frame.Size() / 2f, dust.scale * 1.6f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, color * ((100 - dust.alpha) / 255f) * 0.5f, dust.rotation, dust.frame.Size() / 2f, dust.scale * 1.35f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, color, dust.rotation, dust.frame.Size() / 2f, dust.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, coreColor * (1 - (brightness/255)) /** ((225 - dust.alpha) / 255f) * 0.5f*/, dust.rotation, dust.frame.Size() / 2f, dust.scale * 0.55f, SpriteEffects.None, 0);

            //Main.spriteBatch.Draw(Request<Texture2D>("GoldLeaf/Effects/Dusts/LightDust").Value, dust.position - Main.screenPosition, dust.frame, dust.color * dust.alpha, GoldLeafWorld.rottime * 2, new Vector2(0, 0), dust.scale * 1.5f, SpriteEffects.None, 0);
            //Main.spriteBatch.Draw(Request<Texture2D>("GoldLeaf/Effects/Dusts/LightDust").Value, dust.position - Main.screenPosition, dust.frame, dust.color * dust.alpha, GoldLeafWorld.rottime * -2, new Vector2(0, 0), dust.scale * 1.5f, SpriteEffects.None, 0);
            return false;
        }
    }

    public class LightDustFast : ModDust
    {
        public override string Texture => "GoldLeaf/Effects/Dusts/LightDust";

        private static Asset<Texture2D> tex;
        public override void Load()
        {
            tex = Request<Texture2D>(Texture);
        }

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 14, 14);
            dust.scale *= 0.35f;
            dust.alpha -= 18;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.9f;


            if (dust.alpha > 1) dust.alpha *= (int)1.03f;

            if (dust.alpha < 1)
            {
                //dust.scale += 0.045f;
                dust.alpha += 2;
                dust.scale *= 1.1f;
            }
            else
            {
                //dust.scale -= 0.025f;
                dust.scale *= 0.89f;
                dust.alpha += 14;
            }

            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.3f * dust.scale);

            if (dust.alpha > 250)
            {
                dust.active = false;
            }
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White * ((255 - dust.alpha) / 255f);
        }

        public override bool PreDraw(Dust dust)
        {
            Color color = dust.color;
            int brightness = (color.R + color.G + color.B) / 3;
            Color coreColor = new(color.R + (brightness * 1.4f), color.G + (brightness * 1.4f), color.B + (brightness * 1.4f));
            color.A = 0; coreColor.A = 0;

            Main.spriteBatch.Draw(tex.Value, new Vector2(dust.position.X, dust.position.Y) - Main.screenPosition, dust.frame, Color.Black * ((175 - dust.alpha) / 255f) * 0.2f, dust.rotation, dust.frame.Size() / 2f, dust.scale * 1.6f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex.Value, new Vector2(dust.position.X, dust.position.Y) - Main.screenPosition, dust.frame, color * ((100 - dust.alpha) / 255f) * 0.5f, dust.rotation, dust.frame.Size() / 2f, dust.scale * 1.35f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex.Value, dust.position - Main.screenPosition, dust.frame, color, dust.rotation, dust.frame.Size() / 2f, dust.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex.Value, dust.position - Main.screenPosition, dust.frame, coreColor * (1 - (brightness / 255)) /** ((225 - dust.alpha) / 255f) * 0.5f*/, dust.rotation, dust.frame.Size() / 2f, dust.scale * 0.55f, SpriteEffects.None, 0);

            return false;
        }
    }
}
