using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Tiles.Decor;
using GoldLeaf.Items.Nightshade;
using GoldLeaf.Core;
using Microsoft.Build.Execution;
using Terraria.DataStructures;
using GoldLeaf.Core.Mechanics;

namespace GoldLeaf.Items.Potions
{
    public class PotionPlayer : ModPlayer
    {
        public bool vampirePotion = false;
        public bool consistencyPotion = false;
        public bool vigorPotion = false;

        public override void ResetEffects()
        {
            vampirePotion = false;
            consistencyPotion = false;
            vigorPotion = false;
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

            if (vigorPotion && hit.DamageType.CountsAsClass(DamageClass.Melee))
            {
                //OverhealthManager.AddOverhealth(Player, hit.Damage/10, TimeToTicks(5));
            }
        }
    }
}
