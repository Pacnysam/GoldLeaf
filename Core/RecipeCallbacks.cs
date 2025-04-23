using System;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using GoldLeaf.Items.Grove;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using GoldLeaf.Effects.Dusts;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Items.Blizzard;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using ReLogic.Content;
using GoldLeaf.Items.Blizzard.Armor;
using GoldLeaf.Items.Grove.Boss;

namespace GoldLeaf.Core
{
    public static class RecipeCallbacks
    {
        static readonly Player player = Main.LocalPlayer;

        public static void AetherCraftEffect(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<GoldLeafPlayer>().craftTimer <= 0 && GetInstance<GraphicsConfig>().OnCraftEffects)
            {
                player.GetModPlayer<GoldLeafPlayer>().craftTimer = 60;

                int explosion = Projectile.NewProjectile(player.GetSource_FromThis(), player.MountedCenter, Vector2.Zero, ProjectileType<AetherBurst>(), 0, 0, Main.myPlayer);
                Main.projectile[explosion].ai[0] = 60f;
                AddScreenshake(player, 18);

                SoundEngine.PlaySound(SoundID.Item74);
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/RoR2/EngineerMine") { Volume = 0.4f, Pitch = -0.5f });
            }
        }
        public static void StarCraftEffect(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<GoldLeafPlayer>().craftTimer <= 0 && GetInstance<GraphicsConfig>().OnCraftEffects) 
            {
                player.GetModPlayer<GoldLeafPlayer>().craftTimer = 15;

                DustHelper.DrawStar(player.MountedCenter, DustID.FireworkFountain_Yellow, 5, 2.6f, 1f, 0.55f, 0.6f, 0.5f, true, 0, -1);
                SoundEngine.PlaySound(SoundID.Item4);
            }
        }
        public static void AnvilCraftEffect(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<GoldLeafPlayer>().craftTimer <= 0 && GetInstance<GraphicsConfig>().OnCraftEffects)
            {
                player.GetModPlayer<GoldLeafPlayer>().craftTimer = 10;

                for (float k = 0; k < Main.rand.Next(4, 6); k++)
                {
                    var d = Dust.NewDustDirect(player.MountedCenter, 12, 0, DustID.Torch, 0f, Main.rand.NextFloat(-3.8f, -6.2f), 0, default, Main.rand.NextFloat(0.7f, 0.9f));
                    //d.fadeIn = 1f;
                }
                SoundEngine.PlaySound(SoundID.Item37);
            }
        }
        public static void GelCraftEffect(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<GoldLeafPlayer>().craftTimer <= 0 && GetInstance<GraphicsConfig>().OnCraftEffects)
            {
                player.GetModPlayer<GoldLeafPlayer>().craftTimer = 10;

                for (int k = 0; k < 18; ++k)
                {
                    int dust = Dust.NewDust(player.position, player.width, player.height, DustType<SlimeDustBlue>(), Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-5f, -7f), 0, Color.White, Main.rand.NextFloat(0.8f, 1.2f));
                    Main.dust[dust].alpha = 175;
                    if (Main.rand.NextBool(2)) Main.dust[dust].alpha += 25;
                    if (Main.rand.NextBool(2)) Main.dust[dust].alpha += 25;
                }

                SoundStyle sound1 = new("GoldLeaf/Sounds/SE/HollowKnight/JellyfishEggPop") { Volume = 0.65f, PitchVariance = 0.4f };
                SoundEngine.PlaySound(sound1);
            }
        }
        public static void AuroraCraftEffect(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<GoldLeafPlayer>().craftTimer <= 0 && GetInstance<GraphicsConfig>().OnCraftEffects)
            {
                player.GetModPlayer<GoldLeafPlayer>().craftTimer = 30;

                Projectile proj = Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.MountedCenter, Vector2.Zero, ProjectileType<AuroraStar>(), 0, 0, Main.myPlayer, 3.6f, 0.9f);
                proj.rotation = Main.rand.NextFloat(-0.015f, 0.015f);

                for (float k = 0; k < 6.28f; k += (6.28f / 28))
                {
                    Dust dust = Dust.NewDustPerfect(player.MountedCenter, DustType<AuroraTwinkle>(), Vector2.One.RotatedBy(k) * 8f, 5, ColorHelper.AuroraAccentColor(k * 5.4f), Main.rand.NextFloat(1f, 1.5f));
                    dust.rotation = Main.rand.NextFloat(-18f, 18f);
                    dust.noLight = true;
                    dust.customData = player;
                }
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Frost") { Volume = 0.35f, Pitch = 0.2f });
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Kirby/MassAttack/SputterStarTiny") { Volume = 0.85f, Pitch = 0.2f, PitchVariance = 0.2f });
            }
        }
    }
}
