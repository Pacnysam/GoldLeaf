using System;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using GoldLeaf.Core;
using GoldLeaf.Items.Accessories;
using Terraria.DataStructures;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using GoldLeaf.Core.CrossMod;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;
using GoldLeaf.Effects.Dusts;
using Terraria.Audio;
using Terraria.GameContent.Drawing;

namespace GoldLeaf.Items.Sandstorm
{
    public class SunstoneStaff : ModItem
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            ItemSets.Glowmask[Type] = (glowTex, ColorHelper.AdditiveWhite(175) * 0.65f, true);
            Item.AddElements([Element.Arcane]);
        }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 1, 35, 0);
            Item.rare = ItemRarityID.Green;

            Item.damage = 16;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.ArmorPenetration = 3;

            Item.shootSpeed = 3.75f;
            Item.knockBack = 1.75f;

            Item.useTime = 4;
            Item.useAnimation = 10;
            Item.reuseDelay = 12;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = new SoundStyle("GoldLeaf/Sounds/SE/Kirby/SuperStar/MirrorWave") { PitchVariance = 0.35f, Volume = 0.85f };
            Item.staff[Item.type] = true;
            Item.noMelee = true;

            Item.shoot = ProjectileType<SunstoneStaffBeam>();

            Item.width = 36;
            Item.height = 36;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.AmberStaff);
            recipe.AddRecipeGroup("GoldLeaf:EvilBar", 9);
            //recipe.AddIngredient(ItemType<Sunstone>(), 6);
            recipe.AddIngredient(ItemID.JungleSpores, 4);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    public class SunstoneStaffBeam : ModProjectile
    {
        public override string Texture => EmptyTexString;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.timeLeft = 185;
            Projectile.extraUpdates = 200;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            if (Projectile.Center.Distance(Main.MouseWorld) <= 10 && Main.myPlayer == Projectile.owner)
            {
                Projectile.position = Main.MouseWorld;
                Projectile.netUpdate = true;
                
                Projectile.Kill();
            }
        }

        public override bool? CanDamage() => false;
        public override bool? CanHitNPC(NPC target) => false;

        public override void OnKill(int timeLeft)
        {
            int repeats = Main.rand.Next(1, 2);

            for (int i = 0; i < repeats; i++)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.position + Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Projectile.velocity.Length() * Main.rand.NextFloat(0f, 10.5f), Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Projectile.velocity.Length() * Main.rand.NextFloat(0.3f, 1f), ProjectileType<SunstoneStaffBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Main.rand.NextFloat(0.055f, 0.45f));
                proj.scale = Main.rand.NextFloat(0.65f, 1.2f);

                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact with { Volume = 0.625f, PitchVariance = 0.4f }, proj.Center);

                /*Dust dust = TwinkleDust.SpawnPerfect(new LightDust.LightDustData(Main.rand.NextFloat(0.875f, 0.9f), MathHelper.ToRadians(Main.rand.NextFloat(-16.5f, 16.5f))), proj.position, Vector2.Zero, 0, new Color(52, 229, 66) { A = 0 } * 0.8f, proj.scale * Main.rand.NextFloat(1.15f, 1.75f));
                dust.fadeIn = Main.rand.NextFloat(-0.25f, 0.5f); dust.rotation = MathHelper.ToRadians(Main.rand.NextFloat(90));*/

                /*ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.TerraBlade,
                    new ParticleOrchestraSettings { PositionInWorld = proj.Center });*/

                /*for (float k = 0; k < MathHelper.TwoPi; k += MathHelper.TwoPi / 30f)
                {
                    Dust dust = Dust.NewDustPerfect(proj.Center, DustID.GreenFairy, Vector2.One.RotatedBy(k) * proj.scale * 0.35f, 145, ColorHelper.AdditiveWhite() * 0.65f, 0.55f * proj.scale);
                    //dust.noLight = true;
                }*/
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
    
    public class SunstoneStaffBolt : ModProjectile
    {
        private static Asset<Texture2D> glowTex;
        private static Asset<Texture2D> shineTex;
        private static Asset<Texture2D> bloomTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
            shineTex = Request<Texture2D>("GoldLeaf/Textures/Shine");
            bloomTex = Request<Texture2D>("GoldLeaf/Textures/Masks/Mask1");
        }
        public override void SetStaticDefaults()
        {
            Projectile.AddElements([Element.Arcane]);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 28;
            Projectile.height = 36;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;

            Projectile.DamageType = DamageClass.Magic;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (!Main.dedServ && Main.myPlayer == Projectile.owner)
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.TerraBlade,
                    new ParticleOrchestraSettings { PositionInWorld = Projectile.Center });
            }
        }

        public override void AI()
        {
            Projectile.TryGetOwner(out Player player);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.Counter() >= Projectile.ai[0] * 100)
            {
                Projectile.velocity += Vector2.Normalize(player.Center - Projectile.Center) * ((Projectile.ai[0] * 0.5f) + (Projectile.Counter() * 0.03f));

                if (Projectile.velocity.Length() >= 9)
                {
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 10f;
                }

                if (Projectile.Center.Distance(player.Center) <= 300)
                    Projectile.alpha += 9;
            }
            else
            {
                Projectile.velocity *= 0.98f;
            }
            
            if (Projectile.localAI[0] <= 0)
            {
                Projectile.localAI[0] = (100 - Projectile.Counter()) * 0.035f;

                /*Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.YellowStarDust, Vector2.Zero, 0, new Color(222, 188, 34) { A = 100 } * 0.85f, 0.7f * Projectile.scale);
                dust.noLight = true;
                dust.fadeIn = 0.7f;*/
            }
            Projectile.localAI[0] -= 1;
            //new Color(52, 229, 66)
            if (!Main.dedServ)
                Lighting.AddLight((int)(Projectile.position.X / 16), (int)(Projectile.position.Y / 16), 72 / 255f * 0.4f, 229 / 255f * 0.4f, 26 / 255f * 0.4f);

            if (Projectile.Hitbox.Intersects(player.Hitbox))
                Projectile.Kill();
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Inflate(8, 8);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.6f);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.Counter() < 18) return false;

            return base.CanHitNPC(target);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;

            Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center - Main.screenPosition, null, new Color(222, 141, 7) { A = 0 } * Projectile.Opacity * 0.2f, 0f, bloomTex.Size() / 2f, new Vector2(1f, Math.Clamp(Projectile.velocity.Length() * 0.25f, 0.5f, 1f)) * (1.35f + (float)(Math.Sin(GoldLeafWorld.rottime * 5.25f) * 0.1f)) * Projectile.scale * 0.425f, SpriteEffects.None);
            Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center - Main.screenPosition, null, new Color(52, 229, 66) { A = 0 } * Projectile.Opacity * 0.45f, 0f, bloomTex.Size() / 2f, new Vector2(1f, Math.Clamp(Projectile.velocity.Length() * 0.25f, 0.5f, 1f)) * (1.35f + (float)(Math.Sin(GoldLeafWorld.rottime * 5.25f) * 0.1f)) * Projectile.scale * 0.25f, SpriteEffects.None);

            Main.EntitySpriteDraw(shineTex.Value, Projectile.Center - Main.screenPosition, null, Color.Black * Projectile.Opacity * 0.25f, Projectile.rotation, (shineTex.Size() / 2f) + new Vector2(0, -8), new Vector2(1f, Math.Clamp(Projectile.velocity.Length() * 0.425f, 0.65f, 3.25f)) * Projectile.scale * 1.125f, SpriteEffects.None);
            Main.EntitySpriteDraw(shineTex.Value, Projectile.Center - Main.screenPosition, null, new Color(222, 141, 7) { A = 100 } * Projectile.Opacity * 0.65f, Projectile.rotation, shineTex.Size() / 2f, new Vector2(1f, Math.Clamp(Projectile.velocity.Length() * 0.425f, 0.65f, 3.25f)) * Projectile.scale * 0.725f, SpriteEffects.None);

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite(240) * Projectile.Opacity * 0.5f, Projectile.rotation, tex.Size() / 2f, new Vector2(1f, Math.Clamp(Projectile.velocity.Length() * 0.35f, 0.5f, 4.75f)) * Projectile.scale, SpriteEffects.None);
            
            Main.EntitySpriteDraw(glowTex.Value, Projectile.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite(200) * (Projectile.Opacity + 0.5f), Projectile.rotation, glowTex.Size() / 2f, new Vector2(1f, Math.Clamp(Projectile.velocity.Length() * 0.25f, 0.5f, 1f)) * (1 + (float)(Math.Sin(GoldLeafWorld.rottime * 5.25f) * 0.1f)) * Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(glowTex.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 142) { A = 0 } * 0.25f * (Projectile.Opacity + 0.75f), Projectile.rotation, glowTex.Size() / 2f, new Vector2(1f, Math.Clamp(Projectile.velocity.Length() * 0.25f, 0.5f, 1f)) * (1.35f + (float)(Math.Sin(GoldLeafWorld.rottime * 5.25f) * 0.1f)) * Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
