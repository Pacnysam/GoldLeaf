using Terraria.GameContent;
using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using Microsoft.Xna.Framework;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using GoldLeaf.Items.Misc;

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
                        notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.SlimeStaff, 3));
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
                case NPCID.SkeletronHead:
                    {
                        LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());
                        notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.Bone, 1, 15, 25));
                        npcLoot.Add(notExpertRule);
                        break;
                    }
                case NPCID.HallowBoss:
                    {
                        LeadingConditionRule noDaylightRule = new(new IsNighttime());
                        npcLoot.Add(noDaylightRule);
                        noDaylightRule.OnSuccess(ItemDropRule.Common(ItemID.EmpressBlade, 20));
                        break;
                    }
            }
        }
    }

    public class IsDaytime : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => Main.dayTime;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }
    public class IsNighttime : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => !Main.dayTime;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }

    public class VanillaBossBagLoot : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            switch (item.type)
            {
                case ItemID.SlimeStaff: 
                    {
                        item.rare = ItemRarityID.Blue;
                        item.damage = 10;
                        break;
                    }
            }
        }

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            switch (item.type)
            {
                case ItemID.KingSlimeBossBag:
                    {
                        itemLoot.Add(ItemDropRule.Common(ItemID.SlimeStaff, 2));
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
                case ItemID.SkeletronBossBag:
                    {
                        itemLoot.Add(ItemDropRule.Common(ItemID.Bone, 1, 25, 35));
                        break;
                    }
            }
        }
    }
}
