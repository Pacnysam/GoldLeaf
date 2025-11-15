using GoldLeaf.Core;
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Items.Misc;
using GoldLeaf.Items.Potions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Food
{
    public class CrescentRoll : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;

            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

            ItemID.Sets.IsFood[Type] = true;
            ItemID.Sets.FoodParticleColors[Item.type] = [
                new Color(205, 161, 148),
                new Color(184, 131, 140),
                new Color(163, 116, 123)
                //new Color(125, 83, 103)
            ];
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(26, 24, BuffID.WellFed2, TimeToTicks(8, 0));
            Item.rare = ItemRarityID.Green;
        }

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(BuffType<MinionSpeedPotionBuff>(), TimeToTicks(5, 0));
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<FallenMoon>());
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }
    }
}
