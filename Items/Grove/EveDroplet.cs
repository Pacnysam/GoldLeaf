using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Effects.Dusts;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using GoldLeaf.Core;
using Mono.Cecil;
using Terraria.DataStructures;

namespace GoldLeaf.Items.Grove
{
	public class EveDroplet : ModItem
	{
		//public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Eve Droplet");
		//public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs("Removes immunities");

		public override void SetDefaults()
		{
			Item.damage = 19;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 20;
			Item.height = 24;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			//Item.ammo = 0;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 0, 0, 5);
            Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = false;

			//ItemID.Sets.ItemIconPulse[Item.type] = true;

            Item.shoot = ProjectileType<AetherBeam>();
			Item.shootSpeed = 11f;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 full = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-5, 5)));

            int p = Projectile.NewProjectile(source, position, full, type, damage, knockback, player.whoAmI);
			Main.projectile[p].GetGlobalProjectile<GoldLeafProjectile>().gravity = 0.1f;
            Main.projectile[p].GetGlobalProjectile<GoldLeafProjectile>().gravityDelay = 15;
            Main.projectile[p].timeLeft = 55;

            return false;
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