using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;

namespace GoldLeaf.Core
{
    public class GoldLeafNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public float slowDegree;
		private float slowAmt;
		public bool BeingSlowed => slowDegree > 0;

        public override void ResetEffects(NPC npc)
        {
            slowDegree = 0;
        }

        public override bool PreAI(NPC npc)
        {
            if (BeingSlowed)
            {
                if ((slowAmt += slowDegree) >= 1)
                {
                    slowAmt--;
                    return true;
                }
                return false;
            }
            return true;
        }

        public override void PostAI(NPC npc)
        {
            if (BeingSlowed)
                npc.position -= npc.velocity * (float)(1f - slowAmt) * (npc.boss ? 0.25f : 1f);
        }
    }
}
