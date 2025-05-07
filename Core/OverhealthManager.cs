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
using Terraria.DataStructures;
using Terraria.Localization;

namespace GoldLeaf.Core
{
    public class OverhealthManager : ModPlayer
    {
        //public Dictionary<string, int[]> overhealthPools;

        public int overhealth = 0;
        public int overhealthMax = 20;

        public int overhealthDecayTime = 5;
        public int overhealthTimer = 0;

        //int maxLifePreOverhealth => (Player.statLifeMax2 - overhealth);

        public override void ResetEffects()
        {
            overhealthMax = 20;
            overhealthDecayTime = 10;

            if (Player.dead)
                overhealth = 0;
        }

        public override void PostUpdateMiscEffects()
        {
            if (overhealth > 0 && !Player.dead) 
            {
                overhealthTimer--;
                if (overhealthTimer <= 0)
                {
                    overhealthTimer = overhealthDecayTime;
                    Player.statLife--;
                    overhealth--;

                    if (Player.statLife <= 0)
                    {
                        Player.KillMe(PlayerDeathReason.ByCustomReason(QuickDeathReason("Overhealth", Player)), 0, 0);
                        overhealth = 0;
                    }
                }
            }

            overhealth = Math.Clamp(overhealth, 0, overhealthMax);
            Player.statLifeMax2 += overhealth;
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            overhealth -= info.Damage;
        }

        public static void AddOverhealth(Player player, int amount, int time) 
        {
            int amountAdded;
            
            if (player.GetModPlayer<OverhealthManager>().overhealth < player.GetModPlayer<OverhealthManager>().overhealthMax)
            {
                for (amountAdded = 0; (amountAdded < amount) && (player.GetModPlayer<OverhealthManager>().overhealth < player.GetModPlayer<OverhealthManager>().overhealthMax); amountAdded++)
                {
                    player.GetModPlayer<OverhealthManager>().overhealth++;
                    player.statLife++;
                }

                if (amountAdded > 0)
                    CombatText.NewText(player.Hitbox, ColorHelper.Overhealth, amountAdded, false, true);
            }

            if (time > player.GetModPlayer<OverhealthManager>().overhealthTimer)
                player.GetModPlayer<OverhealthManager>().overhealthTimer = time;
        }

        /*public static void AddOverhealthPool(Player player, string type, int amount, int maxAnount, int time = 240, int decaySpeed = 5)
        {
            player.GetModPlayer<OverhealthManager>().overhealthPools.Add(type, [amount, maxAnount, time, decaySpeed]);
            //overhealth, max overhealth, time, decay speed
        }*/
    }
}
