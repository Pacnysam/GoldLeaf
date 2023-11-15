using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GoldLeaf.Items.Weapons.Magic
{
	public class Quetzalcoatl : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.GoldLeaf.hjson file.

		public override void SetDefaults()
		{
			Item.damage = 50;
			Item.DamageType = DamageClass.Magic;
			Item.width = 54;
			Item.height = 62;
			Item.useTime = 53;
			Item.useAnimation = 53;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.staff[Item.type] = true;
            Item.noMelee = true;

            Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Cyan;
			Item.autoReuse = true;
		}
	}
}