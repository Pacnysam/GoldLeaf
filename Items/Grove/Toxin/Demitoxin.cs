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
            if (Item.timeSinceItemSpawned % 240 >= 120 && Item.timeSinceItemSpawned % 240 < 150)
                squash = 1f;

            //Main.NewText((Item.timeSinceItemSpawned * 60));

            if (squash > 0)
            {
                squash *= 0.98f;
                squash -= 0.005f;
            }
            if (squash < 0f)
                squash = 0f;

            //Main.NewText(squash);

            rottime += (float)Math.PI / 60;
            if (rottime >= Math.PI * 2) rottime = 0;
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

            Vector2 drawPos = Item.position - Main.screenPosition;
            float sin = (float)(Math.Sin(rottime * (2f + (2f * squash))) * 7f) * squash;
            float cos = (float)(Math.Cos(rottime * (2f + (2f * squash))) * 7f) * squash;
            
            spriteBatch.Draw(tex, new Rectangle((int)(drawPos.X - (scale * sin)), (int)(drawPos.Y - (scale * cos)), (int)(scale * tex.Width + (sin*2)), (int)(scale * tex.Height + (cos*2))), null, lightColor, rotation, Vector2.Zero, SpriteEffects.None, 0f);
            //spriteBatch.Draw(tex, new Rectangle((int)(Item.position.X - Main.screenPosition.X - Math.Sin(GoldLeafWorld.rottime / squash) * 2), (int)(Item.position.Y - Main.screenPosition.Y - Math.Cos(GoldLeafWorld.rottime / squash) * 2), (int)(tex.Width + (Math.Sin(GoldLeafWorld.rottime / squash) * 4)), (int)(tex.Height + (Math.Cos(GoldLeafWorld.rottime / squash) * 4))), Color.White);

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