using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using GoldLeaf.Dusts;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GoldLeaf.Items.Wisp
{
	public class Aether : ModItem
	{
		public override void SetDefaults()
		{
			item.width = 28;
            item.mana = 5;
			item.height = 28;
			item.useStyle = 5;
			Item.staff[item.type] = true;
			item.useAnimation = 10;
			item.useTime = 10;
            item.reuseDelay = 10;
			item.shootSpeed = 6f;
			item.knockBack = 2f;
            item.damage = 14;
            item.UseSound = SoundID.DD2_EtherianPortalOpen;
            item.shoot = ProjectileType<AetherBolt>();
			item.rare = 3;
			item.noMelee = true;
			item.magic = true;
			item.channel = true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
            Main.PlaySound(SoundID.DD2_EtherianPortalOpen, player.Center);
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 45f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}
			Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(18f));
			speedX = perturbedSpeed.X;
			speedY = perturbedSpeed.Y;
			return true;
		}

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aether");
			Tooltip.SetDefault("Channels a bolt that bursts on command");
		}
	}

    public class AetherBolt : ModProjectile
    {
        int counter = 0;
        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.penetrate = 4;
            projectile.timeLeft = 160;
            projectile.magic = true;
            projectile.ignoreWater = false;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aether Bolt");
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D face = mod.GetTexture("Items/Wisp/AetherBolt");
            spriteBatch.Draw(mod.GetTexture("Items/Wisp/AetherBolt"), new Vector2(projectile.position.X - Main.screenPosition.X + projectile.width * 0.5f, projectile.position.Y - Main.screenPosition.Y + projectile.height - face.Height * 0.5f + 2f), new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height), Color.White, projectile.rotation, face.Size(), projectile.scale, SpriteEffects.None, 0f);
        }

        public override void AI()
        {
            Lighting.AddLight((int)(projectile.position.X/16), (int)(projectile.position.Y/16), 1.4f, 0.4f, 1.8f);
            counter++;

            for (int k = 0; k < 4; k++)
            {
                float x = (float)Math.Cos(GoldleafWorld.rottime + k) * projectile.ai[0] / 30f;
                float y = (float)Math.Sin(GoldleafWorld.rottime + k) * projectile.ai[0] / 30f;
                Vector2 pos = (new Vector2(x, y)).RotatedBy(k / 12f * 6.28f);

                Dust d = Dust.NewDustPerfect(projectile.Center, DustType<AetherDust>(), pos, 0, default, 1f);
            }
            if (counter == 100) for (float k = 0; k < 6.28f; k += 0.25f)
                    Dust.NewDustPerfect(projectile.position, DustType<AetherDust>(), Vector2.One.RotatedBy(k) * 2, Scale:0.5f);

            Player player = Main.player[projectile.owner];
            if (!player.channel)
            {
                projectile.timeLeft -= 12;
            }
            if (projectile.height <= 18)
            {
                projectile.width++;
                projectile.height++;
            }
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.5f;

            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 25f)
            {
                projectile.velocity.X *= 0.95f;
            }
            Vector2 vectorToCursor = projectile.Center - player.Center;
            bool projDirection = projectile.Center.X < player.Center.X;
            if (projectile.Center.X < player.Center.X)
            {
                vectorToCursor = -vectorToCursor;
            }
            player.direction = ((!projDirection) ? 1 : (-1));
            player.itemRotation = vectorToCursor.ToRotation();
            player.itemTime = 10;
            player.itemAnimation = 10;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (counter > 100) return false; else return true;
        }

        public override void Kill(int timeLeft)
        {
            for (float k = 0; k < 6.28f; k += 0.25f)
                Dust.NewDustPerfect(projectile.position, DustType<AetherDust>(), Vector2.One.RotatedBy(k) * 2, Scale:1.4f);

            Player player = Main.player[projectile.owner];
            Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 14);
            Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 73);
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/SE/AetherBurst"), player.Center);
            Main.PlaySound(SoundID.DD2_WitherBeastAuraPulse, player.Center);
            int explosion = Projectile.NewProjectile(projectile.Center, new Vector2(0f, 0f), ProjectileType<AetherBurst>(), projectile.damage+(counter)/4, projectile.knockBack, player.whoAmI);
            if (counter > 100) Main.projectile[explosion].ai[0] = 80f + 100; else Main.projectile[explosion].ai[0] = 80f + counter;
            
        }
    }

    public class AetherBurst : ModProjectile
    {
        public override string Texture => "GoldLeaf/Textures/Empty";
        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aether Burst");
        }

        public override void AI()
        {
            projectile.height = (int)projectile.ai[0] *(int)1.3;
            projectile.width = (int)projectile.ai[0] *(int)1.3;
            Lighting.AddLight((int)(projectile.position.X/16), (int)(projectile.position.Y/16), 2.1f, 0.6f, 2.7f);
        }
    }
}
