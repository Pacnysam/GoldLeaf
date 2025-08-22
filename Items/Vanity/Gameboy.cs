using static Terraria.ModLoader.ModContent;
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
using Steamworks;

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
        }

        public override void UpdateVanity(Player player) => UpdateAccessory(player, false);
    }

    public class GameboyPlayer : ModPlayer
    {
        public bool gameboy = false;
        public float gameboyOpacity = 0;

        public override void ResetEffects()
        {
            gameboy = false;
        }

        public override void PostUpdateMiscEffects()
        {
            if (gameboy)
                gameboyOpacity += 0.01f;
            else
                gameboyOpacity -= 0.05f;

            gameboyOpacity = MathHelper.Clamp(gameboyOpacity, 0f, 1f);
        }
    }

    public class GameboySystem : ModSystem
    {
        public override void PostUpdateEverything()
        {
            if (Main.dedServ)
                return;

            float opacity = Main.LocalPlayer.GetModPlayer<GameboyPlayer>().gameboyOpacity;

            if (Main.LocalPlayer.GetModPlayer<GameboyPlayer>().gameboy)
            {
                if (!Filters.Scene["Gameboy"].IsActive())
                {
                    Filters.Scene.Activate("Gameboy");
                }
            }
            else
            {
                if (Filters.Scene["Gameboy"].IsActive() && opacity <= 0)
                {
                    Filters.Scene.Deactivate("Gameboy");
                }
            }

            Effect shader = Filters.Scene["Gameboy"].GetShader().Shader;
            shader.Parameters["uOpacity"].SetValue(opacity);
        }
    }
}
