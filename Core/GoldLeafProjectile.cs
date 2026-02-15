using GoldLeaf.Core.Helpers;
using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Accessories;
using GoldLeaf.Items.Nightshade;
using GoldLeaf.Items.Sky;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core
{
    public class GoldLeafProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public float critDamageMod = 0f;

        public float gravity = 0f; //TODO: remove this, very unnecessary
        public int gravityDelay = 0;

        public int lifesteal; //TODO: remove during vampire bat glowup
        public int lifestealMax;

        public int summonCritChance = 0;

        public DamageClass throwingDamageType = DamageClass.Default;

        public int counter = 0;

        public override void OnSpawn(Projectile projectile, IEntitySource source) {
			if (source is IEntitySource_WithStatsFromItem realSource && realSource.Item.IsWeapon()) 
            {
				projectile.GetGlobalProjectile<GoldLeafProjectile>().critDamageMod += realSource.Item.GetGlobalItem<GoldLeafItem>().critDamageMod;
                projectile.netUpdate = true;
			}
            if (source is EntitySource_Parent parent && parent.Entity is Projectile proj)
            {
                projectile.GetGlobalProjectile<GoldLeafProjectile>().critDamageMod = proj.GetGlobalProjectile<GoldLeafProjectile>().critDamageMod;
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];

            if (lifestealMax >= 1 && lifesteal > 0) //TODO: REMOVE LIFESTEAL
            {
                lifestealMax--;
                player.Heal(lifesteal);
            }
        }

        public override void AI(Projectile projectile)
        {
            if (gravity != 0f && counter >= gravityDelay) //TODO: remove gravity
            {
                projectile.velocity.Y += gravity;
                projectile.netUpdate = true;    
            }
            counter++;
        }

        /*public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += critDamageModFlat;
        }*/

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (projectile.type == ProjectileID.BeeHive && Main.rand.NextBool(15))
            {
                Item.NewItem(projectile.GetSource_DropAsItem(), projectile.Hitbox, ItemType<HiveCarcass>());

                SoundEngine.PlaySound(SoundID.Item87, projectile.Center);
                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Honey2, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-3f, -6f));
                }
            }
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(projectile.extraUpdates);
            binaryWriter.Write(counter);
            binaryWriter.Write(gravity);
            //binaryWriter.Write(gravityDelay);
            binaryWriter.Write(critDamageMod);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            projectile.extraUpdates = binaryReader.ReadInt32();
            counter = binaryReader.ReadInt32();
            gravity = binaryReader.ReadInt32();
            //gravityDelay = binaryReader.ReadInt32();
            critDamageMod = binaryReader.ReadSingle();
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (projectile.type == ProjectileID.FallingStar)
            {
                /*for (int i = 0; i < 2; i++)
                {
                    Gore gore2 = Gore.NewGoreDirect(null, projectile.Center, Vector2.Zero, GoreType<ConstellationGore>());
                    gore2.rotation = MathHelper.ToRadians(Main.rand.NextFloat(240, 420)).RandNeg();
                    gore2.velocity.X *= 2f;
                    gore2.velocity.Y = Main.rand.NextFloat(-3f, 2f);
                    gore2.frame = 2;
                    gore2.alpha += Main.rand.Next(20, 50);
                }*/
                for (int i = 0; i < 6; i++)
                {
                    Gore gore3 = Gore.NewGoreDirect(null, projectile.Center, Vector2.Zero, GoreType<ConstellationGore>());
                    gore3.rotation = MathHelper.ToRadians(Main.rand.NextFloat(580, 780)).RandNeg();
                    gore3.velocity.X *= 1.85f;
                    gore3.velocity.Y = Main.rand.NextFloat(-2f, 1f);
                    gore3.frame = (byte)Main.rand.Next(3);
                    gore3.alpha += Main.rand.Next(0, 30);
                }
            }
            
            return base.OnTileCollide(projectile, oldVelocity);
        }
    }
}
