using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core
{
    public class GoldLeafProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public float gravity = 0f;
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
			projectile.position += new Vector2(0, gravity);
        }
    }
}
