﻿using GoldLeaf.Core;
using GoldLeaf.Effects.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Nightshade
{
    public partial class NightshadePlayer : ModPlayer
    {
        public int nightshade = 0;
        public int nightshadeTimer = 60;
        public int nightshadeMin = 0;
        public int nightshadeMax = 10;

        public bool nightshadeRing = false;

        public override void ResetEffects()
        {
            nightshadeMin = 0;
            nightshadeMax = 10;
            nightshadeRing = false;
        }

        public override void PreUpdate()
        {
            //platformTimer--;

            if (nightshade < nightshadeMin)
            {
                nightshade = nightshadeMin;
            }
            if (nightshade > nightshadeMax)
            {
                nightshade = nightshadeMax;
            }

            if (nightshadeTimer <= 0)
            {
                nightshadeTimer = 6 + (4 * (nightshade - nightshadeMin));
                if (nightshade > nightshadeMin)
                {
                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Monolith/Chop") { Volume = 0.25f });
                    nightshade--;
                }
            }
            if (nightshadeTimer > 0) { nightshadeTimer--; }

            if (Main.LocalPlayer.dead) nightshade = nightshadeMin;

            if (Main.rand.NextFloat() < 0.011627907f * nightshade && nightshade > nightshadeMin)
            {
                Dust dust;
                int type = DustType<SparkDustTiny>();
                Color color = new(210, 136, 107);
                Vector2 position = Main.LocalPlayer.Center;

                if (nightshadeRing)
                {
                    color = new(182, 158, 235);
                }
                if (Main.LocalPlayer.HasBuff(BuffType<NightshadeHeist>()))
                {
                    color = new(178, 0, 226);
                }
                /*if (Main.LocalPlayer.HasBuff(BuffType<NightshadeGuard>()))
                {
                    color = new(53, 183, 169);
                }*/

                if (nightshade == nightshadeMax) 
                { 
                    type = DustType<SparkDust>();
                }

                dust = Main.dust[Dust.NewDust(position, 0, 0, type, 0f, -3.0232563f, 0, color, 1f)];
            }
        }
    }
}
