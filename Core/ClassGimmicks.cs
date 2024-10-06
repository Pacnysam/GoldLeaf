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

namespace GoldLeaf.Core
{
    public partial class ClassGimmicks : ModPlayer
    {
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit && IsTargetValid(target))
            {
                if (hit.DamageType == DamageClass.Melee && Main.LocalPlayer.statLife < Main.LocalPlayer.statLifeMax2)
                {
                    Item.NewItem(Player.GetSource_OnHit(target), target.Hitbox, ItemType<HeartTiny>());
                }

                if (hit.DamageType == DamageClass.Ranged && target.defense < 10000)
                {
                    hit.Damage += target.defense / 2;
                }

                if (hit.DamageType == DamageClass.Magic && Main.LocalPlayer.statMana < Main.LocalPlayer.statManaMax2)
                {
                    Item.NewItem(Player.GetSource_OnHit(target), target.Hitbox, ItemType<StarTiny>());
                }
            }
        }
    }
}
