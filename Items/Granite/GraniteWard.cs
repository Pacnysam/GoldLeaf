using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GoldLeaf.Items.Granite
{
	public class GraniteWard : ModItem
	{
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Granite Ward");
		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs("Dealing damage or mining generates a decaying energy barrier that keeps enemies away");

        public override void SetDefaults()
		{
			Item.damage = 1;

            ItemID.Sets.ToolTipDamageMultiplier[Item.type] = 14f;

            Item.value = Item.sellPrice(0, 0, 42, 0);
            Item.rare = ItemRarityID.Blue;

            Item.width = 34;
            Item.height = 38;

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