using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GoldLeaf.Items.Blizzard
{
	public class ArcticFlower : ModItem
	{
        public override void SetDefaults()
		{
			Item.damage = 35;
			Item.DamageType = DamageClass.Summon;
			Item.width = 30;
			Item.height = 36;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(0, 1, 20, 0);
            Item.rare = ItemRarityID.Blue;
			Item.autoReuse = false;
        }

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BorealWood, 15);
			recipe.AddRecipeGroup("GoldLeaf:GoldBars", 5);
            //recipe.AddIngredient(ItemType<AuroraShard>(), 8);
            recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
    }
}