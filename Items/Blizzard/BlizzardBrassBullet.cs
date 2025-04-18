using System;
using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Items.Grove;
using System.Diagnostics.Metrics;
using GoldLeaf.Effects.Dusts;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using GoldLeaf.Items.Misc.Accessories;
using ReLogic.Content;

namespace GoldLeaf.Items.Blizzard
{
    public class BlizzardBrassBullet : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.MusketBall);

            Item.width = 12;
            Item.height = 18;

            Item.damage = 8;
            Item.knockBack = 0.8f;
            Item.DamageType = DamageClass.Ranged;

            Item.value = Item.sellPrice(0, 0, 5, 50);
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;

            Item.shoot = ProjectileType<BlizzardBrassBulletP>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(100);
            recipe.AddIngredient(ItemID.MusketBall, 100);
            recipe.AddIngredient(ItemType<AuroraCluster>());
            recipe.AddTile(TileID.Anvils);
            recipe.AddOnCraftCallback(RecipeCallbacks.AuroraCraftEffect);
            recipe.Register();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            spriteBatch.Draw(TextureAssets.Item[Item.type].Value, Item.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite * 0.3f, rotation, TextureAssets.Item[Item.type].Value.Size() / 2, scale, SpriteEffects.None, 0f);
        }
    }

    public class BlizzardBrassBulletP : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 14;

            //Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.alpha = 255;

            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}
