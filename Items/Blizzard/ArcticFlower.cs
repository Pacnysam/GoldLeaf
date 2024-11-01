using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GoldLeaf.Items.Blizzard
{
	public class ArcticFlower : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.GoldLeaf.hjson file.

		public override void SetDefaults()
		{
			Item.damage = 16;
			Item.DamageType = DamageClass.Summon;
			Item.width = 34;
			Item.height = 40;
            Item.noMelee = true;
            Item.noUseGraphic = true;
			Item.value = 50000;
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = false;
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