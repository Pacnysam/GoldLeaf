using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Effects.Dusts;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using GoldLeaf.Core;
using Mono.Cecil;
using Terraria.DataStructures;
using System;
using System.Diagnostics.Metrics;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using Terraria.GameContent.Drawing;
using GoldLeaf.Core.CrossMod;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;
using GoldLeaf.Items.Grove.Boss.AetherComet;


namespace GoldLeaf.Items.Grove
{
	public class EveDroplet : ModItem
	{
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        int dustTime = 12;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
            AmmoID.Sets.IsSpecialist[Type] = true;

            Item.AddElements([Element.Arcane, Element.Nature]);
        }

        public override void SetDefaults()
		{
			Item.damage = 11;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 20;
			Item.height = 24;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
            Item.ammo = Item.type;
            Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
            Item.value = Item.buyPrice(0, 0, 1, 25);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;

            Item.shoot = ProjectileType<EveDropletP>();
			Item.shootSpeed = 12f;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-5, 5))) * Main.rand.NextFloat(0.9f, 1.1f);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Item.timeSinceItemSpawned % dustTime == 0)
            {
                Dust dust = Dust.NewDustPerfect(Item.Center + new Vector2(0f, Item.height * -0.1f) + Main.rand.NextVector2CircularEdge(Item.width * 0.6f, Item.height * 0.6f) * (0.3f + Main.rand.NextFloat() * 0.5f), 
                    DustID.SilverFlame, new Vector2(0f, (0f - Main.rand.NextFloat()) * 0.3f - 1.5f), 0, new Color(201, 60, 210).Alpha(), Main.rand.NextFloat(0.4f, 0.6f));
                dust.fadeIn = 1.1f;
                dust.noGravity = true;
                dust.noLight = true;

                dustTime = Main.rand.Next(15, 30);
            }
        }

        public override void PostUpdate()
        {
            float glo = 70 * 0.005f;
            glo *= Main.essScale * 0.5f;

            if (!Main.dedServ)
                Lighting.AddLight((int)((Item.position.X + (Item.width / 2)) / 16f), (int)((Item.position.Y + (Item.height / 2)) / 16f), ((238 / 255) * 0.2f) * glo, ((107 / 255) * 0.2f) * glo, ((192 / 255) * 0.2f) * glo);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Color color = new(201, 60, 210) { A = 0 };
            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, color * ((float)Math.Sin(GoldLeafWorld.rottime) * 0.5f), rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);

            return true;
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            //Color color = new(255, 198, 249) { A = 0 };
            spriteBatch.Draw(TextureAssets.Item[Item.type].Value, Item.Center - Main.screenPosition, null, Color.White.Alpha() * ((float)Math.Sin(GoldLeafWorld.rottime - 0.75f) * 0.75f), rotation, TextureAssets.Item[Item.type].Size() / 2, scale, SpriteEffects.None, 0f);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Color color = new(201, 60, 210) { A = 0 };
            spriteBatch.Draw(glowTex.Value, position, null, color * ((float)Math.Sin(GoldLeafWorld.rottime) * 0.5f), 0, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);

            return true;
        }
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(TextureAssets.Item[Item.type].Value, position, null, ColorHelper.AdditiveWhite() * ((float)Math.Sin(GoldLeafWorld.rottime - 0.75f) * 0.75f), 0, origin, scale, SpriteEffects.None, 0f);
        }
    }

	public class EveDropletP : ModProjectile 
	{
        public override void SetStaticDefaults()
        {
			Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

            Projectile.AddElements([Element.Arcane, Element.Nature]);
        }

        public override void SetDefaults()
        {
            //Projectile.aiStyle = -1;
            Projectile.width = 18;
            Projectile.height = 28;
            Projectile.friendly = true;
			Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Main.rand.Next(50, 150);

            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[0] = Main.rand.NextFloat(0.2f, 0.3f);
            Projectile.ai[1] = Main.rand.Next(15, 30);
            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            if (Projectile.Counter() > Projectile.ai[1])
                Projectile.velocity.Y += Projectile.ai[0];

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            Projectile.spriteDirection = -Projectile.direction;
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }
            if (Main.rand.NextBool(3)) Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<EveDust>());
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Vector2 drawOrigin = new (texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * (float)(((float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length) / 2);

                Main.EntitySpriteDraw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0f);
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.HasBuff(BuffType<AetherFlameBuff>()) && target.IsValid())
                target.AddBuff(BuffType<EveDropletBuff>(), Helper.TimeToTicks(8));
        }

        /*public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!target.HasBuff(BuffType<AetherFlameBuff>()))
                target.AddBuff(BuffType<EveDropletBuff>(), Helper.TimeToTicks(8), false);
        }*/

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(new("GoldLeaf/Sounds/SE/HollowKnight/JellyfishEggPop") { Volume = 0.65f, PitchVariance = 0.4f }, Projectile.Center);
                //SoundEngine.PlaySound(SoundID.Shimmer1, Projectile.Center);

                for (int i = 0; i < 12; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<EveDust>(), Main.rand.NextFloat(-3, 3) + Projectile.velocity.X / 3, Main.rand.Next(-6, 3) + Projectile.velocity.Y / 3);
                }
            }
        }
    }

    public class EveDropletBuff : ModBuff
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

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustType<EveDust>(), 0f, Main.rand.NextFloat(0f, -4f));
            }
        }
    }
    
    public class EveDropletNPC : GlobalNPC 
    {
        public override bool InstancePerEntity => true;

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (npc.IsValid() && npc.HasBuff(BuffType<EveDropletBuff>()))
                EveExplode(npc, hit);
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (npc.IsValid() && npc.HasBuff(BuffType<EveDropletBuff>()))
                EveExplode(npc, hit);
        }

        private static void EveExplode(NPC npc, NPC.HitInfo hit)
        {
            int explosionDamage = 40;
            float explosionVolume = 65f;
            int armorPen = 5;
            bool shouldExplode = false;

            if (npc.HasBuff(BuffID.OnFire))
            {
                if (!Main.dedServ)
                {
                    SoundEngine.PlaySound(SoundID.Item74, npc.Center);
                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/RoR2/EngineerMine") { Volume = 0.4f, Pitch = -0.5f }, npc.Center);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.DelBuff(npc.FindBuffIndex(BuffID.OnFire));
                    npc.netUpdate = true;
                }

                shouldExplode = true;
            }
            if (npc.HasBuff(BuffID.OnFire3))
            {
                explosionDamage = 70;
                explosionVolume = 130f;
                armorPen = 15;

                if (!Main.dedServ)
                {
                    SoundEngine.PlaySound(SoundID.Item74, npc.Center);
                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/RoR2/EngineerMine") { Volume = 0.8f, Pitch = 0.25f }, npc.Center);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.DelBuff(npc.FindBuffIndex(BuffID.OnFire3));
                    npc.netUpdate = true;
                }

                shouldExplode = true;
            }

            if (shouldExplode)
            {
                Projectile explosion = Projectile.NewProjectileDirect(npc.GetSource_Buff(npc.FindBuffIndex(BuffType<EveDropletBuff>())), npc.Center, Vector2.Zero, ProjectileType<AetherBurst>(), explosionDamage, 0.5f, -1, explosionVolume, 0, 1f);
                explosion.DamageType = hit.DamageType;
                explosion.ArmorPenetration = armorPen;
                explosion.netUpdate = true;

                npc.AddBuff(BuffType<AetherFlameBuff>(), Helper.TimeToTicks(5));
                npc.RequestBuffRemoval(BuffType<EveDropletBuff>());

                if (!Main.dedServ)
                {
                    CameraSystem.QuickScreenShake(npc.Center, null, 10 + (explosionVolume * 0.1f), 5f, 30 + (int)(explosionVolume * 0.35f), 1500);
                    CameraSystem.QuickScreenShake(npc.Center, 0f.ToRotationVector2(), 10 + (explosionVolume * 0.1f), 10f, 20 + (int)(explosionVolume * 0.25f), 1500);

                    for (int j = 0; j < 10 + explosionVolume / 6f; j++)
                    {
                        var dust = Dust.NewDustDirect(npc.Center, 0, 0, DustType<SpecialSmoke>(), Scale: Main.rand.NextFloat(0.85f, 1.75f) * Math.Clamp(explosionVolume / 90f, 0.85f, 1.5f));
                        dust.velocity = Main.rand.NextVector2Circular(9.5f, 9.5f) * Math.Clamp(explosionVolume / 60f, 1f, 3f);
                        dust.position += dust.velocity * 3f;
                        dust.velocity *= 0.75f;
                        dust.alpha = 85 + Main.rand.Next(60);
                        dust.customData = SpecialSmoke.aetherSmokeGradient;
                    } //smoke

                    for (int j = 0; j < 10 + explosionVolume / 5f; j++)
                    {
                        Color color = new Color(179, 255, 224).Lerp(new Color(255, 169, 255), Main.rand.NextFloat(0.7f));

                        var dust = Dust.NewDustDirect(npc.Center, 0, 0, DustID.FireworksRGB, 0, 0, 10 + Main.rand.Next(60),
                            color.Alpha() * Main.rand.NextFloat(0.45f, 0.85f), Main.rand.NextFloat(0.6f, 0.9f));
                        dust.fadeIn = Main.rand.NextFloat(0.65f, 0.95f);
                        dust.velocity = Main.rand.NextVector2Circular(7f + (explosionVolume / 12.5f), 4.5f + (explosionVolume / 12.5f));
                        dust.velocity.Y -= 2.5f;
                        dust.noGravity = Main.rand.NextBool(2, 5);
                        dust.rotation = Main.rand.NextFloat(6.28f);
                    } //sparks
                }
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.HasBuff(BuffType<EveDropletBuff>())) drawColor = NPC.buffColor(drawColor, 255f/255, 80f/255, 202f/255, 1f);
        }
    }

    /*public class EveDropletPlayer : ModPlayer
    {
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.PvP)
            {
                EveExplode(Player, hurtInfo);
            }
        }

        private static void EveExplode(Player victim, Player.HurtInfo hurtInfo)
        {
            if (!victim.dead && victim.HasBuff(BuffType<EveDropletBuff>()))
            {
                bool didExplode = false;
                if (victim.HasBuff(BuffID.OnFire))
                {
                    int explosion = Projectile.NewProjectile(victim.GetSource_Buff(victim.FindBuffIndex(BuffType<EveDropletBuff>())), victim.MountedCenter, Vector2.Zero, ProjectileType<AetherBurst>(), 30, 0.5f, -1, 65f, 0, 1f);
                    Main.projectile[explosion].netUpdate = true;

                    CameraSystem.AddScreenshake(Main.LocalPlayer, 16, victim.Center);
                    CameraSystem.QuickScreenShake(victim.Center, 0f.ToRotationVector2(), 12.5f, 6.5f, 30, 1500);

                    SoundEngine.PlaySound(SoundID.Item74, victim.Center);
                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/RoR2/EngineerMine") { Volume = 0.4f, Pitch = -0.5f }, victim.Center);

                    victim.ClearBuff(BuffID.OnFire);

                    didExplode = true;
                }
                if (victim.HasBuff(BuffID.OnFire3))
                {
                    int explosion = Projectile.NewProjectile(victim.GetSource_Buff(victim.FindBuffIndex(BuffType<EveDropletBuff>())), victim.MountedCenter, Vector2.Zero, ProjectileType<AetherBurst>(), 55, 0.5f, -1, 130f, 0, 1f);
                    Main.projectile[explosion].netUpdate = true;

                    CameraSystem.AddScreenshake(Main.LocalPlayer, 24, victim.Center);
                    CameraSystem.QuickScreenShake(victim.Center, 0f.ToRotationVector2(), 12.5f, 9f, 30, 1500);

                    SoundEngine.PlaySound(SoundID.Item74, victim.Center);
                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/RoR2/EngineerMine") { Volume = 0.8f, Pitch = 0.25f }, victim.Center);

                    victim.ClearBuff(BuffID.OnFire3);

                    didExplode = true;
                }

                if (didExplode)
                {
                    victim.AddBuff(BuffType<AetherFlameBuff>(), Helper.TimeToTicks(5));
                    victim.ClearBuff(BuffType<EveDropletBuff>());

                    for (int j = 0; j < 10; j++)
                    {
                        var dust = Dust.NewDustDirect(victim.Center, 0, 0, DustType<SpecialSmoke>());
                        dust.velocity = Main.rand.NextVector2Circular(7.5f, 7.5f);
                        dust.scale = Main.rand.NextFloat(0.9f, 1.2f);
                        dust.alpha = 20 + Main.rand.Next(60);
                        dust.rotation = Main.rand.NextFloat(6.28f);
                        dust.customData = SpecialSmoke.aetherSmokeGradient;
                    }
                }
            }
        }
    }*/
}