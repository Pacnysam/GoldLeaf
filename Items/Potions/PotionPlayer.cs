using GoldLeaf.Core;
using GoldLeaf.Core.Mechanics;
using GoldLeaf.Items.Nightshade;
using GoldLeaf.Tiles.Decor;
using Microsoft.Build.Execution;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Potions
{
    public class PotionPlayer : ModPlayer
    {
        public bool vampirePotion = false;
        public bool consistencyPotion = false;
        public int vigorTime = 0;

        public override void ResetEffects()
        {
            vampirePotion = false;
            consistencyPotion = false;
        }

        public override void Load()
        {
            On_NPC.AI_001_Slimes_GenerateItemInsideBody += SlimePotionDrops;
        }
        public override void Unload()
        {
            On_NPC.AI_001_Slimes_GenerateItemInsideBody -= SlimePotionDrops;
        }

        private int SlimePotionDrops(On_NPC.orig_AI_001_Slimes_GenerateItemInsideBody orig, bool isBallooned)
        {
            switch (Main.rand.Next(30))
            {
                case 0:
                    return ItemType<JumpBoostPotion>();
                case 1:
                    return ItemType<FinessePotion>();
                default:
                    break;
            }

            return orig(isBallooned);
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (Player.HasBuff<VigorPotionBuff>())
                vigorTime = (int)Math.Max(vigorTime - (info.Damage * 2.5f), 0);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (consistencyPotion)
            {
                modifiers.DamageVariationScale *= 0;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (vampirePotion && hit.Crit)
            {
                Player.Heal(1 + damageDone / 45);
            }
        }
    }
}
