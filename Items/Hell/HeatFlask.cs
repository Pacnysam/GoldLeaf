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
using ReLogic.Content;
using GoldLeaf.Items.Grove.Boss;
using Terraria.Graphics.Shaders;
using GoldLeaf.Items.Grove.Wood;
using GoldLeaf.Items.Grove;
using GoldLeaf.Tiles.Decor;
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Core.CrossMod;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;


namespace GoldLeaf.Items.Hell
{
	public class HeatFlask : ModItem
	{
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
            ItemSets.Glowmask[Type] = (glowTex, ColorHelper.AdditiveWhite(100), true);
            Item.AddElements([Element.Fire, Element.Explosive]);

            ItemID.Sets.ToolTipDamageMultiplier[Type] = 3f;
        }

        public override void SetDefaults()
		{
			Item.damage = 16;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 22;
			Item.height = 38;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
            Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.value = Item.buyPrice(0, 0, 2, 75);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item106;
			Item.autoReuse = true;

            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;

            Item.shoot = ProjectileType<HeatFlaskP>();
			Item.shootSpeed = 18.5f;
		}
    }
    
	public class HeatFlaskP : ModProjectile 
	{
        public override string Texture => "GoldLeaf/Items/Hell/HeatFlask";

        public static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            Projectile.AddElements([Element.Fire, Element.Explosive]);
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            Projectile.width = 16;
            Projectile.height = 28;
            Projectile.friendly = true;
			Projectile.tileCollide = true;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ai[2] = 8;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[2] = 8;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y > 0)
                Projectile.velocity.Y *= 1.05f;
            else
                Projectile.velocity.Y += 0.1f;

            Projectile.spriteDirection = 2;

            Projectile.ai[1]++;
            if (Projectile.ai[1] > Projectile.ai[2] && Projectile.Counter() <= Helper.TimeToTicks(5))
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.position, Vector2.Zero /*Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 1f*/, ProjectileID.MolotovFire + Main.rand.Next(2), Projectile.damage, Projectile.knockBack * 0.35f, Projectile.owner);
                proj.timeLeft = Helper.TimeToTicks(5);
                proj.penetrate = 1;
                proj.netUpdate = true;

                Projectile.ai[1] = 0;
                Projectile.ai[2] = Main.rand.Next(6, 9);
                Projectile.netUpdate = true;
            }
        }

        public override bool? CanCutTiles() => true;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new (texture.Width * 0.5f, Projectile.height * 0.5f);
            
            //flask
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.FlipVertically, 0f);
            Main.EntitySpriteDraw(glowTex.Value, Projectile.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite(100), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.FlipVertically, 0f);
            
            //afterimage
            for (int k = 1; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                Main.EntitySpriteDraw(glowTex.Value, drawPos, null, Color.Orange.Alpha() * (0.8f - (k / (Projectile.oldPos.Length + 4f))), Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.FlipVertically, 0f);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            
            if (!Main.dedServ) 
            {
                SoundEngine.PlaySound(SoundID.Shatter with { Pitch = -0.4f, PitchVariance = 0.4f, Volume = 0.65f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item74);

                CameraSystem.QuickScreenShake(Projectile.Center, null, 20, 5.5f, 28, 1000);
                CameraSystem.QuickScreenShake(Projectile.Center, (0f).ToRotationVector2(), 10, 10f, 22, 1000);
            }
            
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile burst = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ProjectileType<BasicExplosion>(), (int)(Projectile.damage * 3f), Projectile.knockBack, Projectile.owner, 0, 5f);
                burst.scale = 1.5f;
            }
            for (int j = 0; j < 25; j++)
            {
                var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.FireworksRGB, 0, 0, 10 + Main.rand.Next(60), Color.Orange, Main.rand.NextFloat(0.5f, 0.8f));
                dust.fadeIn = Main.rand.NextFloat(0.65f, 0.95f);
                dust.velocity = Main.rand.NextVector2Circular(13.5f, 9.5f);
                dust.velocity.Y -= 4f;
                dust.noGravity = Main.rand.NextBool();
                dust.rotation = Main.rand.NextFloat(6.28f);
            }
        }
    }
    
    public class BasicExplosion : ModProjectile
    {
        private static Asset<Texture2D> glowTex;
        private static Asset<Texture2D> alphaBloom;
        private static Asset<Texture2D> transparentBloom;

        public override string Texture => "GoldLeaf/Textures/Boom";
        
        public override void Load()
        {
            glowTex = Request<Texture2D>("GoldLeaf/Textures/BoomGlow");
            alphaBloom = Request<Texture2D>("GoldLeaf/Textures/GlowAlpha");
            transparentBloom = Request<Texture2D>("GoldLeaf/Textures/Glow0");
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
            Projectile.AddElements([Element.Fire, Element.Explosive]);
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.penetrate = -1;

            Projectile.width = Projectile.height = 98;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 28;
            Projectile.knockBack = 6.5f;
            Projectile.ai[1] = 1.5f;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.Kill();

                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Math.Sign(target.Center.X - Projectile.Center.X);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] > 0)
                target.AddBuff(BuffID.OnFire, Helper.TimeToTicks(Projectile.ai[0]));
            else if (Projectile.ai[1] > 0)
                target.AddBuff(BuffID.OnFire3, Helper.TimeToTicks(Projectile.ai[1]));
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox) => hitbox.Inflate((int)(Projectile.Counter() * 1.5f), (int)(Projectile.Counter() * 1.5f));

        public override bool? CanHitNPC(NPC target) => (Projectile.Counter() > 20) ? false : base.CanHitNPC(target);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);

            //dark bloom
            Main.EntitySpriteDraw(transparentBloom.Value, Projectile.Center - Main.screenPosition, null, Color.Black * (Projectile.timeLeft / 28f), 0, transparentBloom.Size() / 2, (Projectile.timeLeft / 20f * (Projectile.Counter() / 36f)) * 10f * Projectile.scale, SpriteEffects.None);
            for (int k = 1; k <= 3; k++)
            {
                Rectangle frame2 = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame - k);
                Color colorgrad = new Color(255, 184, 40).Lerp(new(187, 37, 16), k / 3f) with { A = 0 };

                //afterimage
                Main.EntitySpriteDraw(glowTex.Value, Projectile.Center - Main.screenPosition, frame2, colorgrad * (0.5f - (k * 0.115f)) * Projectile.Opacity * 0.325f * (Projectile.timeLeft / 16f), Projectile.rotation, frame.Size() / 2, (1 + (k * (Projectile.timeLeft / 36f * (Projectile.Counter() / 20f)))) * Projectile.scale, SpriteEffects.None, 0f);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame2, colorgrad * (0.5f - (k * 0.115f)) * Projectile.Opacity * 0.45f * (Projectile.timeLeft / 16f), Projectile.rotation, frame.Size() / 2, (1 + (k * ((Projectile.timeLeft / 36f) * (Projectile.Counter() / 20f)))) * Projectile.scale, SpriteEffects.None, 0f);
            }
            //explosion
            Main.EntitySpriteDraw(glowTex.Value, Projectile.Center - Main.screenPosition, frame, new Color(255, 105, 25) { A = 0 } * Projectile.Opacity * 0.45f, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.Orange * Projectile.Opacity, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            
            //bloom
            Main.EntitySpriteDraw(alphaBloom.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 173, 65) { A = 0 } * (Projectile.timeLeft / 16f) * 0.325f, 0, alphaBloom.Size() / 2, Projectile.timeLeft / 40f * 6f * Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}