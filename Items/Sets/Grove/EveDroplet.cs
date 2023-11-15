using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GoldLeaf.Items.Sets.Grove
{
	public class EveDroplet : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.GoldLeaf.hjson file.

		public override void SetDefaults()
		{
			Item.damage = 13;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 20;
			Item.height = 24;
			Item.useTime = 24;
			Item.useAnimation = 48;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			//Item.ammo = 0;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.value = 1000;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
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