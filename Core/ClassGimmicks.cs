using System;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using GoldLeaf.Items.Pickups;
using Microsoft.Build.Execution;
using Terraria.Audio;

namespace GoldLeaf.Core
{
    public partial class ClassGimmicks : ModPlayer
    {
        int heartTimer;
        int superCritTimer;
        int starTimer;

        public override void PreUpdate()
        {
            heartTimer--;
            starTimer--;
            superCritTimer--;
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit && IsTargetValid(target) && GetInstance<GameplayConfig>().ClassGimmicks)
            {
                if (hit.DamageType == DamageClass.Melee && Main.LocalPlayer.statLife < Main.LocalPlayer.statLifeMax2 && heartTimer <= 0 && item.GetGlobalItem<GoldLeafItem>().canSpawnMiniHearts)
                {
                    Item.NewItem(Player.GetSource_OnHit(target), target.Hitbox, ItemType<HeartTiny>());
                    heartTimer = 120;
                }

                if (hit.DamageType == DamageClass.Magic && Main.LocalPlayer.statMana < Main.LocalPlayer.statManaMax2 && starTimer <= 0 && item.GetGlobalItem<GoldLeafItem>().canSpawnMiniStars)
                {
                    Item.NewItem(Player.GetSource_OnHit(target), target.Hitbox, ItemType<StarTiny>());
                    starTimer = 30;
                }
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit && IsTargetValid(target) && GetInstance<GameplayConfig>().ClassGimmicks)
            {
                if (hit.DamageType == DamageClass.Melee && Main.LocalPlayer.statLife < Main.LocalPlayer.statLifeMax2 && heartTimer <= 0 && proj.GetGlobalProjectile<GoldLeafProjectile>().canSpawnMiniHearts)
                {
                    Item.NewItem(Player.GetSource_OnHit(target), target.Hitbox, ItemType<HeartTiny>());
                    heartTimer = 180;
                }

                if (hit.DamageType == DamageClass.Magic && Main.LocalPlayer.statMana < Main.LocalPlayer.statManaMax2 && starTimer <= 0 && proj.GetGlobalProjectile<GoldLeafProjectile>().canSpawnMiniStars)
                {
                    Item.NewItem(Player.GetSource_OnHit(target), target.Hitbox, ItemType<StarTiny>());
                    starTimer = 60;
                }
            }
        }

        /*public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (superCritTimer <= 0 && modifiers.DamageType == DamageClass.Ranged && GetInstance<GameplayConfig>().ClassGimmicks && proj.GetGlobalProjectile<GoldLeafProjectile>().canSuperCrit)
            {
                modifiers.CritDamage *= 2f;
                modifiers.SetCrit();
                superCritTimer = TimeToTicks(10);
            }
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (superCritTimer <= 0 && modifiers.DamageType == DamageClass.Ranged && GetInstance<GameplayConfig>().ClassGimmicks && item.GetGlobalItem<GoldLeafItem>().canSuperCrit)
            {
                modifiers.CritDamage *= 2f;
                modifiers.SetCrit();
                superCritTimer = TimeToTicks(10);
            }
        }*/
    }

    public partial class ClassGimmickItem : GlobalItem 
    {
        public override bool InstancePerEntity => true;

        public override void GrabRange(Item item, Player player, ref int grabRange)
        {
            if (player.lifeMagnet && (item.type == ItemType<HeartTiny>() || item.type == ItemType<HeartLarge>()))
            {
                grabRange += Item.lifeGrabRange;
            }
            if (player.manaMagnet && (item.type == ItemType<StarTiny>() || item.type == ItemType<StarLarge>()))
            {
                grabRange += Item.manaGrabRange;
            }
        }

        public override bool GrabStyle(Item item, Player player)
        {
            if (player.lifeMagnet && (item.type == ItemType<HeartTiny>() || item.type == ItemType<HeartLarge>()))
            {
                PullItem_Pickup(player, item, 15f, 5);
                return true;
            }
            if (player.manaMagnet && (item.type == ItemType<StarTiny>() || item.type == ItemType<StarLarge>()))
            {
                PullItem_Pickup(player, item, 12f, 5);
                return true;
            }
            return base.GrabStyle(item, player);
        }

        public override bool OnPickup(Item item, Player player)
        {
            if ((item.type == ItemID.Star || item.type == ItemID.SoulCake || item.type == ItemID.SugarPlum) && GetInstance<GameplayConfig>().ClassGimmicks)
            {
                player.ManaEffect(80);
                player.statMana += 80;
                return false;
            }
            return base.OnPickup(item, player);
        }

        private static void PullItem_Pickup(Player player, Item item, float speed, int acc)
        {
            Vector2 vector = new(item.position.X + (float)(item.width / 2), item.position.Y + (float)(item.height / 2));
            float num = player.Center.X - vector.X;
            float num2 = player.Center.Y - vector.Y;
            float num3 = (float)Math.Sqrt(num * num + num2 * num2);
            num3 = speed / num3;
            num *= num3;
            num2 *= num3;
            item.velocity.X = (item.velocity.X * (float)(acc - 1) + num) / (float)acc;
            item.velocity.Y = (item.velocity.Y * (float)(acc - 1) + num2) / (float)acc;
        }
    }
}
