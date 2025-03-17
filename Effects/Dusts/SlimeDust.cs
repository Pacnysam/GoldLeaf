﻿using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using GoldLeaf.Core;
using ReLogic.Content;
using Terraria.ID;

namespace GoldLeaf.Effects.Dusts
{
    public class SlimeDust : ModDust
    {
        public override void SetStaticDefaults()
        {
            UpdateType = DustID.t_Slime;
        }

        public override void OnSpawn(Dust dust)
        {
            dust.alpha = 175;
            if (Main.rand.NextBool(2)) dust.alpha += 25; if (Main.rand.NextBool(2)) dust.alpha += 25;
        }
    }
    public class SlimeDustBlue : SlimeDust
    {
        public override void SetStaticDefaults()
        {
            UpdateType = DustID.t_Slime;
        }

        public override void OnSpawn(Dust dust)
        {
            dust.color = Color.White;
        }
    }    
}
