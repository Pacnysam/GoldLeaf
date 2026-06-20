using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Core;
using GoldLeaf.Core.CrossMod;

namespace GoldLeaf.Items.Potions
{
    public class VigorPotion : ModItem
    {
        public static readonly int extraOverhealth = 20;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(extraOverhealth);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Item.AddPotionVat(new Color(214, 69, 60), false);
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
            //recipe.AddIngredient(ItemType<Cradleflopper>());
            .AddIngredient(ItemID.Fireblossom)
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
