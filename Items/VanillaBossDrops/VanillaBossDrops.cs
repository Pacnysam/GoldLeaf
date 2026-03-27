using Terraria.GameContent;
using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using Microsoft.Xna.Framework;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

using GoldLeaf.Tiles.Decor;

namespace GoldLeaf.Items.VanillaBossDrops
{
    public class VanillaBossDrops : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                case NPCID.KingSlime:
                    {
                        LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

                        notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<Goonai>(), 1, 30, 40));
                        npcLoot.Add(notExpertRule);
                        break;
                    }
                case NPCID.EyeofCthulhu:
                    {
                        LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

                        notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<Lunar>(), 20));
                        npcLoot.Add(notExpertRule);
                        break;
                    }
                case NPCID.BrainofCthulhu:
                    {
                        LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

                        notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<ClutterGloveCrimson>(), 3));
                        npcLoot.Add(notExpertRule);
                        break;
                    }
                case NPCID.EaterofWorldsHead:
                case NPCID.EaterofWorldsBody:
                case NPCID.EaterofWorldsTail:
                    {
                        LeadingConditionRule leadingConditionRule = new(new Conditions.LegacyHack_IsABoss());
                        LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

                        leadingConditionRule.OnSuccess(notExpertRule);
                        notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<ClutterGloveCorruption>(), 3));

                        npcLoot.Add(leadingConditionRule);
                        break;
                    }
            }
        }
    }

    public class VanillaBossBagLoot : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            switch (item.type)
            {
                case ItemID.KingSlimeBossBag:
                    {
                        itemLoot.Add(ItemDropRule.Common(ItemType<Goonai>(), 1, 45, 60));
                        break;
                    }
                case ItemID.EyeOfCthulhuBossBag:
                    {
                        itemLoot.Add(ItemDropRule.Common(ItemType<Lunar>(), 10));
                        break;
                    }
                case ItemID.EaterOfWorldsBossBag:
                    {
                        itemLoot.Add(ItemDropRule.Common(ItemType<ClutterGloveCorruption>(), 2));
                        break;
                    }
                case ItemID.BrainOfCthulhuBossBag:
                    {
                        itemLoot.Add(ItemDropRule.Common(ItemType<ClutterGloveCrimson>(), 2));
                        break;
                    }
            }
        }
    }
}
