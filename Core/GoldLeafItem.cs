using GoldLeaf.Items.Accessories;
using GoldLeaf.Items.Hell;
using GoldLeaf.Items.Pickups;
using GoldLeaf.Items.VanillaBossDrops;
using GoldLeaf.Items.Vanity.Watcher;
using GoldLeaf.Tiles.Decor;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core
{
    public partial class GoldLeafItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public float critDamageMod = 0f;

        public int lifesteal;
        public int lifestealMax;

        //public DamageClass throwingDamageType = DamageClass.Default;

        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            float updatedCritMod = (2 + item.GetGlobalItem<GoldLeafItem>().critDamageMod) * Main.LocalPlayer.GetModPlayer<GoldLeafPlayer>().critDamageMult;
            if (updatedCritMod <= 1 && Helper.IsWeapon(item))
                crit *= 0;
        }

        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (lifestealMax >= 1 && lifesteal > 0)
            {
                lifestealMax--;
                player.Heal(lifesteal);
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            switch (item.type) //TODO: make a system for this
            {
                case ItemID.RoyalGel:
                    {
                        TooltipLine tooltipLine = tooltips.Find(n => n.Name == "Tooltip0");
                        int index = tooltips.IndexOf(tooltipLine);
                        if (tooltipLine != null)
                            tooltips.Insert(index + 1, new TooltipLine(Mod, "RoyalGel", Language.GetTextValue("Mods.GoldLeaf.Items.Vanilla.RoyalGel", tooltipLine)));
                        break;
                    }
            }

            GoldLeafPlayer glPlayer = Main.LocalPlayer.GetModPlayer<GoldLeafPlayer>();
            float updatedCritMod = (2 + item.GetGlobalItem<GoldLeafItem>().critDamageMod) * Main.LocalPlayer.GetModPlayer<GoldLeafPlayer>().critDamageMult;

            if (item.DamageType.CountsAsClass(DamageClass.Melee)) updatedCritMod += glPlayer.meleeCritDamageMod;
            if (item.DamageType.CountsAsClass(DamageClass.Ranged)) updatedCritMod += glPlayer.rangedCritDamageMod;
            if (item.DamageType.CountsAsClass(DamageClass.Magic)) updatedCritMod += glPlayer.magicCritDamageMod;

            if (updatedCritMod != 2 && updatedCritMod > 1 && Helper.IsWeapon(item))
            {
                TooltipLine critLine = tooltips.Find(n => n.Name == "CritChance");
                TooltipLine damageLine = tooltips.Find(n => n.Name == "Damage");
                int index = tooltips.IndexOf(critLine);

                if (critLine == null)
                    index = tooltips.IndexOf(damageLine);

                if (critLine != null || damageLine != null)
                    tooltips.Insert(index + 1, new TooltipLine(Mod, "CritMult", Language.GetTextValue("Mods.GoldLeaf.CommonItemTooltip.CriticalDamageMultiplier", updatedCritMod)));
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            if (item.type == ItemID.RoyalGel)
            {
                player.GetModPlayer<GoldLeafPlayer>().royalGel = true;
            }
        }

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (ItemID.Sets.BossBag[item.type]) 
            {
                itemLoot.Add(ItemDropRule.FewFromOptions(5, 50, [ItemType<WatcherEyedrops>(), ItemType<WatcherCloak>(), ItemType<BatPlushie>(), ItemType<RedPlushie>(), ItemType<MadcapPainting>()]));
            }

            switch (item.type)
            {
                /*case ItemID.LavaCrate:
                case ItemID.LavaCrateHard:
                    {
                        itemLoot.Add(ItemDropRule.Common(ItemType<HeatFlask>(), 4, 60, 75));
                        break;
                    }*/
            }
        }

        public override void GrabRange(Item item, Player player, ref int grabRange)
        {
            if (player.lifeMagnet && (item.type == ItemType<HeartTiny>() || item.type == ItemType<HeartLarge>()))
            {
                grabRange += Item.lifeGrabRange;
            }
            if (player.manaMagnet && (item.type == ItemType<StarTiny>() || item.type == ItemType<StarLarge>()))
            {
                grabRange += Item.manaGrabRange;
            }
        }

        public override bool GrabStyle(Item item, Player player)
        {
            if (player.lifeMagnet && (item.type == ItemType<HeartTiny>() || item.type == ItemType<HeartLarge>()))
            {
                PullItem_Pickup(player, item, 15f, 5);
                return true;
            }
            if (player.manaMagnet && (item.type == ItemType<StarTiny>() || item.type == ItemType<StarLarge>()))
            {
                PullItem_Pickup(player, item, 12f, 5);
                return true;
            }
            return base.GrabStyle(item, player);
        }

        private static void PullItem_Pickup(Player player, Item item, float speed, int acc)
        {
            Vector2 vector = new(item.position.X + (float)(item.width / 2), item.position.Y + (float)(item.height / 2));
            float num = player.Center.X - vector.X;
            float num2 = player.Center.Y - vector.Y;
            float num3 = (float)Math.Sqrt(num * num + num2 * num2);
            num3 = speed / num3;
            num *= num3;
            num2 *= num3;
            item.velocity.X = (item.velocity.X * (float)(acc - 1) + num) / (float)acc;
            item.velocity.Y = (item.velocity.Y * (float)(acc - 1) + num2) / (float)acc;
        }

        public override void SetDefaults(Item item)
        {
            /*if (item.GetGlobalItem<GoldLeafItem>().throwingDamageType != DamageClass.Default)
            {
                if (GetInstance<MiscConfig>().ThrowerSupport)
                {
                    item.DamageType = DamageClass.Throwing;
                }
                else
                {
                    item.DamageType = item.GetGlobalItem<GoldLeafItem>().throwingDamageType;
                }
            }*/

            switch (item.type)
            {
                case ItemID.SlimeStaff:
                    {
                        item.rare = ItemRarityID.Blue;
                        item.damage = 10;
                        item.value = Item.sellPrice(0, 0, 75, 0);
                        break;
                    }
                case ItemID.ImpStaff: 
                    {
                        item.damage = 23;
                        break;
                    }
            }
        }
    }
}
