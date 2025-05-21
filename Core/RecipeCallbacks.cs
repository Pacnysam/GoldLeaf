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
using Terraria.Graphics.Shaders;

namespace GoldLeaf.Core
{
    public static class RecipeCallbacks //ive tried reworking this like 4 times i give up
    {
        static readonly Player player = Main.LocalPlayer;

        /*public delegate void CraftEffectDelegate(Player player);
        public static event CraftEffectDelegate MajorOnCraftEvent;
        public static event CraftEffectDelegate MinorOnCraftEvent;

        public static void CraftEffects(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<GoldLeafPlayer>().craftTimer <= 0 && GetInstance<GraphicsConfig>().OnCraftEffects)
            {
                MajorOnCraftEvent?.Invoke(player);
            }
            MinorOnCraftEvent?.Invoke(player);
        }*/

        public static void AetherBurst(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<GoldLeafPlayer>().craftTimer <= 0 && GetInstance<VisualConfig>().OnCraftEffects)
            {
                player.GetModPlayer<GoldLeafPlayer>().craftTimer = 60;

                int explosion = Projectile.NewProjectile(player.GetSource_FromThis(), player.MountedCenter, Vector2.Zero, ProjectileType<AetherBurst>(), 0, 0, Main.myPlayer);
                Main.projectile[explosion].ai[0] = 60f;
                CameraSystem.AddScreenshake(player, 18);

                SoundEngine.PlaySound(SoundID.Item74);
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/RoR2/EngineerMine") { Volume = 0.4f, Pitch = -0.5f });
            }
        }
        public static void Star(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<GoldLeafPlayer>().craftTimer <= 0 && GetInstance<VisualConfig>().OnCraftEffects) 
            {
                player.GetModPlayer<GoldLeafPlayer>().craftTimer = 15;

                DustHelper.DrawStar(player.MountedCenter, DustID.FireworkFountain_Yellow, 5, 2.6f, 1f, 0.55f, 0.6f, 0.5f, true, 0, -1);
                SoundEngine.PlaySound(SoundID.Item4);
            }
        }
        public static void Anvil(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<GoldLeafPlayer>().craftTimer <= 0 && GetInstance<VisualConfig>().OnCraftEffects)
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
        public static void Slime(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<GoldLeafPlayer>().craftTimer <= 0 && GetInstance<VisualConfig>().OnCraftEffects)
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
        public static void AuroraMajor(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<GoldLeafPlayer>().craftTimer <= 0 && GetInstance<VisualConfig>().OnCraftEffects)
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
        public static void AuroraMinor(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<GoldLeafPlayer>().craftTimer <= 0 && GetInstance<VisualConfig>().OnCraftEffects)
            {
                player.GetModPlayer<GoldLeafPlayer>().craftTimer = 10;

                Projectile proj = Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.MountedCenter, Vector2.Zero, ProjectileType<AuroraStar>(), 0, 0, Main.myPlayer, 1.8f, 0.9f);

                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact);
            }
        }
        public static void DyeMinor(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<GoldLeafPlayer>().craftTimer <= 0 && GetInstance<VisualConfig>().OnCraftEffects)
            {
                if (item.dye > 0 || item.hairDye > -1)
                {
                    for (int k = 0; k < 12; ++k)
                    {
                        Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustType<SlimeDust>(), 0, Main.rand.NextFloat(-2.5f, -4f), 125, Color.White, Main.rand.NextFloat(1f, 1.6f));

                        if (item.dye > 0)
                            dust.shader = GameShaders.Armor.GetSecondaryShader(item.dye, Main.LocalPlayer);
                        if (item.hairDye > 0)
                            dust.shader = GameShaders.Armor.GetSecondaryShader(item.hairDye, Main.LocalPlayer);

                        if (Main.rand.NextBool()) dust.alpha += 50; if (Main.rand.NextBool()) dust.alpha += 25;
                    }

                    SoundEngine.PlaySound((Main.rand.NextBool() ? SoundID.Item86 : SoundID.Item87) with { MaxInstances = 2 });
                }
            }
        }
    }
}
