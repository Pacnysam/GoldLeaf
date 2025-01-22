using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Misc.Vanity;
using GoldLeaf.Items.Misc.Vanity.Dyes;
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

namespace GoldLeaf.Core
{
    public partial class GoldLeafPlayer : ModPlayer
    {
        public float temp1, temp2, temp3;

        public int Timer { get; private set; }

        public int screenshake = 0;

        public bool ZoneGrove = false;
        //public bool ZoneCandle = false;

        public int ScreenMoveTime = 0;
        public Vector2 ScreenMoveTarget = new Vector2(0, 0);
        public Vector2 ScreenMovePan = new Vector2(0, 0);
        public bool ScreenMoveHold = false;
        private int ScreenMoveTimer = 0;
        private int panDown = 0;

        //public int platformTimer = 0;

        public int craftTimer = 0;
        public float itemSpeed;
        
        public override float UseTimeMultiplier(Item Item)
		{
			return itemSpeed;
		}

        public override float UseAnimationMultiplier(Item item)
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

        /*public override void PreUpdateBuffs()
        {
            if (ZoneCandle) 
            {
                Main.LocalPlayer.AddBuff(BuffType<WaxCandleBuff>(), 1);
            }
        }*/

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

            Main.screenPosition.Y += (Main.rand.Next(-screenshake, screenshake) + panDown);
            Main.screenPosition.X += Main.rand.Next(-screenshake, screenshake);
            if (screenshake > 0) { screenshake--; }
            if (craftTimer > 0) { craftTimer--; }
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
                        return [new Item(ItemType<BatPlushie>(), 9999), new Item(ItemType<MadcapPainting>())];
                    }
                case "Scout":
                case "King":
                case "Emperor":
                    {
                        return Enumerable.Empty<Item>();
                        //return [new Item(ItemType<EmperorScoutTrousers>()), new Item(ItemType<EmperorScoutTunic>()), new Item(ItemType<EmperorScoutHood>())];
                    }
                case "Cypher":
                    {
                        return Enumerable.Empty<Item>();
                        //return [new Item(ItemType<CypherHat>()), new Item(ItemType<CypherCoat>()), new Item(ItemType<CypherPants>())];
                    }
                case "Gamer":
                case "Gameboy":
                case "Gamer Girl":
                    {
                        return [new Item(ItemType<Gameboy>()), new Item(ItemType<RetroDye>(), 9999)];
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
