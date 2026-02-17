using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Items.Blizzard.Armor;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Grove.Boss.AetherComet;
using GoldLeaf.Items.Sky;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core
{
    public static class RecipeCallbacks //TODO: remove all projectiles trom this
    {
        static readonly Player player = Main.LocalPlayer;

        public static void AetherBurst(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (player.GetModPlayer<GoldLeafPlayer>().craftTimer <= 0 && GetInstance<VisualConfig>().OnCraftEffects)
            {
                player.GetModPlayer<GoldLeafPlayer>().craftTimer = 60;

                for (int j = 0; j < 15; j++)
                {
                    Dust dust = Dust.NewDustDirect(player.MountedCenter, 0, 0, DustType<AetherSmoke>());
                    dust.velocity = Main.rand.NextVector2Circular(9f, 9f);
                    dust.scale = Main.rand.NextFloat(0.9f, 1.2f);
                    dust.alpha = 20 + Main.rand.Next(60);
                    dust.rotation = Main.rand.NextFloat(6.28f);
                }

                int explosion = Projectile.NewProjectile(player.GetSource_FromThis(), player.MountedCenter, Vector2.Zero, ProjectileType<AetherBurst>(), 0, 0, Main.myPlayer);
                Main.projectile[explosion].ai[0] = 60f;
                CameraSystem.AddScreenshake(player, 18);

                SoundEngine.PlaySound(SoundID.Item74);
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/RoR2/EngineerMine") { Volume = 0.4f, Pitch = -0.5f });
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
    }
}
