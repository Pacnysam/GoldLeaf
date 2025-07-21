using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System.Diagnostics.Metrics;
using System;
using GoldLeaf.Effects.Dusts;
using Terraria.Graphics.Effects;
using Terraria.DataStructures;
using ReLogic.Content;
using Terraria.Localization;
using GoldLeaf.Items.Blizzard.Armor;
using System.Collections.Generic;
using GoldLeaf.Items.Blizzard;
using System.Linq;
using GoldLeaf.Prefixes;
using Terraria.Utilities;

namespace GoldLeaf.Items.Grove.Wood.Armor
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

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ItemType<EchobarkHelmet>() && body.type == ItemType<EchobarkBreastplate>() && legs.type == ItemType<EchobarkGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.GoldLeaf.SetBonuses.Echobark", player.GetModPlayer<EchobarkPlayer>().echobarkDefense);
            player.GetModPlayer<EchobarkPlayer>().echobarkArmor = true;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class EchobarkGreaves : ModItem
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
                .AddIngredient(ItemType<Echobark>(), 25)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class EchobarkPlayer : ModPlayer 
    {
        public bool echobarkArmor = false;
        public int echobarkDefense = 0;
        public int echobarkCooldown = 0;

        public override void ResetEffects()
        {
            echobarkArmor = false;
            echobarkDefense = Math.Clamp(echobarkDefense, 0, 15);
        }

        public override void PostUpdateEquips()
        {
            if (echobarkArmor) 
            {
                Player.statDefense += echobarkDefense;
            }

            echobarkCooldown--;
            if (echobarkCooldown <= 0 && echobarkDefense > 0)
            {
                echobarkCooldown = 60;
                echobarkDefense--;
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (echobarkArmor && info.Damage >= 10) 
            {
                int amount = Math.Clamp(info.Damage / 12, 1, 10);

                int amountAdded;
                if (echobarkDefense < 15)
                {
                    for (amountAdded = 0; (amountAdded < amount) && (echobarkDefense < 15); amountAdded++)
                    {
                        echobarkDefense ++;
                    }

                    if (amountAdded > 0)
                        CombatText.NewText(Player.Hitbox, Color.LightGray, amountAdded, true, true);
                }
                echobarkCooldown = TimeToTicks(5);
            }
        }
    }

    public class EchobarkArmorItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemType<EchobarkHelmet>() || entity.type == ItemType<EchobarkBreastplate>() || entity.type == ItemType<EchobarkGreaves>();
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine tipLine = tooltips.Find(n => n.Name == "Defense");

            if (tipLine != null)
            {
                if (Main.LocalPlayer.GetModPlayer<EchobarkPlayer>().echobarkArmor && Main.LocalPlayer.GetModPlayer<EchobarkPlayer>().echobarkDefense > 0)
                {
                    int updatedDefense = item.defense + Main.LocalPlayer.GetModPlayer<EchobarkPlayer>().echobarkDefense;
                    string text = updatedDefense + " (+" + Main.LocalPlayer.GetModPlayer<EchobarkPlayer>().echobarkDefense + ")" + Language.GetTextValue("LegacyTooltip.25");

                    tooltips.ElementAt(tooltips.IndexOf(tipLine)).Text = text;
                }
            }
        }
    }
}
