using GoldLeaf.Core;
using GoldLeaf.Core.Mechanics.Overhealth;
using GoldLeaf.Items.Potions;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Forest.WellspringWatch
{
	public class WellspringWatch : ModItem //TODO: glowmask
	{
		public override void SetDefaults()
		{
			Item.value = Item.sellPrice(0, 1, 50, 0);
			Item.rare = ItemRarityID.Green;

			Item.width = 40;
			Item.height = 34;

			Item.accessory = true;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("GoldLeaf:GoldWatch")
            .AddIngredient(ItemType<FallenMoon>())
            .AddIngredient(ItemID.HealingPotion, 10)
            .AddTile(TileID.Anvils)
            .Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<WellspringWatchPlayer>().wellspringWatch = true;
        }
    }

    public class WellspringWatchPlayer : ModPlayer
	{
        public bool wellspringWatch = false;
        public override void ResetEffects()
        {
            wellspringWatch = false;
        }
    }

    public class WellspringWatchItem : GlobalItem 
    {
        public override bool InstancePerEntity => true;
        public override bool? UseItem(Item item, Player player) //TODO: add sound/visual effects, crossmod compatibility with restoration potions
        {
            if (player.GetModPlayer<WellspringWatchPlayer>().wellspringWatch && item.potion && item.healLife > 0 && ItemSets.PotionCanGainAccessoryEffects[item.type])
            {
                player.AddOverhealth<WellspringPool>(player.GetHealLife(item) * 2);
                return true;
            }
            return base.UseItem(item, player);
        }
    }

    public class WellspringPool : OverhealthPool
    {
        public override int MaxSize => 1000;
        public override int AmountToDecrement => Math.Max((int)(size / 50f), 1);
        public override int TimeToDecrement => 1;
        public override int DefaultDuration => 180;
        public override float Priority => 1f;
        public override bool IgnoreOverhealthPrevention => true;
    }
}