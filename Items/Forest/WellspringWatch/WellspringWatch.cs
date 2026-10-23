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

        public override void PostUpdateEquips()
        {
            if (wellspringWatch)
            {
                if (ModLoader.TryGetMod("PacnyRefresh", out Mod pacnyRefresh) && pacnyRefresh.TryFind("RestorationBuff", out ModBuff restorationBuff) && Player.HasBuff(restorationBuff.Type)
                && Player.buffTime[Player.FindBuffIndex(restorationBuff.Type)] % 30 == 0)
                {
                    Player.AddOverhealth<WellspringPool>(5, 180);
                }

                if (ModLoader.TryGetMod("Split", out Mod split) && split.TryFind("GelidRegeneration", out ModBuff gelidPotionBuff) && Player.HasBuff(gelidPotionBuff.Type)
                && Player.buffTime[Player.FindBuffIndex(gelidPotionBuff.Type)] % 60 == 0)
                {
                    Player.AddOverhealth<WellspringPool>(5, 180);
                }
            }
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
        public override int DefaultDuration => 300;
        public override float Priority => 1f;
        public override bool IgnoreOverhealthPrevention => true;
        public override bool PreUpdateTime(Player player)
        {
            if ((ModLoader.TryGetMod("PacnyRefresh", out Mod pacnyRefresh) && pacnyRefresh.TryFind("RestorationBuff", out ModBuff restorationBuff) && player.HasBuff(restorationBuff.Type))
                || (ModLoader.TryGetMod("Split", out Mod split) && split.TryFind("GelidRegeneration", out ModBuff gelidPotionBuff) && player.HasBuff(gelidPotionBuff.Type))) return false;

            return base.PreUpdateTime(player);
        }
    }
}