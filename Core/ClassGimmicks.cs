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
                if (hit.DamageType == DamageClass.Melee && Main.LocalPlayer.statLife < Main.LocalPlayer.statLifeMax2 && heartTimer <= 0)
                {
                    Item.NewItem(Player.GetSource_OnHit(target), target.Hitbox, ItemType<HeartTiny>());
                    heartTimer = 120;
                }

                if (hit.DamageType == DamageClass.Magic && Main.LocalPlayer.statMana < Main.LocalPlayer.statManaMax2 && starTimer <= 0)
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
        }*/
    }
}
