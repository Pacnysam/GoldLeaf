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
using GoldLeaf.Items.Vanity.Grant;

namespace GoldLeaf.Core
{
    public class GoldLeafPlayer : ModPlayer
    {
        public float temp1, temp2, temp3;

        public int Timer { get; private set; }

        public bool ZoneGrove = false;
        //public bool ZoneCandle = false;

        //public int platformTimer = 0;
        public int craftTimer = 0;

        public float itemSpeed;
        public bool stunned = false;

        public float meleeCritDamageMod = 0f;
        public float rangedCritDamageMod = 0f;
        public float magicCritDamageMod = 0f;
        public float critDamageMult = 1f;

        public int summonCritChance = 0;

        public float damageResistance = 0f;

        #region minor variables
        public bool royalGel = false;
        public bool hasDoneHurtSound = false;

        #endregion minor variables

        public override float UseTimeMultiplier(Item Item) => itemSpeed;
        public override float UseAnimationMultiplier(Item item) => itemSpeed;

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (item.DamageType.CountsAsClass(DamageClass.Summon) && ((summonCritChance > 0 && Main.rand.NextBool(summonCritChance, 100)) || summonCritChance > 100))
            {
                modifiers.SetCrit();
            }

            if (item.DamageType.CountsAsClass(DamageClass.Melee)) modifiers.CritDamage += meleeCritDamageMod;
            if (item.DamageType.CountsAsClass(DamageClass.Ranged)) modifiers.CritDamage += rangedCritDamageMod;
            if (item.DamageType.CountsAsClass(DamageClass.Magic)) modifiers.CritDamage += magicCritDamageMod;

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

            if (proj.DamageType.CountsAsClass(DamageClass.Melee)) modifiers.CritDamage += meleeCritDamageMod;
            if (proj.DamageType.CountsAsClass(DamageClass.Ranged)) modifiers.CritDamage += rangedCritDamageMod;
            if (proj.DamageType.CountsAsClass(DamageClass.Magic)) modifiers.CritDamage += magicCritDamageMod;

            modifiers.CritDamage += (proj.GetGlobalProjectile<GoldLeafProjectile>().critDamageMod);
            modifiers.CritDamage *= critDamageMult;
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            modifiers.FinalDamage *= 1f - Math.Min(damageResistance, 0.9f);
        }

        public delegate void OnHitNPCDelegate(Player player, NPC target, NPC.HitInfo hit, int damageDone);
        public static event OnHitNPCDelegate OnHitNPCEvent;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitNPCEvent?.Invoke(Player, target, hit, damageDone);
        }

        public delegate void ResetEffectsDelegate(Player player);
        public static event ResetEffectsDelegate ResetEffectsEvent;
        public override void ResetEffects()
        {
            ResetEffectsEvent?.Invoke(Player);

            itemSpeed = 1;
            critDamageMult = 1f;
            meleeCritDamageMod = rangedCritDamageMod = magicCritDamageMod = 0f;
            summonCritChance = 0;
            damageResistance = 0f;

            stunned = false;

            #region minor variables
            royalGel = false;
            hasDoneHurtSound = false;
            #endregion minor variables
        }

        public override void Unload()
        {
            ResetEffectsEvent = null;
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

        /*public override void PostUpdateBuffs()
        {
            if (stunned)
            {
                if (Player.velocity.Y != 0f)
                {
                    Player.velocity = new Vector2(0f, 1E-06f);
                }
                else
                {
                    Player.velocity = Vector2.Zero;
                }
                Player.jumpSpeedBoost = 0;
                Player.blockExtraJumps = true;
                //Player.jumpHeight = 0;
                Player.gravity = 0f;
                Player.moveSpeed = 0f;
                Player.dash = 0;
                Player.dashType = 0;
                Player.noKnockback = true;
                Player.RemoveAllGrapplingHooks();
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
                            new Item(ItemType<BatPlushie>()),
                            new Item(ItemType<RedPlushie>()),
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
                case "Grant":
                    {
                        return [new Item(ItemType<GrantMask>()), new Item(ItemType<GrantCuffs>()), new Item(ItemType<GrantPants>()), new Item(ItemType<GrantCloak>())];
                    }
                case "Gameboy":
                case "Game Boy":
                    {
                        return [new Item(ItemType<Gameboy>()), new Item(ItemType<RetroDye>(), 5)];
                    }
            }
            return Enumerable.Empty<Item>();
        }
    }
}
