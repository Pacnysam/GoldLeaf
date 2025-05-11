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
using GoldLeaf.Items.Grove.Wood;
using System.IO;
using Terraria.ModLoader.IO;
using Microsoft.Build.Evaluation;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using Terraria.GameContent;

namespace GoldLeaf.Items.Grove.Boss
{
    public class Aether : ModItem
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }
        public override void SetStaticDefaults()
        {
            ItemSets.Glowmask[Type] = (glowTex, Color.White);
        }

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
            Item.UseSound = new SoundStyle("GoldLeaf/Sounds/SE/RoR2/FireCast") { Volume = 0.85f };
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
            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite, rotation, TextureAssets.Item[Item.type].Size() / 2, scale, SpriteEffects.None, 0f);
            /*Texture2D tex = glowTex.Value;
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
            );*/
        }

        /*public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<Echobark>(), 20);
            recipe.AddIngredient(ItemType<EveDroplet>(), 80);
            recipe.AddIngredient(ItemType<HeavenShard>(), 4);
            recipe.AddIngredient(ItemType<WaxCandle>(), 1);
            recipe.AddTile(TileID.Anvils);
            
            recipe.AddOnCraftCallback(RecipeCallbacks.AetherCraftEffect);
            recipe.Register();
        }*/
    }
    
    public class AetherBolt : ModProjectile
    {
        const int THRESHHOLD = 60;
        ref float Counter => ref Projectile.ai[0];
        ref float ShootCounter => ref Projectile.ai[1];
        ref float ShotsFired => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {

            Projectile.width = 14;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 4;
            Projectile.ArmorPenetration = 4;
            Projectile.timeLeft = 190;
            Projectile.ignoreWater = false;

            Projectile.netImportant = true;

            Projectile.DamageType = DamageClass.Magic;
        }

        /*public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Counter);
            writer.Write(ShootCounter);
            writer.Write(ShotsFired);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Counter = reader.ReadByte();
            ShootCounter = reader.ReadByte();
            ShotsFired = reader.ReadByte();
        }*/

        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Aether Flame");

        public override void PostDraw(Color lightColor)
        {
            if (Counter > THRESHHOLD) 
            {
                //Color color = new Color(255 - (80 - cooldown * 3), 119 - (80 - cooldown * 3), 246 - (80 - cooldown * 3));
                Color color = new(255, 119, 246) { A = 0 };
                float scale = ((float)Counter / THRESHHOLD) + (float)Math.Sin(GoldLeafWorld.rottime * 3.5f /* ((float)Counter / THRESHHOLD)*/);

                Texture2D tex = Request<Texture2D>("GoldLeaf/Textures/Masks/Mask1").Value;
                
                Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(0, -6) - Main.screenPosition, null, color * 0.5f, 0, tex.Size()/2, scale/5, SpriteEffects.None, 0f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            /*Texture2D tex = Request<Texture2D>(Texture).Value;

            //Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length-1; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k+1] - Main.screenPosition + tex.Size()/2;
                Main.spriteBatch.Draw(tex, drawPos, null, ColorHelper.AdditiveWhite * (1f - (0.1f * k)), Projectile.rotation, tex.Size() / 2, Projectile.scale - (0.075f * k), SpriteEffects.None, 0f);
            }*/
            return false;
        }

        public override void AI()
        {
            
            /*if (Main.myPlayer == Projectile.owner)
                Projectile.netUpdate = true;*/

            Lighting.AddLight((int)(Projectile.Center.X/16), (int)(Projectile.Center.Y/16), 0.7f, 0.2f, 0.9f);
            Counter++;

            Player player = Main.player[Projectile.owner];
            if (!player.channel)
            {
                Projectile.timeLeft -= 9;
                if (Counter <= THRESHHOLD/2) 
                {
                    Projectile.velocity += Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 0.1f;
                    //if (Projectile.velocity.Length() > 6) Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 6;

                    Projectile.penetrate = 1;
                }
            }
            /*if (Projectile.height <= 18)
            {
                Projectile.width++;
                Projectile.height++;
            }*/
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.5f;

            if (Counter >= 25f && Counter < THRESHHOLD)
            {
                Projectile.velocity *= 0.97f;
            }
            Vector2 vectorToCursor = Projectile.Center - player.Center;
            bool projDirection = Projectile.Center.X < player.Center.X;
            if (Projectile.Center.X < player.Center.X)
            {
                vectorToCursor = -vectorToCursor;
            }
            player.direction = ((!projDirection) ? 1 : (-1));
            player.itemRotation = vectorToCursor.ToRotation();
            player.itemTime = 15;
            player.itemAnimation = 15;

            /*if (counter < THRESHHOLD && counter >= THRESHHOLD/2)
            {
                Projectile.velocity += Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 0.1f;
                if (Projectile.velocity.Length() > 4) Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 4;
            }*/

            if (Counter == THRESHHOLD)
            {
                Projectile.penetrate += 5;

                for (float k = 0; k < 6.28f; k += 0.2f)
                    Dust.NewDustPerfect(Projectile.Center, DustType<AetherDust>(), Vector2.One.RotatedBy(k) * 2, Scale: 0.9f);
            }

            if (Counter >= THRESHHOLD)
            {
                if (Main.rand.NextBool(16) && Main.myPlayer == Projectile.owner) 
                {
                    int ember = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextFloat(6.28f).ToRotationVector2() * Main.rand.NextFloat(2, 3), ProjectileType<AetherEmber>(), 0, 0, Projectile.owner);
                    Main.projectile[ember].scale = Main.rand.NextFloat(0.45f, 0.75f);
                    Main.projectile[ember].timeLeft /= 3;
                }

                Projectile.velocity += Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 0.65f;
                if (Projectile.velocity.Length() > (4f + (Counter * 0.005f))) Projectile.velocity = Vector2.Normalize(Projectile.velocity) * (4f + (Counter * 0.005f));

                ShootCounter++;
                if (ShootCounter >= 16 - (ShotsFired / 3))
                {
                    ShootCounter = 0;
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
                            SoundStyle sound1 = new("GoldLeaf/Sounds/SE/RoR2/WispDeath") { Volume = 0.45f, Pitch = -0.35f + (ShotsFired * 0.035f) };

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
                            Projectile.timeLeft += (int)(11 - (ShotsFired / 3));
                            //Projectile.damage += 1 + cooldown/120;

                            Vector2 perturbedSpeed = value.RotatedByRandom(MathHelper.ToRadians(8f));

                            ShotsFired++;
                            if (Main.myPlayer == Projectile.owner)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, perturbedSpeed.X * 0.3f, perturbedSpeed.Y * 0.3f, ProjectileType<AetherBeam>(), (int)(Projectile.damage * 0.7f), Projectile.knockBack * 0.35f, Projectile.owner, 0f, 0f);
                        }
                    }
                }
            }

            for (int k = 0; k < 4; k++)
            {
                float x = (float)Math.Cos(GoldLeafWorld.rottime + (Counter * 0.03f) + k) * Counter / 64f;
                float y = (float)Math.Sin(GoldLeafWorld.rottime + (Counter * 0.03f) + k) * Counter / 64f;
                Vector2 pos = (new Vector2(x, y)).RotatedBy(k / 12f * 6.28f);

                Dust d = Dust.NewDustPerfect(Projectile.Center, DustType<AetherDust>(), pos * (0.3f + Counter * 0.0015f), 0, default, 1f + (Counter * 0.0015f));
                d.velocity += new Vector2(0, Counter * -0.0125f);
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Counter >= THRESHHOLD / 2) return false; else return true;
        }

	    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            if (!player.channel && Counter <= (THRESHHOLD/2)) target.immune[Projectile.owner] = 0; else target.immune[Projectile.owner] = 15;

            target.AddBuff(BuffType<AetherFlameBuff>(), Main.rand.Next(Helper.TimeToTicks(1f), Helper.TimeToTicks(1.75f)));

            /*target.buffImmune[BuffID.OnFire] = false;
            target.buffImmune[BuffID.Frostburn] = false;
            target.buffImmune[BuffID.CursedInferno] = false;
            target.buffImmune[BuffID.Ichor] = false;
            target.buffImmune[BuffID.ShadowFlame] = false;*/
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            //Texture2D tex = Request<Texture2D>("GoldLeaf/Textures/RingGlow0").Value;

            SoundStyle sound1 = new("GoldLeaf/Sounds/SE/RoR2/EngineerMine") { Volume = 0.7f };
            SoundStyle sound2 = new("GoldLeaf/Sounds/SE/RoR2/Aftershock") { Pitch = 1.6f, Volume = 0.7f };

            SoundEngine.PlaySound(sound1, player.Center);
            SoundEngine.PlaySound(sound2, player.Center);
            Helper.AddScreenshake(Main.LocalPlayer, 18 + ShotsFired, Projectile.Center);

            if (Main.myPlayer == Projectile.owner) 
            {
                int explosion = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<AetherBurst>(), Projectile.damage + (int)(ShotsFired * 2.2), Projectile.knockBack, player.whoAmI);
                if (Counter >= THRESHHOLD) Main.projectile[explosion].ai[0] = 110f; else { Main.projectile[explosion].ai[0] = 30f + (Counter * 0.65f); Main.projectile[explosion].ai[1] = 12; }
            }
        }
    }
    
    public class AetherBurst : ModProjectile
    {
        public override string Texture => Helper.EmptyTexString;

        public float Radius => Helper.BezierEase(1 - (Projectile.timeLeft / 24f)) * Projectile.ai[0]; 
        
        public float VFXAlpha => Radius / Projectile.ai[0];

        int counter = 0;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 24;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ArmorPenetration = 4;
            Projectile.ai[1] = 20;

            //Projectile.extraUpdates = 1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;

            Projectile.DamageType = DamageClass.Magic;
        }

        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Aether Burst");

        public override void OnSpawn(IEntitySource source)
        {
            for (int j = 0; j < 10 + (Projectile.ai[0]/ 15); j++)
            {
                var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustType<AetherSmoke>());
                dust.velocity = Main.rand.NextVector2Circular(7.5f, 7.5f) * Projectile.ai[0] / 65;
                dust.scale = Main.rand.NextFloat(0.9f, 1.2f);
                dust.alpha = 20 + Main.rand.Next(60);
                dust.rotation = Main.rand.NextFloat(6.28f);
            }

            for (int i = 0; i < 4 + (Projectile.ai[0]/40); i++)
            {
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextFloat(6.28f).ToRotationVector2() * Main.rand.NextFloat(2, 3), ProjectileType<AetherEmber>(), 0, 0, Projectile.owner).scale = Main.rand.NextFloat(0.75f, 1.25f);
            }

            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.TrueExcalibur,
                new ParticleOrchestraSettings { PositionInWorld = Projectile.Center },
                Projectile.owner);
        }

        public override void AI()
        {
            Lighting.AddLight((int)(Projectile.position.X / 16), (int)(Projectile.position.Y / 16), 1.4f, 0.4f, 1.8f);
            //Main.NewText(Radius / Projectile.ai[0]);

            counter++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (/*(Projectile.ai[2] != 0 && counter < 10) ||*/ counter > 20)
                return false;

            return Helper.CheckCircularCollision(Projectile.Center, (int)Radius + 30, targetHitbox);
        }


        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = Request<Texture2D>("GoldLeaf/Textures/Flares/wavering").Value;
            Color color = new(255, 119, 246) { A = 0 };
            //Color color = new Color(196, 43, 255) * (1.2f - (counter * 0.05f));

            //Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, color, 0, tex.Size() / 2, VFXAlpha * Projectile.ai[0], 0, 0);
            
            Main.spriteBatch.Draw
            (
                tex,
                new Vector2
                (
                    Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f,
                    Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f
                ),
                new Rectangle(0, 0, tex.Width, tex.Height),
                color * (1f - (Radius / Projectile.ai[0])),
                0f,
                tex.Size() * 0.5f,
                Radius / Projectile.ai[0], 
                SpriteEffects.None,
                0f
            );
            
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Math.Sign(target.Center.X - Projectile.Center.X);
            modifiers.Knockback.Base += 8f * target.knockBackResist;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit || Projectile.ai[2] != 0)
                target.AddBuff(BuffType<AetherFlameBuff>(), Main.rand.Next(Helper.TimeToTicks(3), Helper.TimeToTicks(4f)));

            //target.velocity += Vector2.Normalize(target.Center - Projectile.Center) * (12 + (damageDone * 0.05f)) * target.knockBackResist;

            //target.immune[Projectile.owner] = 6;
        }
    }

    public class AetherBeam : ModProjectile
    {
        public override string Texture => "GoldLeaf/Effects/Dusts/AetherDust";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
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
            Texture2D tex = Request<Texture2D>("GoldLeaf/Effects/Dusts/AetherDust").Value;
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

            if (hit.Crit)
                target.AddBuff(BuffType<AetherFlameBuff>(), Main.rand.Next(Helper.TimeToTicks(2f), Helper.TimeToTicks(3f)));

            /*target.buffImmune[BuffID.OnFire] = false;
            target.buffImmune[BuffID.Frostburn] = false;
            target.buffImmune[BuffID.CursedInferno] = false;
            target.buffImmune[BuffID.Ichor] = false;
            target.buffImmune[BuffID.ShadowFlame] = false;*/
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
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            Projectile.scale *= 0.98f;

            if (Main.rand.NextBool(2))
            {
                var dust = Dust.NewDustPerfect(Projectile.Center, DustType<AetherSmokeFast>(), Main.rand.NextVector2Circular(1.5f, 1.5f));
                dust.scale = 0.5f * Projectile.scale;
                dust.rotation = Main.rand.NextFloatDirection();
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[0] < 15)
                return false;
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.damage--;
            target.AddBuff(BuffType<AetherFlameBuff>(), Main.rand.Next(Helper.TimeToTicks(1.5f), Helper.TimeToTicks(2.5f)));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage--;
            target.AddBuff(BuffType<AetherFlameBuff>(), Main.rand.Next(Helper.TimeToTicks(1.5f), Helper.TimeToTicks(2.5f)));
        }
    }

    public class AetherFlameBuff : ModBuff
    {
        public override string Texture => Helper.CoolBuffTex(base.Texture);

        public override void SetStaticDefaults()
        {
            BuffID.Sets.LongerExpertDebuff[Type] = false;
            BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;

            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (Main.rand.NextBool(3) && !player.dead)
            {
                Dust.NewDust(player.position, player.width, player.height, DustType<AetherDust>(), Main.rand.NextFloat(-0.05f, 0.05f), Main.rand.NextFloat(-1.25f, -0.9f), 0, Color.White, Main.rand.NextFloat(0.6f, 0.85f));
            }

            if (Main.rand.NextBool(10) && !player.dead)
            {
                Dust.NewDust(player.position, player.width, player.height, DustType<AetherSmoke>(), Main.rand.NextFloat(-0.15f, 0.15f), Main.rand.NextFloat(-0.85f, -0.4f), 0, Color.White * Main.rand.NextFloat(0.4f, 0.55f), Main.rand.NextFloat(0.5f, 0.65f));
            }
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(4)) 
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustType<AetherDust>(), Main.rand.NextFloat(-0.05f, 0.05f), Main.rand.NextFloat(-1.25f, -0.9f), 0, Color.White, Main.rand.NextFloat(0.6f, 1.2f));
            }

            if (Main.rand.NextBool(10))
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustType<AetherSmoke>(), Main.rand.NextFloat(-0.15f, 0.15f), Main.rand.NextFloat(-0.85f, -0.4f), 0, Color.White * Main.rand.NextFloat(0.4f, 0.55f), Main.rand.NextFloat(0.5f, 0.65f));
            }
        }
    }

    public class AetherFlameNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int aetherFlareUpTime;
        
        public override void PostAI(NPC npc)
        {
            if (aetherFlareUpTime > 0)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustType<AetherDust>(), Main.rand.NextFloat(-0.125f, 0.125f), Main.rand.NextFloat(-1.25f, -0.9f), 0, Color.White, Main.rand.NextFloat(0.6f, 1.2f));
                
                if (Main.rand.NextBool())
                    Dust.NewDust(npc.position, npc.width, npc.height, DustType<AetherSmoke>(), Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-1.25f, -0.6f), 0, Color.White * Main.rand.NextFloat(0.2f, 0.35f), Main.rand.NextFloat(0.5f, 0.65f));
            }
            aetherFlareUpTime = Math.Clamp(aetherFlareUpTime -1, 0, 15);
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.HasBuff(BuffType<AetherFlameBuff>()))
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                npc.lifeRegen -= 2 * 9;
                if (damage < 3)
                {
                    damage = 3;
                }
            }
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            AetherFlareUp(npc);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.type != ProjectileType<AetherEmber>())
                AetherFlareUp(npc);
        }

        private void AetherFlareUp(NPC npc) 
        {
            if (npc.HasBuff(BuffType<AetherFlameBuff>()) && aetherFlareUpTime <= 0)
            {
                npc.AddBuff(BuffType<AetherFlameBuff>(), Math.Clamp((int)(npc.buffTime[npc.FindBuffIndex(BuffType<AetherFlameBuff>())] * 1.2f), 15, Helper.TimeToTicks(7.5f)));

                for (int i = 0; i < 4; i++)
                {
                    //if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectileDirect(npc.GetSource_Buff(npc.FindBuffIndex(BuffType<AetherFlameBuff>())), npc.Center, Main.rand.NextFloat(6.28f).ToRotationVector2() * Main.rand.NextFloat(npc.scale * 2.6f, npc.scale * 4.2f), ProjectileType<AetherEmber>(), 3, 0, -1).scale = Main.rand.NextFloat(0.5f, 0.65f);
                }
                SoundEngine.PlaySound(SoundID.NPCHit52, npc.Center);
                SoundEngine.PlaySound(SoundID.Item73 with { Volume = 0.6f, Pitch = 0.3f, PitchVariance = 0.4f }, npc.Center);
                aetherFlareUpTime = 15;
            }
        }
    }

    public class AetherFlamePlayer : ModPlayer
    {
        public int aetherFlareUpTime;

        public override void PostUpdateBuffs()
        {
            if (aetherFlareUpTime > 0)
            {
                Dust.NewDust(Player.position, Player.width, Player.height, DustType<AetherDust>(), Main.rand.NextFloat(-0.125f, 0.125f), Main.rand.NextFloat(-1.25f, -0.9f), 0, Color.White, Main.rand.NextFloat(0.6f, 1.2f));
                
                if (Main.rand.NextBool())
                    Dust.NewDust(Player.position, Player.width, Player.height, DustType<AetherSmoke>(), Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-1.25f, -0.6f), 0, Color.White * Main.rand.NextFloat(0.175f, 0.25f), Main.rand.NextFloat(0.5f, 0.65f));
            }
            aetherFlareUpTime = Math.Clamp(aetherFlareUpTime - 1, 0, 15);
        }

        public override void UpdateBadLifeRegen()
        {
            if (Player.HasBuff(BuffType<AetherFlameBuff>()))
            {
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegenTime = 0f;
                Player.lifeRegen -= 2 * 9;
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            AetherFlareUp(Player);
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            AetherFlareUp(Player);
        }

        private void AetherFlareUp(Player player)
        {
            if (player.HasBuff(BuffType<AetherFlameBuff>()) && aetherFlareUpTime <= 0 )
            {
                player.AddBuff(BuffType<AetherFlameBuff>(), Math.Clamp((int)(60 + player.buffTime[Player.FindBuffIndex(BuffType<AetherFlameBuff>())] * 2f), 120, Helper.TimeToTicks(30)));

                for (int i = 0; i < 4; i++)
                {
                    //if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectileDirect(player.GetSource_Buff(Player.FindBuffIndex(BuffType<AetherFlameBuff>())), Player.Center, Main.rand.NextFloat(6.28f).ToRotationVector2() * Main.rand.NextFloat(2.6f, 4.2f), ProjectileType<AetherEmber>(), 0, 0, -1).scale = Main.rand.NextFloat(0.5f, 0.65f);
                }
                SoundEngine.PlaySound(SoundID.NPCHit52, player.Center);
                SoundEngine.PlaySound(SoundID.Item73 with { Volume = 0.6f, Pitch = 0.3f, PitchVariance = 0.4f }, Player.Center);
                aetherFlareUpTime = 15;
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (Player.HasBuff(BuffType<AetherFlameBuff>()))
            {
                genDust = false;
                playSound = false;
                //damageSource = PlayerDeathReason.ByCustomReason(Player.name + " " + Language.GetTextValue("Mods.GoldLeaf.DeathReasons.ToxicPositivity" + Main.rand.Next(3)));
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if ((Player.HasBuff(BuffType<AetherFlameBuff>())))
            {
                int explosion = Projectile.NewProjectile(Player.GetSource_Death(), Player.MountedCenter, Vector2.Zero, ProjectileType<AetherBurst>(), 0, 0, Main.myPlayer);
                Main.projectile[explosion].ai[0] = 60f;
                Helper.AddScreenshake(Player, 18);

                SoundEngine.PlaySound(SoundID.Item74);
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/RoR2/EngineerMine") { Volume = 0.4f, Pitch = -0.5f });
            }
        }
    }
}
