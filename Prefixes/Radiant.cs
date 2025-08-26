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

namespace GoldLeaf.Prefixes
{
    public class Radiant : TooltipPrefix
    {
        public override PrefixCategory Category => PrefixCategory.Magic;
        public override bool IsNegative => false;
        
        public override void Apply(Item item)
        {
            item.GetGlobalItem<RadiantItem>().radiant = true;
        }

        public override float RollChance(Item item)
        {
            return 0.5f;
        }

        public override bool CanRoll(Item item)
        {
            return !item.CountsAsClass(DamageClass.Summon) && item.mana > 0;
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1.75f;
        }
    }

    public class RadiantPlayer : ModPlayer 
    {
        int starTimer;
        const int STARCOOLDOWN = 120;

        public override void PreUpdate()
        {
            starTimer--;
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit && IsTargetValid(target) && item.GetGlobalItem<VitalItem>().vital && starTimer <= 0)
            {
                int i = Item.NewItem(Player.GetSource_OnHit(target), target.Hitbox, ItemType<StarTiny>(), 1, true, 0, true);
                starTimer = STARCOOLDOWN;
                Main.item[i].playerIndexTheItemIsReservedFor = Player.whoAmI;

                if (Main.netMode == NetmodeID.MultiplayerClient && i >= 0)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 1f);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit && IsTargetValid(target) && proj.GetGlobalProjectile<RadiantProjectile>().radiant && starTimer <= 0)
            {
                int i = Item.NewItem(Player.GetSource_OnHit(target), target.Hitbox, ItemType<StarTiny>(), 1, true, 0, true);
                starTimer = STARCOOLDOWN;
                Main.item[i].playerIndexTheItemIsReservedFor = Player.whoAmI;

                if (Main.netMode == NetmodeID.MultiplayerClient && i >= 0)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 1f);
            }
        }
    }

    public class RadiantItem : GlobalItem 
    {
        public override bool InstancePerEntity => true;
        public bool radiant = false;

        public override void SetDefaults(Item entity)
        {
            radiant = false;
        }
    }

    public class RadiantProjectile : GlobalProjectile 
    {
        public override bool InstancePerEntity => true;
        public bool radiant = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse realSource && realSource.Item.GetGlobalItem<RadiantItem>().radiant)
            {
                projectile.GetGlobalProjectile<RadiantProjectile>().radiant = true;
            }
            if (source is EntitySource_Parent parent && parent.Entity is Projectile proj && proj.GetGlobalProjectile<RadiantProjectile>().radiant)
            {
                projectile.GetGlobalProjectile<RadiantProjectile>().radiant = true;
            }
        }
    }

}
