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
using GoldLeaf.Items.Nightshade;
using GoldLeaf.Items.Grove;
using GoldLeaf.Core.CrossMod;
using GoldLeaf.Items.Misc;

namespace GoldLeaf.Items.Potions
{
    public class FinessePotion : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FinessePotionBuff.ItemSpeed);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Item.AddPotionVat(new Color(104, 111, 157));
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.SwiftnessPotion);

            Item.width = 16;
            Item.height = 32;

            Item.buffType = BuffType<FinessePotionBuff>();
            Item.buffTime = TimeToTicks(5, 0);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ItemType<FallenMoon>());
            recipe.AddIngredient(ItemID.Moonglow);
            recipe.AddIngredient(ItemID.Deathweed);
            recipe.AddTile(TileID.Bottles);
            recipe.Register();
        }
    }

    public class FinessePotionBuff : ModBuff
    {
        public override LocalizedText Description => base.Description.WithFormatArgs(ItemSpeed);
        public override string Texture => CoolBuffTex(base.Texture);
        public static readonly float ItemSpeed = 15f;

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<GoldLeafPlayer>().itemSpeed *= 1f - (ItemSpeed * 0.01f);
        }
    }
}
