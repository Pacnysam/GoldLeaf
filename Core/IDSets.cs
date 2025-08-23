﻿using System;
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
using GoldLeaf.Items.Ocean;

namespace GoldLeaf.Core
{
    [ReinitializeDuringResizeArrays]
    public static class ItemSets
    {
        public static (Asset<Texture2D>, Color, bool)[] Glowmask = ItemID.Sets.Factory.CreateNamedSet("Glowmask")
            .Description("Bool automatically draws an in world glowmask")
            .RegisterCustomSet<(Asset<Texture2D>, Color, bool)>((null, Color.White, false));

        public static bool[] FaceMask = ItemID.Sets.Factory.CreateBoolSet(false);

        public static (Asset<Texture2D>, Color)[] BodyExtra = ItemID.Sets.Factory.CreateNamedSet("BodyExtraLayer")
            .Description("Additional layer that draws over body, is formatted like head, leg sheets")
            .RegisterCustomSet<(Asset<Texture2D>, Color)>((null, default));
    }

    public static class ProjectileSets
    {
        public static bool[] summonSpeedImmune = ProjectileID.Sets.Factory.CreateNamedSet("SummonSpeedImmune")
            .RegisterBoolSet(false, ProjectileID.Spazmamini, ProjectileID.DeadlySphere);

        public static bool[] sentryCanDetonaterExplode = ProjectileID.Sets.Factory.CreateNamedSet("sentryCanDetonaterExplode")
            .RegisterBoolSet(true, ProjectileID.DD2LightningAuraT1, ProjectileID.DD2LightningAuraT2, ProjectileID.DD2LightningAuraT3, ProjectileType<JellyfishSentry>());
    }

    public static class NPCSets
    {
        public static bool[] bossServant = NPCID.Sets.Factory.CreateNamedSet("BossServant")
            .RegisterBoolSet(false, NPCID.ServantofCthulhu, NPCID.Bee, NPCID.BeeSmall, NPCID.Sharkron, NPCID.Sharkron2, NPCID.TheHungry, NPCID.TheHungryII, 
            NPCID.CultistDragonHead, NPCID.CultistDragonBody1, NPCID.CultistDragonBody2, NPCID.CultistDragonBody3, NPCID.CultistDragonBody4, NPCID.CultistDragonTail,
            NPCID.AncientCultistSquidhead, NPCID.AncientDoom, NPCID.CultistBossClone, NPCID.LeechHead, NPCID.LeechBody, NPCID.LeechTail, NPCID.Probe, 
            NPCID.DD2SkeletonT1, NPCID.DD2SkeletonT3, NPCID.QueenSlimeMinionBlue, NPCID.QueenSlimeMinionPink, NPCID.QueenSlimeMinionPurple);
        
        public static bool[] ccImmune = NPCID.Sets.Factory.CreateNamedSet("CC Immune")
            .RegisterBoolSet(false, NPCID.DD2DarkMageT1, NPCID.DD2DarkMageT3, NPCID.DD2OgreT2, NPCID.DD2OgreT3, NPCID.DD2Betsy, NPCID.PirateShip, NPCID.PirateShipCannon, 
            NPCID.BloodNautilus, NPCID.Pumpking, NPCID.PumpkingBlade, NPCID.LunarTowerSolar, NPCID.LunarTowerVortex, NPCID.LunarTowerNebula, NPCID.LunarTowerStardust, 
            NPCID.SkeletronHand, NPCID.DungeonGuardian);
    }

    public static class BuffSets
    {
        public static bool[] Cooldown = BuffID.Sets.Factory.CreateNamedSet("Cooldown")
            .RegisterBoolSet(false, BuffID.PotionSickness, BuffID.ManaSickness, BuffID.ChaosState, BuffID.BrainOfConfusionBuff, BuffType<SnapFreezeBuff>(), BuffType<SafetyBlanketBuff>());

        public static bool[] Cosmetic = BuffID.Sets.Factory.CreateNamedSet("Cosmetic")
            .RegisterBoolSet(false, BuffID.Slimed, BuffID.GelBalloonBuff, BuffID.Lovestruck, BuffID.Stinky, BuffID.Wet);

        public static bool[] IsRemovable = BuffID.Sets.Factory.CreateNamedSet("IsRemovable")
            .Description("Can be removed by safety blanket and similar methods")
            .RegisterBoolSet(true, BuffID.MoonLeech, BuffID.TheTongue, BuffID.Obstructed, BuffID.Horrified, BuffID.Hunger, BuffID.NeutralHunger, BuffID.Starving, BuffType<ToxicPositivityBuff>());
    }

    public static class ArmorSets
    {
        /*public static bool[] FaceMaskLayer = ArmorIDs.Head.Sets.Factory.CreateNamedSet("FaceMaskLayer")
            .Description("Draws helmet on face mask layer")
            .RegisterBoolSet(false);
        public static bool[] BodyBeltLayer = ArmorIDs.Body.Sets.Factory.CreateNamedSet("BodyBeltLayer")
            .Description("Draws belt layer over body, is under front arm")
            .RegisterBoolSet(false);*/
    }
}
