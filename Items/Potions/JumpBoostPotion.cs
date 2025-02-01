using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Tiles.Decor;

namespace GoldLeaf.Items.Potions
{
    public class JumpBoostPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.SwiftnessPotion);

            Item.width = 20;
            Item.height = 34;

            Item.buffType = BuffType<JumpBoostPotionBuff>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ItemID.PinkGel);
            recipe.AddIngredient(ItemID.Cloud);
            recipe.AddTile(TileID.Bottles);
            recipe.Register();
        }
    }

    public class JumpBoostPotionBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void Update(Player player, ref int buffIndex)
        {
            player.jumpSpeedBoost += 2.8f;
            player.autoJump = true;
        }
    }
}
