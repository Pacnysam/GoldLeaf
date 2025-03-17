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
using Terraria.WorldBuilding;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.GameContent.Drawing;

namespace GoldLeaf.Items.VanillaBossDrops
{
    public class Goonai : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults() 
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Ranged;

            Item.GetGlobalItem<GoldLeafItem>().critDamageMod = 1f;

            Item.shoot = ProjectileType<GoonaiP>();
            Item.shootSpeed = 15f;

            Item.width = 30;
            Item.height = 30;

            Item.autoReuse = true;
            Item.useTime = 30;
            Item.useAnimation = 30;

            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;

            Item.UseSound = new SoundStyle("GoldLeaf/Sounds/SE/Overwatch/KirikoKunai") { PitchVariance = 0.3f, Pitch = -0.2f, Volume = 0.55f };
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 0, 0, 20);

            Item.rare = ItemRarityID.Blue;
        }

        public override bool ConsumeItem(Player player)
        {
            if (player.GetModPlayer<GoldLeafPlayer>().royalGel) 
            {
                return Main.rand.NextBool(6, 10);
            }

            return true;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            crit = 0;
        }

        public override void AddRecipes()
        {
            Recipe recipe2 = CreateRecipe(30);
            recipe2.AddIngredient(ItemID.ThrowingKnife, 30);
            recipe2.AddIngredient(ItemID.Gel, 5);
            recipe2.AddTile(TileID.Solidifier);
            recipe2.Register();
        }
    }
    
    public class GoonaiP : ModProjectile
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 3;
            
            Projectile.ai[0] = 0;
            Projectile.ai[1] = 0;

            Projectile.DamageType = DamageClass.Ranged;

            Projectile.GetGlobalProjectile<GoldLeafProjectile>().critDamageMod = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 60)
            {
                //Projectile.extraUpdates = 3;
                Projectile.ai[1] += 0.01f;

                Projectile.velocity.X *= 0.97f;
                Projectile.velocity.Y += Projectile.ai[1];
            }
            else 
            {
                //Projectile.extraUpdates = 2;
                Projectile.velocity *= 0.97f;
            }
            if (Projectile.ai[0] >= 60 && Projectile.velocity.Y > 5 && Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustType<LightDust>(), 0, 0, 0, new Color(255, 255, 118), Main.rand.NextFloat(0.4f, 0.7f));
                dust.velocity = Projectile.velocity * Main.rand.NextFloat(0.1f, 0.4f);
                //if (dust.velocity.Y > 3f) dust.velocity.Y = 3f;
                dust.velocity.X = 0;
                dust.noLight = true;
                dust.alpha = Main.rand.Next(-10, -5);
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (Projectile.ai[0] >= 60 && Projectile.velocity.Y > 5)
            {
                hitbox.Inflate(14, 8);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.Center.Y <= target.Center.Y && Projectile.velocity.Y > 5 && Projectile.ai[0] >= 60) 
            {
                modifiers.SetCrit();
            }
            else modifiers.DisableCrit();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.SilverBulletSparkle,
                                        new ParticleOrchestraSettings { PositionInWorld = Projectile.Center },
                                        Projectile.owner);

                if (Main.netMode != NetmodeID.Server)
                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Overwatch/Headshot") { Volume = 0.55f }, Main.player[Projectile.owner].Center);
                target.AddBuff(BuffID.Slimed, 60);
            }
            if (!target.active)
            {
                if (Main.netMode != NetmodeID.Server)
                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Overwatch/Elimination") { Volume = 0.7f }, Main.player[Projectile.owner].Center);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] >= 60 && Projectile.velocity.Y > 5)
            {
                Vector2 drawOrigin = new(glowTex.Width() * 0.5f, Projectile.height * 0.5f);
                Color color = new(240, 194, 50) { A = 0 };
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin/* + new Vector2(0f, Projectile.gfxOffY)*/;
                    
                    Main.spriteBatch.Draw(glowTex.Value, drawPos, null, color * (0.6f - (0.06f * k )), Projectile.rotation, drawOrigin, Projectile.scale * (1.15f - (0.075f * k)), effects, 0f);
                }
            }
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] < 60)
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }

                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
                Projectile.velocity.X *= 0.8f;
                Projectile.velocity.Y *= 1.2f;

                return false;
            }

            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 18; ++k)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<SlimeDustBlue>(), Main.rand.NextFloat(-1.5f, 1.5f) + (-Projectile.velocity.X / 5), Main.rand.NextFloat(-0.9f, 0.9f) + (-Projectile.velocity.Y / 5), 0, Color.White, Main.rand.NextFloat(0.8f, 1.2f));
                Main.dust[dust].alpha = 175;
                if (Main.rand.NextBool(2)) Main.dust[dust].alpha += 25;
                if (Main.rand.NextBool(2)) Main.dust[dust].alpha += 25;
            }
        }
    }
}
