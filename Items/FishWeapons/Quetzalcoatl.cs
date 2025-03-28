using GoldLeaf.Core;
using static Terraria.ModLoader.ModContent;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using GoldLeaf.Items.Dungeon;
using Terraria.Localization;

namespace GoldLeaf.Items.FishWeapons
{
	public class Quetzalcoatl : ModItem
	{
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Quetzalcoatl");
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs("\tShoots an explosive cluster of light\n" +
                                                                             "<right> to summon a rift to teleport through");

        public override void SetDefaults()
		{
            Item.shoot = ProjectileType<QuetzalP>();
            Item.DamageType = DamageClass.Magic;
            Item.damage = 50;

            Item.width = 44;
			Item.height = 62;
			Item.useTime = 53;
			Item.useAnimation = 53;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.staff[Item.type] = true;
            Item.noMelee = true;
            Item.autoReuse = true;

            Item.shootSpeed = 11f;
            Item.knockBack = 9;

			Item.value = Item.sellPrice(0, 8, 50, 0);
            Item.rare = ItemRarityID.Cyan;

            Item.UseSound = new SoundStyle("GoldLeaf/Sounds/SE/SplashBounce");
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.CrystalSerpent);
            recipe.AddIngredient(ItemID.SoulofFright, 20);
            recipe.AddIngredient(ItemID.SoulofMight, 20);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }

	public class QuetzalP : ModProjectile
	{
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 16;
            //Projectile.aiStyle = 1;

            Projectile.tileCollide = true;
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            Helper.AddScreenshake(player, 20, Projectile.Center);

            for (float k = 0; k < Main.rand.Next(19, 24); k++)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-14, 14), Main.rand.Next(-14, 14), ProjectileType<QuetzalSmallP>(), Projectile.damage / 3, 0, Projectile.owner, 0f, 0f);
            }

            //Gore gore;
            //gore = Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, GoreType<WhirlpoolGore>());

        }
    }

    public class QuetzalSmallP : ModProjectile
    {
        public override string Texture => "GoldLeaf/Items/FishWeapons/QuetzalDust";
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.timeLeft = 15;

            Projectile.tileCollide = true;

            //Projectile.GetGlobalProjectile<GoldLeafProjectile>().gravity = 0.1f;
        }
    }
}