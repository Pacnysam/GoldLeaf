using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Nightshade;
using GoldLeaf.Tiles.Decor;
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

        public float itemSpeed;
        
        public override float UseTimeMultiplier(Item Item)
		{
			return itemSpeed;
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

        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            switch (Main.LocalPlayer.name) 
            {
                case "Pacnysam":
                case "Pacny":
                case "Pac":
                case "Sylvia":
                    {
                        return [new Item(ItemType<BatPlushie>())];
                    }
                case "Scout":
                case "Emperor":
                    {
                        return Enumerable.Empty<Item>();
                        //return [new Item(ItemType<EmperorScoutTrousers>())];
                        //return [new Item(ItemType<EmperorScoutTunic>())];
                        //return [new Item(ItemType<EmperorScoutHood>())];
                    }
                case "Cypher":
                    {
                        return Enumerable.Empty<Item>();
                        //return [new Item(ItemType<CypherHat>())];
                        //return [new Item(ItemType<CypherCoat>())];
                        //return [new Item(ItemType<CypherPants>())];
                    }
            }
            return Enumerable.Empty<Item>();
        }

        public override void Unload()
        {
            ResetEffectsEvent = null;
        }
    }
}
