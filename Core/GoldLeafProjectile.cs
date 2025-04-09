using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Misc.Accessories;
using GoldLeaf.Items.Nightshade;
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

        public bool canSpawnMiniHearts = true;
        public bool canSpawnMiniStars = true;
        
        public float critDamageMod = 0f;

        public float gravity = 0f;
        public int gravityDelay = 0;

        public int lifesteal;
        public int lifestealMax;

        public int summonCritChance = 0;

        public DamageClass throwingDamageType = DamageClass.Default;

        public int counter = 0;

        /*public static void ChangeDebuffDuration(NPC target, float amount)
        {
            for (int i = 0; target.buffType[1] != 0; i++)
            {
                target.AddBuff(target.buffType[i],target.buffTime[i] + (int)(target.buffTime[i]*amount));
            }
        }*/

        public override void OnSpawn(Projectile projectile, IEntitySource source) {
			if (source is EntitySource_ItemUse realSource) {
				projectile.GetGlobalProjectile<GoldLeafProjectile>().critDamageMod += realSource.Item.GetGlobalItem<GoldLeafItem>().critDamageMod;
                projectile.netUpdate = true;
			}
		}

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];

            if (lifestealMax >= 1 && lifesteal > 0) 
            {
                lifestealMax--;
                player.Heal(lifesteal);
            }
        }

        /*public override void SetDefaults(Projectile entity)
        {
            if (entity.GetGlobalProjectile<GoldLeafProjectile>().throwingDamageType != DamageClass.Default)
            {
                if (GetInstance<MiscConfig>().ThrowerSupport)
                {
                    entity.DamageType = DamageClass.Throwing;
                }
                else
                {
                    entity.DamageType = entity.GetGlobalProjectile<GoldLeafProjectile>().throwingDamageType;
                }
            }
        }*/

        public override void AI(Projectile projectile)
        {
            if (gravity != 0f && counter >= gravityDelay)
            {
                projectile.velocity.Y += gravity;
            }

            if ((projectile.type == ProjectileID.FallingStar) && counter % 15 == 0) 
            {
                DustHelper.DrawStar(projectile.Center, DustID.FireworkFountain_Blue, 5, 1.8f, 0.65f, 0.55f, 0.6f, 0.5f, true, 0, -1);
            }

            counter++;
        }

        /*public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += critDamageModFlat;
        }*/

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (projectile.type == ProjectileID.BeeHive && Main.rand.NextBool(10)) 
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
            binaryWriter.Write(counter);
            binaryWriter.Write(gravity);
            binaryWriter.Write(critDamageMod);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            counter = binaryReader.ReadInt32();
            gravity = binaryReader.ReadInt32();
            critDamageMod = binaryReader.ReadSingle();
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (projectile.type == ProjectileID.FallingStar)
            {
                DustHelper.DrawStar(projectile.Center, DustID.FireworkFountain_Yellow, 5, 2.6f, 1f, 0.55f, 0.6f, 0.5f, true, 0, -1);
                DustHelper.DrawStar(projectile.Center, DustID.FireworkFountain_Blue,   5, 4.8f, 1.25f, 0.7f, 0.6f, 0.5f, true, 0, -1);
            }

            return base.OnTileCollide(projectile, oldVelocity);
        }
    }
}
