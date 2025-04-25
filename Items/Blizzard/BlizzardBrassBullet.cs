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
            Item.knockBack = 0.4f;
            Item.DamageType = DamageClass.Ranged;
            Item.shootSpeed = 6.5f;

            Item.value = Item.sellPrice(0, 0, 0, 2);
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;

            Item.shoot = ProjectileType<BlizzardBrassBulletP>();
        }

        /*public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile p = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, Item.crit);
            return false;
        }*/

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
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Bullet);
            Projectile.aiStyle = -1;

            Projectile.width = 3;
            Projectile.height = 3;

            Projectile.friendly = true;
            Projectile.tileCollide = true;

            Projectile.extraUpdates = 1;

            Projectile.DamageType = DamageClass.Ranged;

            Projectile.ai[0] = 0;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.CritChance >= 100) 
            {
                Projectile.ai[0] = 1;
                Projectile.extraUpdates *= 3;
            }
            else if (Main.rand.NextBool(Projectile.CritChance, 100)) 
            {
                Projectile.ai[0] = 1;
                Projectile.extraUpdates *= 2;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] > 0)
                modifiers.SetCrit();
            else
                modifiers.DisableCrit();
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Inflate(7, 7);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            if (Projectile.ai[0] > 0 && Main.rand.NextBool(4)) 
            {
                Dust.NewDustPerfect(Projectile.position + (new Vector2(Main.rand.Next(Projectile.width), Main.rand.Next(Projectile.height)) * Projectile.scale), DustType<LightDustFast>(), Projectile.velocity * 0.5f, 0, Color.White, Main.rand.NextFloat(0.45f, 0.65f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] > 0) //crit
            {
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawOrigin = new(glowTex.Width() * 0.5f, Projectile.height * 0.5f);
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;

                    Main.spriteBatch.Draw(glowTex.Value, drawPos, null, Color.White * (0.8f - (0.035f * k)), Projectile.rotation, drawOrigin, Projectile.scale * (1.25f - (0.035f * k)), SpriteEffects.None, 0f);
                    //Main.EntitySpriteDraw(glowTex.Value, drawPos, null, Color.White * (0.8f - (0.05f * k)), Projectile.oldRot[k], drawOrigin, Projectile.scale * (1f - (0.045f * k)), SpriteEffects.None);
                }
                Main.EntitySpriteDraw(glowTex.Value, Projectile.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite, Projectile.rotation, glowTex.Size() / 2, Projectile.scale, SpriteEffects.None);
            }
            else 
            {
                Texture2D texture = TextureAssets.Projectile[Type].Value;

                for (int k = 0; k < Projectile.oldPos.Length/4; k++)
                {
                    Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;

                    Main.spriteBatch.Draw(texture, drawPos, null, ColorHelper.AdditiveWhite * (0.6f - (0.075f * k)), Projectile.rotation, drawOrigin, Projectile.scale * (1f - (0.1f * k)), SpriteEffects.None, 0f);
                    //Main.EntitySpriteDraw(glowTex.Value, drawPos, null, Color.White * (0.8f - (0.05f * k)), Projectile.oldRot[k], drawOrigin, Projectile.scale * (1f - (0.045f * k)), SpriteEffects.None);
                }
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None);
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite * 0.3f, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

                /*
                for (int k = 0; k < Projectile.oldPos.Length/3; k++)
                {
                    Vector2 drawOrigin = new(glowTex.Width() * 0.5f, Projectile.height * 0.5f);
                    Vector2 drawPos = Projectile.oldPos[k * 4] - Main.screenPosition + drawOrigin;

                    Main.spriteBatch.Draw(glowTex.Value, drawPos, null, lightColor * (0.7f - (0.1f * k)), Projectile.rotation, drawOrigin, Projectile.scale * (1f - (0.045f * (k * 2))), SpriteEffects.None, 0f);
                    //Main.EntitySpriteDraw(glowTex.Value, drawPos, null, lightColor * (0.7f - (0.1f * k)), Projectile.oldRot[k], drawOrigin, Projectile.scale * (1f - (0.045f * (k * 2))), SpriteEffects.None);
                }

                Texture2D texture = TextureAssets.Projectile[Type].Value;
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None);
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite * 0.3f, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
                //Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite * 0.3f, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None);
                */
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] > 0)
            {
                if (Main.myPlayer == Projectile.owner)
                    BlizzardNPC.AddFrost(target);

                for (int i = 0; i < Main.rand.Next(6, 9); i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustType<AuroraTwinkle>(), 0, 0, 5, ColorHelper.AuroraAccentColor(Main.rand.NextFloat(0f, 8f) + (i * 5.4f)), Main.rand.NextFloat(0.6f, 0.9f));
                    dust.rotation = Main.rand.NextFloat(-14.5f, 14.5f);
                    dust.noLight = true;
                    dust.customData = target.whoAmI;
                }
            }
            else 
            {
                for (int i = 0; i < Main.rand.Next(3, 7); i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<BlizzardBrassDust>());
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < Main.rand.Next(3, 7); i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<BlizzardBrassDust>());
            }
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}
