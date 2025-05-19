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
using ReLogic.Content;

namespace GoldLeaf.Items.Granite
{
	public class Bladecaster : ModItem
	{
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            ItemSets.Glowmask[Type] = (glowTex, Color.White with { A = 120 }, true);
        }

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

        /*public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D tex = glowTex.Value;
            Color color = ColorHelper.AdditiveWhite * (1 - ((float)Math.Sin(GoldLeafWorld.rottime) * 0.5f));

            for (float k = 0f; k < 1f; k += 1 / 3f)
            {
                Main.spriteBatch.Draw
                (
                    tex,
                    position + new Vector2(0f, 1.8f).RotatedBy((k + (GoldLeafWorld.rottime / 4)) * (((float)Math.PI * 2f)) + (0.5f - (float)Math.Sin(GoldLeafWorld.rottime) * 0.5f)),
                    new Rectangle(0, 0, tex.Width, tex.Height),
                    color * 0.4f, //ColorHelper.AdditiveWhite, 
                    0,
                    tex.Size() * 0.5f,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }
        }*/

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            //spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite, rotation, TextureAssets.Item[Item.type].Size()/2, scale, SpriteEffects.None, 0f);

            /*Texture2D tex = glowTex.Value;
            Vector2 vector = new
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - tex.Height * 0.5f
                );
            Color color = ColorHelper.AdditiveWhite * (1 - ((float)Math.Sin(GoldLeafWorld.rottime) * 0.5f));

            for (float k = 0f; k < 1f; k += 1/3f)
            {
                Main.spriteBatch.Draw
                (
                    tex, 
                    vector + new Vector2(0f, 2.4f).RotatedBy((k + (GoldLeafWorld.rottime / 4)) * (((float)Math.PI * 2f)) + (0.5f - (float)Math.Sin(GoldLeafWorld.rottime) * 0.5f)), 
                    new Rectangle(0, 0, tex.Width, tex.Height),
                    color * 0.4f, //ColorHelper.AdditiveWhite, 
                    rotation,
                    tex.Size() * 0.5f, 
                    scale, 
                    SpriteEffects.None, 
                    0f
                );
            }*/
        }
    }
}