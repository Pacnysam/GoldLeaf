using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Items.Misc.Accessories;
using GoldLeaf.Items.Misc.Vanity;
using GoldLeaf.Items.Misc.Vanity.Dyes;
using Terraria.GameContent.ItemDropRules;
using GoldLeaf.Tiles.Decor;
using GoldLeaf.Items.VanillaBossDrops;
using GoldLeaf.Items.Potions;

namespace GoldLeaf.Core
{
    public class GoldLeafNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public float critDamageMod = 0f;
        public int defenseMod = 0;
        //public float defenseFactorMod = 1f;

        public bool stunned = false;

        public override void ResetEffects(NPC npc)
        {
            critDamageMod = 0f;
            defenseMod = 0;
            //defenseFactorMod = 1f;

            stunned = false;
        }

        public override void ModifyShop(NPCShop shop)
        {
            switch (shop.NpcType)
            {
                case NPCID.Merchant:
                    {
                        shop.Add(ItemID.Leather);
                        shop.Add(ItemType<WaxCandle>());
                        break;
                    }
                case NPCID.GoblinTinkerer:
                    {
                        shop.Add(ItemType<Gameboy>());
                        break;
                    }
                case NPCID.DyeTrader:
                    {
                        shop.Add<RetroDye>(GoldLeafConditions.UsingGameboy);
                        //shop.Add<RetroDye>(Condition.NpcIsPresent(NPCID.GoblinTinkerer));
                        break;
                    }
                case NPCID.WitchDoctor:
                    {
                        shop.Add<ToxicPositivity>(Condition.TimeDay);
                        //shop.Add<HexWhip>(Condition.TimeNight);

                        shop.Add<WatcherEyedrops>(Condition.MoonPhaseNew, Condition.TimeNight);
                        shop.Add<WatcherCloak>(Condition.MoonPhaseNew, Condition.TimeNight);
                        //shop.Add<MadcapPainting>(Condition.MoonPhaseNew, Condition.TimeNight);
                        //shop.Add<BatPlushie>(Condition.MoonPhaseNew, Condition.TimeNight);
                        break;
                    }
                case NPCID.Painter:
                    {
                        shop.Add<MadcapPainting>(Condition.MoonPhaseNew, Condition.TimeNight);
                        break;
                    }
                case NPCID.Clothier:
                    {
                        shop.Add<BatPlushie>(Condition.MoonPhaseNew, Condition.TimeNight);
                        break;
                    }
            }
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            /*if (npc.onFire && GetInstance<GameplayConfig>().BuffChanges) 
            {
                modifiers.Defense.Flat -= 4;
            }*/

            modifiers.CritDamage += critDamageMod;
            modifiers.Defense.Flat += defenseMod;
            //modifiers.DefenseEffectiveness *= defenseFactorMod;
        }

        public override bool PreAI(NPC npc)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (stunned)
                {
                    if (!npc.boss)
                    {
                        npc.velocity *= 0;
                        npc.frame.Y = 0;
                        return false;
                    }
                }
            }

            return true;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                case NPCID.VampireBat:
                case NPCID.Vampire:
                    {
                        npcLoot.Add(ItemDropRule.Common(ItemType<VampirePotion>(), 2));
                        break;
                    }
            }
        }
    }
}
