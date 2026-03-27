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
            void RegisterVarietyGroup(string name, int[] items, bool prefix = false)
            {
                string groupName = "";

                if (items.Length < 2)
                    return;
                for (int i = 0; i < items.Length - 1; i++)
                {
                    ItemID.Search.TryGetName(items[i], out string itemName);
                    ModItem currentItem = GetModItem(items[i]);
                    string localizedName = (currentItem != null) ? currentItem.DisplayName.Value : Language.GetTextValue("ItemName." + itemName);
                    if (i + 2 == items.Length)
                    {
                        ItemID.Search.TryGetName(items[i + 1], out string nextItemName);
                        ModItem nextItem = GetModItem(items[i + 1]);
                        string nextLocalizedName = (nextItem != null) ? nextItem.DisplayName.Value : Language.GetTextValue("ItemName." + nextItemName);

                        groupName += Language.GetTextValue("Mods.GoldLeaf.RecipeGroups.Or", localizedName, nextLocalizedName);
                        break;
                    }
                    groupName += localizedName + ", ";
                }
                RecipeGroup.RegisterGroup(name, BaseGroup(groupName, items, prefix));
            }

            RecipeGroup woodGrp = RecipeGroup.recipeGroups[RecipeGroup.recipeGroupIDs["Wood"]];
            woodGrp.ValidItems.Add(ItemType<Echobark>());

            #region ores
            RegisterVarietyGroup("GoldLeaf:CopperOre", [ItemID.CopperOre, ItemID.TinOre]);
            RegisterVarietyGroup("GoldLeaf:IronOre", [ItemID.SilverOre, ItemID.TungstenOre]);
            RegisterVarietyGroup("GoldLeaf:SilverOre", [ItemID.SilverOre, ItemID.TungstenOre]);
            RegisterVarietyGroup("GoldLeaf:GoldOre", [ItemID.GoldOre, ItemID.PlatinumOre]);

            RegisterVarietyGroup("GoldLeaf:EvilOre", [ItemID.DemoniteOre, ItemID.CrimtaneOre]);

            RegisterVarietyGroup("GoldLeaf:CobaltOre", [ItemID.CobaltOre, ItemID.PalladiumOre]);
            RegisterVarietyGroup("GoldLeaf:MythrilOre", [ItemID.MythrilOre, ItemID.OrichalcumOre]);
            RegisterVarietyGroup("GoldLeaf:AdamantiteOre", [ItemID.AdamantiteOre, ItemID.TitaniumOre]);
            #endregion ores

            #region bars
            RegisterVarietyGroup("GoldLeaf:CopperBar", [ItemID.CopperBar, ItemID.TinBar]);
            RegisterVarietyGroup("GoldLeaf:SilverBar", [ItemID.SilverBar, ItemID.TungstenBar]);
            RegisterVarietyGroup("GoldLeaf:GoldBar", [ItemID.GoldBar, ItemID.PlatinumBar]);

            RegisterVarietyGroup("GoldLeaf:EvilBar", [ItemID.DemoniteBar, ItemID.CrimtaneBar]);

            RegisterVarietyGroup("GoldLeaf:CobaltBar", [ItemID.CobaltBar, ItemID.PalladiumBar]);
            RegisterVarietyGroup("GoldLeaf:MythrilBar", [ItemID.MythrilBar, ItemID.OrichalcumBar]);
            RegisterVarietyGroup("GoldLeaf:AdamantiteBar", [ItemID.AdamantiteBar, ItemID.TitaniumBar]);
            #endregion bars

            RegisterVarietyGroup("GoldLeaf:JellyfishBait", [ItemID.PinkJellyfish, ItemID.BlueJellyfish, ItemID.GreenJellyfish]);
            RegisterVarietyGroup("GoldLeaf:EvilYoyo", [ItemID.CorruptYoyo, ItemID.CrimsonYoyo]);

            RegisterVarietyGroup("GoldLeaf:EvilMushroom", [ItemID.VileMushroom, ItemID.ViciousMushroom]);
            RegisterVarietyGroup("GoldLeaf:EvilMaterial", [ItemID.RottenChunk, ItemID.Vertebrae]);
            RegisterVarietyGroup("GoldLeaf:EvilBossMaterial", [ItemID.ShadowScale, ItemID.TissueSample]);
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
