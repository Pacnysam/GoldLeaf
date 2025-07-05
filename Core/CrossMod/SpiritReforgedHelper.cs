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
    public static class SpiritReforgedHelper
    {
        public static bool SpiritLoaded(out Mod spirit)
        {
            return ModLoader.TryGetMod("SpiritReforged", out spirit);
        }
        
        /// <summary>
        /// Adds a potion to underground potion vats
        /// Must be put in SetStaticDefaults?
        /// Setting spawnsNaturally to false prevents them from generating underground
        /// </summary>
        public static void AddPotionVat(this Item item, Color color, bool spawnsNaturally = true)
        {
            if (SpiritLoaded(out Mod spirit))
                spirit.Call("AddPotionVat", item.type, color, !spawnsNaturally);
        }

        /// <summary>
        /// Checks if a player has a backpack, can go anywhere
        /// </summary>
        public static bool HasBackpack(this Player player)
        {
            if (SpiritLoaded(out Mod spirit))
                return (bool)spirit.Call("HasBackpack", player);
            return false;
        }
    }
}
