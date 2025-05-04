using GoldLeaf.Items.Misc.Accessories;
using GoldLeaf.Items.Pickups;
using GoldLeaf.Items.VanillaBossDrops;
using GoldLeaf.Items.Vanity.Watcher;
using GoldLeaf.Tiles.Decor;
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
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core
{
    public partial class GoldLeafItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public bool canSpawnMiniHearts = true;
        public bool canSpawnMiniStars = true;
        
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
            float updatedCritMod = (2 + item.GetGlobalItem<GoldLeafItem>().critDamageMod) * Main.LocalPlayer.GetModPlayer<GoldLeafPlayer>().critDamageMult;

            if (updatedCritMod != 2 && updatedCritMod > 1 && Helper.IsWeapon(item))
            {
                string[] text =
                [
                    Language.GetTextValue($"{updatedCritMod}x critical strike multiplier")
                ];

                TooltipLine critLine = tooltips.Find(n => n.Name == "CritChance");
                TooltipLine damageLine = tooltips.Find(n => n.Name == "Damage");
                int index = tooltips.IndexOf(critLine);

                if (critLine == null)
                    index = tooltips.IndexOf(damageLine);

                if (critLine != null || damageLine != null)
                    tooltips.Insert(index + 1, new TooltipLine(Mod, "CritMult", $"{updatedCritMod}x critical strike multiplier"));
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
                itemLoot.Add(ItemDropRule.FewFromOptions(5, 32, [ItemType<WatcherEyedrops>(), ItemType<WatcherCloak>(), ItemType<BatPlushie>(), ItemType<RedPlushie>(), ItemType<MadcapPainting>()]));
            }
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
