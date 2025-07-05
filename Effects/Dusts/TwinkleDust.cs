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
            Color color = dust.color;
            int brightness = (color.R + color.G + color.B) / 3;
            //Color coreColor = new(color.R + (brightness * 1.4f), color.G + (brightness * 1.4f), color.B + (brightness * 1.4f));
            color.A = 0; //coreColor.A = 0;

            //Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.Black * ((175 - dust.alpha) / 255f) * 0.1f, dust.rotation, dust.frame.Size()/2f, dust.scale * 1.6f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, color * ((100 - dust.alpha) / 255f) * 0.5f, dust.rotation, dust.frame.Size()/2f, dust.scale * 1.35f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, color with { A = 0 }, dust.rotation, dust.frame.Size()/2f, dust.scale, SpriteEffects.None, 0);
            //Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, coreColor * (1 - (brightness / 255)) /** ((225 - dust.alpha) / 255f) * 0.5f*/, dust.rotation, dust.frame.Size()/2f, dust.scale * 0.55f, SpriteEffects.None, 0);

            //Main.spriteBatch.Draw(Request<Texture2D>("GoldLeaf/Effects/Dusts/LightDust").Value, dust.position - Main.screenPosition, dust.frame, dust.color * dust.alpha, GoldLeafWorld.rottime * 2, new Vector2(0, 0), dust.scale * 1.5f, SpriteEffects.None, 0);
            //Main.spriteBatch.Draw(Request<Texture2D>("GoldLeaf/Effects/Dusts/LightDust").Value, dust.position - Main.screenPosition, dust.frame, dust.color * dust.alpha, GoldLeafWorld.rottime * -2, new Vector2(0, 0), dust.scale * 1.5f, SpriteEffects.None, 0);
            return false;
        }
    }
}
