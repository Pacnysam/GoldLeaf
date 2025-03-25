using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using System.Collections.Generic;

namespace GoldLeaf.Items.Misc
{
    public class BottledShimmer : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.AmmoReservationPotion);

            Item.width = 20;
            Item.height = 26;

            Item.buffType = BuffID.Shimmer;
            //Item.buffTime = TimeToTicks(2);

            //ItemID.Sets.ShimmerTransformToItem[ItemID.BottledWater] = Item.type;
        }

        /*public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAt(tooltips.IndexOf(tooltips.Find(n => n.Name == "BuffTime")));
        }*/

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Bottle);
            recipe.AddCondition(Condition.NearShimmer);
            recipe.AddCondition(Condition.DownedPlantera);
            recipe.Register();
        }
    }
}
