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
using GoldLeaf.Items.Misc;
using Terraria.GameContent.Drawing;
using GoldLeaf.Items.Grove.Boss;
using GoldLeaf.Tiles.Decor;

namespace GoldLeaf.Items.Grove
{
	public class Aether : ModItem
    {
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Aether's Comet");
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs("Casts a flame that bursts on command, damage ramps up over time\n" +
                                                                             "Channeling the flame for long enough makes it spark at nearby enemies");
        public override void SetDefaults()
		{
			Item.width = 28;
            Item.mana = 16;
			Item.height = 28;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.staff[Item.type] = true;
			Item.noMelee = true;
			Item.useAnimation = 10;
			Item.useTime = 10;
            Item.reuseDelay = 25;
			Item.shootSpeed = 6f;
			Item.knockBack = 6;
            Item.crit = -2;
            Item.damage = 19;
            Item.ArmorPenetration = 4;
            //Item.UseSound = SoundID.DD2_EtherianPortalOpen;
            Item.UseSound = new SoundStyle("GoldLeaf/Sounds/SE/RoR2/FireCast");
            Item.shoot = ProjectileType<AetherBolt>();
			Item.rare = ItemRarityID.Orange;
            Item.DamageType = DamageClass.Magic;
            Item.channel = true;
            Item.value = Item.sellPrice(0, 0, 80, 0);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 45f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}
			Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(9f));
			velocity.X = perturbedSpeed.X;
			velocity.Y = perturbedSpeed.Y;
		}

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D tex = Request<Texture2D>(Texture + "Glow").Value;
            spriteBatch.Draw
            (
                tex,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - tex.Height * 0.5f
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

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<Echobark>(), 20);
            recipe.AddIngredient(ItemType<EveDroplet>(), 80);
            recipe.AddIngredient(ItemType<HeavenShard>(), 4);
            recipe.AddIngredient(ItemType<WaxCandle>(), 1);
            recipe.AddTile(TileID.Anvils);
            
            recipe.AddOnCraftCallback(RecipeCallbacks.AetherCraftEffect);
            recipe.Register();
        }
    }

    public class AetherBolt : ModProjectile
    {
        int counter = 0;
        int shootCounter = 0;
        int shotsFired = 0;

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
            Projectile.ArmorPenetration = 4;
            Projectile.timeLeft = 190;
            Projectile.ignoreWater = false;

            Projectile.DamageType = DamageClass.Magic;
        }

        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Aether Flame");

        public override void PostDraw(Color lightColor)
        {
            if (counter > 60) 
            {
                //Color color = new Color(255 - (80 - cooldown * 3), 119 - (80 - cooldown * 3), 246 - (80 - cooldown * 3));
                Color color = new Color(255 - (Projectile.timeLeft/3), 119 - (Projectile.timeLeft/3), 246 - (Projectile.timeLeft/3));
                color.A = 0;

                Texture2D tex = Request<Texture2D>("GoldLeaf/Textures/Shine").Value;
                Main.spriteBatch.Draw(tex, new Vector2(Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f, Projectile.position.Y - Main.screenPosition.Y + Projectile.height - tex.Height * 0.5f + 2f), new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height), color, GoldLeafWorld.rottime * -1, tex.Size(), Projectile.scale, SpriteEffects.None, 0f);
            }
        }

        public override void AI()
        {
            Lighting.AddLight((int)(Projectile.position.X/16), (int)(Projectile.position.Y/16), 0.7f, 0.2f, 0.9f);
            counter++;

            Player player = Main.player[Projectile.owner];
            if (!player.channel)
            {
                Projectile.timeLeft -= 9;
                if (counter <= 40) 
                {
                    Projectile.velocity += Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 0.1f;
                    //if (Projectile.velocity.Length() > 6) Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 6;

                    Projectile.penetrate = 1;
                }
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
            player.itemTime = 20;
            player.itemAnimation = 20;

            if (counter < 80 && counter >= 40)
            {
                Projectile.velocity += Vector2.Normalize(Main.MouseWorld - Projectile.Center) * (0.1f + (counter/80));
                if (Projectile.velocity.Length() > 7) Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 7;
            }

            if (counter == 80)
            {
                Projectile.penetrate += 5;

                for (float k = 0; k < 6.28f; k += 0.2f)
                    Dust.NewDustPerfect(Projectile.Center, DustType<AetherDust>(), Vector2.One.RotatedBy(k) * 2, Scale: 0.9f);
            }

            if (counter >= 80)
            {
                if (Main.rand.NextBool(16)) 
                {
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Main.rand.NextFloat(6.28f).ToRotationVector2() * Main.rand.NextFloat(2, 3), ProjectileType<AetherEmber>(), 0, 0, Projectile.owner).scale = Main.rand.NextFloat(0.45f, 0.75f);
                }

                Projectile.velocity += Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 0.65f;
                if (Projectile.velocity.Length() > (4 + (counter * 0.005f))) Projectile.velocity = Vector2.Normalize(Projectile.velocity) * (4 + (counter * 0.005f));

                shootCounter++;
                if (shootCounter >= 16 - (shotsFired / 3))
                {
                    shootCounter = 0;
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
                        bool flag = Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[num2].position, Main.npc[num2].width, Main.npc[num2].height) && player.CheckMana(3, true);
                        if (flag)
                        {
                            SoundStyle sound1 = new("GoldLeaf/Sounds/SE/RoR2/WispDeath") { Volume = 0.7f, Pitch = -0.35f + (shotsFired * 0.035f) };

                            //SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/AetherBeam"), player.Center);
                            SoundEngine.PlaySound(sound1, player.Center);
                            Vector2 value = Main.npc[num2].Center - Projectile.Center;
                            float num4 = 25f;
                            float num5 = (float)Math.Sqrt((double)(value.X * value.X + value.Y * value.Y));
                            if (num5 > num4)
                            {
                                num5 = num4 / num5;
                            }
                            value *= num5;
                            Projectile.timeLeft += 11 - (shotsFired / 3);
                            //Projectile.damage += 1 + cooldown/120;

                            Vector2 perturbedSpeed = value.RotatedByRandom(MathHelper.ToRadians(8f));

                            shotsFired++;
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, perturbedSpeed.X * 0.3f, perturbedSpeed.Y * 0.3f, ProjectileType<AetherBeam>(), (int)(Projectile.damage * 0.7f), Projectile.knockBack * 0.35f, Projectile.owner, 0f, 0f);
                        }
                    }
                }
            }

            for (int k = 0; k < 4; k++)
            {
                float x = (float)Math.Cos(GoldLeafWorld.rottime + (counter * 0.03f) + k) * Projectile.ai[0] / 64f;
                float y = (float)Math.Sin(GoldLeafWorld.rottime + (counter * 0.03f) + k) * Projectile.ai[0] / 64f;
                Vector2 pos = (new Vector2(x, y)).RotatedBy(k / 12f * 6.28f);

                Dust d = Dust.NewDustPerfect(Projectile.Center, DustType<AetherDust>(), pos * (0.3f + counter * 0.0015f), 0, default, 1f + (counter * 0.002f));
                d.velocity += new Vector2(0, (counter * -0.02f));
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (counter >= 30) return false; else return true;
        }

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            if (!player.channel && counter <= 40) target.immune[Projectile.owner] = 0; else target.immune[Projectile.owner] = 15;

            target.buffImmune[BuffID.OnFire] = false;
            target.buffImmune[BuffID.Frostburn] = false;
            target.buffImmune[BuffID.CursedInferno] = false;
            target.buffImmune[BuffID.Ichor] = false;
            target.buffImmune[BuffID.ShadowFlame] = false;
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            //Texture2D glowTex = Request<Texture2D>("GoldLeaf/Textures/RingGlow0").Value;

            SoundStyle sound1 = new("GoldLeaf/Sounds/SE/RoR2/EngineerMine") { Volume = 0.8f };
            SoundStyle sound2 = new("GoldLeaf/Sounds/SE/RoR2/Aftershock") { Pitch = 1.6f, Volume = 0.8f };

            SoundEngine.PlaySound(sound1, player.Center);
            SoundEngine.PlaySound(sound2, player.Center);
            int explosion = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<AetherBurst>(), Projectile.damage + (int)(shotsFired * 2.2), Projectile.knockBack, player.whoAmI);
            if (counter >= 80) Main.projectile[explosion].ai[0] = 110f; else Main.projectile[explosion].ai[0] = 30f + (counter * 0.65f);
            Helper.AddScreenshake(player, 18 + shotsFired, Projectile.Center);
        }
    }

    public class AetherBurst : ModProjectile
    {
        public override string Texture => "GoldLeaf/Textures/Empty";

        public float TimeFade => 1 - Projectile.timeLeft / 20f;
        public float Radius => Helper.BezierEase((20 - Projectile.timeLeft) / 20f) * Projectile.ai[0];
        int counter = 0;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 28;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ArmorPenetration = 4;
            //Projectile.extraUpdates = 1;

            Projectile.DamageType = DamageClass.Magic;
        }

        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Aether Burst");

        public override void OnSpawn(IEntitySource source)
        {
            for (int j = 0; j < 10 + (Projectile.ai[0]/ 15); j++)
            {
                var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(16, 16), 0, 0, DustType<AetherSmoke>());
                dust.velocity = Main.rand.NextVector2Circular(7.5f, 7.5f) * Projectile.ai[0] / 65;
                dust.scale = Main.rand.NextFloat(0.9f, 1.2f);
                dust.alpha = 20 + Main.rand.Next(60);
                dust.rotation = Main.rand.NextFloat(6.28f);
            }

            for (int i = 0; i < 4 + (Projectile.ai[0]/40); i++)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextFloat(6.28f).ToRotationVector2() * Main.rand.NextFloat(2, 3), ProjectileType<AetherEmber>(), 0, 0, Projectile.owner).scale = Main.rand.NextFloat(0.75f, 1.25f);
            }

            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.TrueExcalibur,
                new ParticleOrchestraSettings { PositionInWorld = Projectile.Center },
                Projectile.owner);
        }

        public override void AI()
        {
            Lighting.AddLight((int)(Projectile.position.X / 16), (int)(Projectile.position.Y / 16), 1.4f, 0.4f, 1.8f);

            counter++;

            if (counter > 18) 
            {
                Projectile.damage = 0;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Helper.CheckCircularCollision(Projectile.Center, (int)Radius + 30, targetHitbox);
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = Request<Texture2D>("GoldLeaf/Textures/Flares/wavering").Value;
            Color color = new Color(255, 119, 246) * (1.25f - (counter * 0.05f));
            //Color color = new Color(196, 43, 255) * (1.2f - (counter * 0.05f));
            color.A = 0;

            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw //fireball
            (
                tex,
                new Vector2
                (
                    Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f,
                    Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f
                ),
                new Rectangle(0, 0, tex.Width, tex.Height),
                color,
                0f,
                tex.Size() * 0.5f,
                Projectile.scale * 0.15f + (Radius * 0.01f), // + (cooldown * 0.022f),
                SpriteEffects.None,
                0f
            );
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.velocity += Vector2.Normalize(target.Center - Projectile.Center) * (12 + (damageDone * 0.05f)) * target.knockBackResist;

            target.immune[Projectile.owner] = 6;

            target.buffImmune[BuffID.OnFire] = false;
            target.buffImmune[BuffID.Frostburn] = false;
            target.buffImmune[BuffID.CursedInferno] = false;
            target.buffImmune[BuffID.Ichor] = false;
            target.buffImmune[BuffID.ShadowFlame] = false;
        }
    }

    public class AetherBeam : ModProjectile
    {
        public override string Texture => "GoldLeaf/Items/Grove/AetherDust";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            //Projectile.penetrate = 1;
            Projectile.ArmorPenetration = 4;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.extraUpdates = 4;
            Projectile.timeLeft = 160;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.DamageType = DamageClass.Magic;
        }
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Aether Beam");

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<AetherDust>(), Projectile.velocity * 0.4f, 0, Color.White, 0.6f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>("GoldLeaf/Items/Grove/AetherDust").Value;
            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Main.spriteBatch.Draw(tex, drawPos, null, Color.White * (1.0f - (0.05f * k)), Projectile.rotation + MathHelper.PiOver2, drawOrigin, Projectile.scale * 1.2f - (0.06f * k), SpriteEffects.None, 0f);
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 3; i++)
            {
                var dust = Dust.NewDustPerfect(Projectile.Center, DustType<AetherSmokeFast>(), Projectile.velocity * 0.1f + Main.rand.NextVector2Circular(1.5f, 1.5f));
                dust.scale = 0.45f * Projectile.scale;
                dust.rotation = Main.rand.NextFloatDirection();
            }

            target.buffImmune[BuffID.OnFire] = false;
            target.buffImmune[BuffID.Frostburn] = false;
            target.buffImmune[BuffID.CursedInferno] = false;
            target.buffImmune[BuffID.Ichor] = false;
            target.buffImmune[BuffID.ShadowFlame] = false;
        }
    }

    public class AetherEmber : ModProjectile
    {
        public override string Texture => "GoldLeaf/Effects/Dusts/SpecialSmokeDust";

        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.aiStyle = 1;
            Projectile.width = Projectile.height = 12;
            //ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            //ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Projectile.timeLeft = 90;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.scale *= 0.98f;

            if (Main.rand.NextBool(2))
            {
                var dust = Dust.NewDustPerfect(Projectile.Center, DustType<AetherSmokeFast>(), Main.rand.NextVector2Circular(1.5f, 1.5f));
                dust.scale = 0.5f * Projectile.scale;
                dust.rotation = Main.rand.NextFloatDirection();
            }
        }
    }

    public class AetherSmoke : ModDust
    {
        public override string Texture => "GoldLeaf/Effects/Dusts/SpecialSmoke";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 36, 34, 36);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color color;
            if (dust.alpha < 30)
                color = new Color(255, 119, 246);
            else if (dust.alpha < 110)
                color = Color.Lerp(new Color(255, 119, 246), new Color(196, 43, 255), (dust.alpha - 30) / 80f);
            else if (dust.alpha < 140)
                color = Color.Lerp(new Color(196, 43, 255), new Color(54, 47, 56), (dust.alpha - 110) / 50f);
            else
                color = new Color(54, 47, 56);

            return color * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            if (dust.velocity.Length() > 3)
                dust.velocity *= 0.85f;
            else
                dust.velocity *= 0.9f;

            if (dust.alpha > 100)
            {
                dust.scale += 0.012f;
                dust.alpha += 2;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.985f;
                dust.alpha += 4;
            }

            dust.position += dust.velocity;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }

    public class AetherSmokeFast : ModDust
    {
        public override string Texture => "GoldLeaf/Effects/Dusts/SpecialSmoke";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 36, 34, 36);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color color;
            if (dust.alpha < 20)
                color = new Color(179, 255, 224);
            else if (dust.alpha < 60)
                color = Color.Lerp(new Color(179, 255, 224), new Color(255, 119, 246), (dust.alpha - 20) / 40f);
            else if (dust.alpha < 100)
                color = Color.Lerp(new Color(255, 119, 246), new Color(54, 47, 56), (dust.alpha - 60) / 60f);
            else
                color = new Color(54, 47, 56);

            return color * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            if (dust.velocity.Length() > 3)
                dust.velocity *= 0.85f;
            else
                dust.velocity *= 0.92f;

            if (dust.alpha > 60)
            {
                dust.scale += 0.01f;
                dust.alpha += 6;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.985f;
                dust.alpha += 4;
            }

            dust.position += dust.velocity;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }

    public class AetherDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 6, 8);
            dust.scale *= 3;
            dust.velocity *= 1.8f;
            dust.alpha = 300;
        }

        public override bool MidUpdate(Dust dust)
        {
            if (!dust.noGravity)
            {
                dust.velocity.Y = 0f;
            }

            if (dust.noLight)
            {
                return false;
            }


            return false;
        }

        public override bool Update(Dust dust)
        {
            dust.alpha -= 1;
            dust.position += dust.velocity;
            dust.velocity *= 0.95f;
            dust.rotation = (float)Math.Atan2(dust.velocity.Y, dust.velocity.X) + 1.57f;
            dust.scale *= 0.88f;
            if (dust.scale < 0.25f)
            {
                dust.active = false;
            }
            if (dust.noLight == false)
            {
                Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), dust.scale * 0.35f, dust.scale * 0.1f, dust.scale * 0.45f);
            }
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
            => new Color(255, 255, 255, 0);
    }
}
