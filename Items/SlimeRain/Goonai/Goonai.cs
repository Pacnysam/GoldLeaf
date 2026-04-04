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
using static GoldLeaf.Core.CrossMod.RedemptionHelper;

namespace GoldLeaf.Items.SlimeRain.Goonai
{
    public class Goonai : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
            Item.AddElements([Element.Water]);
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

            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            crit *= 0;
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
        private static Asset<Texture2D> bigGlowTex;
        public override void Load()
        {
            //glowTex = Request<Texture2D>(Texture + "Glow");
            glowTex = Request<Texture2D>("GoldLeaf/Textures/ShineSmall");
            bigGlowTex = Request<Texture2D>("GoldLeaf/Textures/Shine");
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            Projectile.AddElements([Element.Water]);
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 3;

            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            if (Projectile.Counter() >= 60)
            {
                //Projectile.extraUpdates = 3;
                Projectile.ai[0] += 0.01f;

                Projectile.velocity.X *= 0.97f;
                Projectile.velocity.Y += Projectile.ai[0];
            }
            else 
            {
                //Projectile.extraUpdates = 2;
                Projectile.velocity *= 0.97f;
            }
            if (Projectile.Counter() >= 60 && Projectile.velocity.Y > 5 && Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustType<LightDust>(), 0, 0, 0, new Color(255, 225, 131).Alpha(120), Main.rand.NextFloat(0.6f, 0.75f));
                dust.velocity = Projectile.velocity * Main.rand.NextFloat(0.1f, 0.4f);
                //if (dust.velocity.Y > 3f) dust.velocity.Y = 3f;
                dust.velocity.X = 0;
                dust.noLight = true;
                dust.alpha = Main.rand.Next(-10, -5);
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (Projectile.Counter() >= 60 && Projectile.velocity.Y > 5)
            {
                hitbox.Inflate(14, 8);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.Center.Y <= target.Center.Y && Projectile.velocity.Y > 5 && Projectile.Counter() >= 60) 
            {
                modifiers.SetCrit();
            }
            else modifiers.DisableCrit();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (hit.Crit)
            {
                if (player.GetModPlayer<GoldLeafPlayer>().royalGel)
                {
                    player.GetItem(player.whoAmI, new Item(ItemType<Goonai>()), new GetItemSettings(false, true));
                }

                /*ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.SilverBulletSparkle,
                                        new ParticleOrchestraSettings { PositionInWorld = Projectile.Center },
                                        Projectile.owner);*/

                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustType<TwinkleDust>(), Vector2.Zero, Main.rand.Next(18, 25), new Color(255, 225, 131).Alpha(120), Main.rand.NextFloat(2.5f, 3.5f));
                dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                dust.fadeIn = 0.5f;
                dust.customData = new LightDust.LightDustData(0.9f, Main.rand.NextFloat(0.1f, 0.25f).RandNeg());

                if (Main.myPlayer == Projectile.owner)
                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Overwatch/Headshot") { Volume = 0.55f }, Main.player[Projectile.owner].Center);
                target.AddBuff(BuffID.Slimed, 60);
            }
            if (!target.active)
            {
                if (Main.myPlayer == Projectile.owner)
                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Overwatch/Elimination") { Volume = 0.7f }, Main.player[Projectile.owner].Center);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.Counter() >= 60 && Projectile.velocity.Y > 5)
            {
                Projectile.localAI[0] += 0.05f;

                Texture2D weaponTexture = TextureAssets.Projectile[Projectile.type].Value;
                Color color = new Color(255, 225, 131).Alpha(120);

                //afterimage
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(weaponTexture.Width/2f, -weaponTexture.Height/2f)/* + new Vector2(0f, Projectile.gfxOffY)*/;
                    
                    Main.EntitySpriteDraw(glowTex.Value, drawPos, null, color * Math.Min(Projectile.localAI[0] * 3f, 1f) * (0.6f - 0.04f * k ), Projectile.rotation, glowTex.Size()/2f, Projectile.scale * (1.25f - 0.075f * k), effects);
                }
                Main.EntitySpriteDraw(bigGlowTex.Value, Projectile.Center + new Vector2(0, -6) - Main.screenPosition, null, color * Math.Min(Projectile.localAI[0] * 3f, 1f), Projectile.rotation, bigGlowTex.Size() / 2f, Projectile.scale * 1.25f, SpriteEffects.None);
            }
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.Counter() < 60)
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
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<SlimeDustBlue>(), Main.rand.NextFloat(-1.5f, 1.5f) + -Projectile.velocity.X / 5, Main.rand.NextFloat(-0.9f, 0.9f) + -Projectile.velocity.Y / 5, 0, Color.White, Main.rand.NextFloat(0.8f, 1.2f));
                Main.dust[dust].alpha = 175;
                if (Main.rand.NextBool(2)) Main.dust[dust].alpha += 25;
                if (Main.rand.NextBool(2)) Main.dust[dust].alpha += 25;
            }
        }
    }
}
