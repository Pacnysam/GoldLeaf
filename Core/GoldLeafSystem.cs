using GoldLeaf.Items.GemSickles;
using GoldLeaf.Items.Grove;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;

namespace GoldLeaf.Core
{
    public class GoldLeafSystem : ModSystem
    {
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
        }
    }
}
