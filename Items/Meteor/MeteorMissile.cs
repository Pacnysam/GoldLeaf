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
using GoldLeaf.Core.CrossMod;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;
using ReLogic.Content;

namespace GoldLeaf.Items.Meteor
{
	public class MeteorMissile : ModItem
	{
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Type] = true;
            ItemSets.Glowmask[Type] = (glowTex, Color.White, true);
        }

        public override void SetDefaults()
		{
            Item.width = 28;
            Item.height = 28;

            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Green;

            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MeteorMissilePlayer>().meteorMissile = true;
        }

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.MeteoriteBar, 10);
            recipe.AddIngredient(ItemID.Grenade, 5);
            recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
    }

    public class MeteorMissileP : ModProjectile
    {
        private int bounces = 1;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontApplyParryDamageBuff[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            Main.projFrames[Projectile.type] = 4;

            Projectile.AddElements([Element.Explosive]);
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.SmallFlying;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.ArmorPenetration = 10;
            Projectile.timeLeft = TimeToTicks(5);
        }

        public override bool? CanHitNPC(NPC target) => Projectile.Counter() > 24 && !target.friendly;

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            Projectile.spriteDirection = Projectile.direction;
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }

            if (Projectile.Counter() > 24) 
                Projectile.velocity *= 1.01f;
            if (Projectile.Counter() > 120) 
                Projectile.velocity *= 1.003f;

            if (!Main.dedServ)
            {
                if (Projectile.Counter() % 3 == 0)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.position, DustType<SpecialSmoke>(), Projectile.velocity * -0.4f, 120, default, 0.35f);
                    dust.fadeIn = 2.5f;
                    dust.noGravity = true;
                    //dust.position -= new Vector2(17f, 18f) * dust.scale;
                } //smoke trail
                if (Projectile.Counter() % 16 == 0)
                {
                    Projectile.localAI[0] = 0f;
                    for (int j = 0; j < 16; j++)
                    {
                        Vector2 vector = Vector2.UnitX * -Projectile.width / 2f;
                        vector += -Vector2.UnitY.RotatedBy((j * MathHelper.Pi / 6f)) * new Vector2(8f, 16f);
                        vector = vector.RotatedBy(Projectile.rotation - MathHelper.PiOver2);
                        Dust dust = Dust.NewDustDirect(Projectile.Center + vector, 0, 0, DustID.Torch, 0f, 0f, 160, new Color(), 1f);
                        dust.noGravity = true;
                        dust.velocity = Projectile.velocity * 0.1f;
                        dust.velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - dust.position) * 1.25f;
                    }
                } //flame rings
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.WallBounce(oldVelocity);

            if (bounces <= 0)
            {
                Projectile.Kill(); 
                return true;
            }
            else
            {
                bounces--; 
                return false;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, TimeToTicks(2.5f));
            Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 2; i++)
                {
                    Projectile projectile = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextFloat(6.28f).ToRotationVector2() * Main.rand.NextFloat(1.6f, 2.4f), ProjectileType<Ember>(), 0, 0, Projectile.owner);
                    projectile.scale = Main.rand.NextFloat(0.85f, 1.15f);
                    projectile.netUpdate = true;
                }
            }
            if (!Main.dedServ)
            {
                for (int j = 0; j < 10; j++)
                {
                    var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustType<SpecialSmoke>(), Scale: Main.rand.NextFloat(0.6f, 0.85f));
                    dust.velocity = Main.rand.NextVector2Circular(3f, 3f);
                    dust.velocity.Y -= Main.rand.NextFloat(2f);
                    dust.position += dust.velocity * 3f;
                    dust.alpha = 45 + Main.rand.Next(45);
                }
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Volume = 0.7f }, Projectile.Center);
            }
        }
    }

    public class MeteorMissilePlayer : ModPlayer
    {
        public bool meteorMissile = false;
        public int cooldown = 0;

        public override void ResetEffects() => meteorMissile = false;

        public override void PostUpdateMiscEffects()
        {
            if (cooldown > 0) cooldown--;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer == Player.whoAmI && cooldown <= 0) 
            {
                if (meteorMissile && target.life <= 0 && hit.Crit)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        Vector2 pos = (new Vector2(0, -3)).RotatedBy(((k / 28f) - 3) * 6.28f);

                        Projectile projectile = Projectile.NewProjectileDirect(Player.GetSource_OnHit(target), Player.Center, pos, ProjectileType<MeteorMissileP>(), Math.Max(1, (int)(damageDone * 0.7f)), 3, Player.whoAmI);
                        projectile.DamageType = hit.DamageType;
                        projectile.frame = k;
                        projectile.netUpdate = true;
                    }
                    cooldown = 240;

                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundID.Item61, Player.Center);
                }
                else if (meteorMissile && target.life <= 0)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Vector2 pos = (new Vector2(0, -3)).RotatedBy(((k / 24f) - 2) * 6.28f);

                        Projectile projectile = Projectile.NewProjectileDirect(Player.GetSource_OnHit(target), Player.Center, pos, ProjectileType<MeteorMissileP>(), Math.Max(1, (int)(damageDone * 0.7f)), 3, Player.whoAmI);
                        projectile.DamageType = hit.DamageType;
                        projectile.frame = k;
                        projectile.netUpdate = true;
                    }
                    cooldown = 120;

                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundID.Item61, Player.Center);
                }
                else if (meteorMissile && hit.Crit && damageDone >= 5)
                {
                    Projectile projectile = Projectile.NewProjectileDirect(Player.GetSource_OnHit(target), Player.Center, new Vector2(Main.rand.NextFloat(-4f, 4f), -3f), ProjectileType<MeteorMissileP>(), Math.Max(1, (int)(damageDone * 0.35f)), 3, Player.whoAmI);
                    projectile.DamageType = hit.DamageType;
                    projectile.netUpdate = true;

                    cooldown = 10;
                    
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundID.Item61, Player.Center);
                }
            }
        }
    }
}