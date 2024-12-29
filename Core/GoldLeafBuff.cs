using GoldLeaf.Items.GemSickles;
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

namespace GoldLeaf.Core
{
    public class GoldLeafBuff : GlobalBuff
    {
        public override void Update(int type, Player player, ref int buffIndex)
        {
            if (type == BuffID.OnFire && GetInstance<GameplayConfig>().BuffChanges) 
            { 
                player.statDefense -= 4; 
            }
        }

        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            #region Vanilla Items
            switch (type)
            {
                case BuffID.OnFire:
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
        }
    }
}
