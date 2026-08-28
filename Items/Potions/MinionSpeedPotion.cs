using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Core.CrossMod;
using GoldLeaf.Core.Mechanics;

namespace GoldLeaf.Items.Potions
{
    public class MinionSpeedPotion : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MinionSpeedPotionBuff.MinionSpeed);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Item.AddPotionVat(new Color(0, 214, 189), false);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.RagePotion);

            Item.width = 20;
            Item.height = 30;

            Item.buffType = BuffType<MinionSpeedPotionBuff>();
            Item.buffTime = TimeToTicks(8, 0);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ItemID.Daybloom);
            //recipe.AddIngredient(ItemType<Witchbane>());
            recipe.AddIngredient(ItemType<AuroraCluster>());
            recipe.AddTile(TileID.Bottles);
            recipe.Register();
        }
    }

    public class MinionSpeedPotionBuff : ModBuff
    {
        public override LocalizedText Description => base.Description.WithFormatArgs(MinionSpeed);
        public override string Texture => CoolBuffTex(base.Texture);
        public static readonly float MinionSpeed = 20f;

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<MinionSpeedPlayer>().minionSpeed += MinionSpeed * 0.01f;
        }
    }
}
