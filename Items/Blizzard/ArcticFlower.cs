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
			Item.damage = 35;
			Item.DamageType = DamageClass.Summon;
            Item.shoot = ProjectileType<ArcticWraith>();
            Item.buffType = BuffType<ArcticWraithBuff>();
            Item.mana = 10;
			Item.width = 30;
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
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
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
        }

        private ref float State => ref Projectile.ai[0];
        private ref float AnimLoops => ref Projectile.ai[1];
        private ref int Counter => ref Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter;

        private const int Idle = 0;
        private const int WindUp = 1;
        private const int Recoil = 2;
        private const int Turning = 3;
        //private const int Spawning = 4;

        const int targetingRange = 900;

        const int animationSpeed = 6;

        bool animReverse = false;
        bool hasTarget = false;

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool MinionContactDamage()
        {
            return false;
        }

        private bool IsFrozen(Entity otherEntity, int currentTarget)
        {
            NPC npc = otherEntity as NPC;
            return npc.HasBuff(BuffID.Frozen);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!MinionCheckBuff(player))
            {
                return;
            }

            #region general behavior
            Vector2 idlePosition = player.Center;
            idlePosition.Y -= 32f;
            float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -player.direction;
            idlePosition.X += minionPositionOffsetX;

            Vector2 vectorToIdlePosition = idlePosition - Projectile.Center;
            float distanceToIdlePosition = vectorToIdlePosition.Length();

            NPC target;

            #region teleport if far
            if (Main.myPlayer == player.whoAmI && distanceToIdlePosition > 1500f)
            {
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
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

            if (State == Idle)
            {
                //Projectile.velocity += Vector2.Normalize(idlePosition - Projectile.Center) * 0.1f;
            }

            #endregion

            #region targeting
            int attackTarget = -1;

            Projectile.Minion_FindTargetInRange(targetingRange, ref attackTarget, false, IsFrozen);
            if (attackTarget == -1)
                Projectile.Minion_FindTargetInRange(targetingRange, ref attackTarget, false);
            if (attackTarget != -1)
            {
                target = Main.npc[attackTarget];
            }
            #endregion targeting

            #region animation
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

                    if (State == WindUp)
                        ChangeState(Recoil);
                    else if (State == Recoil)
                        ChangeState(Idle);

                    Projectile.frame = 0;
                }
                else if (Projectile.frame >= 3 && State == Turning && !animReverse)
                {
                    animReverse = true;
                }
                else if (Projectile.frame == 0 && State == Turning && animReverse)
                    ChangeState(Idle);
            }
            #endregion animation
        }

        private void ChangeState(float newState)
        {
            Projectile.frameCounter = 0;
            Projectile.frame = 0;
            Counter = 0;
            AnimLoops = 0;
            animReverse = false;

            State = newState;
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
            var effects = (Projectile.direction == -1 || animReverse) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 offset = new(0, -1);

            Rectangle rect = new((texture.Width / 4) * (int)State, texture.Height / Main.projFrames[Projectile.type] * Projectile.frame, 
                (texture.Width / 4), texture.Height / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(texture, (Projectile.Center + offset) - Main.screenPosition, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, rect.Size() / 2, Projectile.scale, effects, 0);
            return false;
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