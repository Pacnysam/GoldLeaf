﻿using GoldLeaf.Effects.Dusts;
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
using Terraria.Graphics.CameraModifiers;

namespace GoldLeaf.Core
{
    public class CameraSystem : ModPlayer
    {
        public int ScreenShake = 0;
        public int ScreenMoveTime = 0;
        public Vector2 ScreenMoveTarget = new Vector2(0, 0);
        public Vector2 ScreenMovePan = new Vector2(0, 0);
        public bool ScreenMoveHold = false;
        private int ScreenMoveTimer = 0;
        private int panDown = 0;

        public static void QuickScreenShake(Vector2 position, Vector2 destination, float amount = 20f, float speed = 6f, int frames = 20, float distance = 1000f, string uniqueIdentity = null)
        {
            PunchCameraModifier modifier = new(position, destination, amount, speed, frames, distance, uniqueIdentity);
            Main.instance.CameraModifiers.Add(modifier);
        }
        public static void QuickScreenShake(Vector2 position, float amount = 20f, float speed = 6f, int frames = 20, float distance = 1000f, string uniqueIdentity = null)
        {
            PunchCameraModifier modifier = new(position, (1 * ((float)Math.PI * 2f)).ToRotationVector2(), amount, speed, frames, distance, uniqueIdentity);
            Main.instance.CameraModifiers.Add(modifier);
        }

        public static void AddScreenshake(Player player, float amount, Vector2? position)
        {
            Vector2 value = Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            float multiplier = 1f;

            if (position.HasValue)
            {
                /*position -= Main.screenPosition;
                Vector3 coords = ScreenCoord(new Vector3(position.Value.X, -position.Value.Y, 0));
                float mult = Vector3.Distance(coords, Vector3.Zero);*/

                float distance = Vector2.Distance(position.Value, value);
                multiplier = 1f - distance / ((float)Main.screenWidth * 1.5f);

                if (Vector2.DistanceSquared(Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), position.Value) > 100000000f)
                    amount = 0;

                /*if (!OnScreen(new Vector2(position.Value.X, position.Value.Y)))
                {
                    amount /= mult;
                    amount -= mult / 70f;
                }*/
            }
            multiplier = MathHelper.Clamp(multiplier * GetInstance<VisualConfig>().ShakeIntensity, 0f, 1f);

            player.GetModPlayer<CameraSystem>().ScreenShake += (int)Math.Clamp(amount * multiplier, 0, 150);
        }
        public static void AddScreenshake(Player player, int amount)
        {
            player.GetModPlayer<CameraSystem>().ScreenShake += (int)Math.Clamp(amount * GetInstance<VisualConfig>().ShakeIntensity, 0, 150);
        }

        public override void ModifyScreenPosition()
        {
            if (ScreenMoveTime > 0 && ScreenMoveTarget != Vector2.Zero)
            {
                Vector2 off = new Vector2(Main.screenWidth, Main.screenHeight) / -2;
                if (ScreenMoveTimer <= 30) //go out
                {
                    Main.screenPosition = Vector2.SmoothStep(Main.LocalPlayer.Center + off, ScreenMoveTarget + off, ScreenMoveTimer / 30f);
                }
                else if (ScreenMoveTimer >= ScreenMoveTime - 30) //go in
                {
                    Main.screenPosition = Vector2.SmoothStep((ScreenMovePan == Vector2.Zero ? ScreenMoveTarget : ScreenMovePan) + off, Main.LocalPlayer.Center + off, (ScreenMoveTimer - (ScreenMoveTime - 30)) / 30f);
                }
                else
                {
                    if (ScreenMovePan == Vector2.Zero)
                        Main.screenPosition = ScreenMoveTarget + off; //stay on target

                    else if (ScreenMoveTimer <= ScreenMoveTime - 150)
                        Main.screenPosition = Vector2.Lerp(ScreenMoveTarget + off, ScreenMovePan + off, ScreenMoveTimer / (float)(ScreenMoveTime - 150));

                    else
                        Main.screenPosition = ScreenMovePan + off;
                }

                if (ScreenMoveTimer == ScreenMoveTime)
                {
                    ScreenMoveTime = 0;
                    ScreenMoveTimer = 0;
                    ScreenMoveTarget = Vector2.Zero;
                    ScreenMovePan = Vector2.Zero;
                }

                if (ScreenMoveTimer < ScreenMoveTime - 30 || !ScreenMoveHold)
                    ScreenMoveTimer++;
            }

            if (ScreenShake > 0) { ScreenShake--; }
            if (ScreenShake < 0) { ScreenShake = 0; }

            Main.screenPosition.Y += (Main.rand.Next(-ScreenShake, ScreenShake) + panDown);
            Main.screenPosition.X += Main.rand.Next(-ScreenShake, ScreenShake);
        }
    }
}
