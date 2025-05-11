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
using GoldLeaf.Items.Grove;
using Terraria.GameContent.Drawing;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.Diagnostics.Metrics;
using ReLogic.Content;

namespace GoldLeaf.Items.Dungeon
{
	public class Whirlpool : ModItem
	{
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            ItemSets.Glowmask[Type] = (glowTex, ColorHelper.AdditiveWhite);
        }

        public override void SetDefaults() 
		{
			Item.damage = 57;
			ItemID.Sets.ToolTipDamageMultiplier[Item.type] = 0.8f;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 34;
			Item.width = 44;
			Item.height = 44;
			Item.useTime = 60;
			Item.useAnimation = 60;
			Item.shootSpeed = 12f;
			Item.useStyle = ItemUseStyleID.Shoot;
            Item.staff[Item.type] = true;
            Item.shoot = ProjectileType<WhirlpoolP>();
			Item.knockBack = 9;
			Item.value = 100000;
			Item.rare = ItemRarityID.Orange;
			//Item.UseSound = SoundID.Item167;
            Item.UseSound = new SoundStyle("GoldLeaf/Sounds/SE/HollowKnight/JellyfishMiniDeath");
            Item.autoReuse = false;
			Item.channel = true;
			Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, 5);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Texture2D tex = glowTex.Value;
			spriteBatch.Draw
			(
				tex,
				new Vector2
				(
					Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
					Item.position.Y - Main.screenPosition.Y + Item.height - tex.Height * 0.5f
				),
				new Rectangle(0, 0, tex.Width, tex.Height),
                ColorHelper.AdditiveWhite,
				rotation,
				tex.Size() * 0.5f,
				scale,
				SpriteEffects.None,
				0f
			);
        }
		
		public override void AddRecipes() 
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.AquaScepter);
            //recipe.AddIngredient(ItemType<>(Runestone), 3);
			recipe.AddIngredient(ItemID.Bone, 40);
			//recipe.AddOnCraftCallback(RecipeCallbacks.RunestoneCraftEffect);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
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
        
        public ref float Counter => ref Projectile.ai[0];
        public ref float Bounces => ref Projectile.ai[1];

		public override void SetDefaults()
		{
			Projectile.width = 2;
			Projectile.height = 2;

			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 200;
			Projectile.ignoreWater = true;

            Projectile.netImportant = true;
            Projectile.ai[1] = 5;

            Projectile.DamageType = DamageClass.Magic;

            Counter = Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter;

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
				if (Counter >= 10 && Counter <= 40 && Main.myPlayer == Projectile.owner)
				{
					Projectile.velocity = Projectile.velocity.Length() * Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.MouseWorld) * Projectile.velocity.Length() * 0.5f, 0.15f).SafeNormalize(Vector2.Normalize(Projectile.velocity));
					Projectile.netUpdate = true;
				}
			}
			Counter++;
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

			SoundEngine.PlaySound(new SoundStyle("Goldleaf/Sounds/SE/SplashBounce"), Projectile.Center);

			Bounces--;
			Projectile.velocity *= 0.825f;

			if (Bounces > 0)
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
            SoundStyle sound1 = new("GoldLeaf/Sounds/SE/RoR2/EngineerMine") { Volume = 0.4f, Pitch = -0.5f };

            SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Monolith/CrashShock"), Projectile.Center);
            //SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
            SoundEngine.PlaySound(sound1, Projectile.Center);

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

            //Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<WhirlpoolBurst>(), 0, 0, Projectile.owner);			
		}

		public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>("GoldLeaf/Textures/Vortex1").Value;
            Texture2D tex2 = Request<Texture2D>("Goldleaf/Items/Dungeon/WhirlpoolP").Value;
            //Main.spriteBatch.Draw(glowTex, new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height - glowTex.Height * 0.5f + 2f), Color.White, );

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
    }

    /*public class WhirlpoolGore : ModGore
    {
        public override string Texture => "GoldLeaf/Textures/Vortex1";

        public override Color? GetAlpha(Gore gore, Color lightColor)
        {
            return new Color(73, 106, 233);
        }

        public override void OnSpawn(Gore gore, IEntitySource source)
        {
			
            gore.numFrames = 1;
            gore.behindTiles = false;
            gore.timeLeft = 20;
            gore.velocity = Vector2.Zero;
            ChildSafety.SafeGore[gore.type] = true;
        }

        public override bool Update(Gore gore)
        {
            gore.rotation = gore.timeLeft * 3f;
            gore.alpha = gore.timeLeft * 3;
            gore.scale *= 1.04f;

            if (gore.alpha < 3)
            {
                gore.frameCounter = 0;
                gore.frame++;
                if (gore.frame > 7) gore.frame = 0;
            }
            return false;
        }
    }*/

    /*public class WhirlpoolBurst : ModProjectile
    {
        public override string Texture => "GoldLeaf/Textures/Vortex1";

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.damage = 0;
            Projectile.timeLeft = 40;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation = (Projectile.timeLeft - 4) * 7f;
            Projectile.alpha = (int)(Projectile.timeLeft * 6.5f);
            Projectile.scale *= 1.1f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;
            Color color = new Color(73, 106, 233) * (Projectile.alpha/255);

            Main.spriteBatch.Draw
            (
                tex,
                new Vector2
                (
                    Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f,
                    Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f
                ),
                new Rectangle(0, 0, tex.Width, tex.Height),
                color,
                Projectile.rotation,
                tex.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None,
                0f
            );

            return false;
        }
    }*/
}