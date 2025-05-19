using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using GoldLeaf.Effects.Dusts;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.GameContent.Drawing;
using Terraria.GameContent;


namespace GoldLeaf.Items.Nightshade
{
	public class VampireBat : ModItem
	{
		public override void SetDefaults() 
		{
			Item.damage = 22;
            //Item.GetGlobalItem<GoldLeafItem>().throwingDamageType = DamageClass.Magic;
            Item.DamageType = DamageClass.Magic;

            Item.knockBack = 2;

			Item.mana = 7;
			
			Item.shoot = ProjectileType<VampireBatP>();
			Item.shootSpeed = 12f;
			
			Item.width = 22;
			Item.height = 42;

			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

			Item.value = Item.sellPrice(0, 0, 60, 0);
			Item.rare = ItemRarityID.Orange;
		}

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult = 0;
        }

        public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
            if (player.altFunctionUse == 2)
			{
				if (player.GetModPlayer<NightshadePlayer>().nightshade == player.GetModPlayer<NightshadePlayer>().nightshadeMin) 
				{
					return false;
				}
                for (int k = 0; k <= Main.maxProjectiles; k++)
                {
                    if (Main.projectile[k].active && Main.projectile[k].owner == player.whoAmI && Main.projectile[k].type == ProjectileType<VampireBolt>())
                        return false;
                }
				Item.UseSound = new SoundStyle("GoldLeaf/Sounds/SE/Monolith/Dash");
                Item.shoot = ProjectileType<VampireBolt>();
			}
			else
			{
                Item.UseSound = SoundID.Item1;
                Item.shoot = ProjectileType<VampireBatP>();
			}
			return base.CanUseItem(player);
		}
    }
    
	public class VampireBatP : ModProjectile
	{
		public override string Texture => "GoldLeaf/Items/Nightshade/VampireBat";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Vampire Bat");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
            Projectile.extraUpdates = 1;
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 600;
			Projectile.penetrate = 2;
			Projectile.scale = 1f;

            //Projectile.GetGlobalProjectile<GoldLeafProjectile>().throwingDamageType = DamageClass.Magic;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.GetGlobalProjectile<GoldLeafProjectile>().gravity = 0.012f;
            Projectile.GetGlobalProjectile<GoldLeafProjectile>().gravityDelay = 10;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			Player player = Main.player[Projectile.owner];
			player.GetModPlayer<NightshadePlayer>().nightshade++;
			player.GetModPlayer<NightshadePlayer>().nightshadeTimer = 300;

			if (player.GetModPlayer<NightshadePlayer>().nightshade == player.GetModPlayer<NightshadePlayer>().nightshadeMax && Main.myPlayer == Projectile.owner && Main.netMode != NetmodeID.Server) 
			{
				SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Monolith/BombClick") { Volume = 0.6f }, player.Center);
			}
            else if (player.GetModPlayer<NightshadePlayer>().nightshade <= player.GetModPlayer<NightshadePlayer>().nightshadeMax)
            {
                if (Main.myPlayer == Projectile.owner && Main.netMode != NetmodeID.Server)
                {
                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Deltarune/FountainTarget") { Pitch = -0.5f + ((player.GetModPlayer<NightshadePlayer>().nightshade - player.GetModPlayer<NightshadePlayer>().nightshadeMin) * 0.1f), Volume = 0.8f });
                }
                for (int i = 0; i < 5; i++)
                {
                    Dust.NewDust(target.position, target.width, target.height, DustType<SparkDustTiny>(), 0f, Main.rand.NextFloat(-2f, -5f), 0, new Color(210, 136, 107), Main.rand.NextFloat(0.6f, 0.9f));
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = Request<Texture2D>("GoldLeaf/Items/Nightshade/VampireBatTrail").Value;

			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Main.spriteBatch.Draw(texture, drawPos, null, Color.Lerp(new Color(210, 136, 107) { A = 0 }, new Color(23, 29, 32), (float)(k-2) / Projectile.oldPos.Length) * (0.7f - (0.06f * k)), Projectile.rotation, drawOrigin, Projectile.scale * (1f - (0.05f * k)), SpriteEffects.None, 0f);
			}

            Texture2D glowTex = Request<Texture2D>("GoldLeaf/Items/Nightshade/VampireBatGlowOutline").Value;
            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite * 0.5f, Projectile.rotation, new Vector2(0, -8) + glowTex.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            return true;
		}

		public override void PostDraw(Color lightColor)
		{
            /*Texture2D glowTex = Request<Texture2D>("GoldLeaf/Items/Nightshade/VampireBatGlowOutline").Value;

            Vector2 drawOrigin = new(glowTex.Width * 0.5f, Projectile.height * 0.5f);

            Vector2 drawPos = Projectile.position - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(0, -10) + glowTex.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);*/
            //spriteBatch.Draw(glowTex, projectile.position + projectile.Size / 2 - Main.screenPosition, new Rectangle(0, 0, 26, 48), Color.White, projectile.rotation, glowTex.Size(), projectile.scale, SpriteEffects.None, 0f);
        }

        public override void OnKill(int timeLeft)
        {
			int t = Main.rand.Next(4, 8);
            for (int i = 0; i < t; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<SparkDustTiny>(), Projectile.velocity.X * 0.7f, Projectile.velocity.Y * 0.7f, 0, new Color(210, 136, 107), Main.rand.NextFloat(0.4f, 0.7f));
            }
        }
    }

	public class VampireBolt : ModProjectile
	{
		public override string Texture => "GoldLeaf/Items/Nightshade/VampireBolt";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Vampire Bolt");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
            Projectile.extraUpdates = 2;
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;

            Projectile.GetGlobalProjectile<GoldLeafProjectile>().throwingDamageType = DamageClass.Magic;

            Projectile.GetGlobalProjectile<GoldLeafProjectile>().gravity = 0.012f;
            Projectile.GetGlobalProjectile<GoldLeafProjectile>().gravityDelay = 10;

            Projectile.GetGlobalProjectile<GoldLeafProjectile>().lifesteal = 2;
			Projectile.GetGlobalProjectile<GoldLeafProjectile>().lifestealMax = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];

            Projectile.GetGlobalProjectile<GoldLeafProjectile>().lifesteal = player.GetModPlayer<NightshadePlayer>().nightshade * 2;
            Projectile.GetGlobalProjectile<GoldLeafProjectile>().lifestealMax = 1;
            player.GetModPlayer<NightshadePlayer>().nightshade = player.GetModPlayer<NightshadePlayer>().nightshadeMin;
            //SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Monolith/GhostChirp") { Volume = 0.35f, Pitch = -0.8f }, player.Center);
        }

        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<SparkDustTiny>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, new Color(210, 136, 107), 0.6f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            for (float k = 0; k < 6.28f; k += 0.20f)
                Dust.NewDustPerfect(Projectile.Center, DustType<SparkDustTiny>(), Vector2.One.RotatedBy(k) * 1.6f, 0, new Color(210, 136, 107));
			for (int k = 0; k < Main.rand.Next(8, 11); k++)
				Dust.NewDustPerfect(Projectile.Center, DustType<SparkDust>(), Vector2.One.RotatedBy(k) * Main.rand.NextFloat(1.5f, 2.5f), 0, new Color(210, 136, 107), 1f);

            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
                new ParticleOrchestraSettings { PositionInWorld = Projectile.Center },
                Projectile.owner);

            if (Main.myPlayer == Projectile.owner && Main.netMode != NetmodeID.Server)
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Monolith/GhostWhistle"), player.Center);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			Main.player[Projectile.owner].AddBuff(BuffID.Cursed, 59);
            if (Main.myPlayer == Projectile.owner && Main.netMode != NetmodeID.Server)
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Monolith/GhostChirp") { Volume = 0.25f });

            int t = Main.rand.Next(4, 8);
            for (int i = 0; i < t; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<SparkDustTiny>(), Projectile.velocity.X * 0.7f, Projectile.velocity.Y * 0.7f, 0, new Color(210, 136, 107), Main.rand.NextFloat(0.4f, 0.7f));
            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = (Texture2D)Request<Texture2D>("GoldLeaf/Items/Nightshade/VampireBatTrail");

            Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin/* + new Vector2(0f, Projectile.gfxOffY)*/;
                Main.spriteBatch.Draw(texture, drawPos, null, Color.Lerp(new Color(210, 136, 107) { A = 0 }, new Color(23, 29, 32), (float)(k - 3) / Projectile.oldPos.Length) * (1f - (0.06f * k)), Projectile.rotation, drawOrigin, Projectile.scale * (1f - (0.045f * k)), SpriteEffects.None, 0f);
            }

            return true;
		}

        public override void PostDraw(Color lightColor)
        {
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);
            Vector2 drawPos = Projectile.position - Main.screenPosition + drawOrigin;

            Main.spriteBatch.Draw(texture, drawPos, null, ColorHelper.AdditiveWhite * 0.5f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
        }
    }
}