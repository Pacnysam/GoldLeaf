using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GoldLeaf.Items.Misc
{
	public class MeteorMissile : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.GoldLeaf.hjson file.

		public override void SetDefaults()
		{
			Item.damage = 26;
            Item.width = 30;
            Item.height = 30;
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