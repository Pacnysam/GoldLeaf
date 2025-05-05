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

namespace GoldLeaf.Items.Meteor
{
	public class MeteorMissile : ModItem
	{
        public override void SetDefaults()
		{
            Item.width = 30;
            Item.height = 30;

            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;

            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MeteorMissilePlayer>().meteorMissile = true;
            player.GetModPlayer<MeteorMissilePlayer>().cooldown--;
        }

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.MeteoriteBar, 10);
            recipe.AddIngredient(ItemID.Gel, 25);
            recipe.AddIngredient(ItemID.Grenade, 6);
            recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
    }

    public class MeteorMissileP : ModProjectile
    {
        private int counter = 0;
        private int bounces = 1;
        private int damage;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = 36;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.ArmorPenetration = 3;
            Projectile.timeLeft = 360;

            ProjectileID.Sets.DontApplyParryDamageBuff[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            damage = Projectile.damage;
            Projectile.damage = 0;
            if (damage == 0) damage = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            Projectile.rotation += (float)Math.Sin(GoldLeafWorld.rottime) * (counter * 0.001f);

            Projectile.spriteDirection = Projectile.direction;
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }

            if (counter == 24) { Projectile.damage = damage; }

            if (counter > 24) { Projectile.velocity *= 1.01f;}
            if (counter > 120) { Projectile.velocity *= 1.003f; Projectile.timeLeft -= 2; Projectile.GetGlobalProjectile<GoldLeafProjectile>().gravity += 0.0002f; }

            counter++;

            if (counter % 3 == 0) 
            {
                Dust.NewDustPerfect(Projectile.position, DustType<HotSmokeFast>(), Projectile.velocity * -0.4f, 90, default, 0.2f);
            }

            if (counter % 16 == 0)
            {
                Projectile.localAI[0] = 0f;
                for (int j = 0; j < 16; j++)
                {
                    Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
                    vector2 += -Utils.RotatedBy(Vector2.UnitY, (j * 3.141591734f / 6f), default) * new Vector2(8f, 16f);
                    vector2 = Utils.RotatedBy(vector2, (Projectile.rotation - 1.57079637f), default);
                    int num8 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Torch, 0f, 0f, 160, new Color(), 1f);
                    Main.dust[num8].noGravity = true;
                    Main.dust[num8].position = Projectile.Center + vector2;
                    Main.dust[num8].velocity = Projectile.velocity * 0.1f;
                    Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }

            if (bounces <= 0)
            {
                Projectile.Kill(); return true;
            }
            else
            {
                bounces--; return false;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextFloat(6.28f).ToRotationVector2() * Main.rand.NextFloat(1.6f, 2.4f), ProjectileType<Ember>(), 0, 0, Projectile.owner).scale = Main.rand.NextFloat(0.85f, 1.15f);
            }

            for (int j = 0; j < 10; j++)
            {
                var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(16, 16), 0, 0, DustType<HotSmoke>());
                dust.velocity = Main.rand.NextVector2Circular(1.8f, 1.8f);
                dust.scale = Main.rand.NextFloat(0.35f, 0.65f);
                dust.alpha = 35 + Main.rand.Next(50);
                dust.rotation = Main.rand.NextFloat(6.28f);
            }
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
        }
    }

    public class MeteorMissilePlayer : ModPlayer
    {
        public bool meteorMissile = false;
        public int cooldown = 0;

        public override void ResetEffects()
        {
            meteorMissile = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.LocalPlayer;

            if (Main.myPlayer == Player.whoAmI) 
            {
                if (meteorMissile && target.life <= 0 && hit.Crit && cooldown <= 0)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        Vector2 pos = (new Vector2(0, -3)).RotatedBy(((k / 28f) - 3) * 6.28f);

                        int p = Projectile.NewProjectile(Player.GetSource_OnHit(target), player.Center, pos, ProjectileType<MeteorMissileP>(), (int)(damageDone * 0.35f), 3, player.whoAmI);
                        Main.projectile[p].DamageType = hit.DamageType;
                        Main.projectile[p].frame = k;
                    }
                    cooldown = 240;
                    SoundEngine.PlaySound(SoundID.Item61, Player.Center);
                }
                else if (meteorMissile && target.life <= 0 && cooldown <= 0)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Vector2 pos = (new Vector2(0, -3)).RotatedBy(((k / 24f) - 2) * 6.28f);

                        int p = Projectile.NewProjectile(Player.GetSource_OnHit(target), player.Center, pos, ProjectileType<MeteorMissileP>(), (int)(damageDone * 0.7f), 3, player.whoAmI);
                        Main.projectile[p].DamageType = hit.DamageType;
                        Main.projectile[p].frame = k;
                    }
                    cooldown = 120;
                    SoundEngine.PlaySound(SoundID.Item61, Player.Center);
                }
                else if (meteorMissile && hit.Crit && cooldown <= 0 && damageDone >= 6)
                {
                    int p = Projectile.NewProjectile(Player.GetSource_OnHit(target), player.Center, new Vector2(Main.rand.NextFloat(-4f, 4f), -3f), ProjectileType<MeteorMissileP>(), (int)(damageDone * 0.35f), 3, player.whoAmI);
                    Main.projectile[p].DamageType = hit.DamageType;
                    cooldown = 10;
                    SoundEngine.PlaySound(SoundID.Item61, Player.Center);
                }
            }
        }
    }
}