using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Effects.Dusts;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using GoldLeaf.Core;
using Mono.Cecil;
using Terraria.DataStructures;
using System;
using System.Diagnostics.Metrics;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;

namespace GoldLeaf.Items.Grove
{
	public class EveDroplet : ModItem
	{
        private static Asset<Texture2D> tex;
        public override void Load()
        {
            tex = Request<Texture2D>(Texture);
        }

        int dustTime = 12;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
		{
			Item.damage = 19;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 20;
			Item.height = 24;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
            Item.ammo = Item.type;
            Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 0, 0, 10);
            Item.rare = ItemRarityID.White;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;

            //ItemID.Sets.ItemIconPulse[Item.type] = true;

            Item.shoot = ProjectileType<EveDropletP>();
			Item.shootSpeed = 10f;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-7, 7))), type, damage, knockback, player.whoAmI);
			Main.projectile[p].GetGlobalProjectile<GoldLeafProjectile>().gravity = Main.rand.NextFloat(0.25f, 0.35f);
            Main.projectile[p].GetGlobalProjectile<GoldLeafProjectile>().gravityDelay = Main.rand.Next(10, 20);
            Main.projectile[p].timeLeft = (Main.rand.Next(40, 125));
            Main.projectile[p].velocity *= (Main.rand.NextFloat(0.9f, 1.1f));

            return false;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Item.timeSinceItemSpawned % dustTime == 0) 
            {
                dustTime = Main.rand.Next(9, 38);

                var dust = Dust.NewDustPerfect(new Vector2(Item.position.X + Main.rand.Next(0, Item.width), Item.position.Y + Main.rand.Next(0, Item.height)), DustType<LightDust>(), new Vector2(0, Main.rand.NextFloat(-0.4f, -1.2f)), 0, new Color(255, 152, 221), Main.rand.NextFloat(0.3f, 0.7f));
                dust.noGravity = true;
                dust.fadeIn = 1.4f;
            }

            /*if (Main.rand.NextBool(20))
            {
                //var dust = Dust.NewDustDirect(Item.position, Item.width, Item.height, DustID.FireworksRGB, 0f, -1.7f, 0, new Color(255, 152, 221), 0.65f);
                var dust = Dust.NewDustPerfect(new Vector2(Item.position.X + Main.rand.Next(0, Item.width), Item.position.Y + Main.rand.Next(0, Item.height)), DustType<LightDust>(), new Vector2(0, Main.rand.NextFloat(-0.4f, -1.2f)), 0, new Color(255, 152, 221), Main.rand.NextFloat(0.4f, 0.8f));
                dust.noGravity = true;
                dust.fadeIn = 1.4f;
            }*/
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Color color = Color.White; color.A = 0;

            spriteBatch.Draw
            (

                tex.Value,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - tex.Height() * 0.5f
                ),
                new Rectangle(0, 0, tex.Width(), tex.Height()),
                color * ((float)Math.Sin(GoldLeafWorld.rottime) * 0.5f),
                rotation,
                tex.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }

	public class EveDropletP : ModProjectile 
	{
        public override void SetStaticDefaults()
        {
			//DisplayName.SetDefault("Eve Droplet");
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 18;
            Projectile.height = 28;
            Projectile.friendly = true;
			Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.DamageType = DamageClass.Ranged;

            Projectile.GetGlobalProjectile<GoldLeafProjectile>().gravity = 0.3f;
            Projectile.GetGlobalProjectile<GoldLeafProjectile>().gravityDelay = 15;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            Projectile.spriteDirection = -Projectile.direction;
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }
            if (Main.rand.NextBool(3)) Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<EveDust>());


        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Vector2 drawOrigin = new (texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * (float)(((float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length) / 2);

                Main.spriteBatch.Draw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0f);
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.buffImmune[BuffID.OnFire] = false;
            target.buffImmune[BuffID.Frostburn] = false;
            target.buffImmune[BuffID.CursedInferno] = false;
            target.buffImmune[BuffID.Ichor] = false;
            target.buffImmune[BuffID.ShadowFlame] = false;
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            SoundEngine.PlaySound(SoundID.Shimmer1, Projectile.Center);

            for (int i = 0; i < 12; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<EveDust>(), Main.rand.NextFloat(-3 , 3) + Projectile.velocity.X / 3, Main.rand.Next(-6, 3) + Projectile.velocity.Y / 3);
            }
        }
    }

    internal class EveDust : ModDust
    {
        public override void SetStaticDefaults()
        {
            UpdateType = 33;
        }

        public override void OnSpawn(Dust dust)
        {
            dust.alpha = 80;
            dust.velocity *= 0.7f;
            dust.velocity.Y += 1f;
        }
    }
}