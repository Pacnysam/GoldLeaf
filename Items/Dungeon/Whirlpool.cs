using Terraria.GameContent;
using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using GoldLeaf.Effects.Gores;

namespace GoldLeaf.Items.Dungeon
{
	public class Whirlpool : ModItem
	{
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Cascade Staff");
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs("Shoots whirlpools that slightly home toward the cursor\nExplodes into water droplets on death");

        public override void SetDefaults() 
		{
			Item.damage = 30;
			ItemID.Sets.ToolTipDamageMultiplier[Item.type] = 0.8f;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 27;
			Item.width = 44;
			Item.height = 44;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.shootSpeed = 12f;
			Item.useStyle = ItemUseStyleID.Shoot;
            Item.staff[Item.type] = true;
            Item.shoot = ProjectileType<WhirlpoolP>();
			Item.knockBack = 9;
			Item.value = 100000;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item167;
			Item.autoReuse = false;
			Item.channel = true;
			Item.noMelee = true;
        }
		

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Texture2D texture;
			texture = TextureAssets.Item[Item.type].Value;
			spriteBatch.Draw
			(
				Request<Texture2D>(Texture + "Glow").Value,
				new Vector2
				(
					Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
					Item.position.Y - Main.screenPosition.Y + Item.height - texture.Height * 0.5f
				),
				new Rectangle(0, 0, texture.Width, texture.Height),
				Color.White,
				rotation,
				texture.Size() * 0.5f,
				scale,
				SpriteEffects.None,
				0f
			);
        }
		
		/*public override void AddRecipes() 
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.WaterCandle, 3);
			recipe.AddIngredient(ItemID.GoldBar, 6);
			recipe.AddIngredient(ItemID.Bone, 8);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}*/
	}

	public class WhirlpoolP : ModProjectile 
	{
		public override string Texture => "Goldleaf/Items/Dungeon/WhirlpoolP";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Whirlpool");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		int bounces = 5;
		int counter;
		public override void SetDefaults()
		{
			Projectile.width = 2;
			Projectile.height = 2;

			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 200;
			Projectile.ignoreWater = true;

            Projectile.DamageType = DamageClass.Magic;

            counter = Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter;

			Projectile.GetGlobalProjectile<GoldLeafProjectile>().gravity = 0.35f;
            Projectile.GetGlobalProjectile<GoldLeafProjectile>().gravityDelay = 15;
        }

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Lighting.AddLight((int)(Projectile.position.X / 16), (int)(Projectile.position.Y / 16), 0.38f, 0.68f, 1.16f);
            Dust dust;
            dust = Main.dust[Dust.NewDust(new Vector2(Projectile.position.X + 2, Projectile.position.Y - 2), 30, 30, DustID.DungeonWater)];


            if (Projectile.height < 30)
			{
				Projectile.width+= 2;
				Projectile.height+= 2;
			}

			if (player.channel)
			{
				if (counter >= 10 && counter <= 40 && Main.myPlayer == Projectile.owner)
				{
					Projectile.velocity = Projectile.velocity.Length() * Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.MouseWorld) * Projectile.velocity.Length() * 0.5f, 0.15f).SafeNormalize(Vector2.Normalize(Projectile.velocity));
					Projectile.netUpdate = true;
				}
			}
			counter++;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Player player = Main.player[Projectile.owner];

			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.velocity.X = -oldVelocity.X;
			}

			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}

			SoundEngine.PlaySound(new SoundStyle("Goldleaf/Sounds/SE/SplashBounce"), player.Center);

			bounces--;
			Projectile.velocity *= 0.825f;

			if (bounces > 0)
			{
				return false; 
			} 
			else
			{
				Projectile.Kill(); return true; 
			}
		}

		public override void OnKill(int timeLeft)
		{
            Player player = Main.player[Projectile.owner];

            SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Monolith/CrashShock"), player.Center);

			Helper.AddScreenshake(player, 24, Projectile.Center);

            /*for (float k = 0; k < 6.28f; k += 0.25f)
				Dust.NewDustPerfect(Projectile.position, DustID.DungeonWater, Vector2.One.RotatedBy(k) * 2, Scale: 2f);
			for (float k = 0; k < 6.28f; k += 0.15f)
				Dust.NewDustPerfect(Projectile.position, DustID.DungeonWater, Vector2.One.RotatedBy(k) * 2, Scale: 3f);*/

            for (float k = 0; k < Main.rand.Next(7, 11); k++) 
			{
				int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-7, 7), Main.rand.Next(-16, 7), ProjectileID.WaterBolt, Projectile.damage / 3, 0, Projectile.owner, 0f, 0f);
				Main.projectile[p].GetGlobalProjectile<GoldLeafProjectile>().gravity = 0.7f;
				Main.projectile[p].ai[0] = 3;
			}
			
			//Gore gore;
			//gore = Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, GoreType<WhirlpoolGore>());
			
		}

		public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>("GoldLeaf/Textures/Vortex1").Value;
            Texture2D tex2 = Request<Texture2D>("Goldleaf/Items/Dungeon/WhirlpoolP").Value;
            //Main.spriteBatch.Draw(tex, new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height - tex.Height * 0.5f + 2f), Color.White, );

            Main.spriteBatch.Draw //vortex texture
            (
                tex,
                new Vector2
                (
                    Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f,
                    Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f
                ),
                null,
                new Color(42, 43, 152) * 0.8f,
                GoldLeafWorld.rottime * -4.5f,
                tex.Size() * 0.5f,
                Projectile.scale * 0.9f,
                SpriteEffects.FlipHorizontally,
                0f
            );

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                //Vector2 drawPos3 = Projectile.oldPos[k] - Main.screenPosition + drawOrigin3 + new Vector2(0f, Projectile.gfxOffY);
                //Main.spriteBatch.Draw(tex3, drawPos3, null, Color.White * (0.8f - (0.15f * k)), GoldLeafWorld.rottime * 7, drawOrigin, Projectile.scale - (0.08f * k), SpriteEffects.None, 0f);

                Main.spriteBatch.Draw //trail
				(
					tex2,
                    Projectile.oldPos[k] - Main.screenPosition + Projectile.Size * 0.5f + new Vector2(0f, Projectile.gfxOffY),
                    null,
					new Color(255 - (9 * k), 255 - (9 * k), 255 - (4 * k)) * (0.8f - (0.15f * k)),
                    GoldLeafWorld.rottime * 7f,
	                tex2.Size() * 0.5f,
                    Projectile.scale - (0.06f * k),
                    SpriteEffects.None,
	                0f
				);
            }

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = Request<Texture2D>("Goldleaf/Items/Dungeon/WhirlpoolP").Value;

            Main.spriteBatch.Draw
            (
                tex,
                new Vector2
                (
                    Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f,
                    Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f
                ),
                null,
                Color.White,
                GoldLeafWorld.rottime * 7f,
                tex.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None,
                0f
            );
        }

        /*public override bool PreDraw(ref Color lightColor)
		{
			Texture2D tex = Request<Texture2D>("Goldleaf/Textures/Glow0").Value;
			Texture2D tex2 = Request<Texture2D>("Goldleaf/Textures/Vortex1").Value;
			Texture2D tex3 = Request<Texture2D>("Goldleaf/Items/Sets/Dungeon/WhirlpoolP").Value;

			Vector2 drawOrigin = new Vector2(tex.Width * 0.5f, tex.Height * 0.5f);
			Vector2 drawOrigin2 = new Vector2(tex2.Width * 0.5f, tex2.Height * 0.5f);
            Vector2 drawOrigin3 = new Vector2(tex3.Width * 0.5f, tex3.Height * 0.5f);

            Vector2 drawPos = Projectile.position - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
			Vector2 drawPos2 = Projectile.position - Main.screenPosition + drawOrigin2 + new Vector2(0f, Projectile.gfxOffY);

			//Main.spriteBatch.Draw(tex, drawPos, null, new Color(76, 129, 232), Projectile.rotation, drawOrigin, Projectile.scale * 1.3f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(tex2, drawPos2, null, new Color(42, 43, 152), GoldLeafWorld.rottime * -3f, drawOrigin2, Projectile.scale, SpriteEffects.None, 0f);

			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos3 = Projectile.oldPos[k] - Main.screenPosition + drawOrigin3 + new Vector2(0f, Projectile.gfxOffY);
				Main.spriteBatch.Draw(tex3, drawPos3, null, Color.White * (0.8f - (0.15f * k)), GoldLeafWorld.rottime * 7, drawOrigin, Projectile.scale - (0.08f * k), SpriteEffects.None, 0f);
			}
			return false;
		}

		public override void PostDraw(Color lightColor)
		{
			Texture2D tex = Request<Texture2D>("Goldleaf/Items/Sets/Dungeon/WhirlpoolP").Value;

			Vector2 drawOrigin = new Vector2(tex.Width * 0.5f, Projectile.height * 0.5f);
			Vector2 drawPos = Projectile.position - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

			Main.spriteBatch.Draw(tex, drawPos, null, Color.White, GoldLeafWorld.rottime * 7, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
		}*/
    }
}