using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Enums;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Core;

namespace GoldLeaf.Prefixes
{
    public class Flat : CritDamagePrefix
    {
        public override float CritDamageMult => -0.25f;
    }
    public class Pinpoint : CritDamagePrefix
    {
        public override float CritDamageMult => 0.75f;

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            critBonus -= 2;
        }
    }
    public class Accurate : CritDamagePrefix
    {
        public override float CritDamageMult => 0.25f;

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            critBonus += 2;
        }
    }
    public class Crooked : CritDamagePrefix
    {
        public override float CritDamageMult => -0.25f;

        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + (CritDamageMult / 5) - 0.185f;
        }

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult -= 0.09f;
            useTimeMult += 0.07f;
        }
    }
    public class Lopsided : CritDamagePrefix
    {
        public override float CritDamageMult => -0.25f;

        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + (CritDamageMult / 5) - 0.31f;
        }

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult -= 0.09f;
            knockbackMult -= 0.2f;
            useTimeMult += 0.1f;
        }
    }
    public class Direct : CritDamagePrefix
    {
        public override float CritDamageMult => 1f;
    }
    public class Vindictive : CritDamagePrefix
    {
        public override float CritDamageMult => 0.5f;

        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + (CritDamageMult / 5) + 0.2f;
        }

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult += 0.12f;
            knockbackMult -= 0.15f;
            useTimeMult -= 0.08f;
        }
    }

    public abstract class CritDamagePrefix : ModPrefix
    {
        public virtual float CritDamageMult => 0.25f;

        public override PrefixCategory Category => PrefixCategory.AnyWeapon;

        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + (CritDamageMult/4);
        }

        public override bool CanRoll(Item item)
        {
            return !item.CountsAsClass(DamageClass.Summon);
        }

        public override void Apply(Item item)
        {
            item.GetGlobalItem<GoldLeafItem>().critDamageMod += CritDamageMult;
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            yield return new TooltipLine(Mod, "PrefixWeaponCritMult", CritMultTooltip.Format(CritDamageMult * 100))
            {
                IsModifier = true,
                IsModifierBad = CritDamageMult < 0,
            };
        }

        public static LocalizedText CritMultTooltip { get; private set; }

        public override void SetStaticDefaults()
        {
            CritMultTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(CritMultTooltip)}");
        }
    }
}
