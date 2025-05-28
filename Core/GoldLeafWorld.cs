
using GoldLeaf.Items.Grove;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;

namespace GoldLeaf.Core
{
    public class GoldLeafWorld : ModSystem
    {
        public static float rottime = 0;
        
        public override void PostUpdateEverything()
        {
            if (rottime >= Math.PI * 2)
                rottime = 0;
        }

        public override void PreUpdateWorld()
		{
            rottime += (float)Math.PI / 60;
        }
	}
}
