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

namespace GoldLeaf.Core
{
    public class FirstStrikePlayer : ModPlayer
    {
        public delegate void FirstStrikeDelegate(Player player, NPC target, NPC.HitModifiers hit);
        public static event FirstStrikeDelegate FirstStrikeEvent;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
           if (!target.GetGlobalNPC<FirstStrikeNPC>().hasBeenStruck)
            {
                FirstStrikeEvent?.Invoke(Player, target, modifiers);
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
