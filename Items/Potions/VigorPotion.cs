using GoldLeaf.Core;
using GoldLeaf.Core.CrossMod;
using GoldLeaf.Core.Mechanics;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Grove.Toxin;
using GoldLeaf.Items.Nightshade;
using GoldLeaf.Tiles.Decor;
using Microsoft.Build.Execution;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Potions
{
    public class VigorPotion : ModItem
    {
        public static readonly int extraOverhealth = 40;
        public static readonly int timeUntilGeneration = TimeToTicks(3);
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(extraOverhealth);

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
            Item.buffTime = TimeToTicks(8, 0);
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
            //potionPlayer.vigorPotion = true;
            potionPlayer.vigorTime++;
            
            if (potionPlayer.vigorTime > VigorPotion.timeUntilGeneration && player.buffTime[buffIndex] % 15 == 1) 
            {
                OverhealthManager.AddOverhealth(player, new VigorPool() { size = 1 });
                Main.NewText(player.Overhealth());
            }
        }
    }
    public class VigorPool : OverhealthPool
    {
        public override int MaxSize => VigorPotion.extraOverhealth;
        public override int AmountToDecrement => 1;
        public override int TimeToDecrement => 2;

        public override bool PreUpdateTime(Player player)
        {
            if (player.HasBuff<VigorPotionBuff>())
                return false;

            return true;
        }
        public override void OnHurt(Player player, Player.HurtInfo info, int amountLost)
        {
            player.GetModPlayer<PotionPlayer>().vigorTime = 0;
        }
    }
}
