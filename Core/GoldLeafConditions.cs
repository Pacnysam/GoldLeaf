using System;
using System.Collections.Generic;
using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using Terraria.ModLoader;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using GoldLeaf.Items.Vanity;
using GoldLeaf.Items.VanillaBossDrops;
using Terraria.Localization;
namespace GoldLeaf.Core
{
    public class GoldLeafConditions
    {
        public static Condition InSurface = new("Mods.GoldLeaf.GoldLeafConditions.InSurface", () => !Main.LocalPlayer.ShoppingZone_BelowSurface);
        public static Condition UsingGameboy = new("Mods.GoldLeaf.GoldLeafConditions.UsingGameboy", () => Main.LocalPlayer.GetModPlayer<GameboyPlayer>().gameboy);
        public static Condition HasClutterGlove = new("Mods.GoldLeaf.GoldLeafConditions.HasClutterGlove", () => Main.LocalPlayer.HasItem(ItemType<ClutterGloveCorruption>()) || Main.LocalPlayer.HasItem(ItemType<ClutterGloveCrimson>()));

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

        public static Condition LearnedRecipe(int item) => new("Mods.GoldLeaf.GoldLeafConditions.LearnedRecipe", () => RecipeSystem.learnedRecipes.Contains(item));
    }
}
