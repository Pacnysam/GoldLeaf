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
using GoldLeaf.Items.Grove;

namespace GoldLeaf.Items.Potions
{
    public class VigorPotion : ModItem
    {
        public static readonly int extraOverhealth = 20;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(extraOverhealth);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.InvisibilityPotion);

            Item.width = 16;
            Item.height = 36;

            Item.rare = ItemRarityID.Green;

            Item.buffType = BuffType<VigorPotionBuff>();
            Item.buffTime = TimeToTicks(6, 0);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe()
            .AddIngredient(ItemID.BottledWater)
            .AddIngredient(ItemID.Hive)
            .AddIngredient(ItemID.Fireblossom)
            //recipe.AddIngredient(ItemType<GroveFish>());
            .AddTile(TileID.Bottles)
            .Register();
        }
    }

    public class VigorPotionBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<OverhealthManager>().overhealthMax += VigorPotion.extraOverhealth;
            player.GetModPlayer<PotionPlayer>().vigorPotion = true;
        }
    }
}
