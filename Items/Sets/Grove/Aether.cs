using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using GoldLeaf.Effects.Dusts;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using GoldLeaf.Core;
using Terraria.Audio;
using Microsoft.CodeAnalysis;

namespace GoldLeaf.Items.Sets.Grove
{
	public class Aether : ModItem, IGlowingItem
    {
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Aether's Comet");
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs("Casts a flame that follows the cursor and bursts on command\nChanneling the flame for long enough makes it shoot beams at nearby enemies, damage massively ramps up over time");

        public override void SetDefaults()
		{
			Item.width = 28;
            Item.mana = 70;
			Item.height = 28;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.staff[Item.type] = true;
			Item.useAnimation = 10;
			Item.useTime = 10;
            Item.reuseDelay = 10;
			Item.shootSpeed = 6f;
			Item.knockBack = 2f;
            Item.damage = 6;
            Item.UseSound = SoundID.DD2_EtherianPortalOpen;
            Item.shoot = ProjectileType<AetherBolt>();
			Item.rare = ItemRarityID.Green;
			Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.channel = true;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            //Main.PlaySound(SoundID.DD2_EtherianPortalOpen, player.Center);
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 45f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}
			Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(18f));
			velocity.X = perturbedSpeed.X;
			velocity.Y = perturbedSpeed.Y;
		}

        public void DrawGlowmask(PlayerDrawSet drawInfo)
        {
			Player player = drawInfo.drawPlayer;

			if (player.itemAnimation != 0)
			{
                Texture2D tex = Request<Texture2D>(Texture + "Glow").Value;
                //Texture2D tex = GetTexture(Texture + "Glow");
                Rectangle frame = new Rectangle(0, 0, 28, 28);
				Color color = Lighting.GetColor((int)player.Center.X / 16, (int)player.Center.Y / 16);
				Vector2 origin = new Vector2(player.direction == 1 ? 0 : frame.Width, frame.Height);

                var data = new DrawData(tex, drawInfo.ItemLocation - Main.screenPosition, frame, color, player.itemRotation, origin, player.HeldItem.scale, player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
                drawInfo.DrawDataCache.Add(data);
            }
		}

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D tex = Request<Texture2D>(Texture + "Glow").Value;
            spriteBatch.Draw
            (
                Request<Texture2D>(Texture + "Glow").Value,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - tex.Height * 0.5f + 2f
                ),
                new Rectangle(0, 0, tex.Width, tex.Height),
                Color.White,
                rotation,
                tex.Size() * 0.5f,
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

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 160;
            Projectile.ignoreWater = false;
        }

        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Aether Flame");

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>("GoldLeaf/Items/Sets/Grove/AetherBolt").Value;
            Texture2D tex2 = Request<Texture2D>("GoldLeaf/Textures/Flares/diamond").Value;

            Vector2 drawOrigin = new Vector2(tex.Width * 0.5f, Projectile.height * 0.5f);
            Vector2 drawOrigin2 = new Vector2(tex2.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Main.spriteBatch.Draw(tex, drawPos, null, Color.White * (1.0f - (0.05f * k)), Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(tex, Projectile.Center, null, new Color(255, 119, 246, 125), Projectile.rotation, drawOrigin2, Projectile.scale * 2f, SpriteEffects.None, 1f);
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;
            Main.spriteBatch.Draw(tex, new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height - tex.Height * 0.5f + 2f), new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height), Color.White, Projectile.rotation, tex.Size(), Projectile.scale, SpriteEffects.None, 0f);
        }

        public override void AI()
        {
            Lighting.AddLight((int)(Projectile.position.X/16), (int)(Projectile.position.Y/16), 1.4f, 0.4f, 1.8f);
            counter++;

            for (int k = 0; k < 4; k++)
            {
                float x = (float)Math.Cos(GoldLeafWorld.rottime + k) * Projectile.ai[0] / 30f;
                float y = (float)Math.Sin(GoldLeafWorld.rottime + k) * Projectile.ai[0] / 30f;
                Vector2 pos = (new Vector2(x, y)).RotatedBy(k / 12f * 6.28f);

                Dust d = Dust.NewDustPerfect(Projectile.Center, DustType<AetherDust>(), pos, 0, default, 1f);
            }

            Player player = Main.player[Projectile.owner];
            if (!player.channel)
            {
                Projectile.timeLeft -= 9;
            }
            if (Projectile.height <= 18)
            {
                Projectile.width++;
                Projectile.height++;
            }
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.5f;

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 25f)
            {
                Projectile.velocity.X *= 0.95f;
            }
            Vector2 vectorToCursor = Projectile.Center - player.Center;
            bool projDirection = Projectile.Center.X < player.Center.X;
            if (Projectile.Center.X < player.Center.X)
            {
                vectorToCursor = -vectorToCursor;
            }
            player.direction = ((!projDirection) ? 1 : (-1));
            player.itemRotation = vectorToCursor.ToRotation();
            player.itemTime = 10;
            player.itemAnimation = 10;

            if (counter < 80 && counter >= 40)
            {
                Projectile.velocity += Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 0.1f;
                if (Projectile.velocity.Length() > 6) Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 6;
            }

            if (counter == 80)
            {
                Projectile.penetrate += 5;

                for (float k = 0; k < 6.28f; k += 0.25f)
                    Dust.NewDustPerfect(Projectile.position, DustType<AetherDust>(), Vector2.One.RotatedBy(k) * 2, Scale: 0.9f);
            }

            if (counter >= 80)
            {
                Projectile.velocity += Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 0.65f;
                if (Projectile.velocity.Length() > 4) Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 4;

                shootcounter++;
                if (shootcounter >= 13)
                {
                    shootcounter = 0;
                    float num = 8000f;
                    int num2 = -1;
                    for (int i = 0; i < 200; i++)
                    {
                        float num3 = Vector2.Distance(Projectile.Center, Main.npc[i].Center);
                        if (num3 < num && num3 < 450f && Main.npc[i].CanBeChasedBy(Projectile, false))
                        {
                            num2 = i;
                            num = num3;
                        }
                    }
                    if (num2 != -1)
                    {
                        bool flag = Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[num2].position, Main.npc[num2].width, Main.npc[num2].height);
                        if (flag)
                        {
                            SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/AetherBeam"), player.Center);
                            Vector2 value = Main.npc[num2].Center - Projectile.Center;
                            float num4 = 25f;
                            float num5 = (float)Math.Sqrt((double)(value.X * value.X + value.Y * value.Y));
                            if (num5 > num4)
                            {
                                num5 = num4 / num5;
                            }
                            value *= num5;
                            Projectile.timeLeft += 7;
                            Projectile.damage += 2;
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, value.X, value.Y, ProjectileType<AetherBeam>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack / 2f, Projectile.owner, 0f, 0f);
                        }
                    }
                }

            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (counter >= 30) return false; else return true;
        }

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 0;
            target.buffImmune[BuffID.Ichor] = false;
            target.buffImmune[BuffID.CursedInferno] = false;
        }

        public override void Kill(int timeLeft)
        {
            for (float k = 0; k < 6.28f; k += 0.25f-(counter/1000))
                Dust.NewDustPerfect(Projectile.position, DustType<AetherDust>(), Vector2.One.RotatedBy(k) * 2, Scale:1.4f+(counter/60));

            Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(SoundID.Item14, player.Center);
            SoundEngine.PlaySound(SoundID.Item73, player.Center);
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastAuraPulse, player.Center);
            SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/AetherBurst"), player.Center);
            int explosion = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0f, 0f), ProjectileType<AetherBurst>(), Projectile.damage+(counter)/4, Projectile.knockBack * 3, player.whoAmI);
            if (counter >= 80) Main.projectile[explosion].ai[0] = 80f + 100; else Main.projectile[explosion].ai[0] = 80f + (counter * 1.16f);
            if (counter < 30) Main.projectile[explosion].damage = counter;


        }
    }

    public class AetherBurst : ModProjectile
    {
        public override string Texture => "GoldLeaf/Textures/Empty";

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Aether Burst");

        public override void AI()
        {
            Projectile.height = (int)Projectile.ai[0];
            Projectile.width = (int)Projectile.ai[0];
            Lighting.AddLight((int)(Projectile.position.X/16), (int)(Projectile.position.Y/16), 2.1f, 0.6f, 2.7f);
        }

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 0;
            target.buffImmune[BuffID.Ichor] = false;
            target.buffImmune[BuffID.CursedInferno] = false;
        }
    }

    public class AetherBeam : ModProjectile
    {
        public override string Texture => "GoldLeaf/Effects/Dusts/AetherDust";
        /*public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }*/

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.timeLeft = 80;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Aether Beam");

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            for (int i = 0; i < 10; i++)
            {
                Dust num;
                num = Dust.NewDustPerfect(Projectile.position, DustType<AetherDust>(), new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
                num.velocity = Projectile.velocity/2;
                num.rotation = Projectile.velocity.ToRotation();
                num.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 0;
            target.buffImmune[BuffID.Ichor] = false;
            target.buffImmune[BuffID.CursedInferno] = false;
            //int i = BuffID.Count;
        }
    }
}
