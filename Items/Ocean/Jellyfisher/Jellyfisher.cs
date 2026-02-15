using GoldLeaf.Core;
using GoldLeaf.Core.CrossMod;
using GoldLeaf.Core.Helpers;
using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Items.FishWeapons;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Grove.Boss;
using GoldLeaf.Items.Nightshade;
using GoldLeaf.Items.Sky;
using GoldLeaf.Items.Underground;
using GoldLeaf.Items.Vanity;
using GoldLeaf.Prefixes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;
using static GoldLeaf.Core.Helper;
using static GoldLeaf.Core.ColorHelper;
using static GoldLeaf.Core.Helpers.DrawHelper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Ocean.Jellyfisher
{
    public class Jellyfisher : ModItem
    {
        private static Asset<Texture2D> sentryModeTex;
        private static Asset<Texture2D> sentryModeGlowTex;
        public override void Load()
        {
            sentryModeTex = Request<Texture2D>(Texture + "SentryMode");
            sentryModeGlowTex = Request<Texture2D>(Texture + "SentryModeGlow");
        }
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

            Item.AddElements([Element.Water, Element.Thunder]);
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item87;
            Item.shoot = ProjectileType<JellyfishBobber>();
            Item.DamageType = DamageClass.Summon;
            Item.noMelee = true;

            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = false;

            Item.damage = 32;
            Item.fishingPole = 20;
            Item.sentry = true;

            Item.shootSpeed = 12.5f;

            Item.value = Item.sellPrice(0, 4, 15, 0);
            Item.rare = ItemRarityID.Blue;
            Item.knockBack = 2.5f;

            Item.width = 42;
            Item.height = 30;
        }

        //public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Language.GetTextValue("Mods.GoldLeaf.Items.Jellyfisher." + (sentryMode ? "FishingMode" : "SentryMode")));
        
        public static readonly SoundStyle JellyfishLightningSound = new("GoldLeaf/Sounds/SE/JellyfishLightning") { Volume = 0.75f, Pitch = -0.2f, PitchVariance = 0.4f };
        public bool sentryMode = true;

        public override void UpdateInventory(Player player)
        {
            Item.sentry = sentryMode;
            if (sentryMode)
            {
                Item.UseSound = SoundID.Item87;
                Item.fishingPole = 0;
            }
            else
            {
                Item.UseSound = SoundID.Item1;
                Item.fishingPole = 20;
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (sentryMode)
            {
                foreach (Projectile projectile in Main.projectile) 
                {
                    if (projectile.active && projectile.bobber && projectile.owner == player.whoAmI)
                        return false;
                }
            }
            return base.CanUseItem(player);
        }

        public override float UseTimeMultiplier(Player player) => !sentryMode ? 0.425f : 1f;
        public override float UseAnimationMultiplier(Player player) => !sentryMode ? 0.425f : 1f;
        
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (!sentryMode)
                damage.Base = 0;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (sentryMode)
            {
                type = ProjectileType<JellyfishSentry>();
                velocity = Vector2.Zero;
                position = Main.MouseWorld;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (sentryMode)
            {
                for (float k = 0; k < MathHelper.TwoPi; k += MathHelper.TwoPi / 40f)
                {
                    Dust dust = Dust.NewDustPerfect(Main.MouseWorld, DustID.PortalBoltTrail/*DustID.Cloud*/, Vector2.One.RotatedBy(k) * 1.75f, 120, Color.White, 1.25f);
                    dust.fadeIn = 1.15f;
                    dust.noGravity = true;
                    dust.noLight = true;
                }

                Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, 2, Main.rand.NextFloat((float)Math.PI * 2));
                player.UpdateMaxTurrets();
            }
            return !sentryMode;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (sentryMode)
            {
                spriteBatch.Draw(sentryModeTex.Value, position, frame, drawColor, 0, origin, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(sentryModeGlowTex.Value, position, frame, Color.White.Alpha(160), 0, origin, scale, SpriteEffects.None, 0f);

                spriteBatch.Draw(sentryModeGlowTex.Value, position, frame, Color.White.Alpha() * (float)Math.Sin(GoldLeafWorld.rottime * 2f) * 0.3f, 0, origin, scale, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (sentryMode)
            {
                spriteBatch.Draw(sentryModeTex.Value, Item.Center - Main.screenPosition, null, lightColor, rotation, sentryModeGlowTex.Size() / 2f, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(sentryModeGlowTex.Value, Item.Center - Main.screenPosition, null, Color.White.Alpha(160), rotation, sentryModeGlowTex.Size() / 2f, scale, SpriteEffects.None, 0f);

                spriteBatch.Draw(sentryModeGlowTex.Value, Item.Center - Main.screenPosition, null, Color.White.Alpha() * (float)Math.Sin(GoldLeafWorld.rottime * 2f) * 0.3f, rotation, sentryModeGlowTex.Size() / 2, scale, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }

        public override bool ConsumeItem(Player player) => false;
        public override bool CanRightClick() => Main.LocalPlayer.ownedProjectileCounts[ProjectileType<JellyfishBobber>()] == 0;
        public override void RightClick(Player player)
        {
            sentryMode = !sentryMode;
            Item.NetStateChanged();

            if (sentryMode)
                SoundEngine.PlaySound(SoundID.NPCHit52 with { Volume = 0.65f });
            SoundEngine.PlaySound(sentryMode ? SoundID.DD2_LightningBugHurt : SoundID.Item112);
        }

        #region Save & Load Stuff
        protected override bool CloneNewInstances => true;
        public override ModItem Clone(Item item)
        {
            var clone = (Jellyfisher)base.Clone(item);
            clone.sentryMode = sentryMode;
            return clone;
        }

        public override void SaveData(TagCompound tag)
        {
            tag[nameof(sentryMode)] = sentryMode;
        }
        public override void LoadData(TagCompound tag)
        {
            sentryMode = tag.Get<bool>(nameof(sentryMode));
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(sentryMode);
        }
        public override void NetReceive(BinaryReader reader)
        {
            sentryMode = reader.ReadBoolean();
        }
        #endregion Save & Load Stuff

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!sentryMode)
            {
                foreach (TooltipLine line in tooltips.Where(x => x.Mod == "Terraria" && (x.Name == "Tooltip0" || x.Name == "Tooltip1")))
                    line.Hide();
            }
            else
            {
                foreach (TooltipLine line in tooltips.Where(x => x.Mod == "Terraria" && (x.Name == "Tooltip2" || x.Name == "FishingPower" || x.Name == "NeedsBait")))
                    line.Hide();
            }
        }

        public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor)
        {
            lineOriginOffset = new Vector2(39, -26);

            if (bobber.ModProjectile is JellyfishBobber)
            {
                lineColor = new Color(209, 193, 165);
            }
        }
    }

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
                glowStrength = MathHelper.Lerp(glowStrength, 1f, 0.075f);
            else
                glowStrength = MathHelper.Lerp(glowStrength, 0f, 0.035f);

            if (Projectile.localAI[1] < 0f)
                evilGlowStrength = MathHelper.Lerp(evilGlowStrength, 1f, 0.075f);
            else
                evilGlowStrength = MathHelper.Lerp(evilGlowStrength, 0f, 0.035f);

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

            Color color1 = new(63, 74, 255) { A = 80 };
            Color color2 = new(197, 145, 255) { A = 80 };

            Color bloomColor = Color.Lerp(color2, color1, (float)(Math.Sin(GoldLeafWorld.rottime * 4f) / 2f) + 0.5f) with { A = 0 } * 0.65f;
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
        public override void Load()
        {
            bloomTex = Request<Texture2D>("GoldLeaf/Textures/Glow");
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            Projectile.AddElements([Element.Water, Element.Thunder]);
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
            writer.Write(hasSetup);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SentryPos = reader.ReadVector2();
            hasTarget = reader.ReadBoolean();
            RotDist = reader.ReadSingle();
            hasSetup = reader.ReadBoolean();
        }

        private Vector2 SentryPos;
        bool hasTarget = false;
        float RotDist = 1;
        bool ReverseRotaion => RotDist <= 0;
        bool hasSetup = false;

        private ref float State => ref Projectile.ai[0];
        private ref float Rottime => ref Projectile.ai[1];
        private ref float Charge => ref Projectile.ai[2]; //for the first frame this is instead the target Y position, theres definitely a better way to do this but im sleep deprived rn

        private const int Idle = 0;
        private const int Attacking = 1;
        private const int Swimming = 2;

        private static readonly int AttackRange = 400;

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (State == Swimming)
                hitbox.Inflate(24, 12);
        }

        public override void AI()
        {
            #region setup
            if (!hasSetup)
            {
                SentryPos = Projectile.position;
                Projectile.Center = SentryPos + new Vector2(0, 750);
                State = Swimming;

                hasSetup = true;
            }

            Rottime += (float)Math.PI / 120 * -ReverseRotaion.ToDirectionInt();

            if (Rottime >= Math.PI * 2)
                Rottime = 0;

            if (Projectile.Opacity < 1)
                Projectile.alpha -= 15;
            else
                Projectile.alpha = 0;

            int target = -1;
            #endregion setup

            #region animation
            if (!Main.gamePaused)
                Projectile.frameCounter++;

            if (Projectile.frameCounter >= (State == Attacking? 14 - Projectile.frame * 2 : 6))
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
                Projectile.velocity.X = (float)Math.Sin(Rottime * 2 * Math.Clamp(MathHelper.Distance(Projectile.position.Y, SentryPos.Y) * 0.15f, 2f, 4.75f)) * Math.Clamp(MathHelper.Distance(Projectile.Center.Y, SentryPos.Y) * 0.0225f, 0, 5.75f);

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

                    RotDist = Main.rand.NextFloat(3f, 6.5f).RandNeg();

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
                    Shoot(Main.npc[target]);
                }
                Charge = Math.Clamp(Charge + 0.01f, 0, 1);
            }
            #endregion behavior
        }

        private void Shoot(NPC target) 
        {
            State = Attacking;
            Charge = 0;

            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(SoundID.DD2_LightningBugZap with { Volume = 1.25f }, Projectile.Center);
                SoundEngine.PlaySound(Jellyfisher.JellyfishLightningSound, Projectile.Center);

                for (float k = 0; k < MathHelper.TwoPi; k += Main.rand.NextFloat(0.35f, 0.75f))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(0, -4), DustID.PortalBoltTrail, new Vector2(2.5f).RotatedBy(k) * Main.rand.NextFloat(0.9f, 1.5f), 120, new Color(255, 200, 250).Alpha(80), Main.rand.NextFloat(0.85f, 1.75f));
                    dust.fadeIn = Main.rand.NextFloat(0.75f, 1.5f);
                    dust.velocity.Y *= 0.75f;
                    dust.noGravity = true;
                }
            } //effects

            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 enemyVector = target.Center - Projectile.Center;
                enemyVector.Normalize();
                enemyVector *= 9f;

                Projectile projectile = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, enemyVector, ProjectileType<JellyfishLightning>(), Projectile.damage, Projectile.knockBack, Projectile.owner, target.whoAmI);
                projectile.DamageType = DamageClass.Summon;
            } //projectile
        }

        public override bool? CanDamage() => State != Idle && Projectile.Counter() >= 10;
        public override bool? CanHitNPC(NPC target) => State != Idle && !target.friendly && Projectile.Counter() >= 10;

        public override bool PreDrawExtras()
        {
            float size = 0.85f;
            float glopacity = 0.75f;

            Color color1 = new Color(63, 74, 255) { A = 80 } * 0.85f;
            Color color2 = new Color(197, 145, 255) { A = 80 } * 0.85f;

            if (State != Swimming) 
            {
                //dark bloom
                Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center + new Vector2(0, -4) - Main.screenPosition, null, Color.Black * Projectile.Opacity * Projectile.localAI[0] * 0.85f, 0, bloomTex.Size() / 2, Projectile.scale * (0.8f + (float)(Math.Sin(Rottime * 3) * 0.15f)) * 0.85f, SpriteEffects.None, 0f);
                Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center + new Vector2(0, -4) - Main.screenPosition, null, Color.Black * Projectile.Opacity * Projectile.localAI[0] * 0.85f, 0, bloomTex.Size() / 2, Projectile.scale * (0.8f + (float)(Math.Sin(Rottime * 3) * -0.15f)) * 0.85f, SpriteEffects.None, 0f);

                //bloom
                Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center + new Vector2(0, -4) - Main.screenPosition, null, Color.Lerp(color2, color1, (float)(Math.Sin(Rottime * 8) / 2f) + 0.5f) with { A = 0 } * Projectile.Opacity * Projectile.localAI[0] * glopacity, 0, bloomTex.Value.Size() / 2, Projectile.scale * (0.8f + (float)(Math.Sin(Rottime * 3) * 0.15f)) * size, SpriteEffects.None, 0f);
                Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center + new Vector2(0, -4) - Main.screenPosition, null, Color.Lerp(color2, color1, (float)(Math.Sin(Rottime * 8) / 2f) + 0.5f) with { A = 0 } * Projectile.Opacity * Projectile.localAI[0] * glopacity, 0, bloomTex.Value.Size() / 2, Projectile.scale * (0.8f + (float)(Math.Sin(Rottime * 3) * -0.15f)) * size, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = texture.Frame(Main.projFrames[Projectile.type], 2, Projectile.frame, State != Attacking? 0 : 1);

            Color color1 = new Color(63, 74, 255) { A = 80 };
            Color color2 = new Color(171, 131, 255) { A = 80 };

            //Main.spriteBatch.StartBlendState(BlendState.Additive, DrawContext.InWorld, SpriteSortMode.Deferred);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + rect.Size() / 2 + new Vector2(0, 2);

                //afterimage
                Main.EntitySpriteDraw(texture, drawPos, rect, Color.Lerp(color1, color2, (float)(Math.Sin(Rottime + Main.GlobalTimeWrappedHourly * 10f - k) / 2f) + 0.5f).MultiplyAlpha(0.65f - (k * 0.05f)) * Projectile.Opacity * (0.7f - k / (Projectile.oldPos.Length + 4f)), Projectile.oldRot[k], rect.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            }
            //jellyfish
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, Color.White.Alpha(120) * Projectile.Opacity, Projectile.rotation, rect.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, Color.White.Alpha(0) * Projectile.Opacity * (float)Math.Sin(Rottime * 6) * 0.3f, Projectile.rotation, rect.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

            //Main.spriteBatch.ResetBlendState();
            return false;
        }
        
        public override void OnKill(int timeLeft)
        {
            Projectile.TryGetOwner(out Player owner);

            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PortalBoltTrail, 0, 0, 0, new Color(197, 145, 255) { A = 80 }, Main.rand.NextFloat(0.65f, 0.9f));
                dust.velocity.Y += -3;
            }

            for (int i = 0; i < Main.rand.Next(7, 9); i++)
            {
                Dust dust = TwinkleDust.Spawn(new LightDust.LightDustData(0.925f), Projectile.position, new Vector2(Projectile.width, Projectile.height), Vector2.Zero, Main.rand.Next(0, 15), new Color(197, 145, 255) { A = 0 } * Main.rand.NextFloat(0.5f, 0.85f), Main.rand.NextFloat(0.5f, 0.8f));
                //Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustType<TwinkleDust>(), 0, 0, 0, new Color(197, 145, 255) { A = 120 }, Main.rand.NextFloat(0.75f, 1f));
                dust.shader = GameShaders.Armor.GetShaderFromItemId(owner.GetModPlayer<TurretPaintPlayer>().dyeItem);
                dust.fadeIn = Main.rand.NextFloat(1.9f, 2.75f);
                dust.velocity *= Main.rand.NextFloat(0.85f, 1.35f);
                dust.noGravity = true;
            }

            if (!Main.dedServ)
                SoundEngine.PlaySound(SoundID.Item112, Projectile.Center);
        }
    }
    
    public class JellyfishLightning : ModProjectile
    {
        public override string Texture => "GoldLeaf/Items/Ocean/Jellyfisher/JellyfishLightning";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.SentryShot[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;

            Projectile.AddElements([Element.Water, Element.Thunder]);
        }
        public override void SetDefaults()
        {
            Projectile.damage = 10;
            Projectile.DamageType = DamageClass.Summon;

            Projectile.width = Projectile.height = 4;
            Projectile.penetrate = maxTargets;

            Projectile.timeLeft = Projectile.extraUpdates = 100;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.Opacity = 0f;
        }

        const int maxTargets = 3;
        const int RangeLoss = 100;
        const float MultihitPenalty = 0.7f;

        private Vector2 SentryPos;
        bool HasSetup = false;

        int[] targets = new int[maxTargets];
        ref float Target => ref Projectile.ai[0];
        int HomingRange = 450;
        NPC TargetNPC => Main.npc[(int)Target];

        public override void AI()
        {
            Projectile.TryGetOwner(out Player player);
            
            if (!HasSetup)
            {
                SentryPos = Projectile.position;
                HasSetup = true;

                for (int i = 0; i < targets.Length; i++)
                    targets[i] = -1;

                Projectile.netUpdate = true;
            }//setup

            float targetDistance = 8000f;
            
            if (TargetNPC == null || targets.Contains(TargetNPC.whoAmI))
            {
                Projectile.netUpdate = true;
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    float range = Vector2.Distance(Projectile.Center, npc.Center);
                    if (range < targetDistance && range <= HomingRange && npc.CanBeChasedBy(Projectile) && !targets.Contains(npc.whoAmI) && !npc.friendly && npc.active && 
                        Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                    {
                        Target = npc.whoAmI;
                        targetDistance = range;
                    }
                }
                if (TargetNPC == null || targets.Contains((int)Target) || !Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, TargetNPC.position, TargetNPC.width, TargetNPC.height)) 
                {
                    Projectile.Kill();
                }
            }//targeting

            Projectile.velocity = Vector2.Normalize(TargetNPC.Center - Projectile.Center) * 10f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.rand.NextBool() && !Main.dedServ)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, newColor: new Color(255, 200, 250).Alpha(80));
                dust.noGravity = true;
            }//effects
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(SentryPos);
            writer.Write(HasSetup);
            writer.Write(HomingRange);
            writer.WriteArray(targets);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SentryPos = reader.ReadVector2();
            HasSetup = reader.ReadBoolean();
            HomingRange = reader.ReadInt32();
            targets = reader.ReadArray();
        }

        public override bool? CanHitNPC(NPC target) => TargetNPC == target && !targets.Contains(target.whoAmI) && !target.friendly;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < maxTargets; i++)
            {
                if (targets[i] == -1)
                {
                    targets[i] = target.whoAmI;
                    
                    HomingRange = Math.Max(HomingRange - RangeLoss, 50);
                    Projectile.damage = (int)(Projectile.damage * MultihitPenalty);
                    break;
                }
            }
            Projectile.netUpdate = true;
        }
        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
            {
                for (int i = 0; i < maxTargets; i++)
                {
                    if (targets[i] == -1)
                        break;

                    if (i == 0)
                        Dust.NewDustPerfect(SentryPos, DustType<JellyLightningDust>(), Main.npc[targets[i]].Center);
                    else
                        Dust.NewDustPerfect(Main.npc[targets[i - 1]].Center, DustType<JellyLightningDust>(), Main.npc[targets[i]].Center);
                }
            }
        }
    }

    public class JellyLightningDust : ModDust
    {
        private static Asset<Texture2D> nodeTex;
        private static Asset<Texture2D> bloomTex;
        public override void Load()
        {
            nodeTex = Request<Texture2D>("GoldLeaf/Textures/GlowSolid0");
            bloomTex = Request<Texture2D>("GoldLeaf/Textures/Glow");
        }

        public override string Texture => "GoldLeaf/Items/Ocean/Jellyfisher/JellyfishLightning";
        private static readonly Gradient defaultGradient = new([(new Color(255, 231, 253), 0.1f), (new Color(255, 156, 224), 0.25f), (new Color(197, 145, 255), 0.4f), (new Color(63, 74, 255), 0.55f)]);

        public override void OnSpawn(Dust dust)
        {
            if (dust.fadeIn < 1f)
                dust.fadeIn = 1f;

            dust.alpha = 0;
            dust.frame = Texture2D.Frame(1, 3, 0, Main.rand.Next(3));

            if (dust.customData is not Gradient)
                dust.customData = defaultGradient;
        }

        public override bool Update(Dust dust)
        {
            dust.alpha = (int)MathHelper.SmoothStep(dust.alpha, 280, (dust.fadeIn * 0.15f) + ((1 - dust.Opacity()) * 0.15f));
            if (dust.alpha > 255) dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            if (dust.customData is Gradient gradient)
            {
                float length = 1f / dust.frame.Width * dust.position.Distance(dust.velocity);

                //lightning
                Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.Black * (dust.Opacity() + 0.05f) * 0.5f,
                        dust.position.DirectionTo(dust.velocity).ToRotation(), Vector2.Zero + new Vector2(0, dust.frame.Height / 2f), new Vector2(length, 1.75f * dust.Opacity()), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, gradient.GetColor(1 - dust.Opacity()) * (dust.Opacity() + 0.15f),
                        dust.position.DirectionTo(dust.velocity).ToRotation(), Vector2.Zero + new Vector2(0, dust.frame.Height / 2f), new Vector2(length, 1.5f * dust.Opacity()), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, gradient.GetColor(0.45f + 1 - dust.Opacity()).Alpha(0) * (dust.Opacity() + 0.05f),
                        dust.position.DirectionTo(dust.velocity).ToRotation(), Vector2.Zero + new Vector2(0, dust.frame.Height / 2f), new Vector2(length, 0.25f + (1 - dust.Opacity())), SpriteEffects.None, 0f);
                //node
                Main.spriteBatch.Draw(bloomTex.Value, dust.velocity - Main.screenPosition, null, Color.Black * (dust.Opacity() + 0.2f) * 0.85f, 0,
                    bloomTex.Size() / 2f, (dust.Opacity() * 0.5f) + 0.25f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(nodeTex.Value, dust.velocity - Main.screenPosition, null, gradient.GetColor((1 - dust.Opacity()) * 0.65f).Alpha(160) * (dust.Opacity() + 0.35f), 0, 
                    nodeTex.Size() / 2f, (dust.Opacity() * 0.45f) + 0.2f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(nodeTex.Value, dust.velocity - Main.screenPosition, null, gradient.GetColor((1 - dust.Opacity()) * 0.35f).Alpha(0) * (dust.Opacity() + 0.35f) * 0.7f, 0, 
                    nodeTex.Value.Size() / 2f, dust.Opacity() * 0.65f, SpriteEffects.None, 0f);
            }
            return false;
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