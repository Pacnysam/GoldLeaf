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
using GoldLeaf.Items.Nightshade;
using GoldLeaf.Core;
using Microsoft.Build.Execution;
using Terraria.DataStructures;
using GoldLeaf.Items.Grove.Toxin;

namespace GoldLeaf.Items.Potions
{
    public class VampirePotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.InvisibilityPotion);

            Item.width = 16;
            Item.height = 36;

            Item.rare = ItemRarityID.Pink;

            Item.buffType = BuffType<VampirePotionBuff>();
            Item.buffTime = TimeToTicks(3, 0);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ItemID.CobaltOre);
            recipe.AddIngredient(ItemType<Demitoxin>());
            recipe.AddIngredient(ItemID.LifeCrystal);
            recipe.AddTile(TileID.Bottles);
            recipe.Register();

            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.BottledWater);
            recipe2.AddIngredient(ItemID.PalladiumOre);
            recipe2.AddIngredient(ItemType<Demitoxin>());
            recipe2.AddIngredient(ItemID.LifeCrystal);
            recipe2.AddTile(TileID.Bottles);
            recipe2.Register();
        }
    }

    public class VampirePotionBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<PotionPlayer>().vampirePotion = true;
        }
    }
}
