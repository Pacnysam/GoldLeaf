using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace GoldLeaf.Core.CrossMod
{
    public static class RedemptionHelper
    {
        public static bool RedemptionLoaded(out Mod redemption)
        {
            return ModLoader.TryGetMod("Redemption", out redemption);
        }

        /// <summary>
        /// Adds given list of elements, must be put in SetStaticDefaults
        /// </summary>
        public static void AddElements(this Entity entity, Element[] elements)
        {
            if (RedemptionLoaded(out Mod redemption))
            {
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
        }
        
        /// <summary>
        /// Override type is behavior; 1 adds an element, -1 removes an element, 0 to resets it
        /// Can be put anywhere, does not reset
        /// </summary>
        public static void SetElement(this Entity entity, Element element, int overrideType = 1)
        {
            if (RedemptionLoaded(out Mod redemption))
            {
                if (entity is Item item)
                    redemption.Call("elementOverrideItem", item, (int)element, (sbyte)overrideType);
                else if (entity is Projectile proj)
                    redemption.Call("elementOverrideProj", proj, (int)element, (sbyte)overrideType);
                else if (entity is NPC npc)
                    redemption.Call("elementOverrideNPC", npc, (int)element, (sbyte)overrideType);
            }
        }
        
        /// <summary>
        /// Can be put anywhere, does not reset
        /// </summary>
        public static bool HasElement(this Entity entity, Element element)
        {
            if (RedemptionLoaded(out Mod redemption))
            {
                if (entity is Item item)
                    return (bool)redemption.Call("hasElementItem", item, (int)element);
                else if (entity is Projectile proj)
                    return (bool)redemption.Call("hasElementProj", proj, (int)element);
                else if (entity is NPC npc)
                    return (bool)redemption.Call("hasElementNPC", npc, (int)element);
                return false;
            }
            return false;
        }
        
        /// <summary>
        /// Can be put anywhere, does not reset
        /// </summary>
        public static int GetFirstElement(Entity entity, bool ignoreExplosive = false)
        {
            if (RedemptionLoaded(out Mod redemption))
            {
                if (entity is Item item)
                    return (int)redemption.Call("getFirstElementItem", item, ignoreExplosive);
                else if (entity is NPC npc)
                    return (int)redemption.Call("getFirstElementNPC", npc, ignoreExplosive);
                else if (entity is Projectile proj)
                    return (int)redemption.Call("getFirstElementProj", proj, ignoreExplosive);
            }
            return 0;
        }
        
        /// <summary>
        /// Must be put in SetStaticDefaults
        /// </summary>
        public static void SetBluntSwing(this Item item)
        {
            if (RedemptionLoaded(out Mod redemption))
                redemption.Call("addItemToBluntSwing", item.type);
        }
        
        /// <summary>
        /// Can be put anywhere, does not reset
        /// applyBonus applies the bonus, should be true
        /// </summary>
        public static bool SetSlash(this Item item, bool applyBonus = true)
        {
            if (RedemptionLoaded(out Mod redemption))
                return (bool)redemption.Call("setSlashBonus", item, applyBonus);
            return false;
        }
        
        /// <summary>
        /// Can be put anywhere, does not reset
        /// applyBonus applies the bonus, should be true
        /// </summary>
        public static bool SetAxe(this Entity entity, bool applyBonus = true)
        {
            if (RedemptionLoaded(out Mod redemption))
            {
                if (entity is Item item)
                    return (bool)redemption.Call("setAxeBonus", item, applyBonus);
                if (entity is Projectile proj)
                    return (bool)redemption.Call("setAxeProj", proj, applyBonus);
            }
            return false;
        }
        
        /// <summary>
        /// Can be put anywhere, does not reset
        /// applyBonus applies the bonus, should be true
        /// </summary>
        public static bool SetHammer(this Entity entity, bool applyBonus = true)
        {
            if (RedemptionLoaded(out Mod redemption))
            {
                if (entity is Item item)
                    return (bool)redemption.Call("setHammerBonus", item, applyBonus);
                if (entity is Projectile proj)
                    return (bool)redemption.Call("setHammerProj", proj, applyBonus);
            }
            return false;
        }
        
        /// <summary>
        /// Can be put anywhere, does not reset
        /// applyBonus applies the bonus, should be true
        /// </summary>
        public static bool SetSpear(this Projectile proj, bool applyBonus = true)
        {
            if (RedemptionLoaded(out Mod redemption))
                return (bool)redemption.Call("setSpearProj", proj, applyBonus);
            
            return false;
        }
        
        /// <summary>
        /// Can be put anywhere, does not reset
        /// normal HitInfo stuff, chance is 1 in X
        /// </summary>
        public static bool Decapitate(NPC target, ref int damageDone, ref bool crit, int chance = 200)
        {
            if (RedemptionLoaded(out Mod redemption))
                return (bool)redemption.Call("decapitation", target, damageDone, crit, chance);
            return false;
        }

        /// <summary>
        /// Adds given list of elements, must be put in SetStaticDefaults
        /// Use NPCAttributes for the string input
        /// </summary>
        public static void AddNpcAttribute(int npcType, string attributeString)
        {
            if (RedemptionLoaded(out Mod redemption))
                redemption.Call("addNPCToElementTypeList", attributeString, npcType);
        }

        public enum Element : int
        {
            None = 0,
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
        public static class NPCAttributes
        {
            public const string Skeleton = "Skeleton";
            public const string HumanoidSkeleton = "SkeletonHumanoid";
            public const string Humanoid = "Humanoid";
            public const string Undead = "Undead";
            public const string Spirit = "Spirit";
            public const string Plantlike = "Plantlike";
            public const string Demon = "Demon";
            public const string Cold = "Cold";
            public const string Hot = "Hot";
            public const string Wet = "Wet";
            public const string Draconic = "Dragonlike";
            public const string Inorganic = "Inorganic";
            public const string Robotic = "Robotic";
            public const string Armed = "Armed";
            public const string Hallowed = "Hallowed";
            public const string Dark = "Dark";
            public const string Blood = "Blood";
            public const string Slime = "Slime";
        }
    }
}
