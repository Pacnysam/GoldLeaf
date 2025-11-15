using System;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using GoldLeaf.Items.Grove.Wood;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Items.VanillaBossDrops;
using GoldLeaf.Tiles.Decor;
using Terraria.GameContent.ItemDropRules;
using GoldLeaf.Items;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace GoldLeaf.Core
{
    public class RecipeSystem : ModSystem
    {
        public override void PreWorldGen()
        {
            learnedRecipes.Clear();
        }

        public override void PostAddRecipes()
        {
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];
                /*if (recipe.TryGetIngredient(ItemType<EveDroplet>(), out Item aether) && (recipe.HasCondition(Condition.NearLava) || recipe.HasTile(TileID.Furnaces)))
                {
                    recipe.AddOnCraftCallback(RecipeCallbacks.AetherCraftEffect);
                }*/
                recipe.AddOnCraftCallback(RecipeCallbacks.DyeMinor);


                if (recipe.TryGetIngredient(ItemID.FallenStar, out Item star))
                {
                    recipe.AddOnCraftCallback(RecipeCallbacks.Star);
                }
                if (recipe.HasTile(TileID.Anvils))
                {
                    recipe.AddOnCraftCallback(RecipeCallbacks.Anvil);
                }
                if (recipe.HasTile(TileID.Solidifier) || (recipe.TryGetIngredient(ItemID.Gel, out Item gel) && (!recipe.HasRecipeGroup(RecipeGroup.recipeGroupIDs["Wood"]) || !recipe.HasIngredient(ItemID.Torch))))
                {
                    recipe.AddOnCraftCallback(RecipeCallbacks.Slime);
                }
                /*if (recipe.HasRecipeGroup(RecipeGroup.recipeGroupIDs["GoldLeaf:Dyes"]))
                {
                    recipe.AddOnCraftCallback(RecipeCallbacks.FluidColorful);
                }*/
                
            }       
        }

        public override void AddRecipeGroups()
        {
            RecipeGroup BaseGroup(object GroupName, int[] Items, bool Prefix = true) //yoinked from spirit
            {
                string Name = Prefix? Language.GetTextValue("LegacyMisc.37") + " " : "";

                Name += GroupName switch
                {
                    //modcontent items
                    int i => Lang.GetItemNameValue((int)GroupName),
                    //vanilla item ids
                    short s => Lang.GetItemNameValue((short)GroupName),
                    //custom group names
                    _ => GroupName.ToString(),
                };
                return new RecipeGroup(() => Name, Items);
            }

            RecipeGroup woodGrp = RecipeGroup.recipeGroups[RecipeGroup.recipeGroupIDs["Wood"]];
            woodGrp.ValidItems.Add(ItemType<Echobark>());

            RecipeGroup.RegisterGroup("GoldLeaf:CopperBars", BaseGroup(ItemID.CopperBar, [ItemID.CopperBar, ItemID.TinBar]));
            RecipeGroup.RegisterGroup("GoldLeaf:SilverBars", BaseGroup(ItemID.SilverBar, [ItemID.SilverBar, ItemID.TungstenBar]));
            RecipeGroup.RegisterGroup("GoldLeaf:GoldBars", BaseGroup(ItemID.GoldBar, [ItemID.GoldBar, ItemID.PlatinumBar]));

            RecipeGroup.RegisterGroup("GoldLeaf:EvilBar", BaseGroup(Language.GetTextValue("Mods.GoldLeaf.RecipeGroups.Or", Language.GetTextValue("ItemName.DemoniteBar"), Language.GetTextValue("ItemName.CrimtaneBar")), [ItemID.DemoniteBar, ItemID.CrimtaneBar], false));

            RecipeGroup.RegisterGroup("GoldLeaf:CobaltBars", BaseGroup(ItemID.CobaltBar, [ItemID.CobaltBar, ItemID.PalladiumBar]));
            RecipeGroup.RegisterGroup("GoldLeaf:MythrilBars", BaseGroup(ItemID.MythrilBar, [ItemID.MythrilBar, ItemID.OrichalcumBar]));
            RecipeGroup.RegisterGroup("GoldLeaf:AdamantiteBars", BaseGroup(ItemID.AdamantiteBar, [ItemID.AdamantiteBar, ItemID.TitaniumBar]));

            RecipeGroup.RegisterGroup("GoldLeaf:EvilMaterial", BaseGroup(Language.GetTextValue("Mods.GoldLeaf.RecipeGroups.Or", Language.GetTextValue("ItemName.ShadowScale"), Language.GetTextValue("ItemName.TissueSample")), [ItemID.ShadowScale, ItemID.TissueSample], false));
            RecipeGroup.RegisterGroup("GoldLeaf:JellyfishBait", BaseGroup(Language.GetTextValue("Mods.GoldLeaf.RecipeGroups.Jellyfish"), [ItemID.PinkJellyfish, ItemID.BlueJellyfish, ItemID.GreenJellyfish]));
        }

        public static List<int> learnedRecipes = [];

        public static void LearnRecipie(Item item)
        {
            if (!learnedRecipes.Contains(item.type))
            {
                learnedRecipes.Add(item.type);
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["learnedRecipies"] = learnedRecipes;
        }
        public override void LoadWorldData(TagCompound tag)
        {
            learnedRecipes = (List<int>)tag.GetList<int>("learnedRecipies");
        }
    }
}
