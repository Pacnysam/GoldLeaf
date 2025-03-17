using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Enums;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;

namespace GoldLeaf.Prefixes
{
    public class PrefixItem : GlobalItem 
    {
        public override bool InstancePerEntity => true;

        /*public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            switch (item.prefix)
            {
                case PrefixID.Dull:
                    crit += 20;
                    break;
            }
        }

        public override void HoldItem(Item item, Player player)
        {
            switch (item.prefix) 
            {
                case PrefixID.Dull:
                    item.GetGlobalItem<GoldLeafItem>().critDamageMult -= 0.25f;
                    item.ArmorPenetration -= 4;
                    break;
                case PrefixID.Nimble:
                    item.GetGlobalItem<GoldLeafItem>().critDamageMult -= 0.1f;
                    break;
                case PrefixID.Deadly:
                    item.GetGlobalItem<GoldLeafItem>().critDamageMult += 0.1f;
                    break;
                case PrefixID.Zealous:
                case PrefixID.Legendary:
                case PrefixID.Unreal:
                case PrefixID.Mythical:
                    item.GetGlobalItem<GoldLeafItem>().critDamageMult += 0.15f;
                    break;
                case PrefixID.Broken:
                    item.GetGlobalItem<GoldLeafItem>().critDamageMult -= 0.2f;
                    break;
                case PrefixID.Forceful:
                    item.GetGlobalItem<GoldLeafItem>().critDamageMult += 0.25f;
                    break;
                case PrefixID.Sharp:
                case PrefixID.Powerful:
                    item.GetGlobalItem<GoldLeafItem>().critDamageMult += 0.3f;
                    break;
                case PrefixID.Strong:
                    item.GetGlobalItem<GoldLeafItem>().critDamageMult += 0.35f;
                    break;
                case PrefixID.Intimidating:
                    item.GetGlobalItem<GoldLeafItem>().critDamageMult += 0.35f;
                    item.ArmorPenetration += 2;
                    break;
            }
            if (item.prefix == PrefixID.Forceful) 
            {
                item.GetGlobalItem<GoldLeafItem>().critDamageMult += 0.25f;
            }
        }*/

        public override void ModifyTooltips(Item Item, List<TooltipLine> tooltips)
        {
            /*if (PrefixLoader.GetPrefix(Item.prefix) is CustomTooltipPrefix)
            {
                TooltipLine critLine = tooltips.Find(n => n.Name == "Knockback");
                int index = critLine is null ? tooltips.Count - 1 : tooltips.IndexOf(critLine);

                var line = new TooltipLine(GoldLeaf, "CustomPrefix", prefixLine)
                {
                    IsModifier = true,
                    IsModifierBad = false
                };
                tooltips.Insert(index + 1, line);
            }

            if (critDamageMult != 2)
            {
                string[] text =
                [
                    Language.GetTextValue($"{critDamageMult}x critical strike multiplier")
                ];

                TooltipLine critLine = tooltips.Find(n => n.Name == "CritChance");

                if (critLine != null)
                {
                    int index = tooltips.IndexOf(critLine);
                    tooltips.Insert(index + 1, new TooltipLine(Mod, "CritMult", $"{critDamageMult}x critical strike multiplier"));
                }
            }*/
        }
    }
}
