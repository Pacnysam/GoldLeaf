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
using System.IO;

namespace GoldLeaf.Items.Grove.Boss
{
	public class HeavenShard : ModItem
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
            Item.rare = ItemRarityID.Green;

            ItemID.Sets.ItemNoGravity[Item.type] = true;
            ItemID.Sets.ShimmerTransformToItem[Item.type] = ItemType<EveDroplet>();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D tex = TextureAssets.Item[Item.type].Value;

            for (float k = 0f; k < 1f; k += 0.5f)
            {
                Color color = ColorHelper.AdditiveWhite() * (1 - (float)Math.Sin(Main.GlobalTimeWrappedHourly));
                Main.spriteBatch.Draw
                (
                    tex,
                    position + new Vector2(0f, 1f - (2f * (float)Math.Sin(Main.GlobalTimeWrappedHourly))).RotatedBy((k + (Main.GlobalTimeWrappedHourly * 1.5)) * ((float)Math.PI * 2f)),
                    new Rectangle(0, 0, tex.Width, tex.Height),
                    color * 0.4f,
                    0,
                    tex.Size() * 0.5f,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }
            for (float k = 0f; k < 1f; k += 0.5f)
            {
                Color color = ColorHelper.AdditiveWhite() * (1 - (float)Math.Cos(Main.GlobalTimeWrappedHourly));
                Main.spriteBatch.Draw
                (
                    tex,
                    position + new Vector2(0f, 1f - (2f * (float)Math.Cos(Main.GlobalTimeWrappedHourly))).RotatedBy((k + (Main.GlobalTimeWrappedHourly * -1.5)) * ((float)Math.PI * 2f)),
                    new Rectangle(0, 0, tex.Width, tex.Height),
                    color * 0.4f,
                    0,
                    tex.Size() * 0.5f,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }
            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(TextureAssets.Item[Item.type].Value, position, null, ColorHelper.AdditiveWhite() * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 3) * 0.65f), 0, origin, scale, SpriteEffects.None, 0f);
        }
    }
}