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
using GoldLeaf.Items.Accessories;
using GoldLeaf.Items.Blizzard.Armor;
using GoldLeaf.Items.Armor;

namespace GoldLeaf.Core
{
    [ReinitializeDuringResizeArrays]
    public static class ItemSets
    {
        public static (Asset<Texture2D>, Color, bool)[] Glowmask = ItemID.Sets.Factory.CreateNamedSet("Glowmask")
            .Description("Bool automatically draws an in world glowmask")
            .RegisterCustomSet<(Asset<Texture2D>, Color, bool)>((null, Color.White, false));

        public static bool[] FaceMaskLayer = ItemID.Sets.Factory.CreateNamedSet("FaceMaskLayer")
            .Description("Draws helmet on face mask layer")
            .RegisterBoolSet(false);

        public static bool[] FaceMask = ItemID.Sets.Factory.CreateBoolSet(false);

        /*public static Asset<Texture2D>[] FaceMaskLayer = ItemID.Sets.Factory.CreateNamedSet("FaceMaskLayer")
            .Description("Draws helmet on face mask layer")
            .RegisterCustomSet<Asset<Texture2D>>(null);*/
    }
    public static class ProjectileSets
    {
        public static bool[] summonSpeedImmune = ProjectileID.Sets.Factory.CreateNamedSet("SummonSpeedImmune")
            .RegisterBoolSet(false, ProjectileID.Spazmamini, ProjectileID.DeadlySphere);
    }
    public static class BuffSets
    {
        public static bool[] Cooldown = BuffID.Sets.Factory.CreateNamedSet("Cooldown")
            .RegisterBoolSet(false, BuffID.PotionSickness, BuffID.ManaSickness, BuffID.ChaosState, BuffType<SnapFreezeBuff>(), BuffType<SafetyBlanketBuff>());

        public static bool[] IsRemovable = BuffID.Sets.Factory.CreateNamedSet("IsRemovable")
            .Description("Can be removed by safety blanket and similar methods")
            .RegisterBoolSet(true, BuffID.MoonLeech, BuffID.TheTongue, BuffID.Obstructed, BuffID.Horrified, BuffID.Hunger, BuffID.NeutralHunger, BuffID.Starving, BuffType<ToxicPositivityBuff>());
    }
    public static class ArmorSets
    {
        
    }
}
