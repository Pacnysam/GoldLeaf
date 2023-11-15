using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
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

namespace GoldLeaf.Items.Sets.Nightshade
{
	public class VampireBat : ModItem
	{
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Vampire Bat");
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs("Hits give you a stack of nightshade\nRight clicking shoots a bolt that heals you\nMovement speed and damage increase with every stack");
        
		public override void SetDefaults() 
		{
			Item.damage = 21;
			Item.DamageType = DamageClass.Generic;
            Item.knockBack = 2;
			
			Item.shoot = ProjectileType<VampireBatP>();
			Item.shootSpeed = 12f;

			Item.width = 22;
			Item.height = 40;

			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;

			Item.value = Item.sellPrice(0, 12, 0, 0);
			Item.rare = ItemRarityID.Pink;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				if (player.GetModPlayer<GoldLeafPlayer>().nightshade == 0) 
				{
					return false;
				}
				Item.shoot = ProjectileType<VampireBolt>();
			}
			else
			{
				Item.shoot = ProjectileType<VampireBatP>();
			}
			return base.CanUseItem(player);
		}
    }

	public class VampireBatP : ModProjectile
	{
		public override string Texture => "GoldLeaf/Items/Sets/Nightshade/VampireBat";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Vampire Bat");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.DamageType = DamageClass.Generic;
            Projectile.extraUpdates = 1;
			Projectile.width = 10;
			Projectile.height = 32;
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 600;
			Projectile.penetrate = 2;
			Projectile.scale = 1f;

			Projectile.GetGlobalProjectile<GoldLeafProjectile>().gravity = 0.01f;
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            player.GetModPlayer<GoldLeafPlayer>().nightshade++;
            player.GetModPlayer<GoldLeafPlayer>().nightshadeTimer = 180;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = Request<Texture2D>("GoldLeaf/Items/Sets/Nightshade/VampireBatTrail").Value;

			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Main.spriteBatch.Draw(texture, drawPos, null, Color.White * (1.0f - (0.08f * k)), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}
			return true;
		}

		public override void PostDraw(Color lightColor)
		{
			Texture2D texture = Request<Texture2D>("GoldLeaf/Items/Sets/Nightshade/VampireBatGlow").Value;
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			Vector2 drawPos = Projectile.position - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

			Main.spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			//spriteBatch.Draw(texture, projectile.position + projectile.Size / 2 - Main.screenPosition, new Rectangle(0, 0, 26, 44), Color.White, projectile.rotation, texture.Size(), projectile.scale, SpriteEffects.None, 0f);
		}
	}

	public class VampireBolt : ModProjectile
	{
		public override string Texture => "GoldLeaf/Items/Sets/Nightshade/VampireBolt";

		bool a = false;
		int counter = 0;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Vampire Bolt");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
            Projectile.DamageType = DamageClass.Generic;
            Projectile.extraUpdates = 2;
			Projectile.width = 16;
			Projectile.height = 36;
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 500;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;

			Projectile.GetGlobalProjectile<GoldLeafProjectile>().gravity = 0.1f;

			Projectile.GetGlobalProjectile<GoldLeafProjectile>().lifesteal = 2;
			Projectile.GetGlobalProjectile<GoldLeafProjectile>().lifestealMax = 1;
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (a == false)
            {
				if (counter == 7) 
				{
					Dust.NewDust(Projectile.Center, 8, 8, DustType<VampireDust2>(), Projectile.velocity.X + Main.rand.Next(-7, 7), Projectile.velocity.Y + Main.rand.Next(-7, 7));
                }

                Projectile.GetGlobalProjectile<GoldLeafProjectile>().lifesteal = player.GetModPlayer<GoldLeafPlayer>().nightshade * 2;
                Projectile.GetGlobalProjectile<GoldLeafProjectile>().lifestealMax = 1;
                player.GetModPlayer<GoldLeafPlayer>().nightshade = 0;
                a = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            for (float k = 0; k < 6.28f; k += 0.20f)
                Dust.NewDustPerfect(Projectile.position, DustType<VampireDust>(), Vector2.One.RotatedBy(k) * 2);
			for (int k = 0; k < Main.rand.Next(8, 11); k++)
				Dust.NewDustPerfect(Projectile.position, DustType<VampireDust2>(), new Vector2(Main.rand.Next(8, 12)), Main.rand.Next(8, 12));

            SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Monolith/GhostWhistle"), player.Center);
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = (Texture2D)Request<Texture2D>("GoldLeaf/Items/Sets/Nightshade/VampireBatTrail");

			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Main.spriteBatch.Draw(texture, drawPos, null, Color.White *(1.0f - (0.08f * k)), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}


			Texture2D tex = (Texture2D)Request<Texture2D>("GoldLeaf/Items/Sets/Nightshade/VampireBatGlow");

			SpriteEffects effect = Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			Color col = Lighting.GetColor((int)(Projectile.Center.Y) / 16, (int)(Projectile.Center.Y) / 16);
			var basePos = Projectile.Center - Main.screenPosition + new Vector2(0.0f, Projectile.gfxOffY);

			int height = tex.Height / Main.projFrames[Projectile.type];
			var frame = new Rectangle(0, height * Projectile.frame, tex.Width, height);
			Vector2 origin = frame.Size() / 2f;
			int reps = 1;
			while (reps < 5)
			{
				col = Projectile.GetAlpha(Color.Lerp(col, Color.White, 2.5f));
				float num7 = 5 - reps;
				Color drawCol = col * (num7 / (ProjectileID.Sets.TrailCacheLength[Projectile.type] * 1.5f));
				Vector2 oldPo = Projectile.oldPos[reps];
				float rotation = Projectile.rotation;
				SpriteEffects effects2 = effect;
				if (ProjectileID.Sets.TrailingMode[Projectile.type] == 2)
				{
					rotation = Projectile.oldRot[reps];
					effects2 = Projectile.oldSpriteDirection[reps] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
				}
				Vector2 drawPos = oldPo + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
				Main.spriteBatch.Draw(tex, drawPos, frame, drawCol, rotation + Projectile.rotation * (reps - 1) * -effect.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin, MathHelper.Lerp(Projectile.scale, 3f, reps / 15f), effects2, 0.0f);
				reps++;
			}
			Main.spriteBatch.Draw(texture, basePos, frame, new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 175), Projectile.rotation, origin, Projectile.scale, effect, 0.0f);

			return false;
		}
	}
}