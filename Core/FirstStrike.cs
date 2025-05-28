using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Items.Accessories;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Terraria.WorldBuilding;

namespace GoldLeaf.Core
{
    public class FirstStrikePlayer : ModPlayer
    {
        public delegate void FirstStrikeModificationDelegate(Player player, NPC target, NPC.HitModifiers hit);
        public static event FirstStrikeModificationDelegate ModifyFirstStrike;

        public delegate void FirstStrikeDelegate(Player player, NPC target, NPC.HitInfo hit, int damageDone);
        public static event FirstStrikeDelegate OnFirstStrike;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
           if (!target.GetGlobalNPC<FirstStrikeNPC>().hasBeenStruck && IsTargetValid(target))
           {
                ModifyFirstStrike?.Invoke(Player, target, modifiers);
           }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.GetGlobalNPC<FirstStrikeNPC>().hasBeenStruck && IsTargetValid(target))
            {
                OnFirstStrike?.Invoke(Player, target, hit, damageDone);
                target.GetGlobalNPC<FirstStrikeNPC>().hasBeenStruck = true;
            }
        }
    }

    public class FirstStrikeNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool hasBeenStruck = false;
    }
}
