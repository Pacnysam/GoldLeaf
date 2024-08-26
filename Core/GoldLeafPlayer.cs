using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Nightshade;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core
{
    public partial class GoldLeafPlayer : ModPlayer
    {
        public float temp1, temp2, temp3;

        public int Timer { get; private set; }

        public int Shake = 0;

        public bool ZoneGrove = false;

        public int ScreenMoveTime = 0;
        public Vector2 ScreenMoveTarget = new Vector2(0, 0);
        public Vector2 ScreenMovePan = new Vector2(0, 0);
        public bool ScreenMoveHold = false;
        private int ScreenMoveTimer = 0;
        private int panDown = 0;

        //public int platformTimer = 0;

        public int nightshade = 0;
        public int nightshadeTimer = 60;

        public float itemSpeed;
        
        public override float UseTimeMultiplier(Item Item)
		{
			return itemSpeed;
		}

        public override void PreUpdate()
		{
            //platformTimer--;

            if (nightshade < 0)
            {
                nightshade = 0;
            }
            if (nightshade > 12)
			{
                nightshade = 12;
			}

            nightshadeTimer--;
            if (nightshadeTimer <= 0)
			{
                nightshadeTimer = 60;
                if (nightshade > 0) 
                {
                    nightshade--;
                }
            }

            Player.maxRunSpeed += .08f * nightshade;
            Player.runAcceleration += .02f * nightshade;
            itemSpeed += .02f * nightshade;

            Player.GetDamage(DamageClass.Generic) += (3 * nightshade);

            if (Main.rand.NextFloat() < 0.011627907f * nightshade && nightshade >= 1)
            {
                Dust dust;
                Color color = new(210, 136, 107);
                Vector2 position = Main.LocalPlayer.Center;
                
                if (Main.LocalPlayer.HasBuff(BuffType<NightshadeHeistBuff>()) && Main.LocalPlayer.HeldItem.type != ItemType<VampireBat>()) 
                {
                    color = new(178, 0, 226);
                }

                dust = Main.dust[Dust.NewDust(position, 0, 0, DustType<SparkDust>(), 0f, -3.0232563f, 0, color, 1f)];
            }

        }

        public delegate void ResetEffectsDelegate(GoldLeafPlayer Player);
        public static event ResetEffectsDelegate ResetEffectsEvent;
        public override void ResetEffects()
        {
            ResetEffectsEvent?.Invoke(this);
            itemSpeed = 1;
        }

        public override void PostUpdate()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Player == Main.LocalPlayer) GoldLeafWorld.rottime += (float)Math.PI / 60;
            Timer++;
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

            Main.screenPosition.Y += Main.rand.Next(-Shake, Shake) + panDown;
            Main.screenPosition.X += Main.rand.Next(-Shake, Shake);
            if (Shake > 0) { Shake--; }
        }

        public override void Unload()
        {
            ResetEffectsEvent = null;
        }
    }
}
