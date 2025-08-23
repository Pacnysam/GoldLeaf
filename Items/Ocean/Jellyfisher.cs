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
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Items.Nightshade;
using GoldLeaf.Core.CrossMod;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;
using Terraria.ModLoader.IO;
using System.Net.Mail;
using GoldLeaf.Prefixes;
using GoldLeaf.Items.FishWeapons;
using Terraria.Localization;

namespace GoldLeaf.Items.Ocean
{
    public class Jellyfisher : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
            //ItemID.Sets.ToolTipDamageMultiplier[Item.type] = 0.5f;

            Item.AddElements([Element.Water, Element.Thunder]);
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item87;
            Item.shoot = ProjectileType<JellyfishSentry>();
            Item.DamageType = DamageClass.Summon;
            Item.noMelee = true;

            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = false;

            Item.mana = 20;
            Item.damage = 31;
            Item.fishingPole = 20;
            Item.sentry = true;

            Item.shootSpeed = 12.5f;

            Item.value = Item.sellPrice(0, 4, 15, 0);
            Item.rare = ItemRarityID.Blue;
            Item.knockBack = 2.5f;

            Item.width = 40;
            Item.height = 30;

            /*Item.CloneDefaults(ItemID.ReinforcedFishingPole);
            Item.fishingPole = 20;

            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 40;
            Item.height = 30;
            Item.knockBack = 2.5f;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 4, 15, 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = false;*/
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                for (int p = 0; p < 300; p++)
                {
                    Projectile projectile = Main.projectile[p];
                    if (projectile.active && projectile.bobber && projectile.owner == player.whoAmI)
                        return false;
                }

                Item.UseSound = SoundID.Item87;
                Item.shoot = ProjectileType<JellyfishSentry>();
                Item.fishingPole = 0;

                //Item.useTime = Item.useAnimation = 30;

                //Item.useStyle = ItemUseStyleID.Shoot;
            }
            else
            {
                Item.UseSound = SoundID.Item1;
                Item.shoot = ProjectileType<JellyfishBobber>();
                Item.fishingPole = 20;

                /*ModPrefix prefix = PrefixLoader.GetPrefix(Item.prefix);

                if (prefix is FishingRodPrefix)
                {
                    Item.fishingPole += (prefix as FishingRodPrefix).FishingPower;
                }*/

                //Item.useTime = Item.useAnimation = 8;

                //Item.useStyle = ItemUseStyleID.Swing;
            }
            return base.CanUseItem(player);
        }

        public override void HoldItem(Player player)
        {
            player.accFishingLine = true;
        }

        public override float UseTimeMultiplier(Player player) => (player.altFunctionUse == 2) ? 0.325f : 1f;
        public override float UseAnimationMultiplier(Player player) => (player.altFunctionUse == 2) ? 0.325f : 1f;
        
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.altFunctionUse == 2)
                damage.Base = 0;
        }
        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult = 0;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                velocity = Vector2.Zero;
                position = Main.MouseWorld;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                for (float k = 0; k < MathHelper.TwoPi; k += MathHelper.TwoPi / 40f)
                {
                    Dust dust = Dust.NewDustPerfect(Main.MouseWorld, DustID.PortalBoltTrail/*DustID.Cloud*/, Vector2.One.RotatedBy(k) * 1.75f, 120, Color.White, 1.25f);
                    dust.fadeIn = 1.15f;
                    dust.noGravity = true;
                    dust.noLight = true;
                }

                var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, 2, Main.rand.NextFloat((float)Math.PI * 2));
                
                player.UpdateMaxTurrets();
            }
            return player.altFunctionUse == 2;
        }

        public override bool? UseItem(Player player)
        {
            Item.NetStateChanged();
            return base.UseItem(player);
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor)
        {
            lineOriginOffset = new Vector2(39, -26);

            if (bobber.ModProjectile is JellyfishBobber)
            {
                lineColor = new Color(209, 193, 165);
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.ReinforcedFishingPole);
            recipe.AddIngredient(ItemID.Coral, 8);
            recipe.AddIngredient(ItemID.Seashell, 2);
            recipe.AddRecipeGroup("GoldLeaf:JellyfishBait");
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
    // TODO: bobber doesnt show up to other players in mp, you can use primary fire while bobber exists on other screens
    public class JellyfishBobber : ModProjectile
    {
        private static Asset<Texture2D> bloomTex;
        private static Asset<Texture2D> darkBloomTex;
        public override void Load()
        {
            bloomTex = Request<Texture2D>("GoldLeaf/Textures/Masks/Mask0");
            darkBloomTex = Request<Texture2D>("GoldLeaf/Textures/Glow0");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BobberReinforced);

            DrawOriginOffsetY = -4;
        }

        float glowStrength = 0f;
        float evilGlowStrength = 0f;

        public override Color? GetAlpha(Color lightColor) => ColorHelper.AdditiveWhite(160) * Projectile.Opacity;

        public override void PostAI()
        {
            if (Projectile.ai[1] != 0 && Projectile.localAI[1] > 0f)
                glowStrength = LerpFloat(glowStrength, 1f, 0.075f);
            else
                glowStrength = LerpFloat(glowStrength, 0f, 0.035f);

            if (Projectile.localAI[1] < 0f)
                evilGlowStrength = LerpFloat(evilGlowStrength, 1f, 0.075f);
            else
                evilGlowStrength = LerpFloat(evilGlowStrength, 0f, 0.035f);

            Color dustColor = Color.Lerp(new Color(63, 74, 255) { A = 80 }, new Color(197, 145, 255) { A = 160 }, (float)(Math.Sin(GoldLeafWorld.rottime * 4f) / 2f) + 0.5f) with { A = 0 } * 0.325f;
            Color evilDustColor = Color.Lerp(new Color(255, 155, 142) { A = 0 }, new Color(222, 41, 52) { A = 0 }, (float)(Math.Sin(GoldLeafWorld.rottime * 4f) / 2f) + 0.5f) with { A = 0 } * 0.325f;

            if (glowStrength >= 0.5f && Main.rand.NextBool(8))
            {
                Dust dust = TwinkleDust.Spawn(new LightDust.LightDustData(0.9f), Projectile.position, Projectile.Size, new Vector2(0, Main.rand.NextFloat(-1.5f, -3f)), Main.rand.Next(-10, 0), dustColor, Main.rand.NextFloat(0.25f, 0.5f));
                dust.velocity.X *= 0.5f;
            }
            if (evilGlowStrength >= 0.5f && Main.rand.NextBool(8))
            {
                Dust dust = TwinkleDust.Spawn(new LightDust.LightDustData(0.9f), Projectile.position, Projectile.Size, new Vector2(0, Main.rand.NextFloat(-1.5f, -3f)), Main.rand.Next(-10, 0), evilDustColor, Main.rand.NextFloat(0.25f, 0.5f));
                dust.velocity.X *= 0.5f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            
            Color bloomColor = Color.Lerp(new Color(197, 145, 255) { A = 160 }, new Color(63, 74, 255) { A = 80 }, (float)(Math.Sin(GoldLeafWorld.rottime * 4f) / 2f) + 0.5f) with { A = 0 } * 0.65f;
            Color evilBloomColor = Color.Lerp(new Color(222, 41, 52) { A = 0 }, new Color(255, 155, 142) { A = 0 }, (float)(Math.Sin(GoldLeafWorld.rottime * 4f) / 2f) + 0.5f) with { A = 0 } * 0.65f;
            
            if (glowStrength >= 0.01f)
            {
                //dark bloom
                Main.EntitySpriteDraw(darkBloomTex.Value, Projectile.Center + new Vector2(0, -4) - Main.screenPosition, null, Color.Black * Projectile.Opacity * glowStrength * 0.75f, 0, darkBloomTex.Size() / 2, Projectile.scale * (0.8f + (float)(Math.Sin(GoldLeafWorld.rottime * 1.5f) * 0.15f)) * 0.7f * glowStrength, SpriteEffects.None, 0f);
                Main.EntitySpriteDraw(darkBloomTex.Value, Projectile.Center + new Vector2(0, -4) - Main.screenPosition, null, Color.Black * Projectile.Opacity * glowStrength * 0.75f, 0, darkBloomTex.Size() / 2, Projectile.scale * (0.8f + (float)(Math.Sin(GoldLeafWorld.rottime * 1.5f) * -0.15f)) * 0.7f * glowStrength, SpriteEffects.None, 0f);
                //bloom
                Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center + new Vector2(0, -4) - Main.screenPosition, null, bloomColor * Projectile.Opacity * glowStrength, 0, bloomTex.Size() / 2, Projectile.scale * (0.8f + (float)(Math.Sin(GoldLeafWorld.rottime * 1.5f) * 0.15f)) * 0.325f * glowStrength, SpriteEffects.None, 0f);
                Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center + new Vector2(0, -4) - Main.screenPosition, null, bloomColor * Projectile.Opacity * glowStrength, 0, bloomTex.Size() / 2, Projectile.scale * (0.8f + (float)(Math.Sin(GoldLeafWorld.rottime * 1.5f) * -0.15f)) * 0.325f * glowStrength, SpriteEffects.None, 0f);
            }
            if (evilGlowStrength >= 0.01f)
            {
                //dark bloom
                Main.EntitySpriteDraw(darkBloomTex.Value, Projectile.Center + new Vector2(0, -4) - Main.screenPosition, null, Color.Black * Projectile.Opacity * evilGlowStrength * 0.75f, 0, darkBloomTex.Size() / 2, Projectile.scale * (0.8f + (float)(Math.Sin(GoldLeafWorld.rottime * 1.5f) * 0.15f)) * 0.7f * evilGlowStrength, SpriteEffects.None, 0f);
                Main.EntitySpriteDraw(darkBloomTex.Value, Projectile.Center + new Vector2(0, -4) - Main.screenPosition, null, Color.Black * Projectile.Opacity * evilGlowStrength * 0.75f, 0, darkBloomTex.Size() / 2, Projectile.scale * (0.8f + (float)(Math.Sin(GoldLeafWorld.rottime * 1.5f) * -0.15f)) * 0.7f * evilGlowStrength, SpriteEffects.None, 0f);
                //bloom
                Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center + new Vector2(0, -4) - Main.screenPosition, null, evilBloomColor * Projectile.Opacity * evilGlowStrength, 0, bloomTex.Size() / 2, Projectile.scale * (0.8f + (float)(Math.Sin(GoldLeafWorld.rottime * 1.5f) * 0.15f)) * 0.325f * evilGlowStrength, SpriteEffects.None, 0f);
                Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center + new Vector2(0, -4) - Main.screenPosition, null, evilBloomColor * Projectile.Opacity * evilGlowStrength, 0, bloomTex.Size() / 2, Projectile.scale * (0.8f + (float)(Math.Sin(GoldLeafWorld.rottime * 1.5f) * -0.15f)) * 0.325f * evilGlowStrength, SpriteEffects.None, 0f);
            }

            /*float blinkIntensity = 1f;
            Color blinkColor = ColorHelper.AdditiveWhite();

            if (Projectile.ai[1] != 0 && Projectile.localAI[1] > 0f) //checks for item
            {
                blinkIntensity = 2.75f;
            }
            if (Projectile.localAI[1] < 0f) //checks for enemy
            {
                blinkIntensity = 2.75f;
                blinkColor = Color.Red with { A = 0 };
            }*/

            //main sprite
            //Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite(160) * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            //glowmask
            //Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, blinkColor * Projectile.Opacity * (float)Math.Sin(GoldLeafWorld.rottime * 2f * blinkIntensity) * (0.3f * blinkIntensity), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            float blinkIntensity = 1f;
            Color blinkColor = ColorHelper.AdditiveWhite();

            if (Projectile.ai[1] != 0 && Projectile.localAI[1] > 0f) //checks for item
            {
                blinkIntensity = 2.75f;
            }
            if (Projectile.localAI[1] < 0f) //checks for enemy
            {
                blinkIntensity = 2.75f;
                blinkColor = Color.Red with { A = 0 };
            }

            //glowmask
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, blinkColor * Projectile.Opacity * (float)Math.Sin(GoldLeafWorld.rottime * 2f * blinkIntensity) * (0.3f * blinkIntensity), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
        }
    }

    public class JellyfishSentry : ModProjectile
    {
        private static Asset<Texture2D> bloomTex;
        private static Asset<Texture2D> darkBloomTex;
        public override void Load()
        {
            bloomTex = Request<Texture2D>("GoldLeaf/Textures/Masks/Mask0");
            darkBloomTex = Request<Texture2D>("GoldLeaf/Textures/Glow0");
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 36;

            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.sentry = true;
            Projectile.DamageType = DamageClass.Summon;

            Projectile.alpha = 255;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.netImportant = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(SentryPos);
            writer.Write(hasTarget);
            writer.Write(RotDist);
            writer.Write(reverseRotaion);
            writer.Write(hasTarget);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SentryPos = reader.ReadVector2();
            hasTarget = reader.ReadBoolean();
            RotDist = reader.ReadSingle();
            reverseRotaion = reader.ReadBoolean();
            hasTarget = reader.ReadBoolean();
        }

        private Vector2 SentryPos;
        bool hasTarget = false;
        bool reverseRotaion = false;
        bool hasSetup = false;

        const int animationSpeed = 7;

        float RotDist;

        private ref float State => ref Projectile.ai[0];
        private ref float Rottime => ref Projectile.ai[1];
        private ref float Charge => ref Projectile.ai[2]; //for the first frame this is instead the target Y position, theres definitely a better way to do this but im sleep deprived rn

        private const int Idle = 0;
        private const int Attacking = 1;
        private const int Swimming = 2;

        private const int AttackRange = 450;

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (State == Swimming)
                hitbox.Inflate(24, 12);
        }

        public override bool PreAI()
        {
            if (!hasSetup)
            {
                SentryPos = Projectile.position;
                Projectile.Center = SentryPos + new Vector2(0, 750);
                State = Swimming;

                hasSetup = true;
            }
            return base.PreAI();
        }

        public override void AI()
        {
            #region misc variables
            Rottime += (float)Math.PI / 120 * (reverseRotaion ? -1 : 1);

            if (Rottime >= Math.PI * 2)
                Rottime = 0;

            if (Projectile.Opacity < 1)
                Projectile.alpha -= 15;
            else
                Projectile.alpha = 0;

            int target = -1;
            #endregion misc variables

            #region animation
            if (!Main.gamePaused)
                Projectile.frameCounter++;

            if (Projectile.frameCounter >= animationSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;

                    if (State == Attacking)
                        State = Idle;
                }
            }
            #endregion animation

            #region targeting
            if (State == Idle)
            {
                NPC ownerMinionAttackTargetNPC = Projectile.OwnerMinionAttackTargetNPC;
                if (ownerMinionAttackTargetNPC != null && ownerMinionAttackTargetNPC.CanBeChasedBy(this) && Projectile.position.Distance(ownerMinionAttackTargetNPC.Center) <= AttackRange 
                    && ownerMinionAttackTargetNPC.IsValid() && Projectile.CanHitWithOwnBody(ownerMinionAttackTargetNPC))
                {
                    target = ownerMinionAttackTargetNPC.whoAmI;
                }
                else
                {
                    float targetDist = AttackRange;
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        int npcID = npc.whoAmI;

                        if (Projectile.Center.Distance(npc.Center) <= AttackRange && npc.IsValid() && Projectile.CanHitWithOwnBody(npc) && Projectile.Center.Distance(npc.Center) < targetDist)
                        {
                            targetDist = Projectile.Center.Distance(npc.Center);
                            target = npcID;
                        }
                    }
                }
            }
            #endregion targeting

            #region behavior
            if (State == Swimming)
            {
                Projectile.velocity.X = (float)Math.Sin((Rottime * 2) * Math.Clamp(MathHelper.Distance(Projectile.position.Y, SentryPos.Y) * 0.15f, 2f, 4.75f)) * Math.Clamp(MathHelper.Distance(Projectile.Center.Y, SentryPos.Y) * 0.0225f, 0, 5.75f);

                if (MathHelper.Distance(Projectile.position.Y, SentryPos.Y) >= 10)
                {
                    Projectile.velocity.Y = Math.Clamp(MathHelper.Distance(Projectile.position.Y, SentryPos.Y - 24) * -0.0425f, -10.5f, -3.5f);

                    Projectile.localAI[0] += Math.Clamp(Math.Abs(Projectile.velocity.Y * 0.25f), 0, 8);
                    if (Projectile.localAI[0] >= 24f)
                    {
                        Projectile.localAI[0] = 0;

                        if (!Main.dedServ)
                            SoundEngine.PlaySound(SoundID.Splash with { Volume = Math.Clamp(Math.Abs(Projectile.velocity.Y * 0.05f), 0.25f, 0.65f), MaxInstances = 0 }, Projectile.Center);
                    }
                }
                else
                {
                    Projectile.localAI[0] = 0;
                    State = Idle;

                    RotDist = Main.rand.NextFloat(3f, 6.5f);
                    reverseRotaion = Main.rand.NextBool();

                    Projectile.netUpdate = true;
                }

                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
                Projectile.position = Vector2.Lerp(Projectile.position, SentryPos + Vector2.One.RotatedBy(Rottime * 2) * RotDist, 0.0075f);

                if (Main.rand.NextBool(2, 5))
                { 
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PortalBoltTrail, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.5f, 0, new Color(197, 145, 255) { A = 80 }, Main.rand.NextFloat(0.7f, 0.95f) * Projectile.Opacity);
                    //dust.noGravity = true;
                    dust.velocity *= 0.65f;
                }
            }

            if (State == Idle || State == Attacking)
            {
                Projectile.velocity *= 0.95f;
                Projectile.rotation = 0;

                if (Projectile.localAI[0] < 1)
                    Projectile.localAI[0] += 0.025f;
                else
                    Projectile.localAI[0] = 1;

                Projectile.position = Vector2.Lerp(Projectile.position, SentryPos + Vector2.One.RotatedBy(Rottime * 2) * RotDist, 0.1f);

                if (Main.rand.NextBool(2, 5))
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PortalBoltTrail, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, new Color(197, 145, 255) { A = 80 }, Main.rand.NextFloat(0.75f, 1.15f) * Charge);
                    dust.noGravity = true;
                }
            }

            if (State == Idle)
            {
                if (target != -1 && Charge >= 1f) //shoot
                {
                    NPC npc = Main.npc[target];

                    State = Attacking;
                    Charge = 0;

                    if (!Main.dedServ)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap with { Volume = 1.45f }, Projectile.Center);

                        for (float k = 0; k < MathHelper.TwoPi; k += Main.rand.NextFloat(0.35f, 0.75f))
                        {
                            Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(0, -4), DustID.PortalBoltTrail, new Vector2(2.5f).RotatedBy(k) * Main.rand.NextFloat(0.9f, 1.5f), 120, new Color(197, 145, 255) { A = 80 }, Main.rand.NextFloat(0.85f, 1.75f));
                            dust.fadeIn = Main.rand.NextFloat(0.75f, 1.5f);
                            dust.velocity.Y *= 0.75f;
                            dust.noGravity = true;
                        }
                    }

                    if (Main.myPlayer == Projectile.owner)
                    {
                        Vector2 enemyVector = npc.Center - Projectile.Center;
                        enemyVector.Normalize();
                        enemyVector *= 9f;

                        Projectile projectile = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, enemyVector, ProjectileID.ThunderStaffShot, Projectile.damage, Projectile.knockBack, Projectile.owner, target);
                        projectile.extraUpdates = 20;
                        projectile.DamageType = DamageClass.Summon;
                        projectile.netImportant = true;
                    }
                }
                Charge = Math.Clamp(Charge + 0.01f, 0, 1);
            }
            #endregion behavior
        }

        public override bool? CanDamage() => (State != Idle) && Projectile.Counter() >= 10;
        public override bool? CanHitNPC(NPC target) => (State != Idle) && !target.friendly && Projectile.Counter() >= 10;
        
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = texture.Frame(Main.projFrames[Projectile.type], 2, Projectile.frame, (State != Attacking)? 0 : 1);

            Color color1 = new(63, 74, 255) { A = 80 };
            Color color2 = new(197, 145, 255) { A = 160 };

            if (State != Swimming) 
            {
                //dark bloom
                Main.spriteBatch.Draw(darkBloomTex.Value, Projectile.Center + new Vector2(0, -4) - Main.screenPosition, null, Color.Black * Projectile.Opacity * Projectile.localAI[0] * 0.75f, 0, darkBloomTex.Size() / 2, Projectile.scale * (0.8f + (float)(Math.Sin(Rottime * 3) * 0.15f)) * 0.85f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(darkBloomTex.Value, Projectile.Center + new Vector2(0, -4) - Main.screenPosition, null, Color.Black * Projectile.Opacity * Projectile.localAI[0] * 0.75f, 0, darkBloomTex.Size() / 2, Projectile.scale * (0.8f + (float)(Math.Sin(Rottime * 3) * -0.15f)) * 0.85f, SpriteEffects.None, 0f);
                //bloom
                Main.spriteBatch.Draw(bloomTex.Value, Projectile.Center + new Vector2(0, -4) - Main.screenPosition, null, Color.Lerp(color2, color1, (float)(Math.Sin(Rottime * 8) / 2f) + 0.5f) with { A = 0 } * Projectile.Opacity * Projectile.localAI[0], 0, bloomTex.Size() / 2, Projectile.scale * (0.8f + (float)(Math.Sin(Rottime * 3) * 0.15f)) * 0.325f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(bloomTex.Value, Projectile.Center + new Vector2(0, -4) - Main.screenPosition, null, Color.Lerp(color2, color1, (float)(Math.Sin(Rottime * 8) / 2f) + 0.5f) with { A = 0 } * Projectile.Opacity * Projectile.localAI[0], 0, bloomTex.Size() / 2, Projectile.scale * (0.8f + (float)(Math.Sin(Rottime * 3) * -0.15f)) * 0.325f, SpriteEffects.None, 0f);
            }

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + rect.Size() / 2 + new Vector2(0, 2);

                //afterimage
                Main.EntitySpriteDraw(texture, drawPos, rect, Color.Lerp(color1, color2, (float)(Math.Sin(Rottime) / 2f) + 0.5f).MultiplyAlpha(0.65f) * Projectile.Opacity * (0.7f - (k / (Projectile.oldPos.Length + 4f))), Projectile.oldRot[k], rect.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            }
            //jellyfish
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, ColorHelper.AdditiveWhite(160) * Projectile.Opacity, Projectile.rotation, rect.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, ColorHelper.AdditiveWhite(0) * Projectile.Opacity * (float)Math.Sin(Rottime * 6) * 0.3f, Projectile.rotation, rect.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }
        
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PortalBoltTrail, 0, 0, 0, new Color(197, 145, 255) { A = 80 }, Main.rand.NextFloat(0.65f, 0.9f));
                dust.velocity.Y += -3;
            }

            for (int i = 0; i < Main.rand.Next(7, 9); i++)
            {
                Dust dust = TwinkleDust.Spawn(new LightDust.LightDustData(0.925f), Projectile.position, new Vector2(Projectile.width, Projectile.height), Vector2.Zero, Main.rand.Next(0, 15), new Color(197, 145, 255) { A = 0 } * Main.rand.NextFloat(0.5f, 0.85f), Main.rand.NextFloat(0.5f, 0.8f));
                //Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustType<TwinkleDust>(), 0, 0, 0, new Color(197, 145, 255) { A = 120 }, Main.rand.NextFloat(0.75f, 1f));
                dust.fadeIn = Main.rand.NextFloat(1.9f, 2.75f);
                dust.velocity *= Main.rand.NextFloat(0.85f, 1.35f);
                dust.noGravity = true;
            }

            if (!Main.dedServ)
                SoundEngine.PlaySound(SoundID.Item112, Projectile.Center);
        }
    }

    public class JellyfisherPlayer : ModPlayer
    {
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (attempt.playerFishingConditions.PoleItemType == ItemType<Jellyfisher>())
            {
                if ((attempt.common || attempt.uncommon) && Main.rand.NextBool(4)) //items
                {
                    if (!Main.hardMode)
                    {
                        itemDrop = Utils.SelectRandom(Main.rand, [ItemID.BlueJellyfish, ItemID.PinkJellyfish]);
                    }
                    else
                    {
                        itemDrop = Utils.SelectRandom(Main.rand, [ItemID.BlueJellyfish, ItemID.PinkJellyfish, ItemID.GreenJellyfish]);
                    }
                    if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                    {
                        if (thorium.TryFind("JellyfishResonator", out ModItem queenJellyfishSummon) && Main.rand.NextBool(10) && Player.ZoneBeach) //thorium queen jellyfish summon
                        {
                            itemDrop = queenJellyfishSummon.Type;
                        }
                        if (Main.rand.NextBool(3) && thorium.TryFind("ZealousJellyfish", out ModNPC zealousJelly) && thorium.TryFind("SpittingJellyfish", out ModNPC spittingJelly)) //thorium queen jellyfish minions
                        {
                            itemDrop = 0;
                            npcSpawn = Utils.SelectRandom(Main.rand, [zealousJelly.Type, spittingJelly.Type]);
                        }
                    }
                }
                else if ((attempt.common || attempt.uncommon) && Main.rand.NextBool(6)) //enemies
                {
                    if (!Main.hardMode)
                    {
                        itemDrop = 0;
                        npcSpawn = Utils.SelectRandom(Main.rand, [NPCID.BlueJellyfish, NPCID.PinkJellyfish]);
                    }
                    else
                    {
                        itemDrop = 0;
                        npcSpawn = Utils.SelectRandom(Main.rand, [NPCID.BlueJellyfish, NPCID.PinkJellyfish, NPCID.GreenJellyfish]);
                    }
                    if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                    {
                        if (thorium.TryFind("DepthsBiome", out ModBiome aquaticDepths) && Player.InModBiome(aquaticDepths)) //thorium aquatic depths jellyfish
                        {
                            if (thorium.TryFind("ManofWar", out ModNPC manOfWar))
                            {
                                itemDrop = 0;
                                npcSpawn = manOfWar.Type;
                            }
                        }
                        if (Main.rand.NextBool(3) && thorium.TryFind("ZealousJellyfish", out ModNPC zealousJelly) && thorium.TryFind("SpittingJellyfish", out ModNPC spittingJelly)) //thorium queen jellyfish minions
                        {
                            itemDrop = 0;
                            npcSpawn = Utils.SelectRandom(Main.rand, [zealousJelly.Type, spittingJelly.Type]);
                        }
                    }
                    if (ModLoader.TryGetMod("Redemption", out Mod redemption)) //redemption wasteland jellyfish
                    {
                        if (redemption.TryFind("Wasteland", out ModBiome wasteland) && Player.InModBiome(wasteland) && NPC.downedMechBossAny)
                        {
                            if (redemption.TryFind("RadioactiveJelly", out ModNPC radioactiveJelly))
                            {
                                itemDrop = 0;
                                npcSpawn = radioactiveJelly.Type;
                            }
                        }
                    }
                    if (NPC.downedAncientCultist && Main.rand.NextBool(100)) //teehee
                    {
                        itemDrop = 0;
                        npcSpawn = NPCID.StardustJellyfishBig;
                    }
                }
            }
        }
    }
}