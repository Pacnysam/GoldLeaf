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
        }

        public override void SetDefaults()
		{
			Item.damage = 26;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 22;
			Item.height = 40;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
            Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.value = Item.sellPrice(0, 0, 2, 75);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
            Item.GetGlobalItem<GoldLeafItem>().critDamageMod = 0.25f;

            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;

            Item.shoot = ProjectileType<HeatFlaskP>();
			Item.shootSpeed = 14.5f;
		}

        /*public override void AddRecipes()
        {
            CreateRecipe(5)
            .AddIngredient(ItemID.MolotovCocktail, 5)
            .AddIngredient(ItemID.HellstoneBar)
            .AddTile(TileID.Bottles)
            //.AddCondition(Condition.NearLava)
            .Register();
        }*/
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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = 2;
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
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.position, Vector2.Zero /*Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 1f*/, ProjectileID.MolotovFire + Main.rand.Next(2), (int)(Projectile.damage * 0.65f), Projectile.knockBack * 0.35f, Projectile.owner);
                proj.timeLeft = Helper.TimeToTicks(3);
                proj.penetrate = 1;
                proj.netUpdate = true;

                Projectile.ai[1] = 0;
                Projectile.ai[2] = Main.rand.Next(6, 9);
                Projectile.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Vector2 drawOrigin = new (texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 1; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;

                Main.EntitySpriteDraw(texture, drawPos, null, Color.Orange with { A = 0 }, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.FlipVertically, 0f);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.FlipVertically, 0f);
            Main.EntitySpriteDraw(glowTex.Value, Projectile.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite(100), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.FlipVertically, 0f);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            
            SoundEngine.PlaySound(SoundID.Shatter with { Pitch = -0.4f, PitchVariance = 0.4f, Volume = 0.65f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item74);

            //CameraSystem.AddScreenshake(Main.LocalPlayer, 10, Projectile.Center);
            CameraSystem.QuickScreenShake(Projectile.Center, null, 28, 5.5f, 15, 1000);

            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < 3; i++)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextFloat(6.28f).ToRotationVector2() * Main.rand.NextFloat(3.6f, 4.2f), ProjectileType<Ember>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    proj.scale = Main.rand.NextFloat(1f, 1.35f);
                    proj.timeLeft = Main.rand.Next(69, 96);
                    proj.netUpdate = true;
                }
                Projectile burst = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ProjectileID.InfernoFriendlyBlast, (int)(Projectile.damage * 1.25f), Projectile.knockBack, Projectile.owner);
                burst.timeLeft = 15;
                burst.scale = 1.5f;
                burst.usesLocalNPCImmunity = true;
                burst.localNPCHitCooldown = -1;
                burst.netUpdate = true;
            }
            for (int j = 0; j < 15; j++)
            {
                var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(16, 16), 0, 0, DustType<HotSmoke>());
                dust.velocity = Main.rand.NextVector2Circular(3.2f, 3.2f);
                dust.velocity.Y -= Main.rand.NextFloat(1.5f, 3f);
                dust.scale = Main.rand.NextFloat(0.4f, 0.7f);
                dust.alpha = 10 + Main.rand.Next(60);
                dust.rotation = Main.rand.NextFloat(6.28f);
            }
        }
    }
}