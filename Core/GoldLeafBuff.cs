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
        public override void SetStaticDefaults()
        {
            Main.pvpBuff[BuffID.Chilled] = true;
        }

        public override void Update(int type, Player player, ref int buffIndex)
        {
            if ((type == BuffID.OnFire || type == BuffID.OnFire3)) 
            { 
                player.statDefense -= 4; 
            }
        }

        public override void Update(int type, NPC npc, ref int buffIndex)
        {
            if ((type == BuffID.OnFire || type == BuffID.OnFire3))
            {
                npc.GetGlobalNPC<GoldLeafNPC>().defenseModFlat -= 4;
            }
            if (type == BuffID.CursedInferno)
            {
                //npc.GetGlobalNPC<GoldLeafNPC>().critDamageModFlat += 0.3f;
                npc.GetGlobalNPC<GoldLeafNPC>().damageModFlat += 3;
            }
            if (type == BuffID.Chilled)
            {
                npc.GetGlobalNPC<GoldLeafNPC>().movementSpeed *= 0.5f;
            }
            if (type == BuffID.Frozen)
            {
                npc.GetGlobalNPC<GoldLeafNPC>().stunned = true;
                npc.GetGlobalNPC<GoldLeafNPC>().movementSpeed *= 0f;
            }
        }

        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            #region Vanilla Items
            switch (type)
            {
                case BuffID.OnFire:
                case BuffID.OnFire3:
                    {
                        tip += Language.GetTextValue("Mods.GoldLeaf.Buffs.Vanilla.OnFire");
                        break;
                    }
                case BuffID.Ichor:
                    {
                        tip = Language.GetTextValue("Mods.GoldLeaf.Buffs.Vanilla.Ichor");
                        break;
                    }
            }
            #endregion

            if (Main.LocalPlayer.GetModPlayer<SafetyBlanketPlayer>().safetyBlanket && Main.debuff[type] && !BuffSets.Cosmetic[type] && type != BuffType<SafetyBlanketBuff>())
            {
                if (IsValidDebuff(type, Main.LocalPlayer.buffTime[Main.LocalPlayer.FindBuffIndex(type)] + 2))
                    tip += "\n[c/78BE78:" + Language.GetTextValue("Mods.GoldLeaf.CommonItemTooltip.BuffCanBeCleansed") + "]";
                else
                    tip += "\n[c/BE7878:" + Language.GetTextValue("Mods.GoldLeaf.CommonItemTooltip.BuffCanNotBeCleansed") + "]";
            }
        }
    }
}
