using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Effects.Dusts;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using GoldLeaf.Core;
using Mono.Cecil;
using Terraria.DataStructures;
using System;
using System.Diagnostics.Metrics;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;

namespace GoldLeaf.Items.Grove.Toxin
{
	public class Demitoxin : ModItem //this is atrocious but toxin stuff is years away so idgaf
	{
        public float squash;
        public float rottime;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.LightRed;

            ItemID.Sets.ItemNoGravity[Item.type] = true;
            //ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.ShimmerTransformToItem[Item.type] = ItemType<EveDroplet>();
        }

        public override void PostUpdate()
        {
            rottime += (float)Math.PI / 120;

            squash = MathHelper.Lerp(squash, 0f, 0.0125f);
            /*if (squash > 0)
            {
                squash *= 0.98f;
                squash -= 0.005f;
            }
            if (squash < 0f)
                squash = 0f;*/

            if (rottime >= Math.PI * 2)
            {
                squash = 1.5f;
                rottime = 0;
            }
        }

        public override void UpdateInventory(Player player)
        {
            rottime += (float)Math.PI / 120;

            squash = MathHelper.Lerp(squash, 0f, 0.0125f);
            /*if (squash > 0)
            {
                squash *= 0.98f;
                squash -= 0.005f;
            }
            if (squash < 0f)
                squash = 0f;*/
            
            if (rottime >= Math.PI * 2) 
            {
                squash = 1.5f;
                rottime = 0;
            }
        }

        /*public override void UpdateInventory(Player player)
        {
            if (Main.GlobalTimeWrappedHourly*60 % 240 < 30)
                squash = 1f;

            //Main.NewText((int)(Main.GlobalTimeWrappedHourly * 60));

            if (squash > 0)
            {
                squash *= 0.98f;
                squash -= 0.005f;
            }
            if (squash < 0f)
                squash = 0f;

            //Main.NewText(squash);
        }*/

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Item.type].Value;

            //Vector2 drawPos = Item.position - Main.screenPosition;
            float sin = (float)(Math.Sin(rottime * (2f + (3f * squash))) * 7f) * Math.Clamp(squash, 0f, 1f);
            float cos = (float)(Math.Cos(rottime * (2f + (3f * squash))) * 7f) * Math.Clamp(squash, 0f, 1f);

            Color color = new(46, 10, 79);

            spriteBatch.Draw(tex, Item.Center + new Vector2(0f, 3f - Math.Clamp(squash, 0f, 1f)).RotatedBy((Main.GlobalTimeWrappedHourly * 1.5f) * ((float)Math.PI)) - Main.screenPosition, null, color * (float)Math.Sin(rottime * 2.5f) * 0.55f, rotation, tex.Size()/2, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(tex, Item.Center + new Vector2(0f, -3f + Math.Clamp(squash, 0f, 1f)).RotatedBy((Main.GlobalTimeWrappedHourly * -1.5f) * ((float)Math.PI)) - Main.screenPosition, null, color * (float)Math.Sin(-rottime * 2) * 0.55f, rotation, tex.Size() / 2, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(tex, Item.Center + new Vector2(0f, 3f - Math.Clamp(squash, 0f, 1f)).RotatedBy((Main.GlobalTimeWrappedHourly * 1.5f) * ((float)Math.PI)) - Main.screenPosition, null, color with { A = 0 } * (float)Math.Cos(rottime * 1.5f) * 0.7f, rotation, tex.Size() / 2, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(tex, Item.Center + new Vector2(0f, -3f + Math.Clamp(squash, 0f, 1f)).RotatedBy((Main.GlobalTimeWrappedHourly * -1.5f) * ((float)Math.PI)) - Main.screenPosition, null, color with { A = 0 } * (float)Math.Cos(-rottime * 1f) * 0.7f, rotation, tex.Size() / 2, scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(tex, Item.Center - Main.screenPosition, null, lightColor, rotation, tex.Size()/2f, new Vector2(scale) * (new Vector2(1) + new Vector2(cos * 0.08f, sin * 0.08f)), SpriteEffects.None, 0f);
            //spriteBatch.Draw(tex, new Rectangle((int)(drawPos.X - (scale * sin)), (int)(drawPos.Y - (scale * cos)), (int)(scale * tex.Width + (sin*2)), (int)(scale * tex.Height + (cos*2))), null, lightColor, rotation, Vector2.Zero, SpriteEffects.None, 0f);
            //spriteBatch.Draw(tex, new Rectangle((int)(Item.position.X - Main.screenPosition.X - Math.Sin(GoldLeafWorld.rottime / squash) * 2), (int)(Item.position.Y - Main.screenPosition.Y - Math.Cos(GoldLeafWorld.rottime / squash) * 2), (int)(tex.Width + (Math.Sin(GoldLeafWorld.rottime / squash) * 4)), (int)(tex.Height + (Math.Cos(GoldLeafWorld.rottime / squash) * 4))), Color.White);

            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D tex = TextureAssets.Item[Item.type].Value;

            float sin = (float)(Math.Sin(GoldLeafWorld.rottime * (2f + (6f * Math.Clamp(squash, 0f, 1f)))) * 7f) * Math.Clamp(squash, 0f, 1f) * 0.7f;
            float cos = (float)(Math.Cos(GoldLeafWorld.rottime * (2f + (6f * Math.Clamp(squash, 0f, 1f)))) * 7f) * Math.Clamp(squash, 0f, 1f) * 0.7f;

            Color color = new(46, 10, 79);

            spriteBatch.Draw(tex, position + new Vector2(0f, 2f - Math.Clamp(squash, 0f, 1f)).RotatedBy((Main.GlobalTimeWrappedHourly * 1.5f) * ((float)Math.PI)), frame, color * (float)Math.Sin(GoldLeafWorld.rottime * 2.5f) * 0.55f, 0, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(tex, position + new Vector2(0f, -2f + Math.Clamp(squash, 0f, 1f)).RotatedBy((Main.GlobalTimeWrappedHourly * -1.5f) * ((float)Math.PI)), frame, color * (float)Math.Sin(-GoldLeafWorld.rottime * 2) * 0.55f, 0, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(tex, position + new Vector2(0f, 2f - Math.Clamp(squash, 0f, 1f)).RotatedBy((Main.GlobalTimeWrappedHourly * 1.5f) * ((float)Math.PI)), frame, color with { A = 0 } * (float)Math.Cos(GoldLeafWorld.rottime * 1.5f) * 0.7f, 0, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(tex, position + new Vector2(0f, -2f + Math.Clamp(squash, 0f, 1f)).RotatedBy((Main.GlobalTimeWrappedHourly * -1.5f) * ((float)Math.PI)), frame, color with { A = 0 } * (float)Math.Cos(-GoldLeafWorld.rottime * 1f) * 0.7f, 0, origin, scale, SpriteEffects.None, 0f);

            if (Main.mouseItem == Item) return true;

            spriteBatch.Draw(tex, position, frame, drawColor, 0f, origin, new Vector2(scale) * (new Vector2(1) + new Vector2(cos * 0.08f, sin * 0.08f)), SpriteEffects.None, 0f);
            //spriteBatch.Draw(tex, new Rectangle((int)position.X, (int)position.Y, (int)(scale * tex.Width + (sin * 2)), (int)(scale * tex.Height + (cos * 2))), frame, drawColor, 0f, origin, SpriteEffects.None, 0f);
            return false;
        }

        /*public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D tex = TextureAssets.Item[Item.type].Value;

            float sin = (float)(Math.Sin(GoldLeafWorld.rottime * (2.4f + (2f * squash))) * 6f) * squash;
            float cos = (float)(Math.Cos(GoldLeafWorld.rottime * (2.4f + (2f * squash))) * 6f) * squash;

            //spriteBatch.Draw(tex, new Rectangle((int)(position.X - (scale * cos)), (int)(position.Y - (scale * sin)), (int)(scale * tex.Width + (cos * 2)), (int)(scale * tex.Height + (sin * 2))), null, drawColor, 0, Vector2.Zero, SpriteEffects.None, 0f);
            spriteBatch.Draw(tex, new Rectangle((int)(position.X - (scale * cos)), (int)(position.Y - (scale * sin)), (int)(scale * tex.Width + (cos * 2)), (int)(scale * tex.Height + (sin * 2))), null, drawColor, 0, tex.Size()/2, SpriteEffects.None, 0f);

            return false;
        }*/
    }
}