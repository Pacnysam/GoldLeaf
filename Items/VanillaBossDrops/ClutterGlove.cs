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
using GoldLeaf.Items.Grove;
using Terraria.GameContent.ItemDropRules;
using Terraria.UI;
using Terraria.ModLoader.IO;
using System.Collections.Generic;
using GoldLeaf.Core.CrossMod;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;

namespace GoldLeaf.Items.VanillaBossDrops
{
    public abstract class ClutterGlove : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 2;
            Item.crit = 8;

            Item.width = 28;
            Item.height = 38;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.shoot = ProjectileID.PurificationPowder;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Blue;

            Item.SetElement(Element.Arcane, -1);
            Item.SetElement(Element.Nature, -1);

            Item.shootSpeed = 11f;

            SafeSetDefaults();
        }

        public virtual void SafeSetDefaults() { }

        public override bool? CanChooseAmmo(Item ammo, Player player) => ammo.type == ItemID.ShadowScale || ammo.type == ItemID.TissueSample;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-3, 3)));
        }
    }

    public class ClutterGloveCorruption : ClutterGlove 
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
            Item.AddElements([Element.Shadow]);
        }
        public override void SafeSetDefaults()
        {
            Item.useAmmo = ItemID.ShadowScale;
            Item.shoot = ProjectileType<ClutterScale>();
            Item.shootSpeed = 9f;
        }

        public override bool? CanChooseAmmo(Item ammo, Player player)
        {
            if (ammo.type == ItemID.ShadowScale)
            {
                return true;
            }
            return null;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (ammo.type == ItemID.ShadowScale)
            {
                return Main.rand.NextBool();
            }
            else
            { 
                return true; 
            }
        }
    }

    public class ClutterGloveCrimson : ClutterGlove
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
            Item.AddElements([Element.Blood]);
        }
        public override void SafeSetDefaults()
        {
            Item.useAmmo = ItemID.TissueSample;
            Item.shoot = ProjectileType<ClutterTissue>();
        }

        public override bool? CanChooseAmmo(Item ammo, Player player)
        {
            if (ammo.type == ItemID.TissueSample)
            {
                return true;
            }
            return null;
        } 
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (ammo.type == ItemID.TissueSample)
            {
                return Main.rand.NextBool();
            }
            else
            {
                return true;
            }
        }
    }

    public class ClutterScale : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

            Projectile.AddElements([Element.Shadow]);
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 16;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.ArmorPenetration = 10;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 1;

            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            if (Projectile.Counter() > 25)
                Projectile.velocity.Y += 0.025f + (Projectile.Counter() * 0.0025f);

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            Projectile.spriteDirection = Projectile.direction;
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * (float)(((float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length) / 2);

                Main.EntitySpriteDraw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0f);
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.75f);
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, Projectile.Center);

                for (int k = 0; k < 12; ++k)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, Main.rand.NextFloat(-3, 3) + Projectile.velocity.X / 3, Main.rand.NextFloat(-3, 3) + Projectile.velocity.Y / 3, 0, new Color(), 1f);
                    Main.dust[dust].noGravity = true;
                }
            }
        }
    }

    public class ClutterTissue : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

            Projectile.AddElements([Element.Blood]);
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 16;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            
            Projectile.DamageType = DamageClass.Ranged;
        }

        ref float Bounces => ref Projectile.ai[0];

        public override void OnSpawn(IEntitySource source)
        {
            Bounces = Main.rand.Next(2, 5);
            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            if (Projectile.Counter() >= 10)
                Projectile.velocity.Y += 0.15f;

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            Projectile.spriteDirection = Projectile.direction;
            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * (float)(((float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length) / 2);

                Main.EntitySpriteDraw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0f);
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(SoundID.DD2_LightningBugHurt, Projectile.Center);

            NPC.HitInfo hitInfo = new()
            {
                Damage = (int)(damageDone * 0.7),
                DamageType = DamageClass.Ranged,
                Crit = hit.Crit,
                Knockback = 0,
                HitDirection = hit.HitDirection,
            };

            Main.player[Projectile.owner].StrikeNPCDirect(target, hitInfo);
        }

        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
            {
                for (int i = 0; i < 12; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimslime, Main.rand.NextFloat(-3, 3) + Projectile.velocity.X / 3, Main.rand.Next(-3, 3) + Projectile.velocity.Y / 3);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;

            Projectile.velocity *= 0.7f;

            if (!Main.dedServ)
            {
                for (int k = 0; k < 4; ++k)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimslime);
                }
                SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);
            }
            if (Bounces <= 0)
            {
                Projectile.Kill(); return true;
            }
            else
            {
                Bounces--; return false;
            }
        }
    }

    public class ClutterAmmo : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            switch (item.type)
            {
                case ItemID.ShadowScale:
                    {
                        item.ammo = item.type;
                        break;
                    }
                case ItemID.TissueSample:
                    {
                        item.ammo = item.type;
                        break;
                    }
            }
        }
    }
}
