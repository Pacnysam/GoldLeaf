using GoldLeaf.Core;
using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Items.Blizzard.Armor;
using GoldLeaf.Items.Jungle.ToxicPositivity;
using GoldLeaf.Items.Ocean;
using GoldLeaf.Items.Pickups;
using GoldLeaf.Tiles.Grove;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core
{
    [ReinitializeDuringResizeArrays]
    public static class ItemSets
    {
        public static (Asset<Texture2D>, Color, bool)[] Glowmask = ItemID.Sets.Factory.CreateNamedSet("Glowmask")
            .Description("Adds a glowmask to this item, Bool automatically draws an in world glowmask")
            .RegisterCustomSet<(Asset<Texture2D>, Color, bool)>((null, Color.White, false));

        public static bool[] FaceMask = ItemID.Sets.Factory.CreateBoolSet(false);

        public static (Asset<Texture2D>, Color, bool)[] BodyExtra = ItemID.Sets.Factory.CreateNamedSet("BodyExtraLayer")
            .Description("Additional layer that draws over body, is formatted like head or leg sheets, boolean is to use leg frames")
            .RegisterCustomSet<(Asset<Texture2D>, Color, bool)>((null, default, false));

        /*public static (Asset<Texture2D>, Color, bool)[] ArmorLongCoat = ItemID.Sets.Factory.CreateNamedSet("ArmorLongCoat")
            .Description("ArmorLongCoat")
            .RegisterCustomSet<(Asset<Texture2D>, Color, bool)>((null, default, false));*/

        public static bool[] HeartPickup = ItemID.Sets.Factory.CreateNamedSet("HeartPickup")
            .RegisterBoolSet(false, ItemID.Heart, ItemID.CandyApple, ItemID.CandyCane, ItemType<HeartTiny>(), ItemType<HeartLarge>());

        public static bool[] isASword = ItemID.Sets.Factory.CreateNamedSet("isASword")
            .Description("Enables unique interactions such as clashing, and unique prefixes.")
            .RegisterBoolSet(false, ItemID.IronBroadsword, ItemID.IronShortsword, ItemID.Terragrim);

        public static bool[] thrownFlail = ItemID.Sets.Factory.CreateNamedSet("thrownFlail")
            .Description("Not used for launched flails like anchor or chain knife.")
            .RegisterBoolSet(false, ItemID.BallOHurt, ItemID.BlueMoon, ItemID.Sunfury, ItemID.DaoofPow, ItemID.TheMeatball, ItemID.FlowerPow,
            ItemID.Flairon, ItemID.DripplerFlail, ItemID.Mace, ItemID.FlamingMace);
    }

    public static partial class ProjectileSets
    {
        public static bool[] summonSpeedImmune = ProjectileID.Sets.Factory.CreateNamedSet("SummonSpeedImmune")
            .Description("Some minions break when using Gold Leaf's summon speed system. Setting this to true will give these minions a damage bonus instead")
            .RegisterBoolSet(false, ProjectileID.Spazmamini, ProjectileID.DeadlySphere);

        public static bool[] sentryCanDetonaterExplode = ProjectileID.Sets.Factory.CreateNamedSet("sentryCanDetonaterExplode")
            .Description("Spawns explosion visual effect when detonated")
            .RegisterBoolSet(true, ProjectileID.DD2LightningAuraT1, ProjectileID.DD2LightningAuraT2, ProjectileID.DD2LightningAuraT3/*, ProjectileType<JellyfishSentry>()*/);
    }

    public static class NPCSets
    {
        public static bool[] bossServant = NPCID.Sets.Factory.CreateNamedSet("BossServant")
            .RegisterBoolSet(false, NPCID.ServantofCthulhu, NPCID.Bee, NPCID.BeeSmall, NPCID.Sharkron, NPCID.Sharkron2, NPCID.TheHungry, NPCID.TheHungryII, 
            NPCID.CultistDragonHead, NPCID.CultistDragonBody1, NPCID.CultistDragonBody2, NPCID.CultistDragonBody3, NPCID.CultistDragonBody4, NPCID.CultistDragonTail,
            NPCID.AncientCultistSquidhead, NPCID.AncientDoom, NPCID.CultistBossClone, NPCID.LeechHead, NPCID.LeechBody, NPCID.LeechTail, NPCID.Probe, 
            NPCID.DD2SkeletonT1, NPCID.DD2SkeletonT3, NPCID.QueenSlimeMinionBlue, NPCID.QueenSlimeMinionPink, NPCID.QueenSlimeMinionPurple);
        
        public static bool[] ccImmune = NPCID.Sets.Factory.CreateNamedSet("CCImmune")
            .RegisterBoolSet(false, NPCID.DD2DarkMageT1, NPCID.DD2DarkMageT3, NPCID.DD2OgreT2, NPCID.DD2OgreT3, NPCID.DD2Betsy, NPCID.PirateShip, NPCID.PirateShipCannon, 
            NPCID.BloodNautilus, NPCID.Pumpking, NPCID.PumpkingBlade, NPCID.LunarTowerSolar, NPCID.LunarTowerVortex, NPCID.LunarTowerNebula, NPCID.LunarTowerStardust, 
            NPCID.SkeletronHand, NPCID.DungeonGuardian);

        public static bool[] ccSusceptibleException = NPCID.Sets.Factory.CreateNamedSet("ccSusceptibleException")
            .Description("Whether or not a given NPC will be susceptible to CC despite having knockback immunity")
            .RegisterBoolSet(false, NPCID.Antlion, NPCID.DesertDjinn, NPCID.DD2DrakinT2, NPCID.DD2DrakinT3, NPCID.GrayGrunt, NPCID.HeadlessHorseman, NPCID.Paladin, 
            NPCID.ShadowFlameApparition, NPCID.SolarSroller, NPCID.ThePossessed, NPCID.EyeballFlyingFish, NPCID.Yeti, NPCID.ZombieMerman);
    }

    public static class BuffSets
    {
        public static bool[] Cooldown = BuffID.Sets.Factory.CreateNamedSet("Cooldown")
            .RegisterBoolSet(false, BuffID.PotionSickness, BuffID.ManaSickness, BuffID.ChaosState, BuffID.BrainOfConfusionBuff, BuffType<SnapFreezeBuff>(), BuffType<SafetyBlanketBuff>());

        public static bool[] Cosmetic = BuffID.Sets.Factory.CreateNamedSet("Cosmetic")
            .RegisterBoolSet(false, BuffID.Slimed, BuffID.GelBalloonBuff, BuffID.Lovestruck, BuffID.Stinky, BuffID.Wet);

        public static bool[] NoCleanseTooltip = BuffID.Sets.Factory.CreateNamedSet("NoCleanseTooltip")
            .Description("Does not display tooltip for being cleansable")
            .RegisterBoolSet(false, BuffType<SafetyBlanketBuff>(), BuffID.StarInBottle);

        public static bool[] IsRemovable = BuffID.Sets.Factory.CreateNamedSet("IsRemovable")
            .Description("Can be removed by safety blanket and similar methods")
            .RegisterBoolSet(true, BuffID.MoonLeech, BuffID.TheTongue, BuffID.Obstructed, BuffID.Horrified, BuffID.Hunger, BuffID.NeutralHunger, BuffID.Starving, BuffID.StarInBottle, BuffType<ToxicPositivityBuff>());
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
