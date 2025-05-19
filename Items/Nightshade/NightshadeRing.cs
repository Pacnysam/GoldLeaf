using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Effects.Dusts;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using GoldLeaf.Core;
using Mono.Cecil;
using Terraria.DataStructures;
using GoldLeaf.Items.Grove;
using Microsoft.Xna.Framework.Graphics;


namespace GoldLeaf.Items.Nightshade
{
	public class NightshadeRing : ModItem
	{
		public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Nightshade Ring");
		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs("Increases nightshade capacity");

		public override void SetDefaults()
		{
            Item.value = Item.sellPrice(0, 0, 65, 0);
            Item.rare = ItemRarityID.Orange;

            Item.width = 18;
            Item.height = 26;

            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GoldLeafPlayer>().itemSpeed *= 1f - (player.GetModPlayer<NightshadePlayer>().nightshade * 0.02f);
            player.GetModPlayer<NightshadePlayer>().nightshadeRing = true;
        }
    }
}