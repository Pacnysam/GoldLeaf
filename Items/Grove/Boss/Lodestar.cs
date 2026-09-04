using GoldLeaf.Core;
using GoldLeaf.Effects.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Grove.Boss
{
	public class Lodestar : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.LightRed;

            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            
            spriteBatch.Draw(texture, position, frame, Color.DarkViolet * 0.75f, 0, origin, scale, SpriteEffects.None, 0f); //shadow
            for (float k = 0f; k < 1f; k += 1/3f)
            {
                Color color = ColorHelper.QuickGradient([Color.Coral, Color.LightGoldenrodYellow, Color.HotPink], false)
                    .GetColor((float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.5f + 0.5f).Alpha();

                Vector2 drawPosition = position + new Vector2(0f, 1.5f + ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f) * 0.5f + 0.5f) * (k * 4f))
                    .RotatedBy(((-k * (MathHelper.Pi/5f)) + (Main.GlobalTimeWrappedHourly * 2f)) * ((float)Math.PI * 2f));
                
                spriteBatch.Draw(texture, drawPosition, frame, color * 0.35f, 0, origin, scale, SpriteEffects.None, 0f);
            } //spinning effect
            spriteBatch.Draw(texture, position, frame, Color.White.Alpha(225) * 0.85f, 0, origin, scale, SpriteEffects.None, 0f); //texture

            //TODO: reuse effect for chalcedony
            /*float pulseScale = (float)(Main.GlobalTimeWrappedHourly * 2f % 1f / 1f);
            spriteBatch.Draw(texture, position, frame, Color.White.Alpha() * (1f - pulseScale) * 0.5f, 0, origin, (1f + (pulseScale * 1.25f)) * scale, SpriteEffects.None, 0f);*/
            return false;
        }
    }
}