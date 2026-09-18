using GoldLeaf.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using ReLogic.Content;
using Terraria.Localization;
using System.Collections.Generic;
using System.Linq;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;

namespace GoldLeaf.Items.Grove.Echobark.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class EchobarkHelmet : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;

            Item.defense = 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<Echobark>(), 20)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => EchobarkPlayer.AddEchobarkDefenseTooltip(Item, tooltips);
        public override bool IsArmorSet(Item head, Item body, Item legs) => head.type == ItemType<EchobarkHelmet>() && body.type == ItemType<EchobarkBreastplate>() && legs.type == ItemType<EchobarkGreaves>();
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.GoldLeaf.SetBonuses.Echobark");
            player.GetModPlayer<EchobarkPlayer>().echobarkArmor = true;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class EchobarkBreastplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;

            Item.defense = 2;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<Echobark>(), 30)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => EchobarkPlayer.AddEchobarkDefenseTooltip(Item, tooltips);
    }

    [AutoloadEquip(EquipType.Legs)]
    public class EchobarkGreaves : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;

            Item.defense = 2;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<Echobark>(), 25)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => EchobarkPlayer.AddEchobarkDefenseTooltip(Item, tooltips);
    }

    public class EchobarkPlayer : ModPlayer 
    {
        private static int MaxEchobarkDefense => 15;
        private static int EchobarkDecayTime => 30;

        public bool echobarkArmor = false;
        public int echobarkDefense = 0;
        public int echobarkCooldown = 0;

        public static void AddEchobarkDefenseTooltip(Item item, List<TooltipLine> tooltips)
        {
            if (Main.LocalPlayer.GetModPlayer<EchobarkPlayer>().echobarkArmor && Main.LocalPlayer.GetModPlayer<EchobarkPlayer>().echobarkDefense > 0)
            {
                foreach (TooltipLine line in tooltips.Where(x => x.Mod == "Terraria" && x.Name == "Defense"))
                    line.Text = item.defense + " (+" + Main.LocalPlayer.GetModPlayer<EchobarkPlayer>().echobarkDefense + ")" + Language.GetTextValue("LegacyTooltip.25");
            }
        }

        public override void ResetEffects()
        {
            echobarkArmor = false;
        }

        public override void PostUpdateEquips()
        {
            echobarkDefense = Math.Clamp(echobarkDefense, 0, MaxEchobarkDefense);

            if (echobarkArmor)
                Player.statDefense += echobarkDefense;

            if (echobarkCooldown-- <= 0 && echobarkDefense > 0)
            {
                echobarkCooldown = EchobarkDecayTime;
                echobarkDefense--;
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (echobarkArmor) 
            {
                int amount = Math.Clamp((int)(info.Damage / 12.5f) + 1, 1, MaxEchobarkDefense);

                if (echobarkDefense < 15)
                {
                    int amountAdded;
                    for (amountAdded = 0; amountAdded < amount && echobarkDefense < MaxEchobarkDefense; amountAdded++)
                    {
                        echobarkDefense++;
                    }

                    if (amountAdded > 0)
                        CombatText.NewText(Player.Hitbox, Color.LightGray, amountAdded, true, true);
                }
                echobarkCooldown = TimeToTicks(8);
                Player.statDefense += echobarkDefense;
            }
        }
    }
}
