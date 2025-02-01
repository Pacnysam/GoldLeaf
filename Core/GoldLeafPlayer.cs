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
using static GoldLeaf.Core.Helper;

namespace GoldLeaf.Core
{
    public partial class GoldLeafPlayer : ModPlayer
    {
        public float temp1, temp2, temp3;

        public int Timer { get; private set; }

        public bool ZoneGrove = false;
        //public bool ZoneCandle = false;

        //public int platformTimer = 0;
        public int craftTimer = 0;

        public int overhealth = 0;
        public float itemSpeed;

        #region minor variables
        public bool royalGel = false;

        #endregion minor variables

        public override float UseTimeMultiplier(Item Item)
		{
			return itemSpeed;
		}

        public override float UseAnimationMultiplier(Item item)
        {
            return itemSpeed;
        }

        public override void UpdateEquips()
        {
            if (HasAccessory(Player, ItemID.RoyalGel)) royalGel = true;
        }

        public delegate void ResetEffectsDelegate(GoldLeafPlayer Player);
        public static event ResetEffectsDelegate ResetEffectsEvent;
        public override void ResetEffects()
        {
            ResetEffectsEvent?.Invoke(this);

            overhealth = 0;
            itemSpeed = 1;

            royalGel = false;
        }

        public override void PostUpdate()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Player == Main.LocalPlayer) GoldLeafWorld.rottime += (float)Math.PI / 60;
            Timer++;

            if (craftTimer > 0) { craftTimer--; }
        }

        /*public override void PreUpdateBuffs()
        {
            if (ZoneCandle) 
            {
                Main.LocalPlayer.AddBuff(BuffType<WaxCandleBuff>(), 1);
            }
        }*/

        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            switch (Main.LocalPlayer.name) 
            {
                case "Pacnysam":
                case "Pacny":
                case "Pac":
                case "Sylvia":
                    {
                        return 
                            [
                            new Item(ItemType<BatPlushie>(), 9999), 
                            new Item(ItemType<MadcapPainting>()), 
                            new Item(ItemType<WatcherEyedrops>()), 
                            new Item(ItemType<WatcherCloak>())
                            ];
                    }
                case "Scout":
                case "Emperor":
                case "Hunter":
                case "Belos":
                    {
                        return Enumerable.Empty<Item>();
                        //return [new Item(ItemType<EmperorScoutTrousers>()), new Item(ItemType<EmperorScoutTunic>()), new Item(ItemType<EmperorScoutHood>())];
                    }
                case "Cypher":
                    {
                        return Enumerable.Empty<Item>();
                        //return [new Item(ItemType<CypherHat>()), new Item(ItemType<CypherCoat>()), new Item(ItemType<CypherPants>())];
                    }
                case "Gameboy":
                case "Game Boy":
                    {
                        return [new Item(ItemType<Gameboy>()), new Item(ItemType<RetroDye>(), 9999)];
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
