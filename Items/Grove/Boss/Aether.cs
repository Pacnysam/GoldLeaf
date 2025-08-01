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
using System.Diagnostics.Metrics;
using GoldLeaf.Core.CrossMod;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;

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
            ItemSets.Glowmask[Type] = (glowTex, Color.White, false);
            Item.AddElements([Element.Fire, Element.Arcane, Element.Holy]);
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
            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite(), rotation, TextureAssets.Item[Item.type].Size() / 2, scale, SpriteEffects.None, 0f);
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

            Projectile.AddElements([Element.Fire, Element.Arcane, Element.Holy]);
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 4;
            Projectile.ArmorPenetration = 4;
            Projectile.timeLeft = 190;
            Projectile.ignoreWater = false;

            Projectile.netImportant = true;

            Projectile.DamageType = DamageClass.Magic;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.timeLeft);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.timeLeft = reader.ReadInt32();
        }

        public override void PostDraw(Color lightColor)
        {
            if (Counter > THRESHHOLD) 
            {
                //Color color = new Color(255 - (80 - cooldown * 3), 119 - (80 - cooldown * 3), 246 - (80 - cooldown * 3));
                Color color = new(255, 119, 246) { A = 0 };
                float scale = ((float)Counter / THRESHHOLD) + (float)Math.Sin(GoldLeafWorld.rottime * 3.5f /* ((float)Counter / THRESHHOLD)*/);

                Texture2D tex = Request<Texture2D>("GoldLeaf/Textures/Masks/Mask1").Value;
                
                Main.EntitySpriteDraw(tex, Projectile.Center + new Vector2(0, -6) - Main.screenPosition, null, color * 0.5f, 0, tex.Size()/2, scale/5f, SpriteEffects.None, 0f);
            }
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            /*Texture2D tex = Request<Texture2D>(Texture).Value;

            //Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length-1; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k+1] - Main.screenPosition + tex.Size()/2;
                Main.spriteBatch.Draw(tex, drawPos, null, ColorHelper.AdditiveWhite() * (1f - (0.1f * k)), Projectile.rotation, tex.Size() / 2, Projectile.scale - (0.075f * k), SpriteEffects.None, 0f);
            }*/
            return false;
        }
        
        public override void AI()
        {
            if (!Main.dedServ)
                Lighting.AddLight((int)(Projectile.Center.X/16), (int)(Projectile.Center.Y/16), 0.7f, 0.2f, 0.9f);

            Counter++;

            Player player = Main.player[Projectile.owner];
            if (!player.channel)
            {
                Projectile.timeLeft -= 9;
                if (Counter <= THRESHHOLD/2 && Main.myPlayer == Projectile.owner) 
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

            Projectile.netUpdate = true;

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
                //for (float k = 0; k < 6.28f; k += 0.2f) Dust.NewDustPerfect(Projectile.position, DustType<AetherDust>(), Vector2.One.RotatedBy(k) * 2, Scale: 0.9f);
            }

            if (Counter >= THRESHHOLD)
            {
                /*if (Main.rand.NextBool(16) && Main.myPlayer == Projectile.owner) 
                {
                    int ember = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextFloat(6.28f).ToRotationVector2() * Main.rand.NextFloat(2, 3), ProjectileType<AetherEmber>(), 0, 0, Projectile.owner);
                    Main.projectile[ember].scale = Main.rand.NextFloat(0.45f, 0.75f);
                    Main.projectile[ember].timeLeft /= 3;
                }*/

                Projectile.velocity += Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 0.65f;
                if (Projectile.velocity.Length() > (4f + (Counter * 0.0135f))) Projectile.velocity = Vector2.Normalize(Projectile.velocity) * (4f + (Counter * 0.0135f));

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
                            //SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/AetherBeam"), player.Center);
                            SoundEngine.PlaySound(new("GoldLeaf/Sounds/SE/RoR2/WispDeath") { Volume = 0.45f, Pitch = -0.35f + (ShotsFired * 0.035f) }, Projectile.Center);
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
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, perturbedSpeed.X * 0.35f, perturbedSpeed.Y * 0.35f, ProjectileType<AetherBeam>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack * 0.35f, Projectile.owner, 0f, 0f);
                        }
                    }
                }
            }

            for (int k = 0; k < 4; k++)
            {
                float x = (float)Math.Cos(GoldLeafWorld.rottime + (Counter * 0.03f) + k) * Counter / 64f;
                float y = (float)Math.Sin(GoldLeafWorld.rottime + (Counter * 0.03f) + k) * Counter / 64f;
                Vector2 pos = (new Vector2(x, y)).RotatedBy(k / 12f * 6.28f);

                Dust d = Dust.NewDustPerfect(Projectile.Center, DustType<AetherDust>(), pos * (0.3f + Counter * 0.00135f), 0, default, 1f + (Counter * 0.00135f));
                d.position += d.velocity * 2f;
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

            //target.AddBuff(BuffType<AetherFlameBuff>(), Helper.TimeToTicks(1f));
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            
            SoundEngine.PlaySound(new("GoldLeaf/Sounds/SE/RoR2/EngineerMine") { Volume = 0.7f }, Projectile.Center);
            SoundEngine.PlaySound(new("GoldLeaf/Sounds/SE/RoR2/Aftershock") { Pitch = 1.6f, Volume = 0.7f }, Projectile.Center);

            //CameraSystem.AddScreenshake(Main.LocalPlayer, 18 + ShotsFired, Projectile.Center);
            CameraSystem.QuickScreenShake(Projectile.Center, null, 14.5f + (ShotsFired * 0.25f), 5.5f + (ShotsFired * 0.085f), 26 + (int)(ShotsFired * 1.15f), 1500);
            CameraSystem.QuickScreenShake(Projectile.Center, 0f.ToRotationVector2(), 14.5f + (ShotsFired * 0.25f), 10f + (ShotsFired * 0.115f), 18 + (int)(ShotsFired * 0.85f), 1500);
            
            float explosionVolume = (Counter >= THRESHHOLD) ? 110f : 30f + (Counter * 0.65f);

            for (int j = 0; j < 10 + (explosionVolume / 8f); j++)
            {
                var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustType<AetherSmoke>());
                dust.velocity = Main.rand.NextVector2Circular(6.75f, 6.75f) * Math.Clamp(explosionVolume / 85f, 1f, 2f);
                dust.scale = Main.rand.NextFloat(0.9f, 1.8f);
                dust.alpha = 40 + Main.rand.Next(60);
                dust.rotation = Main.rand.NextFloat(6.28f);
            }

            if (Main.myPlayer == Projectile.owner) 
            {
                Projectile explosion = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<AetherBurst>(), (int)(Projectile.damage * 2f) + (int)(ShotsFired * 2.2), Projectile.knockBack, player.whoAmI);
                explosion.ai[0] = explosionVolume;
                if (Counter < THRESHHOLD) explosion.ai[1] = 12;
                if (ShotsFired >= 20) explosion.ai[2] = 1;
            }
        }
    }
    
    public class AetherBurst : ModProjectile
    {
        private static Asset<Texture2D> ringTex;
        private static Asset<Texture2D> coronaTex;
        private static Asset<Texture2D> flareTex;
        public override void Load()
        {
            ringTex = Request<Texture2D>("GoldLeaf/Textures/Flares/wavering");
            coronaTex = Request<Texture2D>("GoldLeaf/Textures/Flares/aura");
            flareTex = Request<Texture2D>("GoldLeaf/Textures/Flares/Flare4");
        }
        public override string Texture => Helper.EmptyTexString;

        public float Radius => Helper.BezierEase(1 - (Projectile.timeLeft / 24f)) * Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            Projectile.AddElements([Element.Fire, Element.Arcane, Element.Holy, Element.Explosive]);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 24;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ArmorPenetration = 4;
            Projectile.ai[1] = 20;

            Projectile.extraUpdates = 1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;

            Projectile.DamageType = DamageClass.Magic;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 4 + (Projectile.ai[0]/40); i++)
            {
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextFloat(6.28f).ToRotationVector2() * Main.rand.NextFloat(2, 3), ProjectileType<AetherEmber>(), 0, 0, Projectile.owner).scale = Main.rand.NextFloat(0.75f, 1.25f);
            }

            /*ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.TrueExcalibur,
                new ParticleOrchestraSettings { PositionInWorld = Projectile.Center },
                Projectile.owner);*/
        }

        public override bool PreAI()
        {
            Projectile.extraUpdates = (Projectile.Counter() <= 8) ? 1 : 0;
            return true;
        }

        public override void AI()
        {
            Projectile.netUpdate = true;

            if (!Main.dedServ)
                Lighting.AddLight((int)(Projectile.position.X / 16), (int)(Projectile.position.Y / 16), 1.4f, 0.4f, 1.8f);
            //Main.NewText(Radius / Projectile.ai[0]);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (/*(Projectile.ai[2] != 0 && counter < 10) ||*/ Projectile.Counter() > 20)
                return false;

            return Helper.CheckCircularCollision(Projectile.Center, (int)Radius + 30, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw //Wave
            (
                ringTex.Value,
                new Vector2
                (
                    Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f,
                    Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f
                ),
                null,
                new Color(255, 119, 246) { A = 0 } * (1f - (Radius / Projectile.ai[0])),
                0f,
                ringTex.Size()/2f,
                1.075f * ((Radius + 30)/110f),
                //Projectile.scale * 0.1f + (((Radius * Projectile.ai[0]) + 30)/5500f),
                SpriteEffects.None,
                0f
            );

            Main.EntitySpriteDraw //Light Ring
            (
                coronaTex.Value,
                new Vector2
                (
                    Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f,
                    Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f
                ),
                null,
                new Color(228, 139, 245) { A = 0 } * (1f - (Radius / Projectile.ai[0])) * 0.65f,
                0f,
                coronaTex.Size() / 2f,
                2f * ((Radius + 30) / 90f),
                //Projectile.scale * 0.1f + (((Radius * Projectile.ai[0]) + 30)/5500f),
                SpriteEffects.None,
                0f
            );

            if (Projectile.ai[0] > 80 || Projectile.ai[2] != 0) 
            {
                Main.EntitySpriteDraw //Flare
                (
                    flareTex.Value,
                    new Vector2
                    (
                       Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f,
                        Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f
                    ),
                    null,
                    new Color(255, 231, 206) { A = 0 } * (1f - (Radius / (Projectile.ai[0] * 1.15f))) * 0.45f,
                    0f,
                    flareTex.Size() / 2f,
                    2.25f - (1f * ((Radius + 30) / 110f)),
                    //Projectile.scale * 0.1f + (((Radius * Projectile.ai[0]) + 30)/5500f),
                    SpriteEffects.None,
                    0f
                );
            }

            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Math.Sign(target.Center.X - Projectile.Center.X);
            modifiers.Knockback.Base += 8f * target.knockBackResist;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (/*hit.Crit || */Projectile.ai[2] != 0)
                target.AddBuff(BuffType<AetherFlameBuff>(), Helper.TimeToTicks(2.5f));

            //target.velocity += Vector2.Normalize(target.Center - Projectile.Center) * (12 + (damageDone * 0.05f)) * target.knockBackResist;

            //target.immune[Projectile.owner] = 6;
        }

        /*public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.rand.NextBool(10) || Projectile.ai[2] != 0)
                target.AddBuff(BuffType<AetherFlameBuff>(), Helper.TimeToTicks(3.5f), false);
        }*/
    }
    
    public class AetherBeam : ModProjectile
    {
        public override string Texture => "GoldLeaf/Textures/Shine";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

            Projectile.AddElements([Element.Fire, Element.Arcane, Element.Holy]);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            //Projectile.penetrate = 1;
            Projectile.ArmorPenetration = 4;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = 160;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.DamageType = DamageClass.Magic;
        }
        
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity *= 0.985f;

            Projectile.alpha = (int)Math.Clamp((160 - Projectile.timeLeft) * 2.25f, 0, 255);
            Projectile.scale = Math.Clamp(Projectile.timeLeft * 0.1f, 0.25f, 1f);
            
            /*for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<AetherDust>(), Projectile.velocity * 0.4f, 0, Color.White, 0.6f);
            }*/
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            //Vector2 drawOrigin = new(tex.Width * 0.5f, 24f);

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(255, 119, 246) { A = 0 } * Projectile.Opacity, Projectile.rotation, tex.Size()/2f, new Vector2(1f, Math.Clamp(Projectile.velocity.Length() * 0.75f, 0.35f, 4f) * 0.5f), SpriteEffects.None);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Main.EntitySpriteDraw(tex, Projectile.oldPos[k] + Projectile.Size / 2f - Main.screenPosition, null, new Color(255, 119, 246) { A = 0 } * (Projectile.Opacity - (0.3f * k)), Projectile.rotation, tex.Size() / 2f, new Vector2(1f, Math.Clamp(Projectile.velocity.Length() * 0.75f, 0.35f, 4f) * 0.5f), SpriteEffects.None);
                //Main.spriteBatch.Draw(tex, drawPos, null, Color.White * (1.0f - (0.05f * k)), Projectile.rotation + MathHelper.PiOver2, drawOrigin, Projectile.scale * 1.2f - (0.06f * k), SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 3; i++)
            {
                var dust = Dust.NewDustPerfect(Projectile.Center, DustType<AetherSmoke>(), Projectile.velocity * 0.1f + Main.rand.NextVector2Circular(1.5f, 1.5f));
                dust.scale = 0.45f * Projectile.scale;
                dust.rotation = Main.rand.NextFloatDirection();
                dust.alpha += Main.rand.Next(70, 90);
            }

            /*if (hit.Crit)
                target.AddBuff(BuffType<AetherFlameBuff>(), Main.rand.Next(Helper.TimeToTicks(2f), Helper.TimeToTicks(3f)));*/
        }
    }
    
    public class AetherEmber : ModProjectile
    {
        public override string Texture => "GoldLeaf/Effects/Dusts/SpecialSmokeDust";

        public override void SetStaticDefaults()
        {
            Projectile.AddElements([Element.Fire, Element.Arcane, Element.Holy]);
        }

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
        /*
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.damage--;
            target.AddBuff(BuffType<AetherFlameBuff>(), Main.rand.Next(Helper.TimeToTicks(1.5f), Helper.TimeToTicks(2.5f)));
        }*/

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage--;
            //target.AddBuff(BuffType<AetherFlameBuff>(), Helper.TimeToTicks(1f));
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
            if (Main.rand.NextBool(8) && !player.dead)
            {
                Dust smoke = Dust.NewDustDirect(player.position, player.width, player.height, DustType<AetherSmoke>(), 0f, Main.rand.NextFloat(-0.85f, -0.4f), 0, Color.White * Main.rand.NextFloat(0.2f, 0.3f), Main.rand.NextFloat(0.5f, 0.65f));
                smoke.velocity.X *= 0.35f;
                smoke.alpha = Main.rand.Next(60, 90);

                if (smoke.velocity.Y > 0)
                    smoke.velocity.Y *= -1;
            }

            if (player.HasBuff(BuffID.OnFire))
                player.ClearBuff(BuffID.OnFire);
            if (player.HasBuff(BuffID.OnFire3))
                player.ClearBuff(BuffID.OnFire3);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(8))
            {
                Dust smoke = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustType<AetherSmoke>(), 0f, Main.rand.NextFloat(-0.85f, -0.4f), 0, Color.White * Main.rand.NextFloat(0.2f, 0.3f), Main.rand.NextFloat(0.5f, 0.65f));
                smoke.velocity.X *= 0.35f;
                smoke.alpha = Main.rand.Next(60, 90);

                if (smoke.velocity.Y > 0)
                    smoke.velocity.Y *= -1;
            }

            if (npc.HasBuff(BuffID.OnFire))
                npc.DelBuff(npc.FindBuffIndex(BuffID.OnFire));
            if (npc.HasBuff(BuffID.OnFire3))
                npc.DelBuff(npc.FindBuffIndex(BuffID.OnFire3));
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
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustType<AetherSparkDust>(), Main.rand.NextFloat(-0.125f, 0.125f), Main.rand.NextFloat(-1.25f, -0.9f), 0, Color.White, Main.rand.NextFloat(0.75f, 1.2f));
                dust.velocity.X *= 0.25f;

                if (dust.velocity.Y > 0)
                    dust.velocity.Y *= -1f;

                if (Main.rand.NextBool(3))
                { 
                    Dust smoke = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustType<AetherSmoke>(), Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-1.25f, -0.6f), 0, Color.White * Main.rand.NextFloat(0.125f, 0.2f), Main.rand.NextFloat(0.5f, 0.65f));
                    smoke.alpha = Main.rand.Next(60, 90);
                }
            }
            else if (npc.HasBuff<AetherFlameBuff>() && Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustType<AetherDust>(), 0f, Main.rand.NextFloat(-1.25f, -0.9f), 0, Color.White, Main.rand.NextFloat(0.6f, 1.2f));
                dust.velocity.X *= 0.25f;

                if (dust.velocity.Y > 0)
                    dust.velocity.Y *= -1;
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

                for (int i = 0; i < 3; i++)
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
                Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustType<AetherSparkDust>(), Main.rand.NextFloat(-0.125f, 0.125f), Main.rand.NextFloat(-1.25f, -0.9f), 0, Color.White, Main.rand.NextFloat(0.75f, 1.2f));
                dust.velocity.X *= 0.25f;

                if (dust.velocity.Y > 0)
                    dust.velocity.Y *= -1f;

                if (Main.rand.NextBool(3))
                {
                    Dust smoke = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustType<AetherSmoke>(), Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-1.25f, -0.6f), 0, Color.White * Main.rand.NextFloat(0.175f, 0.25f), Main.rand.NextFloat(0.5f, 0.65f));
                    smoke.alpha = Main.rand.Next(60, 90);
                }
            }
            else if (Player.HasBuff<AetherFlameBuff>() && Main.rand.NextBool(3) && !Player.dead)
            {
                Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustType<AetherDust>(), 0f, Main.rand.NextFloat(-1.25f, -0.9f), 0, Color.White, Main.rand.NextFloat(0.6f, 0.85f));
                dust.velocity.X *= 0.25f;

                if (dust.velocity.Y > 0)
                    dust.velocity.Y *= -1f;
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
                for (int j = 0; j < 15; j++)
                {
                    var dust = Dust.NewDustDirect(Player.MountedCenter, 0, 0, DustType<AetherSmoke>());
                    dust.velocity = Main.rand.NextVector2Circular(9f, 9f);
                    dust.scale = Main.rand.NextFloat(0.9f, 1.2f);
                    dust.alpha = 20 + Main.rand.Next(60);
                    dust.rotation = Main.rand.NextFloat(6.28f);
                }

                int explosion = Projectile.NewProjectile(Player.GetSource_Death(), Player.MountedCenter, Vector2.Zero, ProjectileType<AetherBurst>(), 0, 0, Main.myPlayer);
                Main.projectile[explosion].ai[0] = 60f;

                CameraSystem.AddScreenshake(Player, 18);

                SoundEngine.PlaySound(SoundID.Item74);
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/RoR2/EngineerMine") { Volume = 0.4f, Pitch = -0.5f });
            }
        }
    }
}
