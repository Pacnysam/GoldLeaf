using System;
using System.Collections.Generic;
using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using Terraria.ModLoader;
using Terraria;
using Terraria.GameContent.ItemDropRules;
namespace GoldLeaf.Core
{
    public static class GoldLeafConditions
    {
        public static Condition InSurface = new Condition("Mods.GoldLeaf.GoldLeafConditions.InSurface", () => !Main.LocalPlayer.ShoppingZone_BelowSurface);

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
    }
}
