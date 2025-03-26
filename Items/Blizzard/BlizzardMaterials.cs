using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Effects.Dusts;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using GoldLeaf.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using GoldLeaf.Items.Grove;
using Terraria.GameContent.ItemDropRules;
using Terraria.UI;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using System;
using ReLogic.Content;

namespace GoldLeaf.Items.Blizzard
{
    public class AuroraCluster : ModItem
    {
        /*private static Asset<Texture2D> greyTex;
        public override void Load()
        {
            greyTex = Request<Texture2D>(Texture + "Grey");
        }*/

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 50;
        }

        public override string Texture => "GoldLeaf/Items/Blizzard/AuroraCluster";

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 5, 0);
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;

            Item.alpha = 30;

            Item.width = 20;
            Item.height = 22;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            Item.color = ColorHelper.AuroraColor(Item.timeSinceItemSpawned * 0.05f) * (0.875f + (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f);

            if (Main.rand.NextBool(8))
            {
                Dust dust = Dust.NewDustDirect(Item.position, Item.width, Item.height, DustType<AuroraTwinkle>(), 0f, -1f, 0, ColorHelper.AuroraAccentColor(Item.timeSinceItemSpawned * 0.05f), Main.rand.NextFloat(0.25f, 0.5f));
                dust.noLight = true;
                dust.velocity *= 0.2f;
                dust.rotation = Main.rand.NextFloat(-14, 14);

                dust.customData = Item;
            }
        }

        public override void UpdateInventory(Player player)
        {
            Item.color = ColorHelper.AuroraColor(Item.timeSinceItemSpawned * 0.05f) * (0.875f + (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Color color = ColorHelper.AuroraColor(GoldLeafWorld.Timer * 0.05f); color.A = 0;

            spriteBatch.Draw(TextureAssets.Item[Item.type].Value, Item.Center - Main.screenPosition, null, ColorHelper.AuroraColor(Item.timeSinceItemSpawned * 0.05f) * (0.875f + (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f), rotation, TextureAssets.Item[Item.type].Size()/2, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureAssets.Item[Item.type].Value, Item.Center - Main.screenPosition, null, color * (0.3f - (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f), rotation, TextureAssets.Item[Item.type].Size() / 2, scale, SpriteEffects.None, 0f);
            //spriteBatch.Draw(TextureAssets.Item[Item.type].Value, Item.Center - Main.screenPosition, null, color * (0.5f - (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f), rotation, TextureAssets.Item[Item.type].Size() / 2, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Color color = ColorHelper.AuroraColor(GoldLeafWorld.Timer * 0.05f); color.A = 0;

            spriteBatch.Draw(TextureAssets.Item[Item.type].Value, position, null, ColorHelper.AuroraColor(GoldLeafWorld.Timer * 0.05f) * 0.75f * (0.875f + (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f), 0, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureAssets.Item[Item.type].Value, position, null, color * (0.3f - (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f), 0, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
    
    public class FrostCloth : ModItem
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        int dustTime = 12;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override string Texture => "GoldLeaf/Items/Blizzard/FrostCloth";

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 7, 50);
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;

            Item.width = 26;
            Item.height = 26;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Item.timeSinceItemSpawned % dustTime == 0)
            {
                Dust dust2 = Dust.NewDustPerfect(Item.Center + new Vector2(0f, Item.height * -0.1f) + Main.rand.NextVector2CircularEdge(Item.width * 0.6f, Item.height * 0.6f) * (0.3f + Main.rand.NextFloat() * 0.5f), DustType<LightDust>(), new Vector2(0f, (0f - Main.rand.NextFloat()) * 0.2f - 0.6f), 0, new Color(206, 174, 125) { A = 0 } * 0.5f, Main.rand.NextFloat(0.5f, 0.7f));
                dust2.fadeIn = 1.1f;
                dust2.noGravity = true;
                dust2.noLight = true;

                dustTime = Main.rand.Next(15, 30);
            }
            /*if (Item.timeSinceItemSpawned % dustTime == 0)
            {
                Color color = ColorHelper.AuroraColor(GoldLeafWorld.Timer * 0.05f); color.A = 0;

                dustTime = Main.rand.Next(14, 32);

                var dust = Dust.NewDustPerfect(new Vector2(Item.Center.X + Main.rand.Next(Item.width / -3, Item.width / 3), Item.position.Y + Main.rand.Next(0, Item.height)), DustType<ArcticDust>(), new Vector2(0, Main.rand.NextFloat(-0f, -0f)), 0, Color.White, 1);
                dust.noGravity = true;
            }*/
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite * 0.3f, rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);
        }
    }
}
