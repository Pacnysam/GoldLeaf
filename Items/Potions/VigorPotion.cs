using GoldLeaf.Core;
using GoldLeaf.Core.CrossMod;
using GoldLeaf.Core.Mechanics.Overhealth;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Potions
{
    public class VigorPotion : ModItem
    {
        public static int ExtraOverhealth => 40;
        public static int TimeUntilGeneration => TimeToTicks(3.5f);
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExtraOverhealth);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Item.AddPotionVat(new Color(226, 70, 13), true);
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
            PotionPlayer potionPlayer = player.GetModPlayer<PotionPlayer>();
            
            if (potionPlayer.vigorTime < VigorPotion.TimeUntilGeneration)
                potionPlayer.vigorTime++;

            if (potionPlayer.vigorTime >= VigorPotion.TimeUntilGeneration && player.buffTime[buffIndex] % 20 == 1 && player.GetOverhealthOfType<VigorPool>() < VigorPotion.ExtraOverhealth)
            {
                player.AddOverhealth<VigorPool>();

                if (player.HasItem(ItemType<DebugItem>()))
                    Main.NewText("Vigor Overhealth: " + player.GetOverhealthOfType<VigorPool>(), new Color(226, 70, 13));
            }
        }
    }
    public class VigorPool : OverhealthPool
    {
        public override int MaxSize => VigorPotion.ExtraOverhealth;
        public override int AmountToDecrement => 1;
        public override int TimeToDecrement => 2;
        public override int DefaultDuration => 0;

        public override bool PreUpdateTime(Player player) => !player.HasBuff<VigorPotionBuff>();
    }
}
