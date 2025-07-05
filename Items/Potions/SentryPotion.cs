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
using GoldLeaf.Core.CrossMod;

namespace GoldLeaf.Items.Potions
{
    public class SentryPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Item.AddPotionVat(new Color(201, 163, 96), false);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.SummoningPotion);

            Item.width = 20;
            Item.height = 32;

            Item.buffType = BuffType<SentryPotionBuff>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ItemID.Shiverthorn);
            recipe.AddIngredient(ItemID.Hive);
            recipe.AddTile(TileID.Bottles);
            recipe.Register();
        }
    }

    public class SentryPotionBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void Update(Player player, ref int buffIndex)
        {
            player.maxTurrets += 1;
        }
    }
}
