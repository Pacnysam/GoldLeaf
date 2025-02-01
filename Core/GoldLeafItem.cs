using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
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
        public bool canSuperCrit = true;

        public float critDamageMod = 2f;

        public int lifesteal;
        public int lifestealMax;

        public DamageClass throwingDamageType = DamageClass.Default;

        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (lifestealMax >= 1 && lifesteal > 0)
            {
                lifestealMax--;
                player.Heal(lifesteal);
            }
        }

        public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += (critDamageMod - 2);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ItemID.RoyalGel)
            {
                string[] text =
                [
                    Language.GetTextValue("Mods.GoldLeaf.Items.Vanilla.RoyalGel")
                ];

                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] != string.Empty)
                        
                        tooltips.Insert(2, new TooltipLine(Mod, "Tooltip1", text[i]));
                }
            }
        }

        public override void UpdateInventory(Item item, Player player)
        {
            if (item.GetGlobalItem<GoldLeafItem>().throwingDamageType != DamageClass.Default)
            {
                if (GetInstance<MiscConfig>().ThrowerSupport)
                {
                    item.DamageType = DamageClass.Throwing;
                }
                else
                {
                    item.DamageType = item.GetGlobalItem<GoldLeafItem>().throwingDamageType;
                }
                
                item.NetStateChanged();
            }
        }

        public override void SetDefaults(Item item)
        {
            if (item.GetGlobalItem<GoldLeafItem>().throwingDamageType != DamageClass.Default)
            {
                if (GetInstance<MiscConfig>().ThrowerSupport)
                {
                    item.DamageType = DamageClass.Throwing;
                }
                else
                {
                    item.DamageType = item.GetGlobalItem<GoldLeafItem>().throwingDamageType;
                }
            }

            switch (item.type)
            {
                case ItemID.SlimeStaff:
                    {
                        item.rare = ItemRarityID.Blue;
                        item.damage = 10;
                        break;
                    }
            }
        }
    }
}
