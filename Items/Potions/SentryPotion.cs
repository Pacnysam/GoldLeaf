using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Core.CrossMod;

namespace GoldLeaf.Items.Potions
{
    public class SentryPotion : ModItem
    {
        public override LocalizedText Tooltip => (ThoriumHelper.ThoriumLoaded(out Mod thorium) && thorium.TryFind("ArtilleryBuff", out ModBuff artilleryBuff)) ? 
            this.GetLocalization("ThoriumTooltip").WithFormatArgs(1, artilleryBuff.DisplayName, Language.GetTextValue("ItemName.SummoningPotion")) : base.Tooltip.WithFormatArgs(1);

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

        public override bool? UseItem(Player player)
        {
            if (ThoriumHelper.ThoriumLoaded(out Mod thorium))
            {
                if (ThoriumHelper.HasSentryPotionBuff(player, out ModBuff artilleryBuff))
                    player.ClearBuff(artilleryBuff.Type);
                if (player.HasBuff(BuffID.Summoning))
                    player.ClearBuff(BuffID.Summoning);
            }
            return null;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BottledWater)
            .AddIngredient(ItemID.VariegatedLardfish)//.AddIngredient(ItemType<BronzeRivuline>())
            .AddIngredient(ItemID.Shiverthorn)//.AddIngredient(ItemType<Witchbane>())
            .AddTile(TileID.Bottles)
            .Register();
        }
    }

    public class SentryPotionBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void Update(Player player, ref int buffIndex)
        {
            player.maxTurrets += 1;

            if (ThoriumHelper.ThoriumLoaded(out Mod thorium) && (ThoriumHelper.HasSentryPotionBuff(player, out ModBuff artilleryBuff) || player.HasBuff(BuffID.Summoning)))
                player.DelBuff(buffIndex);
        }
    }
}
