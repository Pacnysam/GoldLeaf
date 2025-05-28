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

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.LocalPlayer.GetModPlayer<EchobarkPlayer>().echobarkArmor && Main.LocalPlayer.GetModPlayer<EchobarkPlayer>().echobarkDefense > 0) 
            {
                string text = Language.GetTextValue("Mods.GoldLeaf.SetBonuses.EchobarkDefense", Item.defense, Main.LocalPlayer.GetModPlayer<EchobarkPlayer>().echobarkDefense);

                int index = tooltips.IndexOf(tooltips.Find(n => n.Name == "Defense"));

                for (int i = 0; i < text.Length; i++)
                {
                    tooltips.Insert(index + 1, new TooltipLine(Mod, "Defense", text));
                    tooltips.Remove(tooltips.Find(n => n.Name == "Defense"));
                }
            }
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
            if (echobarkArmor && info.Damage >= 20) 
            {
                echobarkDefense += Math.Clamp(info.Damage / 20, 1, 5);
                echobarkCooldown = 240;
            }
        }
    }
}
