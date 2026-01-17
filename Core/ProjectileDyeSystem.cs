using GoldLeaf.Items.Vanity;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core
{
    public enum DyeGroup
    {
        None,
        Head,
        Body,
        Legs,
        Minion,
        Sentry,
        Pet,
        LightPet,
        Grapple,
        Special,
    }

    public static partial class ProjectileSets
    {
        public static DyeGroup[] DyeGroup = ItemID.Sets.Factory.CreateNamedSet("DyeGroup")
            .Description("The dye group this projectile will use")
            .RegisterCustomSet(Core.DyeGroup.None);
    }

    public static partial class Helper 
    {
        public static int GetDye(this Projectile proj)
        {
            if (proj.owner != 255)
            {
                Player owner = Main.player[proj.owner];
                return ProjectileSets.DyeGroup[proj.type] switch
                {
                    DyeGroup.Head => owner.cHead,
                    DyeGroup.Body => owner.cBody,
                    DyeGroup.Legs => owner.cLegs,
                    DyeGroup.Minion => owner.cMinion,
                    DyeGroup.Sentry => owner.GetModPlayer<TurretPaintPlayer>().cSentry,
                    DyeGroup.Pet => owner.cPet,
                    DyeGroup.LightPet => owner.cLight,
                    DyeGroup.Grapple => owner.cGrapple,
                    //DyeGroup.Special => owner.cMinecart, TODO: implement item or ui to activate this
                    _ => 0,
                };
            }
            return 0;
        }
        public static ArmorShaderData GetDyeShader(this Projectile proj)
        {
            return GameShaders.Armor.GetSecondaryShader(GetDye(proj), Main.player[proj.owner]);
        }
    }

    public class ProjectileDyeSystem : ModPlayer
    {
        public override void Load()
        {
            On_Main.GetProjectileDesiredShader += EvaluateDyeGroups;
        }
        public override void Unload()
        {
            On_Main.GetProjectileDesiredShader -= EvaluateDyeGroups;
        }

        private int EvaluateDyeGroups(On_Main.orig_GetProjectileDesiredShader orig, Projectile proj)
        {
            int dye = orig(proj);

            if (proj.owner != 255)
            {
                if (proj.sentry)
                    dye = Main.player[proj.owner].GetModPlayer<TurretPaintPlayer>().cSentry;

                if (ProjectileSets.DyeGroup[proj.type] != DyeGroup.None)
                    dye = proj.GetDye();
            }
            return dye;
        }
    }
}
