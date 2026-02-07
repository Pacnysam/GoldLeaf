using GoldLeaf.Core;
using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Grove.Boss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using static GoldLeaf.Core.ColorHelper;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Blizzard
{
	public class ArcticFlower : ModItem
	{
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;

            Item.AddElements([Element.Ice, Element.Arcane, Element.Nature]);
        }

        public override void SetDefaults()
		{
			Item.damage = 37;
			Item.DamageType = DamageClass.Summon;
            Item.shoot = ProjectileType<ArcticWraith>();
            Item.buffType = BuffType<ArcticWraithBuff>();
			Item.width = 30;
            Item.knockBack = 6.5f;
			Item.height = 36;
            Item.noMelee = true;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.UseSound = new SoundStyle("GoldLeaf/Sounds/SE/Monolith/GhostWhistle") { Volume = 0.85f };
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 1, 20, 0);
            Item.rare = ItemRarityID.Green;
			Item.autoReuse = false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);

            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;
            projectile.netUpdate = true;

            for (float k = 0; k < 6.28f; k += Main.rand.NextFloat(0.15f, 0.3f))
            {
                Dust dust = Dust.NewDustPerfect(position + new Vector2(5), DustType<ArcticDust>(), Vector2.One.RotatedBy(k) * Main.rand.NextFloat(1.75f, 2.25f), 0, Color.White);
                dust.noLight = true;
            }

            return false;
        }

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BorealWood, 12);
			recipe.AddRecipeGroup("GoldLeaf:GoldBar", 6);
            recipe.AddIngredient(ItemType<AuroraCluster>(), 14);
            recipe.AddIngredient(ItemID.Shiverthorn, 4);
            recipe.AddOnCraftCallback(RecipeCallbacks.AuroraMajor);
            recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
    }
    
    public class ArcticWraith : ModProjectile
    {
        private static Asset<Texture2D> glowTex;
        private static Asset<Texture2D> trimTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
            trimTex = Request<Texture2D>(Texture + "Trim");
        }
        
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

            Projectile.AddElements([Element.Ice, Element.Arcane, Element.Nature]);
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 44;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1; 
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.manualDirectionChange = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.GetGlobalProjectile<GoldLeafProjectile>().critDamageMod = -0.5f;
        }

        private ref float State => ref Projectile.ai[0];
        private ref float AnimLoops => ref Projectile.ai[1];

        private const int Idle = 0;
        private const int WindUp = 1;
        private const int Recoil = 2;
        private const int Turning = 3;
        //private const int Spawning = 4;

        private static readonly int teleportRange = 1600;
        private static readonly int targetingRange = 800;
        private static readonly float attackingRange = 450;

        private static readonly float speed = 12f;
        private static readonly float inertia = 25f;
        private static readonly float shootSpeed = 6.75f;

        private static readonly int animationSpeed = 6;

        bool animReverse = false;
        bool hasTarget = false;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(animReverse);
            writer.Write(hasTarget);
            writer.Write(Projectile.frame);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            animReverse = reader.ReadBoolean();
            hasTarget = reader.ReadBoolean();
            Projectile.frame = reader.ReadInt32();
        }

        public override bool? CanCutTiles() => false;
        public override bool MinionContactDamage() => false;
        
        private bool NotFrozen(Entity otherEntity, int currentTarget)
        {
            if (otherEntity is not NPC)
                return false;

            return !(otherEntity as NPC).HasBuff(BuffType<FreezeBuff>());
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!MinionCheckBuff(player)) return;

            #region animation
            if (!Main.gamePaused)
                Projectile.frameCounter++;

            if (Projectile.frameCounter >= animationSpeed)
            {
                Projectile.frameCounter = 0;

                if (animReverse)
                    Projectile.frame--;
                else
                    Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type] && State != Turning)
                {
                    AnimLoops++;

                    Projectile.frame = 0;
                }

                if (Projectile.frame > 4 && State == Turning)
                {
                    Projectile.frame--;
                    animReverse = true;
                }
            }
            #endregion animation

            #region variables
            Vector2 idlePosition = player.Center;
            idlePosition.Y -= 24f;
            float minionPositionOffsetX = (40 + Projectile.minionPos * 32) * -player.direction;
            idlePosition.X += minionPositionOffsetX;

            Vector2 vectorToIdlePosition = idlePosition - Projectile.Center;
            float distanceToIdlePosition = vectorToIdlePosition.Length();

            NPC target = null;
            Vector2 targetCenter = Projectile.Center;
            hasTarget = false;
            #endregion variables

            #region teleport if far
            if (Main.myPlayer == player.whoAmI && distanceToIdlePosition > teleportRange)
            {
                Projectile.position = idlePosition - Projectile.Size/2;
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;

                for (float k = 0; k < 6.28f; k += Main.rand.NextFloat(0.15f, 0.3f))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(5), DustType<ArcticDust>(), Vector2.One.RotatedBy(k) * Main.rand.NextFloat(1.75f, 2.25f), 0, Color.White);
                    dust.noLight = true;
                }
            }
            #endregion teleport if far

            #region push
            if (State != Turning)
            {
                foreach (var other in Main.ActiveProjectiles)
                {
                    if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width * 1.5f)
                    {
                        if (Projectile.position.X < other.position.X)
                        {
                            Projectile.velocity.X -= 0.05f;
                        }
                        else
                        {
                            Projectile.velocity.X += 0.05f;
                        }

                        if (Projectile.position.Y < other.position.Y)
                        {
                            Projectile.velocity.Y -= 0.05f;
                        }
                        else
                        {
                            Projectile.velocity.Y += 0.05f;
                        }
                    }
                }
            }
            #endregion push
            
            #region targeting
            int attackTarget = -1;

            Projectile.Minion_FindTargetInRange(targetingRange, ref attackTarget, false, NotFrozen);
            if (attackTarget == -1)
                Projectile.Minion_FindTargetInRange(targetingRange, ref attackTarget, false);
            if (attackTarget != -1)
            {
                target = Main.npc[attackTarget];
                targetCenter = target.Center;
                hasTarget = true;
            }
            #endregion targeting

            #region behavior
            if (hasTarget)
            {
                Projectile.direction = Projectile.spriteDirection = (Projectile.Center.X > targetCenter.X) ? -1 : 1;

                if ((State == Idle || State == Turning) && AnimLoops >= 3 && Projectile.Center.Distance(targetCenter) <= attackingRange) 
                {
                    ChangeState(WindUp);
                } else if (State == WindUp && AnimLoops >= 1)
                {
                    if (hasTarget)
                    {
                        Vector2 enemyVector = targetCenter - Projectile.Center;
                        enemyVector.Normalize();
                        enemyVector *= shootSpeed;

                        Shoot(target, enemyVector);
                    }
                }
                else if (State == Recoil && AnimLoops >= 1)
                { 
                    ChangeState(Idle); 
                }
                else if (State == Turning)
                {
                    if (Projectile.frame >= 3 && State == Turning && !animReverse)
                    {
                        animReverse = true;
                    }
                    else if (Projectile.frame <= 0 && State == Turning && animReverse)
                        ChangeState(Idle);
                }

                /*Vector2 direction = targetCenter - Projectile.Center;
                direction.Normalize();
                direction *= speed;*/

                if (Projectile.Center.Distance(targetCenter) > attackingRange)
                {
                    //Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                    Projectile.velocity -= (Projectile.Center - targetCenter) / 1200;
                }
                else 
                {
                    if (MathHelper.Distance(Projectile.Center.X, targetCenter.X) <= attackingRange - 100)
                    {
                        if (Projectile.Center.X < targetCenter.X)
                        {
                            Projectile.velocity.X -= ((attackingRange - 100) - Math.Abs(Projectile.Center.X - targetCenter.X)) / 700;
                        }
                        else
                        {
                            Projectile.velocity.X += ((attackingRange - 100) - Math.Abs(Projectile.Center.X - targetCenter.X)) / 700;
                        }

                        //Projectile.velocity.X += Math.Abs((Projectile.Center.X - targetCenter.X) / 700);
                        //Projectile.velocity.X = -0.35f * ((Projectile.velocity.X * (inertia - 1) + direction.X) / inertia);
                    }
                    else
                    {
                        Projectile.velocity.X *= 0.9f;
                    }
                    if (MathHelper.Distance(Projectile.Center.Y, targetCenter.Y) <= attackingRange/2)
                    {
                        Projectile.velocity.Y *= 0.925f;
                    }

                    if (Projectile.Center.Distance(targetCenter) <= attackingRange / 2)
                    {
                        Projectile.velocity *= 0.9f;
                    }
                }
            }
            else 
            {
                if (AnimLoops >= 1 && !(State == Idle || State == Turning))
                    ChangeState(Idle);
                else if (Projectile.frame >= 3 && State == Turning && !animReverse)
                {
                    animReverse = true;
                }
                else if (Projectile.frame <= 0 && State == Turning && animReverse)
                    ChangeState(Idle);

                if (State == Idle)
                {
                    Vector2 direction = idlePosition - Projectile.Center;
                    direction.Normalize();
                    direction *= speed;

                    if (Projectile.Center.Distance(idlePosition) <= 30)
                    {
                        Projectile.velocity *= 0.9f;
                        if (Projectile.velocity.Length() < 0.5f)
                            Projectile.velocity = Vector2.Zero;
                        else
                            Projectile.velocity = (Projectile.velocity * (20 - 1) + direction) / 20;

                        /*if (Projectile.direction != player.direction && Counter > TimeToTicks(5))
                        {
                            Projectile.direction = Projectile.spriteDirection = player.direction;
                            ChangeState(Turning);
                        }
                        */
                        //Projectile.position = Vector2.Lerp(Projectile.position, idlePosition - (Projectile.Size / 2), 0.1f);
                    }
                    else 
                    {
                        Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                    }
                }
                if (State == Turning)
                {
                    Projectile.direction = Projectile.spriteDirection = player.direction;
                    //if (Projectile.Center.Distance(player.MountedCenter) <= 100)
                        Projectile.position.X = Vector2.SmoothStep(Projectile.position, idlePosition - (Projectile.Size/2), 0.125f).X;
                }

                Projectile.direction = Projectile.spriteDirection = (Projectile.Center.X > player.Center.X) ? -1 : 1;
            }
            Projectile.position.Y += 0.3f * (float)Math.Sin(GoldLeafWorld.rottime + (Projectile.minionPos * 1.25f));
            Projectile.rotation = Projectile.velocity.X * 0.03f;
            #endregion behavior

            #region misc visuals
            if (!Main.dedServ && Projectile.velocity.Length() >= 4f && Main.rand.NextBool(3) && hasTarget && State != Recoil)
            {
                Vector2 position = (Projectile.velocity.X > 0) ? Projectile.TopLeft : Projectile.position;//Projectile.position + new Vector2((Projectile.velocity.X > 0) ? 12f : -12f, 0);
                Vector2 velocity = Projectile.velocity;
                velocity.Normalize();
                velocity *= Main.rand.NextFloat(-2.25f, -0.85f);

                Dust dust = Dust.NewDustDirect(position + new Vector2(0, 4), Projectile.width, Projectile.height - 8, DustType<ArcticDust>(), Alpha: 60);
                dust.frame = new Rectangle(0, 10 * Main.rand.Next(1, 3), 10, 10);
                dust.velocity = velocity;
            }
            #endregion misc visuals
        }

        public override void PostAI()
        {
            if (hasTarget)
            {
                Projectile.localAI[0] = MathHelper.SmoothStep(Projectile.localAI[0], 1f, 0.125f);
            }
            else
            {
                Projectile.localAI[0] = MathHelper.SmoothStep(Projectile.localAI[0], 0f, 0.15f);
            }
        }

        private void ChangeState(float newState)
        {
            Projectile.frameCounter = 0;
            Projectile.frame = 0;
            AnimLoops = 0;
            animReverse = false;

            Projectile.netUpdate = true;

            State = newState;
        }

        private void Shoot(NPC target, Vector2 velocity) 
        {
            /*for (float n = 0; n <= 6.28f; n += 0.42f)
            {
                Vector2 targetDirection = Vector2.Normalize(Projectile.Center - target.Center);
                Vector2 off = new Vector2((float)Math.Cos(n), (float)Math.Sin(n) * 2) * (7f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + off.RotatedBy(targetDirection.ToRotation()), DustType<ArcticDust>(), targetDirection * -2.5f, 0, Color.White, 1f);
                dust.fadeIn = Main.rand.NextFloat(0.5f, 0.85f);
                dust.rotation = Main.rand.NextFloat(-9, 9);
                dust.noGravity = true;
                dust.frame = new Rectangle(0, Main.rand.Next(3) * 10, 10, 10);
            }*/
            
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity += (Projectile.Center - target.Center) / 30;

                Vector2 position;

                if (target.Center.X > Projectile.Center.X)
                    position = Projectile.Right;
                else
                    position = Projectile.Left;

                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), position, velocity, ProjectileType<ArcticWraithOrb>(), Projectile.damage, Projectile.knockBack, Projectile.owner, target.whoAmI);
            }
            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Kirby/ForgottenLand/StarShot") { Volume = 0.45f, PitchVariance = 0.5f, MaxInstances = 0 }, Projectile.Center);
                SoundEngine.PlaySound(new SoundStyle("Goldleaf/Sounds/SE/SplashBounce") { Volume = 0.3f, Pitch = 0.15f, PitchVariance = 0.25f, MaxInstances = 0 }, Projectile.Center);
            }
            ChangeState(Recoil);
        }

        private bool MinionCheckBuff(Player player) 
        {
            if (player.dead || !player.active)
            {
                player.ClearBuff(BuffType<ArcticWraithBuff>());
                return false;
            }
            if (player.HasBuff(BuffType<ArcticWraithBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var effects = (Projectile.spriteDirection == -1 || animReverse) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 offset = new(0, -1);

            Rectangle rect = new((texture.Width / 4) * (int)State, texture.Height / Main.projFrames[Projectile.type] * Projectile.frame, 
                (texture.Width / 4), texture.Height / Main.projFrames[Projectile.type]);
            
            //trail
            if (Projectile.localAI[0] >= 0.05f)
            {
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    var oldEffects = (Projectile.oldSpriteDirection[k] == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + rect.Size()/2 + new Vector2(0, 2f + (k));
                    Color color1 = new Color(0, 205, 241).Alpha();
                    Color color2 = new Color(0, 38, 128).Alpha();
                    Color finalColor = new Gradient([(color1, 0f), (color2, 1f)]).GetColor(k / (Projectile.oldPos.Length + 2f));

                    Main.EntitySpriteDraw(texture, drawPos, rect, finalColor * (0.8f - (k / (Projectile.oldPos.Length + 4f))) * Projectile.localAI[0], Projectile.oldRot[k], rect.Size() / 2, Projectile.scale * (1.15f - (0.1f * k)), oldEffects, 0f);
                }
            }
            //aura
            float time = Main.GlobalTimeWrappedHourly * 0.15f;
            for (float k = 0f; k < 1f; k += 1 / 3f)
            {
                Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0f, 2f).RotatedBy((k + (time * -1.75f)) * ((float)Math.PI * 2f)) - Main.screenPosition, rect, new Color(97, 235, 251).Alpha() * 0.35f, Projectile.rotation, rect.Size() / 2, Projectile.scale, effects, 0f);
            }
            //minion
            Main.EntitySpriteDraw(texture, (Projectile.Center + offset) - Main.screenPosition, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, rect.Size()/2, Projectile.scale, effects, 0f);
            //metal trim
            float brightness = Lighting.Brightness((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);
            if (brightness <= 0.3f)
                Main.EntitySpriteDraw(trimTex.Value, (Projectile.Center + offset) - Main.screenPosition, rect, Color.White.Alpha() * (0.3f - brightness), Projectile.rotation, rect.Size()/2, Projectile.scale, effects, 0);
            //glow
            Main.EntitySpriteDraw(glowTex.Value, (Projectile.Center + offset) - Main.screenPosition, rect, Color.White, Projectile.rotation, rect.Size()/2, Projectile.scale, effects, 0f);

            return false;
        }
    }
    
    public class ArcticWraithOrb : ModProjectile
    {
        private static Asset<Texture2D> alphaBloom;
        public override void Load()
        {
            alphaBloom = Request<Texture2D>("GoldLeaf/Textures/Glow");
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

            Projectile.AddElements([Element.Ice, Element.Arcane, Element.Nature]);
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = TimeToTicks(15);
            Projectile.extraUpdates = 1;
            Projectile.localAI[0] = 1f;

            Projectile.DamageType = DamageClass.Summon;

            Projectile.GetGlobalProjectile<GoldLeafProjectile>().critDamageMod = -0.5f;
        }

        private static readonly float accelerationSpeed = 0.7f;
        private static readonly float maxSpeed = 7.25f;

        public override bool? CanHitNPC(NPC target)
        {
            if (HasTarget && IsTargetValid(target))
                return target == Target;

            return !target.friendly;
        }

        bool HasTarget => (Target.CanBeChasedBy(Projectile) && Projectile.Center.Distance(Target.Center) <= 550) && Counter <= TimeToTicks(10);
        NPC Target => Main.npc[(int)Projectile.ai[0]];
        private ref int Counter => ref Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter;

        public override void AI()
        {
            Projectile.Opacity = MathHelper.SmoothStep(Projectile.Opacity, 0, 0.125f);

            Projectile.tileCollide = !HasTarget;

            if (HasTarget && IsTargetValid(Target))
            {
                Projectile.velocity += Vector2.Normalize(Target.Center - Projectile.Center) * accelerationSpeed;
            }

            if (Projectile.velocity.Length() > maxSpeed) 
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * maxSpeed;

            if (++Projectile.frameCounter >= 12)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }

            if (!Main.dedServ)
            {
                Lighting.AddLight((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16), (0 / 255f) * 0.6f, (164 / 255f) * 0.6f, (242 / 255f) * 0.6f);
                
                if (Main.rand.NextBool(4))
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustType<ArcticDust>());
                    dust.velocity = Projectile.velocity * 0.35f;
                    dust.frame = new Rectangle(0, 10 * Main.rand.Next(0, 2), 10, 10);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.localAI[0] = MathHelper.SmoothStep(Projectile.localAI[0], 0, 0.15f);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D flare = Request<Texture2D>("GoldLeaf/Textures/Flares/FlareSmall").Value;
            Rectangle rect = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            
            //trail
            for (int k = Projectile.oldPos.Length - 1; k >= 0; k--)
            {
                Vector2 drawOrigin = new(rect.Width * 0.5f, rect.Height * 0.5f);
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                
                Color color1 = Color.CornflowerBlue.Alpha(145);
                Color color2 = new Color(0, 38, 148).Alpha(145);
                Color finalColor = new Gradient([(color1, 0f), (color2, 1f)]).GetColor(k / (Projectile.oldPos.Length + 2f));

                Main.EntitySpriteDraw(texture, drawPos, rect, finalColor * (0.85f - (k / 10f)), Projectile.rotation, drawOrigin, Projectile.scale * (1f - (0.065f * k)), SpriteEffects.None);
            }
            //glow
            Main.EntitySpriteDraw(alphaBloom.Value, Projectile.Center - Main.screenPosition, null, Color.RoyalBlue.Alpha(180) * MathHelper.Clamp(Projectile.Opacity - 0.45f, 0.4f, 0.85f), Projectile.rotation, alphaBloom.Size() / 2, Projectile.scale * MathHelper.Clamp(Projectile.localAI[0] * 1.5f, 0.65f, 0.95f) * 1.05f, SpriteEffects.None);
            //projectile
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, Color.White, Projectile.rotation, rect.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            //flare
            Main.EntitySpriteDraw(flare, Projectile.Center - Main.screenPosition, null, Color.LightSkyBlue.Alpha() * MathHelper.Clamp(Projectile.localAI[0] - 0.25f, 0f, 0.85f), Projectile.rotation + (Main.GlobalTimeWrappedHourly * 12f % MathHelper.TwoPi), flare.Size() / 2, Projectile.scale * MathHelper.Clamp((Projectile.localAI[0] - 0.1f) * 2f, 0f, 1.75f), SpriteEffects.None);
            //front glow
            Main.EntitySpriteDraw(alphaBloom.Value, Projectile.Center - Main.screenPosition, null, Color.LightSkyBlue.Alpha() * MathHelper.Clamp(Projectile.Opacity - 0.25f, 0f, 0.6f), Projectile.rotation, alphaBloom.Size() / 2, Projectile.scale * MathHelper.Clamp(Projectile.localAI[0] * 1.75f, 0.5f, 1.1f), SpriteEffects.None);
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer == Projectile.owner)
                FrostNPC.AddFrost(target, 2);

            SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Frost") { Volume = 0.4f, Pitch = 0.6f, PitchVariance = 0.4f }, Projectile.Center);
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            for (float k = 0; k < 6.28f; k += Main.rand.NextFloat(0.3f, 0.45f))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(5), DustType<ArcticDust>(), Vector2.One.RotatedBy(k) * Main.rand.NextFloat(0.9f, 1.35f), 0, Color.White);
                dust.noLight = true;
            }
        }
    }
    
    public class ArcticWraithBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ProjectileType<ArcticWraith>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}