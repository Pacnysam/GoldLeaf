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
using GoldLeaf.Items.Vanity;
using GoldLeaf.Items.Dyes;
using Terraria.GameContent.ItemDropRules;
using GoldLeaf.Tiles.Decor;
using GoldLeaf.Items.VanillaBossDrops;
using GoldLeaf.Items.Potions;
using Terraria.GameContent.Bestiary;
using GoldLeaf.Items.Grove;

namespace GoldLeaf.Core
{
    public class GoldLeafNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int damageModFlat = 0;
        public float critDamageModFlat = 0f;
        public int defenseModFlat = 0;
        public float defenseFactorMod = 1f;

        public bool stunned = false;

        public override void ResetEffects(NPC npc)
        {
            damageModFlat = 0;
            critDamageModFlat = 0f;
            defenseModFlat = 0;
            defenseFactorMod = 1f;

            stunned = false;
        }

        public override void ModifyShop(NPCShop shop)
        {
            switch (shop.NpcType)
            {
                case NPCID.Merchant:
                    {
                        shop.InsertBefore(ItemID.Rope, ItemID.Leather);
                        shop.InsertAfter(ItemID.Rope, ItemType<WaxCandle>());
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
                        shop.InsertAfter(ItemID.PygmyNecklace, ItemType<ToxicPositivity>(), Condition.TimeDay);
                        break;
                    }
                case NPCID.ArmsDealer:
                    {
                        shop.Add(ItemID.TissueSample, GoldLeafConditions.HasClutterGlove);
                        shop.Add(ItemID.ShadowScale, GoldLeafConditions.HasClutterGlove);
                        //shop.Add(ItemType<EveDroplet>(), GoldLeafConditions.HasClutterGlove, GoldLeafConditions.InSurface);
                        break;
                    }
            }
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            modifiers.FlatBonusDamage += damageModFlat;
            modifiers.CritDamage += critDamageModFlat;
            modifiers.Defense.Flat += defenseModFlat;
            modifiers.DefenseEffectiveness *= defenseFactorMod;
        }

        public override bool PreAI(NPC npc)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (stunned)
                {
                    if (!npc.boss)
                    {
                        npc.netUpdate = true;
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
