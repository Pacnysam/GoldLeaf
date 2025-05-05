using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Vanity;
using GoldLeaf.Items.Dyes;
using GoldLeaf.Items.Nightshade;
using GoldLeaf.Items.VanillaBossDrops;
using GoldLeaf.Tiles.Decor;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;

namespace GoldLeaf.Core
{
    public class OverhealthManager : ModPlayer
    {
        public int overhealth = 0;
        public int overhealthMax = 20;

        public int overhealthDecayTime = 5;
        int overhealthTimer = 0;

        public override void ResetEffects()
        {
            overhealthMax = 20;
            overhealthDecayTime = 10;
        }

        public override void PostUpdateMiscEffects()
        {
            if (overhealth > 0) 
            {
                overhealthTimer--;
                if (overhealthTimer <= 0)
                {
                    overhealthTimer = overhealthDecayTime;
                    Player.statLife--;
                    overhealth--;
                }

                overhealth = Math.Clamp(overhealth, 0, overhealthMax);
                Player.statLifeMax2 += overhealth;
            }
        }

        /*public override void OnHurt(Player.HurtInfo info)
        {
            info.Damage -= overhealth;
            overhealth -= info.Damage;
        }*/

        public static void AddOverhealth(Player player, int amount, int time) 
        {
            player.statLife += amount;
            player.GetModPlayer<OverhealthManager>().overhealth += amount;
            player.GetModPlayer<OverhealthManager>().overhealthTimer += time;

            CombatText.NewText(player.Hitbox, ColorHelper.Overhealth, amount, false, true);
        }
    }
}
