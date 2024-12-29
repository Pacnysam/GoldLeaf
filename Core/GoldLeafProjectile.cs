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
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core
{
    public class GoldLeafProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool canSpawnMiniHearts = true;
        public bool canSpawnMiniStars = true;

        public float gravity = 0f;
        public int gravityDelay = 0;
        public int lifesteal;
        public int lifestealMax;
        public int counter = 0;

        public void ChangeDebuffDuration(NPC target, float amount)
        {
            for (int i = 0; target.buffType[1] != 0; i++)
            {
                target.AddBuff(target.buffType[i],target.buffTime[i] + (int)(target.buffTime[i]*amount));
            }

            //look through buff list for debuffs and extend them
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

        public override void AI(Projectile projectile)
        {
            if (gravity != 0f && counter >= gravityDelay)
            {
                projectile.velocity += new Vector2(0, gravity);
            }

            if ((projectile.type == ProjectileID.FallingStar) && counter % 15 == 0) 
            {
                DustHelper.DrawStar(projectile.Center, DustID.FireworkFountain_Blue, 5, 1.8f, 0.65f, 0.55f, 0.6f, 0.5f, true, 0, -1);
            }

            counter++;
        }

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (projectile.type == ProjectileID.BeeHive && Main.rand.NextBool(10)) 
            {
                Item.NewItem(projectile.GetSource_Death(), projectile.Hitbox, ItemType<HiveCarcass>());
                SoundEngine.PlaySound(SoundID.Item87, projectile.Center);
                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Honey2, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-3f, -6f));
                }
            }
        }
    }
}
