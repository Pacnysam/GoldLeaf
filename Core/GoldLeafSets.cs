using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using GoldLeaf.Effects.Dusts;
using GoldLeaf.Tiles.Grove;
using Terraria.DataStructures;
using Terraria.Audio;
using GoldLeaf.Core;
using ReLogic.Content;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Items.Misc.Accessories;
using GoldLeaf.Items.Blizzard.Armor;

namespace GoldLeaf.Core
{
    [ReinitializeDuringResizeArrays]
    public static class GoldLeafSets
    {
        public static bool[] Cooldown = BuffID.Sets.Factory.CreateNamedSet("Cooldown")
            .RegisterBoolSet(false, BuffID.PotionSickness, BuffID.ManaSickness, BuffID.ChaosState, BuffType<SnapFreezeBuff>(), BuffType<SafetyBlanketBuff>());

        public static bool[] IsRemovable = BuffID.Sets.Factory.CreateNamedSet("IsRemovable")
            .Description("Can be removed by safety blanket and similar methods")
            .RegisterBoolSet(true, BuffID.MoonLeech, BuffID.TheTongue, BuffID.Obstructed, BuffID.Horrified, BuffID.Hunger, BuffID.NeutralHunger, BuffID.Starving, BuffType<ToxicPositivityBuff>());
    }
}
