using Terraria;
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
using GoldLeaf.Items.Grove;
using Terraria.GameContent.ItemDropRules;
using Terraria.UI;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Items.Blizzard;

namespace GoldLeaf.Items.Granite
{
	public class Bladecaster : ModItem
	{
        public override void SetDefaults()
		{
			Item.damage = 19;
			Item.DamageType = DamageClass.Melee;

            Item.UseSound = SoundID.NPCDeath55;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 30;
			Item.useAnimation = 30;

			Item.crit = 28;
			Item.GetGlobalItem<GoldLeafItem>().critDamageMod = -0.5f;

            Item.knockBack = 8f;

            Item.value = Item.sellPrice(0, 1, 25, 0);
            Item.rare = ItemRarityID.Blue;

            Item.width = 38;
            Item.height = 42;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Granite, 12)
                //.AddIngredient(ItemType<Magnesium>(), 18)
                .AddRecipeGroup("GoldLeaf:SilverBars", 7)
                .AddCondition(GoldLeafConditions.LearnedRecipe(Item.type))
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                //left click
            }
            else
            {
                
            }
            return true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            //player.itemTime += 10;
            //player.itemAnimation += 10;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D tex = Request<Texture2D>(Texture + "Glow").Value;
            Color color = ColorHelper.AdditiveWhite * (1 - ((float)Math.Sin(GoldLeafWorld.rottime) * 0.5f));

            for (float k = 0f; k < 1f; k += 1 / 3f)
            {
                Main.spriteBatch.Draw
                (
                    tex,
                    position + new Vector2(0f, 1.8f).RotatedBy((k + (GoldLeafWorld.rottime / 4)) * (((float)Math.PI * 2f)) + (0.5f - (float)Math.Sin(GoldLeafWorld.rottime) * 0.5f))/* globalTimeWrappedHourly*/,
                    new Rectangle(0, 0, tex.Width, tex.Height),
                    color * 0.4f, //ColorHelper.AdditiveWhite, 
                    0,
                    tex.Size() * 0.5f,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D tex = Request<Texture2D>(Texture + "Glow").Value;
            Vector2 vector = new
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - tex.Height * 0.5f
                );
            Color color = ColorHelper.AdditiveWhite * (1 - ((float)Math.Sin(GoldLeafWorld.rottime) * 0.5f));
            /*spriteBatch.Draw
            (
                tex,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - tex.Height * 0.5f
                ),
                new Rectangle(0, 0, tex.Width, tex.Height),
                Color.White,
                rotation,
                tex.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );*/

            /*float num4 = GoldLeafWorld.rottime;
            float globalTimeWrappedHourly = Main.GlobalTimeWrappedHourly;
            globalTimeWrappedHourly %= 4f;
            globalTimeWrappedHourly /= 2f;
            if (globalTimeWrappedHourly >= 1f)
            {
                globalTimeWrappedHourly = 2f - globalTimeWrappedHourly;
            }
            globalTimeWrappedHourly = globalTimeWrappedHourly * 0.5f + 0.5f;*/
            for (float k = 0f; k < 1f; k += 1/3f)
            {
                Main.spriteBatch.Draw
                (
                    tex, 
                    vector + new Vector2(0f, 2.4f).RotatedBy((k + (GoldLeafWorld.rottime / 4)) * (((float)Math.PI * 2f)) + (0.5f - (float)Math.Sin(GoldLeafWorld.rottime) * 0.5f))/* globalTimeWrappedHourly*/, 
                    new Rectangle(0, 0, tex.Width, tex.Height),
                    color * 0.4f, //ColorHelper.AdditiveWhite, 
                    rotation,
                    tex.Size() * 0.5f, 
                    scale, 
                    SpriteEffects.None, 
                    0f
                );
            }
            /*for (float k = 0f; k < 1f; k += 0.34f)
            {
                Main.spriteBatch.Draw(texture, vector3 + new Vector2(0f, 4f).RotatedBy((k + num4) * ((float)Math.PI * 2f)) * globalTimeWrappedHourly, new Rectangle(0, 0, tex.Width, tex.Height), new Color(140, 120, 255, 77), num, vector, scale, SpriteEffects.None, 0f);
            }*/

            /*for (int i = 0; i < 1; i++)
            {
                float num8 = (float)(Math.Cos((double)Main.GlobalTimeWrappedHourly % 2.40000009536743 / 2.40000009536743 * 6.28318548202515) / 5 + 0.5);
                float num10 = 0.0f;
                Vector2 vector2_3 = new(tex.Width / 2, (tex.Height / 1 / 2));
               Color color2 = ColorHelper.AdditiveWhite;
                Rectangle r = tex.Frame(1, 1, 0, 0);
                for (int k = 0; k < 4; ++k)
                {
                    Color newColor2 = color2;
                    Color color3 = Item.GetAlpha(newColor2) * (0.85f - num8);
                    Vector2 position2 = new Vector2(Item.Center.X, Item.Center.Y) + ((float)(k / 4 * 6.28318548202515) + rotation + num10).ToRotationVector2() * (float)(4.0 * (double)num8 + 2.0) - Main.screenPosition - new Vector2(tex.Width, (tex.Height / 1)) * Item.scale / 2f + vector2_3 * Item.scale;
                    spriteBatch.Draw(tex, position2, new Microsoft.Xna.Framework.Rectangle?(r), color3, rotation, vector2_3, Item.scale * 1.1f, SpriteEffects.None, 0.0f);
                }
            }*/
        }
    }
}