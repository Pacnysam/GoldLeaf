using GoldLeaf.Core;
using GoldLeaf.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Effects.Dusts
{
    public class ArcticDust : ModDust
    {
        public static readonly int afterImageCount = 6;
        private static Asset<Texture2D> bloomTex;
        private static Asset<Texture2D> flareTex;
        public override void Load()
        {
            bloomTex = Request<Texture2D>("GoldLeaf/Textures/Glow");
            flareTex = Request<Texture2D>("GoldLeaf/Textures/Flares/FlareSmall");
        }

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

            //dust.position += new Vector2(5, 5);
        }

        public override bool Update(Dust dust)
        {
            /*if (dust.customData is not List<Vector2>)
            {
                dust.position -= new Vector2(9, 9) * dust.scale;
                dust.customData = new List<Vector2>();
            }
            else if (dust.customData is List<Vector2>)
            {
                List<Vector2> oldPos = dust.customData as List<Vector2>;

                if (dust.velocity.Length() > 0)
                    oldPos.Add(dust.position);

                if (oldPos.Count > afterImageCount || (oldPos.Count > 0 && dust.velocity.Length() <= 0.15f))
                {
                    oldPos.RemoveAt(0);
                }

                dust.customData = oldPos;
            }*/

            if (dust.alpha % 30 == 25)
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

        public override bool PreDraw(Dust dust)
        {
            Vector2 dustCenter = dust.position + (dust.frame.Size() / 2f * dust.scale);
            Texture2D flare = Request<Texture2D>("GoldLeaf/Textures/Flare").Value;
            //trail
            /*if (dust.customData is List<Vector2>) 
            {
                Vector2[] oldPos = [.. dust.customData as List<Vector2>];

                for (int k = oldPos.Length - 1; k >= 0; k--)
                {
                    Vector2 drawPos = oldPos[k] - Main.screenPosition;// - (dust.frame.Size() / 2f * dust.scale);
                    Color color1 = new(0, 225, 241) { A = 185 };
                    Color color2 = new(0, 38, 128) { A = 185 };

                    Main.spriteBatch.Draw(Texture2D.Value, drawPos, dust.frame, Color.Lerp(color1, color2, k / (oldPos.Length + 2f)) * (0.65f - (k / 6f)), dust.rotation, dust.frame.Size() / 2, dust.scale, SpriteEffects.None, 0);
                }
            }*/
            //glow
            Main.spriteBatch.Draw(bloomTex.Value, dust.position - Main.screenPosition, null, Color.RoyalBlue.Alpha(180) * MathHelper.Clamp(dust.Opacity() - 0.45f, 0f, 0.4f), dust.rotation, bloomTex.Size() / 2, dust.scale * MathHelper.Clamp(dust.Opacity() * 1.5f, 0.45f, 0.95f) * 0.425f, SpriteEffects.None, 0);
            //dust
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.White, dust.rotation, dust.frame.Size() / 2f, dust.scale, SpriteEffects.None, 0);
            //flare
            Main.spriteBatch.Draw(flare, dust.position - Main.screenPosition, null, Color.LightSkyBlue.Alpha(0) * MathHelper.Clamp(dust.Opacity() - 0.45f, 0f, 0.15f) * 0.35f, dust.rotation, flare.Size() / 2, new Vector2(Math.Min((1 - dust.Opacity()) * 3f, 3f), 0.85f) * dust.scale * MathHelper.Clamp(dust.Opacity() * 1.75f, 0.35f, 0.85f) * 0.125f, SpriteEffects.None, 0);
            //front glow
            Main.spriteBatch.Draw(bloomTex.Value, dust.position - Main.screenPosition, null, Color.LightSkyBlue.Alpha() * MathHelper.Clamp(dust.Opacity() - 0.25f, 0f, 0.25f), dust.rotation, bloomTex.Size() / 2, dust.scale * MathHelper.Clamp((1 - dust.Opacity()) * 1.75f, 0.35f, 0.85f) * 0.25f, SpriteEffects.None, 0);
            
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
    
    public class SnowCloud : ModDust 
    {
        public override string Texture => "GoldLeaf/Effects/Dusts/SpecialSmoke";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 36, 34, 36);
            dust.color = new Color(238, 248, 252);
            dust.scale *= 0.1f;
            dust.alpha += 30;
            dust.position -= dust.frame.Size()/2 * dust.scale;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color color = Color.Lerp(Color.White, Color.CornflowerBlue, 0.05f);

            return Color.Lerp(color * 0.75f, lightColor, 0.65f) * (1f - (dust.alpha/255f)) * 0.5f;
        }

        public override bool Update(Dust dust)
        {
            dust.scale *= 1.015f;

            if (dust.alpha > 120)
            {
                dust.velocity *= 0.925f;
                dust.alpha += 4;
            }
            else
            {
                dust.velocity *= 0.935f;
                dust.alpha += 6;
            }

            dust.position += dust.velocity;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }
    
    public class FrostShard : ModDust
    {
        public override string Texture => "GoldLeaf/Items/Blizzard/FrostShards";

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(Main.rand.Next(7) * 14, 0, 14, 24);
            dust.position -= dust.frame.Size() / 2 * dust.scale;

            if (dust.noGravity)
                dust.alpha -= 30;
        }

        public override bool Update(Dust dust)
        {
            if (dust.noGravity)
            {
                dust.velocity *= 0.9f;
                dust.rotation *= 0.95f;
            }   
            else
            {
                dust.velocity.X *= 0.97f;
                dust.velocity.Y += 0.15f;

                dust.rotation = dust.velocity.ToRotation() + 1.57f;
            }

            dust.position += dust.velocity;

            Tile tile = Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16];
            if (tile.HasTile && Main.tileSolid[tile.TileType] && !dust.noGravity)
            {
                dust.velocity *= 0.6f;
                /*if (dust.customData != null && dust.customData is Vector2 oldVelocity)
                {
                    if (dust.velocity.X != oldVelocity.X)
                    {
                        dust.velocity.X = -oldVelocity.X;
                    }
                    if (dust.velocity.Y != oldVelocity.Y)
                    {
                        dust.velocity.Y = -oldVelocity.Y;
                    }

                    dust.velocity *= 0.6f;
                }
                dust.customData = dust.velocity;*/
            }

            if (dust.velocity.Length() <= 2f)
            {
                dust.alpha += 5;
                //dust.noGravity = true;
            }
            else 
            {
                dust.alpha += 1;
            }

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }
}
