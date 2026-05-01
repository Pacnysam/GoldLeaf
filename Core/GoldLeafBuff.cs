using GoldLeaf.Items.Grove;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Items.VanillaBossDrops;
using GoldLeaf.Tiles.Decor;
using GoldLeaf.Items.Blizzard;

namespace GoldLeaf.Core
{
    public class GoldLeafBuff : GlobalBuff
    {
        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            int buffTime = Main.LocalPlayer.buffTime[Main.LocalPlayer.FindBuffIndex(type)];

            if (Main.LocalPlayer.GetModPlayer<SafetyBlanketPlayer>().safetyBlanket && buffTime > 2 && Main.debuff[type] && !Main.buffNoTimeDisplay[type] && !BuffSets.Cosmetic[type] && !BuffSets.RemoveCleanseTooltip[type])
            {
                if (IsValidDebuff(type, buffTime + 2))
                    tip += "\n[c/78BE78:" + Language.GetTextValue("Mods.GoldLeaf.CommonItemTooltip.BuffCanBeCleansed") + "]";
                else
                    tip += "\n[c/BE7878:" + Language.GetTextValue("Mods.GoldLeaf.CommonItemTooltip.BuffCanNotBeCleansed") + "]";
            }
        }
    }
}
