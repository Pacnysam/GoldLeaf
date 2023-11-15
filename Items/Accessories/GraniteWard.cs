using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GoldLeaf.Items.Accessories
{
	public class GraniteWard : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.GoldLeaf.hjson file.

		public override void SetDefaults()
		{
			Item.damage = 14;
            Item.width = 34;
            Item.height = 40;
            Item.value = 10000;
            Item.rare = 2;
        }

		/*public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}*/
	}
}