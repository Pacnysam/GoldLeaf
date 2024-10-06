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
using GoldLeaf.Items.Grove;
using Microsoft.Xna.Framework.Graphics;

namespace GoldLeaf.Items.Nightshade
{
	public class NoxGauntlet : ModItem
	{
		//public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Eve Droplet");
		//public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs("Removes immunities");

		public override void SetDefaults()
		{
			Item.damage = 19;
			Item.DamageType = DamageClass.Melee;
            Item.knockBack = 2;

            Item.width = 38;
			Item.height = 34;

			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.autoReuse = true;

            Item.value = Item.sellPrice(0, 0, 60, 0);
            Item.rare = ItemRarityID.Orange;
			
			Item.UseSound = SoundID.Item1;

			Item.shoot = ProjectileType<AetherBolt>();
			Item.shootSpeed = 11f;
		}

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D tex = Request<Texture2D>(Texture + "Glow").Value;
            spriteBatch.Draw
            (
                Request<Texture2D>(Texture + "Glow").Value,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - tex.Height * 0.5f
                ),
                new Rectangle(0, 0, tex.Width, tex.Height),
                Color.White,
                rotation,
                tex.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
        }

        /*public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}*/
    }

    public class NightshadeHeistBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Nightshade Heist");
            //Description.SetDefault("WIP");
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
    }
}