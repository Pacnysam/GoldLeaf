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
using GoldLeaf.Items.Accessories;
using GoldLeaf.Items.Vanity;
using GoldLeaf.Items.Dyes;
using Terraria.GameContent.ItemDropRules;
using GoldLeaf.Tiles.Decor;
using GoldLeaf.Items.VanillaBossDrops;
using GoldLeaf.Items.Potions;
using Terraria.GameContent.Bestiary;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Dyes.HairDye;
using Terraria.ModLoader.IO;
using GoldLeaf.Items.Granite;
using Terraria.DataStructures;
using GoldLeaf.Items.Misc;

namespace GoldLeaf.Core
{
    public class GoldLeafNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int damageModFlat = 0;
        public float critDamageModFlat = 0f;
        public int defenseModFlat = 0;
        public float defenseFactorMod = 1f;

        public float movementSpeed = 1f;

        public int counter = 0;

        public bool Slowed => movementSpeed < 1f;
        public bool stunned = false;

        public override void ResetEffects(NPC npc)
        {
            damageModFlat = 0;
            critDamageModFlat = 0f;
            defenseModFlat = 0;
            defenseFactorMod = 1f;

            movementSpeed = 1f;
            stunned = false;
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(movementSpeed);
            binaryWriter.Write(stunned);
            binaryWriter.Write(counter);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            movementSpeed = binaryReader.ReadSingle();
            stunned = binaryReader.ReadBoolean();
            counter = binaryReader.ReadInt32();
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

                        shop.InsertAfter(ItemID.SilverDye, ItemType<BrassDye>());
                        break;
                    }
                case NPCID.WitchDoctor:
                    {
                        shop.InsertAfter(ItemID.PygmyNecklace, ItemType<ToxicPositivity>(), Condition.TimeDay);
                        break;
                    }
                case NPCID.Stylist:
                    {
                        shop.Add(ItemType<AuroraHairDye>(), Condition.InSnow, Condition.TimeNight);
                        shop.Add(ItemType<SunstoneHairDye>(), Condition.InDesert, Condition.TimeDay);
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

        public static bool CanBeStunned(NPC npc) => (!npc.boss && npc.aiStyle != NPCAIStyleID.Worm/* && npc.knockBackResist != 0f*/);

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            /*if (npc.type == NPCID.GoldenSlime && !Main.remixWorld && Main.slimeRain)
                npc.ai[1] = Utils.SelectRandom(Main.rand, [ItemID.HermesBoots, ItemID.BandofRegeneration, ItemID.MagicMirror, ItemID.CloudinaBottle, 
                    ItemID.ShinyRedBalloon, ItemID.Aglet, ItemID.AnkletoftheWind, ItemID.LuckyHorseshoe]);*/
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (stunned)
                return false;

            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }

        public override bool PreAI(NPC npc)
        {
            if (Main.netMode != NetmodeID.Server && stunned && !npc.boss && npc.aiStyle != NPCAIStyleID.Worm/* && npc.knockBackResist != 0f*/)
            {
                npc.velocity = Vector2.Zero;
                npc.GetGlobalNPC<GoldLeafNPC>().movementSpeed *= 0f;
                return false;
            }
            return true;
        }

        public override void AI(NPC npc)
        {
            counter++;
        }

        public override void PostAI(NPC npc)
        {
            if (Slowed && !npc.boss && npc.aiStyle != NPCAIStyleID.Worm/* && npc.knockBackResist != 0f*/)
            {
                npc.position -= npc.velocity * (1 - movementSpeed);
            }
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
                case NPCID.GraniteGolem:
                    {
                        npcLoot.Add(ItemDropRule.Common(ItemType<GraniteVoice>(), 50));
                        break;
                    }
                case NPCID.DD2OgreT2:
                case NPCID.DD2OgreT3:
                    {
                        npcLoot.Add(ItemDropRule.NormalvsExpert(ItemType<OgreVoice>(), 20, 10));
                        break;
                    }
            }
        }
    }
}
