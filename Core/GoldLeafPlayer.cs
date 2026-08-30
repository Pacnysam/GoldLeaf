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

        public bool ZoneGrove = false;
        public int craftTimer = 0;

        public float meleeCritDamageMod = 0f;
        public float rangedCritDamageMod = 0f;
        public float magicCritDamageMod = 0f;
        public float critDamageMult = 1f;

        public int summonCritChance = 0;

        public float damageResistance = 0f;

        #region minor variables
        public bool royalGel = false;
        public bool hiveCarcass = false;
        public bool hasDoneHurtSound = false;

        #endregion minor variables

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
            modifiers.ModifyHurtInfo += CalculateDamageResistance;
            void CalculateDamageResistance(ref Player.HurtInfo info) => info.Damage = (int)(info.Damage * (1f - Math.Min(damageResistance, 0.9f)));
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

            critDamageMult = 1f;
            meleeCritDamageMod = rangedCritDamageMod = magicCritDamageMod = 0f;
            summonCritChance = 0;
            damageResistance = 0f;

            #region minor variables
            royalGel = false;
            hiveCarcass = false;
            hasDoneHurtSound = false;
            #endregion minor variables
        }

        public override void Unload()
        {
            ResetEffectsEvent = null;
        }

        public override void PostUpdate()
        {
            if (craftTimer > 0) craftTimer--;
        }

        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            switch (Main.LocalPlayer.name.ToLower()) 
            {
                case "pacnysam":
                case "pacny":
                case "pac":
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
                /*case "scout":
                case "emperor":
                case "hunter":
                case "belos":
                    {
                        return Enumerable.Empty<Item>();
                        return [new Item(ItemType<EmperorCovenTrousers>()), new Item(ItemType<EmperorCovenTunic>()), new Item(ItemType<EmperorCovenHood>())];
                    }*/
                /*case "cypher":
                    {
                        return [new Item(ItemType<CypherHat>()), new Item(ItemType<CypherCoat>()), new Item(ItemType<CypherPants>())];
                    }*/
                case "grant":
                    {
                        return [new Item(ItemType<GrantMask>()), new Item(ItemType<GrantCuffs>()), new Item(ItemType<GrantPants>()), new Item(ItemType<GrantCloak>())];
                    }
                case "gameboy":
                case "game Boy":
                    {
                        return [new Item(ItemType<Gameboy>()), new Item(ItemType<RetroDye>(), 5)];
                    }
            }
            return Enumerable.Empty<Item>();
        }
    }
}
