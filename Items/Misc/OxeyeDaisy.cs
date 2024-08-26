using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GoldLeaf.Items.Misc
{
	public class OxeyeDaisy : ModItem
	{
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Oxeye Daisy");
		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs("Loves me... loves me not...");

        public override void SetDefaults()
		{
            Item.value = Item.sellPrice(0, 0, 0, 30);
            Item.rare = ItemRarityID.Blue;

            Item.width = 30;
            Item.height = 32;

            Item.accessory = true;
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