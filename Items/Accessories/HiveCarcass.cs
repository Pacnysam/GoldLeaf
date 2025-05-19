using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Items.Grove;
using System.Diagnostics.Metrics;
using System;
using GoldLeaf.Effects.Dusts;
namespace GoldLeaf.Items.Misc.Accessories
{
    internal class HiveCarcass : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 26;

            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;

            ItemID.Sets.IsAMaterial[Item.type] = false;

            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<HiveCarcassPlayer>().hiveCarcass = true;
        }
    }

    public class HiveCarcassPlayer : ModPlayer
    {
        public bool hiveCarcass = false;

        public override void ResetEffects()
        {
            hiveCarcass = false;
        }
    }

    public class HiveCarcassItem : GlobalItem
    {
        public override bool? UseItem(Item item, Player player)
        {
            if (player.GetModPlayer<HiveCarcassPlayer>().hiveCarcass && item.potion)
            {
                player.AddBuff(BuffID.Honey, 60 * 20);
            }
            return null;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ItemID.BottledHoney, 5);
            recipe.AddIngredient(ItemID.Bottle, 5);
            recipe.AddIngredient(ItemType<HiveCarcass>());
            recipe.Register();
        }
    }
}
