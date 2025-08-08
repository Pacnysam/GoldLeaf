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
using GoldLeaf.Items.Grove.Boss;
using GoldLeaf.Tiles.Decor;
using GoldLeaf.Tiles.Grove;

namespace GoldLeaf.Items.Grove
{
    public class GroveHook : ModItem //TODO: move to sunstone
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 40;
            Item.CloneDefaults(ItemID.AmethystHook);
            Item.shootSpeed = 20f;
            Item.shoot = ProjectileType<GroveHookP>();
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, 0, 80, 0);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            spriteBatch.Draw
            (
                glowTex.Value,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - glowTex.Height() * 0.5f
                ),
                new Rectangle(0, 0, glowTex.Width(), glowTex.Height()),
                ColorHelper.AdditiveWhite() * ((float)Math.Sin(GoldLeafWorld.rottime) * 0.75f + 0.75f),
                rotation,
                glowTex.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw
            (
                glowTex.Value,
                position,
                new Rectangle(0, 0, glowTex.Width(), glowTex.Height()),
                ColorHelper.AdditiveWhite() * ((float)Math.Sin(GoldLeafWorld.rottime) * 0.75f + 0.75f),
                0,
                origin,
                scale,
                SpriteEffects.None,
                0f
            );
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe()
                .AddIngredient(ItemType<Echoslate>(), 25)
                .AddIngredient(ItemType<EveDroplet>(), 60)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class GroveHookP : ModProjectile 
    {
        private static Asset<Texture2D> glowTex;
        private static Asset<Texture2D> chainTex;

        private int grappleTime;
        private float grappleSpeed;
        private float grappleDistance;

        private static readonly float minSpeed = 3.5f;
        private static readonly float maxSpeed = 20;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Hook);

            //Projectile.width = 18;
            //Projectile.height = 18;
        }

        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
            chainTex = Request<Texture2D>(Texture + "Chain");
        }

        public override float GrappleRange() => 645;
        public override void GrappleRetreatSpeed(Player player, ref float speed) { speed = maxSpeed; }
        public override void GrapplePullSpeed(Player player, ref float speed) 
        { 
            speed = grappleSpeed;
        }
        public override void NumGrappleHooks(Player player, ref int numHooks) { numHooks = 1; }

        public override void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY)
        {
            grappleDistance = Vector2.Distance(player.Center, new Vector2(grappleX, grappleY));
            grappleTime++;

            grappleSpeed = Helper.LerpFloat(0, maxSpeed, grappleTime * 0.0165f);

            if (grappleSpeed < minSpeed) grappleSpeed = minSpeed;
            if (grappleSpeed > maxSpeed) grappleSpeed = maxSpeed;
        }

        public override bool? CanUseGrapple(Player player)
        {
            int hooksOut = 0;
            foreach (var projectile in Main.ActiveProjectiles)
            {
                if (projectile.owner == Main.myPlayer && projectile.type == Projectile.type)
                {
                    hooksOut++;
                }
            }

            return hooksOut <= 1;
        }

        public override void PostAI()
        {
            Projectile.spriteDirection = Projectile.direction;
        }

        public override bool PreDrawExtras()
        {
            var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
            Vector2 center = Projectile.Center;
            Vector2 directionToPlayer = playerCenter - Projectile.Center;
            float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
            float distanceToPlayer = directionToPlayer.Length();

            while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer))
            {
                directionToPlayer /= distanceToPlayer; 
                directionToPlayer *= chainTex.Height(); 

                center += directionToPlayer; 
                directionToPlayer = playerCenter - center; 
                distanceToPlayer = directionToPlayer.Length();

                Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

                Main.EntitySpriteDraw(chainTex.Value, center - Main.screenPosition,
                    chainTex.Value.Bounds, drawColor, chainRotation,
                    chainTex.Size() * 0.5f, 1f, effects, 0);
            }
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Color color = Color.White; color.A = 0;

            var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 drawOrigin = new(glowTex.Width() * 0.5f, Projectile.height * 0.5f);

            Vector2 drawPos = Projectile.position - Main.screenPosition + drawOrigin;
            Main.spriteBatch.Draw(glowTex.Value, drawPos, null, color * ((float)Math.Sin(GoldLeafWorld.rottime) * 0.75f + 0.75f), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0f);
        }
    }
}
