﻿using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Items.Grove;
using System.Diagnostics.Metrics;
using System;
using GoldLeaf.Effects.Dusts;
using static tModPorter.ProgressUpdate;
using Terraria.Graphics.Effects;
using GoldLeaf.Items.Accessories;
using ReLogic.Content;
using Terraria.Graphics.Shaders;

namespace GoldLeaf.Items.Vanity
{
    public class Gameboy : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Asset<Effect> gameboyFilterShader = Request<Effect>("GoldLeaf/Effects/Gameboy");

                Filters.Scene["Gameboy"] = new Filter(new ScreenShaderData(gameboyFilterShader, "GameboyPass"), EffectPriority.VeryHigh);
            }
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 36;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;

            Item.accessory = true;
            Item.vanity = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GameboyPlayer>().gameboy = true;

            Filters.Scene.Activate("Gameboy");
            Filters.Scene["Gameboy"].Opacity = 1f;
        }

        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<GameboyPlayer>().gameboy = true;

            Filters.Scene.Activate("Gameboy");
            Filters.Scene["Gameboy"].Opacity = 1f;
        }
    }

    public class GameboyPlayer : ModPlayer
    {
        public bool gameboy = false;

        public override void ResetEffects()
        {
            if (Main.netMode != NetmodeID.Server && !gameboy)
            {
                Filters.Scene["Gameboy"].Opacity = 0f;
                Filters.Scene["Gameboy"].Deactivate();
            }
            gameboy = false;
        }

        
    }
}
