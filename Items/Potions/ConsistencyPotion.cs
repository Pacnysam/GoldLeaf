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
    public class ConsistencyPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Item.AddPotionVat(new Color(151, 151, 151), false);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.SummoningPotion);

            Item.width = 24;
            Item.height = 28;

            Item.buffType = BuffType<ConsistencyPotionBuff>();
            Item.buffTime = TimeToTicks(15, 0);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ItemID.Flounder);
            recipe.AddIngredient(ItemID.StoneBlock);
            recipe.AddTile(TileID.Bottles);
            recipe.Register();
        }
    }
    
    public class ConsistencyPotionBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<PotionPlayer>().consistencyPotion = true;
        }
    }
}
