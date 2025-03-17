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
        /*public List<int[]> overhealth = new();

        public int overhealthTotal = 0;

        public int overhealthDecaySpeed = 1;
        public int overhealthTimer = 0;

        public override void ResetEffects()
        {
            overhealthTotal = 0;

            overhealthDecaySpeed = 1;
            overhealthTimer = 0;
        }

        public override void PostUpdateMiscEffects()
        {
            for (int k = 0; k < overhealth.Count - 1; k++)
            {
                overhealthTotal += overhealth.ElementAtOrDefault(k)[0];

                if (overhealth.ElementAtOrDefault(k)[2] > 0) 
                {
                    if (overhealth.ElementAtOrDefault(k)[1] == 0) overhealth.RemoveAt(k); k--;
                    overhealth.ElementAtOrDefault(k)[2]--;
                }
                else
                {
                    overhealth.ElementAtOrDefault(k)[0] -= overhealth.ElementAtOrDefault(k)[1];
                }

                if (overhealth.ElementAtOrDefault(k)[0] <= 0) overhealth.RemoveAt(k); k--;
            }

            Player.statLifeMax2 += overhealthTotal;
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            base.ModifyHurt(ref modifiers);
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            base.OnHurt(info);
        }

        public int[] OverhealthPool; */
    }

    /*public static partial class Helper 
    {
        public static void AddOverhealth(Player player, int size, int decayTime, int timer)
        {
            player.GetModPlayer<OverhealthManager>().overhealth.Add([size, decayTime, timer]);
        }
        public static void AddOverhealth(Player player, int size)
        {
            player.GetModPlayer<OverhealthManager>().overhealth.Add([size, player.GetModPlayer<OverhealthManager>().overhealthDecaySpeed, 300]);
        }
    }*/
}
