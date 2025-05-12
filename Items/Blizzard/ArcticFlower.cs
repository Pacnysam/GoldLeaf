using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Core;
using GoldLeaf.Items.Grove;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.DataStructures;
using GoldLeaf.Effects.Dusts;
using System;
using ReLogic.Content;
using GoldLeaf.Items.Grove.Boss;
using System.IO;

namespace GoldLeaf.Items.Blizzard
{
	public class ArcticFlower : ModItem
	{
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;
        }

        public override void SetDefaults()
		{
			Item.damage = 32;
			Item.DamageType = DamageClass.Summon;
            Item.shoot = ProjectileType<ArcticWraith>();
            Item.buffType = BuffType<ArcticWraithBuff>();
            Item.mana = 10;
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
			recipe.AddRecipeGroup("GoldLeaf:GoldBars", 6);
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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 44;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.MagicSummonHybrid;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
        }

        private ref float State => ref Projectile.ai[0];
        private ref float AnimLoops => ref Projectile.ai[1];
        private ref int Counter => ref Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter;

        private const int Idle = 0;
        private const int WindUp = 1;
        private const int Recoil = 2;
        private const int Turning = 3;
        //private const int Spawning = 4;

        const int targetingRange = 800;
        const float attackingRange = 400;

        const float speed = 9f;
        const float inertia = 50f;
        const float shootSpeed = 4.5f;

        const int animationSpeed = 6;

        bool animReverse = false;
        bool hasTarget = false;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(animReverse);
            writer.Write(hasTarget);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            animReverse = reader.ReadBoolean();
            hasTarget = reader.ReadBoolean();
        }

        public override bool? CanCutTiles() => false;
        public override bool MinionContactDamage() => false;

        private bool IsFrozen(Entity otherEntity, int currentTarget)
        {
            NPC npc = otherEntity as NPC;
            return npc.HasBuff(BuffID.Frozen);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!MinionCheckBuff(player)) return;

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
            if (Main.myPlayer == player.whoAmI && distanceToIdlePosition > 1500f)
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
                    if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
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

            //Projectile.Minion_FindTargetInRange(targetingRange, ref attackTarget, false, IsFrozen);
            //if (attackTarget == -1)
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

                Vector2 direction = targetCenter - Projectile.Center;
                direction.Normalize();
                direction *= speed;
                if (Projectile.Center.Distance(targetCenter) >= attackingRange)
                {
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                }
                else 
                {
                    if (MathHelper.Distance(Projectile.Center.X, targetCenter.X) <= attackingRange - 50)
                    {
                        Projectile.velocity.X = -0.35f * ((Projectile.velocity.X * (inertia - 1) + direction.X) / inertia);
                    }
                    else
                    { 
                        Projectile.velocity.X *= 0.9f; 
                    }
                    if (MathHelper.Distance(Projectile.Center.Y, targetCenter.Y) <= attackingRange/2)
                    {
                        Projectile.velocity.Y *= 0.925f;
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

                    if (Projectile.velocity.Length() < 4f && Projectile.Center.Distance(idlePosition) <= 20)
                    {
                        Projectile.velocity *= 0.8f;
                        if (Projectile.velocity.Length() < 0.2f)
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
                        Projectile.position = Vector2.Lerp(Projectile.position, idlePosition - (Projectile.Size/2), 0.1f);
                }

                Projectile.direction = Projectile.spriteDirection = (Projectile.Center.X > player.Center.X) ? -1 : 1;
            }
            Projectile.position.Y += 0.8f * (float)Math.Sin(GoldLeafWorld.rottime + (Projectile.minionPos * 1.25f));

            #endregion behavior
        }

        private void ChangeState(float newState)
        {
            Projectile.frameCounter = 0;
            Projectile.frame = 0;
            Counter = 0;
            AnimLoops = 0;
            animReverse = false;
            Counter = 0;

            State = newState;
        }

        private void Shoot(NPC target, Vector2 velocity) 
        {
            for (int j = 0; j < 16; j++)
            {
                Vector2 vector = Vector2.UnitX * -Projectile.width / 2f;
                //vector += -Utils.RotatedBy(Vector2.UnitY, (j * 3.141591734f / 6f), default) * new Vector2(8f, 16f);
                vector = Utils.RotatedBy(vector, (velocity.ToRotation() - 1.57079637f), default);
                Dust dust = Dust.NewDustDirect(Projectile.Center + vector + (velocity * 2), 0, 0, DustType<ArcticDust>(), 0f, 0f, 160, new Color(), 1f);
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - dust.position) * 2.25f;
            }

            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, velocity, ProjectileType<ArcticWraithOrb>(), Projectile.damage, Projectile.knockBack, Projectile.owner, target.whoAmI);
            SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Kirby/ForgottenLand/StarShot") { Volume = 0.5f, PitchVariance = 0.5f }, Projectile.Center);
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

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var effects = (Projectile.spriteDirection == -1 || animReverse) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 offset = new(0, -1);

            Rectangle rect = new((texture.Width / 4) * (int)State, texture.Height / Main.projFrames[Projectile.type] * Projectile.frame, 
                (texture.Width / 4), texture.Height / Main.projFrames[Projectile.type]);

            if (hasTarget)
            {
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    var oldEffects = (Projectile.oldSpriteDirection[k] == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + rect.Size()/2;
                    Color color1 = new(0, 225, 241);
                    Color color2 = new(0, 38, 128);

                    Main.spriteBatch.Draw(texture, drawPos, rect, Color.Lerp(color1, color2, k / (Projectile.oldPos.Length + 2f)) with { A = 0 } * (0.8f - (k / (Projectile.oldPos.Length + 4f))), Projectile.oldRot[k], rect.Size() / 2, Projectile.scale, oldEffects, 0f);
                }
            }
            else
            {
                //Projectile.oldPos = [];
            }

            Main.EntitySpriteDraw(texture, (Projectile.Center + offset) - Main.screenPosition, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, rect.Size()/2, Projectile.scale, effects, 0f);

            float brightness = Lighting.Brightness((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);
            if (brightness <= 0.3f)
                Main.spriteBatch.Draw(trimTex.Value, (Projectile.Center + offset) - Main.screenPosition, rect, ColorHelper.AdditiveWhite * (0.3f - brightness), Projectile.rotation, rect.Size()/2, Projectile.scale, effects, 0);
            
            Main.spriteBatch.Draw(glowTex.Value, (Projectile.Center + offset) - Main.screenPosition, rect, Color.White, Projectile.rotation, rect.Size()/2, Projectile.scale, effects, 0f);

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

            return false;
        }
    }

    public class ArcticWraithOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Eve Droplet");
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = TimeToTicks(15);

            Projectile.DamageType = DamageClass.MagicSummonHybrid;
        }

        const float accelerationSpeed = 1.3f;
        const float maxSpeed = 14.5f;

        bool HasTarget => (Main.npc[(int)Projectile.ai[0]].active && Main.npc[(int)Projectile.ai[0]].chaseable && Projectile.Distance(Main.npc[(int)Projectile.ai[0]].Center) <= 500) && Counter <= TimeToTicks(10);
        private ref int Counter => ref Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter;

        public override void AI()
        {
            NPC target = Main.npc[(int)Projectile.ai[0]];

            Projectile.tileCollide = !HasTarget;

            if (HasTarget)
            {
                Projectile.velocity += Vector2.Normalize(target.Center - Projectile.Center) * accelerationSpeed;
            }

            if (Projectile.velocity.Length() > maxSpeed) 
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * maxSpeed;

            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }

            Lighting.AddLight((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16), (0 / 255f) * 0.6f, (164 / 255f) * 0.6f, (242 / 255f) * 0.6f);

            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustType<ArcticDust>());
                dust.velocity = Vector2.Zero;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle? rect = new Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame));

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + texture.Size() / 2;
                Color color1 = new(0, 225, 241);
                Color color2 = new(0, 38, 128);

                Main.spriteBatch.Draw(texture, drawPos, rect, Color.Lerp(color1, color2, k / (Projectile.oldPos.Length + 2f)) /*with { A = 0 }*/ * (1f - (k / 10f)), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            }

            Main.EntitySpriteDraw(texture, Projectile.position - Main.screenPosition + texture.Size() / 2, rect, Color.White, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer == Projectile.owner)
                BlizzardNPC.AddFrost(target, 2);

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