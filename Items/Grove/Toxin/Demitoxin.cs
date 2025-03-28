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
	public class Demitoxin : ModItem
	{
        public float squash = 0;
        public float rottime = 0;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Orange;

            ItemID.Sets.ItemNoGravity[Item.type] = true;
            //ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.ShimmerTransformToItem[Item.type] = ItemType<EveDroplet>();
        }

        public override void PostUpdate()
        {
            if (rottime == Math.PI)
                squash = 1f;

            if (squash > 0)
                squash -= 0.015f;
            if (squash < 0)
                squash = 0;

            rottime += (float)Math.PI / 60;
            if (rottime >= Math.PI * 2) rottime = 0;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Item.type].Value;

            Vector2 drawPos = new(Item.position.X - Main.screenPosition.X, Item.position.Y - Main.screenPosition.Y);
            float sin = (float)(Math.Sin(rottime * (2 + (3f * squash))) * (10 * squash));
            float cos = (float)(Math.Cos(rottime * (2 + (3f * squash))) * (10 * squash));
            
            spriteBatch.Draw(tex, new Rectangle((int)(drawPos.X - (sin)), (int)(drawPos.Y - (cos)), tex.Width + (int)(sin*2), tex.Height + (int)(cos*2)), Color.White);
            //spriteBatch.Draw(tex, new Rectangle((int)(Item.position.X - Main.screenPosition.X - Math.Sin(GoldLeafWorld.rottime / squash) * 2), (int)(Item.position.Y - Main.screenPosition.Y - Math.Cos(GoldLeafWorld.rottime / squash) * 2), (int)(tex.Width + (Math.Sin(GoldLeafWorld.rottime / squash) * 4)), (int)(tex.Height + (Math.Cos(GoldLeafWorld.rottime / squash) * 4))), Color.White);

            return false;
        }

        /*public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            //DrawBubble(Item.position - Main.screenPosition, spriteBatch, timer, lightColor);

            Texture2D tex = Request<Texture2D>(Texture).Value;

            int sin = (int)(Math.Sin(timer) * 4 * squash);
            int sin2 = (int)(Math.Sin(timer + 0.5f) * 4 * squash);

            Rectangle rectangle = new((int)Item.Center.X - sin, (int)Item.Center.Y + sin2, Item.width + (sin*2), Item.height - (sin2*2));
            //Rectangle rectangle = new(0 - sin, 0 + sin2, Item.width + sin, Item.height + sin2);

            //Rectangle bubbleTarget = new((int)pos.X - sin / 2, (int)pos.Y + sin2 / 2, 28 + sin, 26 - sin2);

            spriteBatch.Draw(tex, rectangle, lightColor);
            //spriteBatch.Draw(tex, rectangle, lightColor);


            /*Texture2D tex = Request<Texture2D>(Texture).Value;
            int sin = (int)(Math.Sin(timer) * 4);
            int sin2 = (int)(Math.Sin(timer + 0.75f) * 4);

            Rectangle rectangle = new(Item.width/2, 0, Item.width*2, Item.height);
            //Rectangle rectangle = new((int)Item.position.X - sin / 2, (int)Item.position.Y + sin2 / 2, 32 + sin, 32 - sin2);
            spriteBatch.Draw(tex, Item.Center - Main.screenPosition, rectangle, lightColor, rotation, tex.Size() / 2, scale, SpriteEffects.None, 0f);*

            //spriteBatch.Draw(tex, Item.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite * ((float)Math.Sin(GoldLeafWorld.rottime - 0.75f) * 0.75f), rotation, tex.Size() / 2, scale, SpriteEffects.None, 0f);

            
            //float sin = GoldLeafWorld.rottime;
            //float cos = GoldLeafWorld.rottime + (float)Math.PI;
            
            //Rectangle rectangle = new((int)(Item.position.X - sin) / 2, (int)(Item.position.Y - cos) / 2, Item.width + (int)sin, Item.height + (int)cos);

            //spriteBatch.Draw(tex, rectangle, lightColor);

            //spriteBatch.Draw(tex, Item.Center - Main.screenPosition, null, Color.White, rotation, tex.Size() / 2, scale, SpriteEffects.None, 0f);
            return false;
        }

        public virtual void DrawBubble(Vector2 pos, SpriteBatch spriteBatch, float time, Color color)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;
            int sin = (int)(Math.Sin(time) * 4);
            int sin2 = (int)(Math.Sin(time + 0.5f) * 4);
            var bubbleTarget = new Rectangle((int)pos.X - sin / 2, (int)pos.Y + sin2 / 2, 28 + sin, 26 - sin2);
            spriteBatch.Draw(tex, bubbleTarget, color);

            return;
        }*/
    }
}