using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf
{
    public partial class GoldleafWorld : ModWorld
    {
        public static float rottime = 0;

        public static int Timer;

        public override void PreUpdate()
        {
            //World Rotation Timer
            Timer++;
            rottime += (float)Math.PI / 60;
            if (rottime >= Math.PI * 2) rottime = 0;
        }
    }
}