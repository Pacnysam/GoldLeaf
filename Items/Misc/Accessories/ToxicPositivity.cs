using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Items.Grove;
using System.Diagnostics.Metrics;
using System;
using GoldLeaf.Effects.Dusts;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace GoldLeaf.Items.Misc.Accessories
{
	public class ToxicPositivity : ModItem
	{
        public override void SetDefaults()
		{
            Item.width = 28;
            Item.height = 32;

            Item.value = Item.buyPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Green;

            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ToxicPositivePlayer>().toxicPositivity = true;
        }
    }

    public class ToxicPositivityBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;

            BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.Poisoned);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<ToxicPositiveNPC>().toxicPositive = true;
            npc.GetGlobalNPC<GoldLeafNPC>().defenseMod -= 6;
        }
    }

    public class ToxicPositivePlayer : ModPlayer
    {
        public bool toxicPositivity = false;

        public override void ResetEffects()
        {
            toxicPositivity = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (toxicPositivity && Main.rand.NextBool(20 + (Main.LocalPlayer.CountBuffs() * 2), 100))
            {
                target.AddBuff(BuffType<ToxicPositivityBuff>(), 120 + (Main.LocalPlayer.CountBuffs() * 120));
                target.AddBuff(BuffID.Poisoned, 120 + (Main.LocalPlayer.CountBuffs() * 120));
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (toxicPositivity && Main.rand.NextBool(Main.LocalPlayer.CountBuffs(), 100))
            {
                Main.LocalPlayer.AddBuff(BuffID.Poisoned, 180);
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (toxicPositivity && Main.rand.NextBool(Main.LocalPlayer.CountBuffs(), 100))
            {
                Main.LocalPlayer.AddBuff(BuffID.Poisoned, 60 + (Main.LocalPlayer.CountBuffs() * 20));
            }
        }
    }

    public class ToxicPositiveNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool toxicPositive = false;

        public override void ResetEffects(NPC npc)
        {
            toxicPositive = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (toxicPositive)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                npc.lifeRegen -= 2 * 12;
                if (damage < 12)
                {
                    damage = 12;
                }
            }
        }
    }
}