using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using GoldLeaf.Dusts;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;

namespace GoldLeaf.Items.Wisp
{
	public class Aether : ModItem, IGlowingItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aether");
            Tooltip.SetDefault("Channels a bolt that bursts on command\nChanneling the bolt for long enough makes it shoot beams at nearby enemies");
        }

        public override void SetDefaults()
		{
			item.width = 28;
            item.mana = 20;
			item.height = 28;
			item.useStyle = 5;
			Item.staff[item.type] = true;
			item.useAnimation = 10;
			item.useTime = 10;
            item.reuseDelay = 10;
			item.shootSpeed = 6f;
			item.knockBack = 2f;
            item.damage = 18;
            item.UseSound = SoundID.DD2_EtherianPortalOpen;
            item.shoot = ProjectileType<AetherBolt>();
			item.rare = 3;
			item.noMelee = true;
			item.summon = true;
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

        public void DrawGlowmask(PlayerDrawInfo info)
        {
			Player player = info.drawPlayer;

			if (player.itemAnimation != 0)
			{
				Texture2D tex = GetTexture(Texture + "Glow");
				Rectangle frame = new Rectangle(0, 0, 28, 28);
				Color color = Lighting.GetColor((int)player.Center.X / 16, (int)player.Center.Y / 16);
				Vector2 origin = new Vector2(player.direction == 1 ? 0 : frame.Width, frame.Height);

				Main.playerDrawData.Add(new DrawData(tex, info.itemLocation - Main.screenPosition, frame, color, player.itemRotation, origin, player.HeldItem.scale, info.spriteEffects, 0));
			}
		}

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture;
            texture = Main.itemTexture[item.type];
            spriteBatch.Draw
            (
                GetTexture(Texture + "Glow"),
                new Vector2
                (
                    item.position.X - Main.screenPosition.X + item.width * 0.5f,
                    item.position.Y - Main.screenPosition.Y + item.height - texture.Height * 0.5f + 2f
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
	}

    public class AetherBolt : ModProjectile
    {
        int counter = 0;
        int shootcounter = 0;
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
            if (counter == 80)
            {
                projectile.penetrate += 4;

                for (float k = 0; k < 6.28f; k += 0.25f)
                    Dust.NewDustPerfect(projectile.position, DustType<AetherDust>(), Vector2.One.RotatedBy(k) * 2, Scale: 0.4f);
            }

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

            if (counter >= 80)
            {
                shootcounter++;
                if (shootcounter >= 16)
                {
                    shootcounter = 0;
                    float num = 8000f;
                    int num2 = -1;
                    for (int i = 0; i < 200; i++)
                    {
                        float num3 = Vector2.Distance(projectile.Center, Main.npc[i].Center);
                        if (num3 < num && num3 < 450f && Main.npc[i].CanBeChasedBy(projectile, false))
                        {
                            num2 = i;
                            num = num3;
                        }
                    }
                    if (num2 != -1)
                    {
                        bool flag = Collision.CanHit(projectile.position, projectile.width, projectile.height, Main.npc[num2].position, Main.npc[num2].width, Main.npc[num2].height);
                        if (flag)
                        {
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/SE/AetherBeam"), player.Center);
                            Vector2 value = Main.npc[num2].Center - projectile.Center;
                            float num4 = 25f;
                            float num5 = (float)Math.Sqrt((double)(value.X * value.X + value.Y * value.Y));
                            if (num5 > num4)
                            {
                                num5 = num4 / num5;
                            }
                            value *= num5;
                            projectile.timeLeft += 8;
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value.X, value.Y, ProjectileType<AetherBeam>(), (int)(projectile.damage * 0.8f), projectile.knockBack / 2f, projectile.owner, 0f, 0f);
                        }
                    }
                }

                projectile.velocity += Vector2.Normalize(Main.MouseWorld - projectile.Center) * 0.15f;
                if (projectile.velocity.Length() > 5) projectile.velocity = Vector2.Normalize(projectile.velocity) * 5;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (counter >= 80) return false; else return true;
        }

        public override void Kill(int timeLeft)
        {
            for (float k = 0; k < 6.28f; k += 0.25f-(counter/1000))
                Dust.NewDustPerfect(projectile.position, DustType<AetherDust>(), Vector2.One.RotatedBy(k) * 2, Scale:1.4f+(counter/60));

            Player player = Main.player[projectile.owner];
            Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 14);
            Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 73);
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/SE/AetherBurst"), player.Center);
            Main.PlaySound(SoundID.DD2_WitherBeastAuraPulse, player.Center);
            int explosion = Projectile.NewProjectile(projectile.Center, new Vector2(0f, 0f), ProjectileType<AetherBurst>(), projectile.damage+(counter)/4, projectile.knockBack, player.whoAmI);
            if (counter >= 80) Main.projectile[explosion].ai[0] = 80f + 100; else Main.projectile[explosion].ai[0] = 80f + counter;
            
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
            projectile.height = (int)projectile.ai[0];
            projectile.width = (int)projectile.ai[0];
            Lighting.AddLight((int)(projectile.position.X/16), (int)(projectile.position.Y/16), 2.1f, 0.6f, 2.7f);
        }
    }

    public class AetherBeam : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.width = 8;
            projectile.height = 8;
            projectile.timeLeft = 80;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aether Beam");
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();

            for (int i = 0; i < 10; i++)
            {
                Dust num;
                num = Dust.NewDustPerfect(projectile.position, DustType<AetherDust>(), new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
                num.velocity = projectile.velocity/2;
                num.rotation = projectile.velocity.ToRotation();
                num.noGravity = true;
            }
        }
    }
}
