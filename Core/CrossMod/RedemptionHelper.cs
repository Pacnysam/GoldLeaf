using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;

namespace GoldLeaf.Core.CrossMod
{
    public static class RedemptionHelper
    {
        public static void AddElements(this Entity entity, Element[] elements)
        {
            if (!ModLoader.TryGetMod("Redemption", out var redemption))
                return;

            for (int i = 0; i < elements.Length; i++)
            {
                if (entity is Item item)
                    redemption.Call("addElementItem", (int)elements[i], item.type);
                if (entity is Projectile proj)
                    redemption.Call("addElementProj", (int)elements[i], proj.type);
                if (entity is NPC npc)
                    redemption.Call("addElementNPC", (int)elements[i], npc.type);
            }
        }
        public static void SetElements(this Entity entity, Element[] elements, int overrideType = 1) //TODO: fix this
        {
            /*if (!ModLoader.TryGetMod("Redemption", out var redemption))
                return;

            for (int i = 0; i < elements.Length; i++)
            {
            if (entity is Item item)
                redemption.Call("elementOverrideItem", item, (int)elements[i], (byte)overrideType);
            else if (entity is Projectile proj)
                redemption.Call("elementOverrideProj", proj, (int)elements[i], (byte)overrideType);
            else if (entity is NPC npc)
                redemption.Call("elementOverrideNPC", npc, (int)elements[i], (byte)overrideType);
            }*/
        }
        public static bool HasElement(this Entity entity, Element element)
        {
            if (!ModLoader.TryGetMod("Redemption", out var redemption))
                return false;

            if (entity is Item item)
                return (bool)redemption.Call("hasElementItem", item, (int)element);
            else if (entity is Projectile proj)
                return (bool)redemption.Call("hasElementProj", proj, (int)element);
            else if (entity is NPC npc)
                return (bool)redemption.Call("hasElementNPC", npc, (int)element);
            return false;
        }

        public static void SetBluntSwing(int itemType)
        {
            if (!ModLoader.TryGetMod("Redemption", out var redemption))
                return;

            redemption.Call("addItemToBluntSwing", itemType);
        }
        public static bool SetSlash(this Item item, bool setBonus = true)
        {
            if (!ModLoader.TryGetMod("Redemption", out var redemption))
                return false;

            return (bool)redemption.Call("setSlashBonus", item, setBonus);
        }
        public static bool SetAxe(Entity entity, bool setBonus = true)
        {
            if (!ModLoader.TryGetMod("Redemption", out var redemption))
                return false;

            if (entity is Item item)
                return (bool)redemption.Call("setAxeBonus", item, setBonus);
            if (entity is Projectile proj)
                return (bool)redemption.Call("setAxeProj", proj, setBonus);
            return false;
        }
        public static bool SetHammer(Entity entity, bool setBonus = true)
        {
            if (!ModLoader.TryGetMod("Redemption", out var redemption))
                return false;

            if (entity is Item item)
                return (bool)redemption.Call("setHammerBonus", item, setBonus);
            if (entity is Projectile proj)
                return (bool)redemption.Call("setHammerProj", proj, setBonus);
            return false;
        }
        public static bool SetSpear(Projectile proj, bool setBonus = true)
        {
            if (!ModLoader.TryGetMod("Redemption", out var redemption))
                return false;

            return (bool)redemption.Call("setSpearProj", proj, setBonus);
        }

        public enum Element : int
        {
            Arcane = 1,
            Fire = 2,
            Water = 3,
            Ice = 4,
            Earth = 5,
            Wind = 6,
            Thunder = 7,
            Holy = 8,
            Shadow = 9,
            Nature = 10,
            Poison = 11,
            Blood = 12,
            Psychic = 13,
            Celestial = 14,
            Explosive = 15
        }
    }
}
