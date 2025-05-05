using GoldLeaf.Effects.Dusts;
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
using GoldLeaf.Items.Vanity.Watcher;
using GoldLeaf.Biomes;

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

        public float itemSpeed;

        public float meleeCritDamageMult = 1f;
        public float rangedCritDamageMult = 1f;
        public float magicCritDamageMult = 1f;
        public float summonCritDamageMult = 1f;
        public float critDamageMult = 1f;

        public int summonCritChance = 0;

        #region minor variables
        public bool royalGel = false;

        #endregion minor variables

        public override float UseTimeMultiplier(Item Item) => itemSpeed;
        public override float UseAnimationMultiplier(Item item) => itemSpeed;

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (item.DamageType.CountsAsClass(DamageClass.Summon) && ((summonCritChance > 0 && Main.rand.NextBool(summonCritChance, 100)) || summonCritChance > 100))
            {
                modifiers.SetCrit();
            }

            if (item.DamageType == DamageClass.Melee) critDamageMult *= meleeCritDamageMult;
            if (item.DamageType == DamageClass.Ranged) critDamageMult *= rangedCritDamageMult;
            if (item.DamageType == DamageClass.Magic) critDamageMult *= magicCritDamageMult;
            if (item.DamageType == DamageClass.Summon) critDamageMult *= summonCritDamageMult;

            modifiers.CritDamage += (item.GetGlobalItem<GoldLeafItem>().critDamageMod);
            modifiers.CritDamage *= critDamageMult;
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            int totalSummonCritChance = summonCritChance + proj.GetGlobalProjectile<GoldLeafProjectile>().summonCritChance;
            if (proj.DamageType.CountsAsClass(DamageClass.Summon) && ((summonCritChance > 0 && Main.rand.NextBool(totalSummonCritChance, 100)) || summonCritChance > 100))
            {
                modifiers.SetCrit();
            }

            modifiers.CritDamage += (proj.GetGlobalProjectile<GoldLeafProjectile>().critDamageMod);
            modifiers.CritDamage *= critDamageMult;
        }

        public delegate void DoubleTapDelegate(Player player);
        public static event DoubleTapDelegate DoubleTapEvent;
        public static event DoubleTapDelegate DoubleTapPrimaryEvent;
        public static event DoubleTapDelegate DoubleTapSecondaryEvent;

        public void DoubleTap(Player player, int keyDir)
        {
            DoubleTapEvent?.Invoke(player);

            if ((Main.ReversedUpDownArmorSetBonuses && keyDir == 1) || (!Main.ReversedUpDownArmorSetBonuses && keyDir == 0))
                DoubleTapPrimaryEvent?.Invoke(player);

            if ((Main.ReversedUpDownArmorSetBonuses && keyDir == 0) || (!Main.ReversedUpDownArmorSetBonuses && keyDir == 1))
                DoubleTapSecondaryEvent?.Invoke(player);
        }

        public delegate void ResetEffectsDelegate(GoldLeafPlayer player);
        public static event ResetEffectsDelegate ResetEffectsEvent;
        public override void ResetEffects()
        {
            ResetEffectsEvent?.Invoke(this);

            itemSpeed = 1;
            critDamageMult = 1f;
            meleeCritDamageMult = rangedCritDamageMult = magicCritDamageMult = summonCritDamageMult = 1f;
            summonCritChance = 0;

            #region minor variables
            royalGel = false;
            #endregion minor variables
        }

        public override void Load()
        {
            On_Player.KeyDoubleTap += DoubleTapKey;
        }

        public override void Unload()
        {
            On_Player.KeyDoubleTap -= DoubleTapKey;

            ResetEffectsEvent = null;
        }

        private static void DoubleTapKey(On_Player.orig_KeyDoubleTap orig, Player self, int keyDir)
        {
            orig(self, keyDir);

            self.GetModPlayer<GoldLeafPlayer>().DoubleTap(self, keyDir);
        }

        public override void PreUpdateBuffs()
        {
            if (Player.InModBiome<ZoneCandle>()) Player.AddBuff(BuffType<WaxCandleBuff>(), 2);
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
    }
}
