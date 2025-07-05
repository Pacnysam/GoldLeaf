using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace GoldLeaf.Core.CrossMod
{
    public static class ThoriumHelper
    {
        public static bool ThoriumLoaded(out Mod thorium)
        {
            return ModLoader.TryGetMod("ThoriumMod", out thorium);
        }
    }
}
