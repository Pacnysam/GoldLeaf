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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            
            Projectile.ai[0] = 0;
            Projectile.ai[1] = 0;

            Projectile.DamageType = DamageClass.Ranged;

            Projectile.GetGlobalProjectile<GoldLeafProjectile>().critDamageMod = 3;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 60)
            {
                Projectile.extraUpdates = 2;
                Projectile.ai[1] += 0.01f;

                Projectile.velocity.X *= 0.97f;
                Projectile.velocity.Y += Projectile.ai[1];
            }
            else 
            {
                Projectile.extraUpdates = 3;
                Projectile.velocity *= 0.97f;
            }

            if (Projectile.ai[0] >= 60 && Projectile.velocity.Y > 5 && Main.rand.NextBool(8))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustType<LightDust>(), 0, 0, 0, new Color(255, 255, 118), Main.rand.NextFloat(0.3f, 0.6f));
                dust.velocity = Projectile.velocity;
                if (dust.velocity.Y > 2.4f) dust.velocity.Y = 2.4f;
                dust.velocity.X = 0;
                dust.noLight = true;
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
                //Dust.NewDustPerfect(Projectile.Center, DustType<LightDust>(), Vector2.Zero, 0, Color.White, 2f);
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Overwatch/Headshot") { Volume = 0.55f }, Main.player[Projectile.owner].Center);
                target.AddBuff(BuffID.Slimed, 60);
            }
            if (!target.active)
            {
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Overwatch/Elimination") { Volume = 0.7f }, Main.player[Projectile.owner].Center);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] >= 60 && Projectile.velocity.Y > 5)
            {
                Vector2 drawOrigin = new(glowTex.Width() * 0.5f, Projectile.height * 0.5f);

                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin/* + new Vector2(0f, Projectile.gfxOffY)*/;
                    
                    Main.spriteBatch.Draw(glowTex.Value, drawPos, null, new Color(240, 181, 0) * (1.0f - (0.15f * k)), Projectile.rotation, drawOrigin, Projectile.scale * (0.8f - (0.05f * k)), effects, 0f);
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
            for (int k = 0; k < 12; ++k)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Slime, Main.rand.NextFloat(-3, 3) + Projectile.velocity.X / 3, Main.rand.NextFloat(-3, 3) + Projectile.velocity.Y / 3, 0, new Color(), 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].color = new Color(112, 172, 244);
            }
        }
    }
}
