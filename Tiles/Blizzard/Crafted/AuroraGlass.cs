using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using GoldLeaf.Tiles.Grove.Ancient;
using Terraria.ObjectData;
using Terraria.Audio;
using GoldLeaf.Effects.Dusts;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using GoldLeaf.Items.Accessories;
using Terraria.GameContent;
using System;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Tiles.Marble;
using GoldLeaf.Core;
using ReLogic.Content;

namespace GoldLeaf.Tiles.Blizzard.Crafted
{
    public class AuroraGlassItem : ModItem
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemSets.Glowmask[Type] = (glowTex, ColorHelper.AdditiveWhite() * 0.3f, false);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<AuroraGlass>());
            Item.value = Item.sellPrice(0, 0, 0, 20);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 1.5f) * 0.4f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(20)
            .AddIngredient(ItemType<AuroraCluster>())
            .AddTile(TileID.IceMachine)
            .AddOnCraftCallback(RecipeCallbacks.AuroraMinor)
            .Register();

            Recipe recipe2 = CreateRecipe()
            .AddIngredient(ItemType<AuroraGlassWallItem>(), 4)
            .AddTile(TileID.WorkBenches)
            .AddOnCraftCallback(RecipeCallbacks.AuroraMinor)
            .Register();
        }

        public override void UseAnimation(Player player)
        {
            Item.color = ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 1.5f);
        }
        public override void UpdateInventory(Player player)
        {
            Item.color = ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 1.5f);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            spriteBatch.Draw(TextureAssets.Item[Type].Value, Item.Center - Main.screenPosition, null, ColorHelper.AuroraAccentColor(Item.timeSinceItemSpawned * 0.05f) * 0.4f, rotation, TextureAssets.Item[Type].Value.Size() / 2, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite() * 0.3f, rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(TextureAssets.Item[Type].Value, position, null, ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 1.5f) * 0.4f, 0, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glowTex.Value, position, null, ColorHelper.AdditiveWhite() * 0.3f, 0, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class AuroraGlass : ModTile
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            LocalizedText name = CreateMapEntryName();
            RegisterItemDrop(ItemType<AuroraGlassItem>());
            AddMapEntry(new Color(0, 225, 255), name);

            MineResist = 0.1f;
            HitSound = SoundID.DD2_WitherBeastCrystalImpact;
            DustType = DustType<AuroraTwinkle>();
            
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = false;

            TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.IceSkateSlippery[Type] = true;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            if (item.type == ItemType<AuroraGlassItem>())
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, new Vector2(i * 16, j * 16));
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GoldLeafTile.DrawSlopedGlowMask(i, j, TextureAssets.Tile[Type].Value, ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 6f + ((i + j)/4f)) * 0.4f, Vector2.Zero);
            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GoldLeafTile.DrawSlopedGlowMask(i, j, glowTex.Value, ColorHelper.AdditiveWhite() * 0.3f, Vector2.Zero);
        }

        public override void FloorVisuals(Player player)
        {
            if (Main.rand.NextBool(4) && Math.Abs(player.velocity.X) > 2)
            {
                Vector2 position = (player.direction == 1 ? player.BottomLeft : player.BottomRight);
                Dust dust = Dust.NewDustPerfect(position, DustType<AuroraTwinkle>(), new Vector2(-player.direction * Main.rand.NextFloat(0.5f, 1.5f), Main.rand.NextFloat(-1.8f, -3.2f)), Main.rand.Next(30, 70), ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 1.5f), Main.rand.NextFloat(0.6f, 0.9f));
                dust.rotation = Main.rand.NextFloat(-8f, 8f);
                dust.noLight = true;
            }
        }
    }

    public class AuroraGlassProjectile : GlobalProjectile 
    {
        public override bool InstancePerEntity => true;

        public int auroraBounces = 0;

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            bool checktop = Main.tile[(int)(projectile.Top.X / 16), (int)(projectile.Top.Y / 16) - 1].TileType == TileType<AuroraGlass>();
            bool checkbottom = Main.tile[(int)(projectile.Bottom.X / 16), (int)(projectile.Bottom.Y / 16) + 1].TileType == TileType<AuroraGlass>();
            bool checkright = Main.tile[(int)(projectile.Right.X / 16) + 1, (int)(projectile.Right.Y / 16)].TileType == TileType<AuroraGlass>();
            bool checkleft = Main.tile[(int)(projectile.Left.X / 16) - 1, (int)(projectile.Left.Y / 16)].TileType == TileType<AuroraGlass>();

            if ((checkleft || checkright || checktop || checkbottom) && !Main.projPet[projectile.type] && auroraBounces < 3)
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }

                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }

                projectile.netUpdate = true;

                auroraBounces++;

                AuroraBounce(projectile, projectile.Center + projectile.velocity.SafeNormalize(Vector2.UnitX) * 8f, (-projectile.velocity).SafeNormalize(Vector2.UnitX));
                return false;
            }
            return base.OnTileCollide(projectile, oldVelocity);
        }

        private static void AuroraBounce(Projectile projectile, Vector2 hitPoint, Vector2 normal)
        {
            int l = 3;
            if (projectile.damage > 0)
            {
                SoundEngine.PlaySound(SoundID.Item150, projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, projectile.Center);
                l = 7;
            }

            Vector2 spinningpoint = Vector2.Reflect(projectile.velocity, normal);
            for (int i = 0; i < l; i++)
            {
                Dust dust = Dust.NewDustPerfect(hitPoint, DustType<AuroraTwinkle>(), spinningpoint.RotatedBy((float)Math.PI / 4f * Main.rand.NextFloatDirection()) * 0.6f * Main.rand.NextFloat(), Main.rand.Next(20, 60), ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 1.5f), Main.rand.NextFloat(0.45f, 0.8f));
                dust.rotation = Main.rand.NextFloat(-5f, 5f);
                dust.noLight = true;
                dust.velocity *= 0.4f;
                //Dust dust = Dust.NewDustPerfect(hitPoint, DustID.GemTopaz, spinningpoint.RotatedBy((float)Math.PI / 4f * Main.rand.NextFloatDirection()) * 0.6f * Main.rand.NextFloat(), 100, Color.Yellow, 1.0f);
            }
        }
    }
}