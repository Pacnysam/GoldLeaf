using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Enums;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Core;
using GoldLeaf.Items.Pickups;
using Terraria.DataStructures;

namespace GoldLeaf.Prefixes.Enchantments
{
    public class Vital : TooltipPrefix
    {
        public override PrefixCategory Category => PrefixCategory.AnyWeapon;
        public override bool IsNegative => false;
        
        public override void Apply(Item item)
        {
            item.GetGlobalItem<VitalItem>().vital = true;
        }

        public override float RollChance(Item item)
        {
            return 0.25f;
        }

        public override bool CanRoll(Item item)
        {
            return !item.CountsAsClass(DamageClass.Summon);
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 2f;
        }
    }

    public class VitalPlayer : ModPlayer 
    {
        int heartTimer;
        const int HEARTCOOLDOWN = 180;

        public override void PreUpdate()
        {
            heartTimer--;
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit && IsTargetValid(target) && item.GetGlobalItem<VitalItem>().vital && heartTimer <= 0)
            {
                int i = Item.NewItem(Player.GetSource_OnHit(target), target.Hitbox, ItemType<HeartTiny>());
                heartTimer = HEARTCOOLDOWN;
                Main.item[i].playerIndexTheItemIsReservedFor = Player.whoAmI;
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit && IsTargetValid(target) && proj.GetGlobalProjectile<VitalProjectile>().vital && heartTimer <= 0)
            {
                int i = Item.NewItem(Player.GetSource_OnHit(target), target.Hitbox, ItemType<HeartTiny>());
                heartTimer = HEARTCOOLDOWN;
                Main.item[i].playerIndexTheItemIsReservedFor = Player.whoAmI;
            }
        }
    }

    public class VitalItem : GlobalItem 
    {
        public override bool InstancePerEntity => true;
        public bool vital = false;

        public override void SetDefaults(Item entity)
        {
            vital = false;
        }
    }

    public class VitalProjectile : GlobalProjectile 
    {
        public override bool InstancePerEntity => true;
        public bool vital = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse realSource && realSource.Item.GetGlobalItem<VitalItem>().vital)
            {
                projectile.GetGlobalProjectile<VitalProjectile>().vital = true;
            }
            if (source is EntitySource_Parent parent && parent.Entity is Projectile proj && proj.GetGlobalProjectile<VitalProjectile>().vital)
            {
                projectile.GetGlobalProjectile<VitalProjectile>().vital = true;
            }
        }
    }

}
