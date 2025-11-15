using GoldLeaf.Core;
using GoldLeaf.Core.CrossMod;
using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Accessories;
using GoldLeaf.Prefixes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Sandstorm
{
    public class SunstoneStaff : ModItem
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Item.ArmorPenetration);

        const int RANGE = 175;
        const float RADIUSMULT = 0.75f;

        public override void SetStaticDefaults()
        {
            ItemSets.Glowmask[Type] = (glowTex, ColorHelper.AdditiveWhite(175) * 0.65f, true);
            Item.AddElements([Element.Arcane]);
        }
        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 1, 35, 0);
            Item.rare = ItemRarityID.Green;

            Item.damage = 12;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 9;
            Item.ArmorPenetration = 5;

            Item.shootSpeed = 3.75f;
            Item.knockBack = 0.5f;

            Item.useTime = 4;
            Item.useAnimation = 12;
            Item.reuseDelay = 10;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = new SoundStyle("GoldLeaf/Sounds/SE/Kirby/SuperStar/MirrorWave") { PitchVariance = 0.35f, Volume = 0.85f };
            Item.staff[Item.type] = true;
            Item.noMelee = true;

            Item.shoot = ProjectileType<SunstoneStaffBolt>();

            Item.width = 30;
            Item.height = 30;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            int distance = 0;
            while (!WorldGen.SolidTile((int)(position.X / 16f), (int)(position.Y / 16f)) && distance++ < RANGE)
            {
                position += velocity;

                if (position.Distance(Main.MouseWorld) <= 5)
                {
                    position = Main.MouseWorld;
                    break;
                }
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact with { Volume = 0.625f, PitchVariance = 0.4f }, position);
            
            Projectile proj = Projectile.NewProjectileDirect(source, position + Vector2.One.RotatedByRandom(MathHelper.TwoPi) * velocity.Length() * Main.rand.NextFloat(0f, 10.5f) * RADIUSMULT, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * velocity.Length() * Main.rand.NextFloat(0.3f, 1f) * RADIUSMULT, type, damage, knockback, player.whoAmI, Main.rand.NextFloat(0.055f, 0.45f));
            proj.ai[0] = Main.rand.NextFloat(0.055f, 0.45f);
            proj.scale = Main.rand.NextFloat(0.65f, 1.2f);
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.WandofFrosting);
            //recipe.AddIngredient(ItemType<Sunstone>(), 6);
            recipe.AddIngredient(ItemID.JungleSpores, 4);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    public class SunstoneStaffBolt : ModProjectile
    {
        private static Asset<Texture2D> glowTex;
        private static Asset<Texture2D> shineTex;
        private static Asset<Texture2D> bloomTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
            shineTex = Request<Texture2D>("GoldLeaf/Textures/Shine");
            bloomTex = Request<Texture2D>("GoldLeaf/Textures/Masks/Mask1");
        }
        public override void SetStaticDefaults()
        {
            Projectile.AddElements([Element.Arcane]);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 28;
            Projectile.height = 36;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.scale = Main.rand.NextFloat(0.65f, 1.2f);
            Projectile.ai[0] = Main.rand.NextFloat(0.055f, 0.45f);

            Projectile.DamageType = DamageClass.Magic;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (!Main.dedServ && Main.myPlayer == Projectile.owner)
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.TerraBlade,
                    new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = Projectile.velocity.RotatedBy(MathHelper.PiOver2) });
            }
        }

        public override void AI()
        {
            Projectile.TryGetOwner(out Player player);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.Counter() >= Projectile.ai[0] * 100)
            {
                Projectile.velocity += Vector2.Normalize(player.Center - Projectile.Center) * ((Projectile.ai[0] * 0.5f) + (Projectile.Counter() * 0.03f));

                if (Projectile.velocity.Length() >= 9)
                {
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 10f;
                }

                if (Projectile.Center.Distance(player.Center) <= 300)
                    Projectile.alpha += 9;
            }
            else
            {
                Projectile.velocity *= 0.98f;
            }
            
            if (Projectile.localAI[0] <= 0)
            {
                Projectile.localAI[0] = (100 - Projectile.Counter()) * 0.035f;

                /*Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.YellowStarDust, Vector2.Zero, 0, new Color(222, 188, 34) { A = 100 } * 0.85f, 0.7f * Projectile.scale);
                dust.noLight = true;
                dust.fadeIn = 0.7f;*/
            }
            Projectile.localAI[0] -= 1;
            //new Color(52, 229, 66)
            if (!Main.dedServ)
                Lighting.AddLight((int)(Projectile.position.X / 16), (int)(Projectile.position.Y / 16), 72 / 255f * 0.4f, 229 / 255f * 0.4f, 26 / 255f * 0.4f);

            if (Projectile.Hitbox.Intersects(player.Hitbox))
                Projectile.Kill();
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Inflate(8, 8);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.65f) + 1;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.Counter() < 18) return false;

            return base.CanHitNPC(target);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;

            Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center - Main.screenPosition, null, new Color(222, 141, 7) { A = 0 } * Projectile.Opacity * 0.2f, 0f, bloomTex.Size() / 2f, new Vector2(1f, Math.Clamp(Projectile.velocity.Length() * 0.25f, 0.5f, 1f)) * (1.35f + (float)(Math.Sin(GoldLeafWorld.rottime * 5.25f) * 0.1f)) * Projectile.scale * 0.425f, SpriteEffects.None);
            Main.EntitySpriteDraw(bloomTex.Value, Projectile.Center - Main.screenPosition, null, new Color(52, 229, 66) { A = 0 } * Projectile.Opacity * 0.45f, 0f, bloomTex.Size() / 2f, new Vector2(1f, Math.Clamp(Projectile.velocity.Length() * 0.25f, 0.5f, 1f)) * (1.35f + (float)(Math.Sin(GoldLeafWorld.rottime * 5.25f) * 0.1f)) * Projectile.scale * 0.25f, SpriteEffects.None);

            Main.EntitySpriteDraw(shineTex.Value, Projectile.Center - Main.screenPosition, null, Color.Black * Projectile.Opacity * 0.25f, Projectile.rotation, (shineTex.Size() / 2f) + new Vector2(0, -8), new Vector2(1f, Math.Clamp(Projectile.velocity.Length() * 0.425f, 0.65f, 3.25f)) * Projectile.scale * 1.125f, SpriteEffects.None);
            Main.EntitySpriteDraw(shineTex.Value, Projectile.Center - Main.screenPosition, null, new Color(222, 141, 7) { A = 100 } * Projectile.Opacity * 0.65f, Projectile.rotation, shineTex.Size() / 2f, new Vector2(1f, Math.Clamp(Projectile.velocity.Length() * 0.425f, 0.65f, 3.25f)) * Projectile.scale * 0.725f, SpriteEffects.None);

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite(240) * Projectile.Opacity * 0.5f, Projectile.rotation, tex.Size() / 2f, new Vector2(1f, Math.Clamp(Projectile.velocity.Length() * 0.35f, 0.5f, 4.75f)) * Projectile.scale, SpriteEffects.None);
            
            Main.EntitySpriteDraw(glowTex.Value, Projectile.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite(200) * (Projectile.Opacity + 0.5f), Projectile.rotation, glowTex.Size() / 2f, new Vector2(1f, Math.Clamp(Projectile.velocity.Length() * 0.25f, 0.5f, 1f)) * (1 + (float)(Math.Sin(GoldLeafWorld.rottime * 5.25f) * 0.1f)) * Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(glowTex.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 142) { A = 0 } * 0.25f * (Projectile.Opacity + 0.75f), Projectile.rotation, glowTex.Size() / 2f, new Vector2(1f, Math.Clamp(Projectile.velocity.Length() * 0.25f, 0.5f, 1f)) * (1.35f + (float)(Math.Sin(GoldLeafWorld.rottime * 5.25f) * 0.1f)) * Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
