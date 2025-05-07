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
using GoldLeaf.Core;
using GoldLeaf.Items.Grove;

namespace GoldLeaf.Items.Potions
{
    public class MinionCritPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.RagePotion);

            Item.width = 20;
            Item.height = 30;

            Item.buffType = BuffType<MinionCritPotionBuff>();
            Item.buffTime = TimeToTicks(6, 0);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ItemID.Deathweed);
            //recipe.AddIngredient(ItemType<GroveFish>());
            recipe.AddTile(TileID.Bottles);
            recipe.Register();
        }
    }

    public class MinionCritPotionBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<GoldLeafPlayer>().summonCritChance += 6;
        }
    }
}
