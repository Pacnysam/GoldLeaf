using GoldLeaf.Core;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using GoldLeaf.Items.Grove;
using Terraria.ID;

namespace GoldLeaf.Effects.Dusts
{
    public class HotSmoke : ModDust //i did not make any of this! i nabbed it from starlight river
    {
        public override string Texture => "GoldLeaf/Effects/Dusts/SpecialSmoke";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 36, 34, 36);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color color;
            if (dust.alpha < 100)
                color = Color.Lerp(Color.Yellow, new Color(246, 68, 14), dust.alpha / 100f);
            else if (dust.alpha < 140)
                color = Color.Lerp(new Color(246, 68, 14), new Color(30, 30, 30), (dust.alpha - 100) / 80f);
            else
                color = new Color(30, 30, 30);

            return color * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            if (dust.velocity.Length() > 3)
                dust.velocity *= 0.85f;
            else
                dust.velocity *= 0.9f;

            if (dust.alpha > 100)
            {
                dust.scale += 0.012f;
                dust.alpha += 2;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.985f;
                dust.alpha += 4;
            }

            dust.position += dust.velocity;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }

    public class HotSmokeFast : ModDust
    {
        public override string Texture => "GoldLeaf/Effects/Dusts/SpecialSmoke";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 36, 34, 36);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color color;
            if (dust.alpha < 40)
                color = Color.Lerp(Color.Yellow, new Color(246, 68, 14), dust.alpha / 40f);
            else if (dust.alpha < 80)
                color = Color.Lerp(new Color(246, 68, 14), new Color(25, 25, 25), (dust.alpha - 40) / 80f);
            else
                color = new Color(25, 25, 25);

            return color * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            if (dust.velocity.Length() > 3)
                dust.velocity *= 0.85f;
            else
                dust.velocity *= 0.92f;

            if (dust.alpha > 60)
            {
                dust.scale += 0.01f;
                dust.alpha += 6;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.985f;
                dust.alpha += 4;
            }

            dust.position += dust.velocity;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }

    public class Ember : AetherEmber //nabbed from slr
    {
        public override string Texture => "GoldLeaf/Effects/Dusts/SpecialSmokeDust";

        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.aiStyle = 1;
            Projectile.width = Projectile.height = 12;
            //ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            //ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Projectile.timeLeft = 70;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.scale *= 0.98f;

            if (Main.rand.NextBool(2))
            {
                var dust = Dust.NewDustPerfect(Projectile.Center, DustType<HotSmokeFast>(), Main.rand.NextVector2Circular(1.5f, 1.5f));
                dust.scale = 0.65f * Projectile.scale;
                dust.rotation = Main.rand.NextFloatDirection();
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.damage--;
            target.AddBuff(BuffID.OnFire, Main.rand.Next(Helper.TimeToTicks(6f), Helper.TimeToTicks(8.5f)));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage--;
            target.AddBuff(BuffID.OnFire, Main.rand.Next(Helper.TimeToTicks(6f), Helper.TimeToTicks(8.5f)));
        }
    }
}