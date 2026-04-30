using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using GoldLeaf.Core;
using ReLogic.Content;
using Terraria.ID;

namespace GoldLeaf.Effects.Dusts
{
    public class TwinkleDust : LightDust
    {
        private static Asset<Texture2D> bloomTex;
        public override void Load() => bloomTex = Request<Texture2D>("GoldLeaf/Textures/GlowSharp");

        public override string Texture => "GoldLeaf/Textures/Flares/FlareSmall";

        public static new Dust Spawn(LightDustData data, Vector2 position, Vector2 spawnBox, Vector2 velocity, int alpha = 0, Color color = default, float scale = 1f)
        {
            Dust dust = Dust.NewDustDirect(position, (int)spawnBox.X, (int)spawnBox.Y, DustType<TwinkleDust>(), velocity.X, velocity.Y, alpha, color, scale);
            dust.customData = data;
            return dust;
        }
        public static new Dust SpawnPerfect(LightDustData data, Vector2 position, Vector2 velocity, int alpha = 0, Color color = default, float scale = 1f)
        {
            Dust dust = Dust.NewDustPerfect(position, DustType<TwinkleDust>(), velocity, alpha, color, scale);
            dust.customData = data;
            return dust;
        }

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 74, 74);
            dust.scale *= 0.15f;
            dust.rotation = MathHelper.ToRadians(45);
            dust.alpha -= 25;
            dust.fadeIn += 1;

            dust.customData ??= new LightDustData(0.925f);
        }

        public override void SafeUpdate(Dust dust)
        {
            if (dust.customData is LightDustData data)
                data.rotationVelocity *= data.drag;
        }

        public override bool PreDraw(Dust dust)
        {
            Color color = dust.color.Alpha();
            int brightness = (color.R + color.G + color.B) / 3;

            if (!dust.noLightEmittence)
                Main.spriteBatch.Draw(bloomTex.Value, dust.position - Main.screenPosition, null, color * ((180 - dust.alpha) / 255f) * 0.85f, dust.rotation, bloomTex.Size() / 2f, dust.scale * 2.35f, SpriteEffects.None, 0);
            
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, color * ((100 - dust.alpha) / 255f) * 0.5f, dust.rotation, dust.frame.Size()/2f, dust.scale * 1.35f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, color with { A = 0 }, dust.rotation, dust.frame.Size()/2f, dust.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
