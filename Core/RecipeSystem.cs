using System;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using GoldLeaf.Items.Grove.Wood;

namespace GoldLeaf.Core
{
    public class RecipeSystem : ModSystem
    {
        public override void PostAddRecipes()
        {
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];

                /*if (recipe.TryGetIngredient(ItemType<EveDroplet>(), out Item aether) && (recipe.HasCondition(Condition.NearLava) || recipe.HasTile(TileID.Furnaces)))
                {
                    recipe.AddOnCraftCallback(RecipeCallbacks.AetherCraftEffect);
                }*/
                if (recipe.TryGetIngredient(ItemID.FallenStar, out Item star))
                {
                    recipe.AddOnCraftCallback(RecipeCallbacks.StarCraftEffect);
                }
                if (recipe.HasTile(TileID.Anvils))
                {
                    recipe.AddOnCraftCallback(RecipeCallbacks.AnvilCraftEffect);
                }
                if (recipe.HasTile(TileID.Solidifier) || (recipe.TryGetIngredient(ItemID.Gel, out Item gel) && (!recipe.HasRecipeGroup(RecipeGroup.recipeGroupIDs["Wood"]) || !recipe.HasIngredient(ItemID.Torch))))
                {
                    recipe.AddOnCraftCallback(RecipeCallbacks.GelCraftEffect);
                }
            }       
        }

        public override void AddRecipeGroups()
        {
            RecipeGroup BaseGroup(object GroupName, int[] Items) //yoinked from spirit
            {
                string Name = "";
                Name += GroupName switch
                {
                    //modcontent items
                    int i => Lang.GetItemNameValue((int)GroupName),
                    //vanilla item ids
                    short s => Lang.GetItemNameValue((short)GroupName),
                    //custom group names
                    _ => GroupName.ToString(),
                };
                return new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " " + Name, Items);
            }

            RecipeGroup woodGrp = RecipeGroup.recipeGroups[RecipeGroup.recipeGroupIDs["Wood"]];
            woodGrp.ValidItems.Add(ItemType<Echobark>());

            RecipeGroup.RegisterGroup("GoldLeaf:CopperBars", BaseGroup(ItemID.CopperBar, [ItemID.CopperBar, ItemID.TinBar]));

            RecipeGroup.RegisterGroup("GoldLeaf:SilverBars", BaseGroup(ItemID.SilverBar, [ItemID.SilverBar, ItemID.TungstenBar]));

            RecipeGroup.RegisterGroup("GoldLeaf:GoldBars", BaseGroup(ItemID.GoldBar, [ItemID.GoldBar, ItemID.PlatinumBar]));

            RecipeGroup.RegisterGroup("GoldLeaf:CobaltBars", BaseGroup(ItemID.CobaltBar, [ItemID.CobaltBar, ItemID.PalladiumBar]));

            RecipeGroup.RegisterGroup("GoldLeaf:MythrilBars", BaseGroup(ItemID.MythrilBar, [ItemID.MythrilBar, ItemID.OrichalcumBar]));

            RecipeGroup.RegisterGroup("GoldLeaf:AdamantiteBars", BaseGroup(ItemID.AdamantiteBar, [ItemID.AdamantiteBar, ItemID.TitaniumBar]));
        }
    }
}
