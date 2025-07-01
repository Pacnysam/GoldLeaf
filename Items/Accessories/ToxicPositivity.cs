using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Items.Grove;
using System.Diagnostics.Metrics;
using System;
using GoldLeaf.Effects.Dusts;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using GoldLeaf.Items.Blizzard;

namespace GoldLeaf.Items.Accessories
{
	public class ToxicPositivity : ModItem
	{
        public override void SetDefaults()
		{
            Item.width = 28;
            Item.height = 32;

            Item.value = Item.buyPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Lime;

            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ToxicPositivePlayer>().toxicPositivity = true;
            player.GetModPlayer<ToxicPositivePlayer>().toxicPositiveItem = Item;
        }
    }
    
    public class ToxicPositivityBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;

            //BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.Poisoned);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<ToxicPositiveNPC>().toxicPositive = true;
            npc.GetGlobalNPC<GoldLeafNPC>().defenseModFlat -= Math.Clamp(npc.buffTime[buffIndex] / 60, 1, 30);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<ToxicPositivePlayer>().toxicPositive = true;
            player.statDefense -= 3;
            player.DefenseEffectiveness *= 0.9f;
        }
    }

    public class ToxicPositivityEffect : ModProjectile 
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BrainOfConfusion);
            
            Projectile.width = 28;
            Projectile.height = 42;

            Projectile.damage = 0;

            Projectile.ArmorPenetration = 9999;
        }
    }

    public class ToxicPositivePlayer : ModPlayer
    {
        public bool toxicPositivity = false;
        public bool toxicPositive = false;
        public Item toxicPositiveItem;

        public override void ResetEffects()
        {
            toxicPositivity = false;
            toxicPositiveItem = null;
            toxicPositive = false;
        }

        public override void Load()
        {
            FirstStrikePlayer.OnFirstStrike += ToxicFirstStrike;
        }
        public override void Unload()
        {
            FirstStrikePlayer.OnFirstStrike -= ToxicFirstStrike;
        }

        private void ToxicFirstStrike(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (player.GetModPlayer<ToxicPositivePlayer>().toxicPositivity && !target.buffImmune[BuffType<ToxicPositivityBuff>()])
            {
                Projectile.NewProjectile(player.GetSource_OnHit(target), target.position, Vector2.Zero, ProjectileType<ToxicPositivityEffect>(), 0, 0, player.whoAmI);
                target.AddBuff(BuffType<ToxicPositivityBuff>(), TimeToTicks(Math.Clamp(player.CountBuffs(), 1, 30)));
                SoundEngine.PlaySound(SoundID.Zombie60, target.Center);
            }
        }

        /*public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (toxicPositivity && Main.rand.NextBool(6 + (Main.LocalPlayer.CountBuffs() * 2), 100) && !target.buffImmune[BuffType<ToxicPositivityBuff>()])
            {
                target.AddBuff(BuffType<ToxicPositivityBuff>(), 180 + (Main.LocalPlayer.CountBuffs() * 20));

                //SoundEngine.PlaySound(SoundID.Zombie60, target.Center);

                Projectile.NewProjectile(Player.GetSource_OnHit(target), target.position, Vector2.Zero, ProjectileType<ToxicPositivityEffect>(), 0, 0, Player.whoAmI);
            }
        }*/

        public override void UpdateBadLifeRegen()
        {
            if (toxicPositive)
            {
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegenTime = 0f;
                Player.lifeRegen -= 2 * 20;
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (toxicPositive) 
            {
                genDust = false;
                playSound = false;
                damageSource = PlayerDeathReason.ByCustomReason(QuickDeathReason("ToxicPositivity", Player, 3));
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if ((toxicPositive || Player.poisoned || Player.venom) && Main.myPlayer == Player.whoAmI) 
            {
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/PoisonDeath"), Player.Center);
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.GasTrap,
                                        new ParticleOrchestraSettings { PositionInWorld = Player.Center },
                                        Player.whoAmI);
            }
        }
        
        /*public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (toxicPositivity && Main.rand.NextBool(Main.LocalPlayer.CountBuffs() * 2, 100))
            {
                Main.LocalPlayer.AddBuff(BuffType<ToxicPositivityBuff>(), 60);
                SoundEngine.PlaySound(SoundID.DD2_KoboldIgnite, Player.Center);

                Projectile.NewProjectile(Player.GetSource_Accessory_OnHurt(toxicPositiveItem, npc), Player.position, Vector2.Zero, ProjectileType<ToxicPositivityEffect>(), 0, 0, Player.whoAmI);
            }
        }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (toxicPositivity && Main.rand.NextBool(Main.LocalPlayer.CountBuffs() * 2, 100))
            {
                Main.LocalPlayer.AddBuff(BuffType<ToxicPositivityBuff>(), 60);
                SoundEngine.PlaySound(SoundID.DD2_KoboldIgnite, Player.Center);

                Projectile.NewProjectile(Player.GetSource_Accessory_OnHurt(toxicPositiveItem, proj), Player.position, Vector2.Zero, ProjectileType<ToxicPositivityEffect>(), 0, 0, Player.whoAmI);
            }
        }*/

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (toxicPositive && !Player.dead) 
            { 
                r = 0.65f; g = 1f; b = 0.75f;

                if (Main.rand.NextBool(8))
                {
                    int gore = Gore.NewGore(null, new Vector2(Main.rand.Next((int)Player.position.X, (int)Player.position.X + Player.width) - 17, Main.rand.Next((int)Player.position.Y, (int)Player.position.Y + Player.height) - 17), Vector2.Zero, GoreID.FartCloud1, 1);
                    Main.gore[gore].alpha = 145;
                    Main.gore[gore].velocity *= 0.3f;

                    drawInfo.GoreCache.Add(gore);
                }
                if (Main.rand.NextBool(30))
                {
                    Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Poisoned, 0f, 0f, 120, default, 0.4f);
                    dust.noGravity = true;
                    dust.fadeIn = 2f;

                    drawInfo.DustCache.Add(dust.dustIndex);
                }
                if (Main.rand.NextBool(15))
                {
                    Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.SparksMech, 0f, 0f, 120, new Color(182, 255, 182), 0.2f);
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(58, Main.LocalPlayer);
                    dust.fadeIn = 3f;

                    drawInfo.DustCache.Add(dust.dustIndex);
                }
            }
        }
    }

    public class ToxicPositiveNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool toxicPositive = false;

        public override void ResetEffects(NPC npc)
        {
            toxicPositive = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (toxicPositive)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                npc.lifeRegen -= 2 * 20;
                if (damage < 10)
                {
                    damage = 10;
                }
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (toxicPositive) drawColor = NPC.buffColor(drawColor, 0.65f, 1f, 0.75f, 1f);

            if (toxicPositive && Main.rand.NextBool(8))
            {
                int gore = Gore.NewGore(null, new Vector2(Main.rand.Next((int)npc.position.X, (int)npc.position.X + npc.width) - 17, Main.rand.Next((int)npc.position.Y, (int)npc.position.Y + npc.height) - 17), Vector2.Zero, GoreID.FartCloud1, 1);
                Main.gore[gore].alpha = 145;
                Main.gore[gore].velocity *= 0.3f;
            }
            if (toxicPositive && Main.rand.NextBool(30)) 
            {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Poisoned, 0f, 0f, 120, default, 0.4f);
                dust.noGravity = true;
                dust.fadeIn = 2f;
            }
            if (toxicPositive && Main.rand.NextBool(15))
            {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.SparksMech, 0f, 0f, 120, new Color(182, 255, 182), 0.2f);
                dust.noGravity = true;
                dust.noLight = true;
                dust.shader = GameShaders.Armor.GetSecondaryShader(58, Main.LocalPlayer);
                dust.fadeIn = 3f;
            }
        }
    }
}