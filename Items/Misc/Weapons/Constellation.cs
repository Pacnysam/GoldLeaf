using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using GoldLeaf.Items.Grove;
using System.Diagnostics.Metrics;
using System;
using System.Threading;
using GoldLeaf.Effects.Dusts;
using ReLogic.Content;
namespace GoldLeaf.Items.Misc.Weapons
{
    public class Constellation : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ConstellationTag.TagDamage);

        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ProjectileType<ConstellationP>(), 11, 1.2f, 3f, 45);

            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 75, 0);
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.FallenStar, 5);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.AddCondition(Condition.TimeNight);
            recipe.Register();

            /*Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.FallenStar, 10);
            recipe2.AddRecipeGroup(RecipeGroupID.IronBar, 8);
            recipe2.AddTile(TileID.Anvils);
            recipe2.AddCondition(GoldLeafConditions.InSurface);
            recipe2.AddCondition(Condition.TimeNight);
            recipe2.Register();*/
        }

        public override bool MeleePrefix()
        {
            return true;
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
                ColorHelper.AdditiveWhite,
                rotation,
                glowTex.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
        }

        /* PreDrawInWorld
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            for (int i = 0; i < 1; i++)
            {
                Texture2D glowTex = Request<Texture2D>("GoldLeaf/Items/Misc/ConstellationGlowFlat").Value;
                Vector2 vec = new((glowTex.Width / 2), (glowTex.Height / 1 / 2));
                //Vector2 position1 = Item.Center - Main.screenPosition - new Vector2(glowTex.Width, (glowTex.Height / 1)) * Item.scale / 2f + vec * Item.scale + new Vector2(0.0f, 0f + addHeight + 0);
                for (int k = 0; k < 4; ++k)
                {
                    Color color = Item.GetAlpha(new Color(180, 224, 255)) * (0.85f - (float)(Math.Cos(GoldLeafWorld.rottime)));
                    Vector2 position = new Vector2(Item.Center.X, Item.Center.Y) + ((float)((double)k / 4 * 6.28318548202515) + rotation + 0f).ToRotationVector2() * (float)(4.0 * (double)(float)(Math.Cos(GoldLeafWorld.rottime)) + 2.0) - Main.screenPosition - new Vector2(glowTex.Width, (glowTex.Height)) * Item.scale / 2f + vec * Item.scale + new Vector2(0.0f, 0f + 0);
                    Main.spriteBatch.Draw(glowTex, position, new Microsoft.Xna.Framework.Rectangle?(glowTex.Frame(1, 1, 0, 0)), color, rotation, vec, Item.scale * 1.1f, SpriteEffects.None, 0.0f);
                }
            }
            return true;
        }*/
    }

    public class ConstellationP : ModProjectile
    {
        private static Asset<Texture2D> glowTex;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            Projectile.WhipSettings.Segments = 8;
            Projectile.WhipSettings.RangeMultiplier = 0.6f;
        }

        ref float Timer => ref Projectile.ai[0];

        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];

            Projectile.WhipSettings.Segments += player.GetModPlayer<ConstellationPlayer>().extraSegments;
            Projectile.WhipSettings.RangeMultiplier += (0.125f * Main.player[Projectile.owner].GetModPlayer<ConstellationPlayer>().extraSegments);
        }

        public override void AI()
        {
            List<Vector2> list = [];
            Projectile.FillWhipControlPoints(Projectile, list);
            Vector2 pos = list[^1];

            Dust d = Dust.NewDustPerfect(pos, DustID.FireworkFountain_Blue);
            //Dust d = Dust.NewDustPerfect(pos, DustType<StarDust>());
            d.noGravity = true;
            //d.color = new Color(255, 237, 187);
            d.scale = 0.6f;
            //d.fadeIn = 1f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            target.AddBuff(BuffType<ConstellationTag>(), 180);
            player.MinionAttackTargetNPC = target.whoAmI;
            
            player.GetModPlayer<ConstellationPlayer>().segmentTimer = 300;

            if (player.GetModPlayer<ConstellationPlayer>().extraSegments < player.GetModPlayer<ConstellationPlayer>().maxExtraSegments)
            {
                player.GetModPlayer<ConstellationPlayer>().extraSegments++;
                DustHelper.DrawStar(target.Center, DustID.FireworkFountain_Blue, 5, target.scale * 3.8f, 1f, 0.7f, 0.85f, 0.5f, true, 0, 1);
                //DustHelper.DrawStar(target.Center, DustType<StarDust>(), 5, target.scale * 3.8f, 1f, 0.7f, 1f, 0.5f, true, 0, 1);

                SoundEngine.PlaySound(SoundID.NPCDeath7, player.Center);
            }
            else 
            {
                Projectile.damage = (int)(Projectile.damage * 0.7f + (player.GetModPlayer<ConstellationPlayer>().extraSegments * 0.05f));
                DustHelper.DrawStar(target.Center, DustID.FireworkFountain_Blue, 5, target.scale * 1.4f, 1f, 0.7f, 0.85f, 0.5f, true, 0, -1);
            }
        }

        private void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), new Color(180, 224, 255));
                Vector2 scale = new(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);
            Vector2 pos = list[0];
            //Vector2 tipPos = list[list.Count - 1];

            //DrawLine(list);

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.instance.LoadProjectile(Type);

            for (int i = 0; i < list.Count - 1; i++)
            {
                Rectangle frame = new Rectangle(0, 0, 22, 26); //handle
                Vector2 origin = new Vector2(5, 8);
                float scale = 1;
                if (i == list.Count - 2)
                {
                    frame.Y = 104; //whip length minus tip
                    frame.Height = 26;

                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else if (i > 10)
                {
                    //close segment
                    frame.Y = 78;
                    frame.Height = 26;
                }
                else if (i > 5)
                {
                    //mid segment
                    frame.Y = 52;
                    frame.Height = 26;
                }
                else if (i > 0)
                {
                    //far segment
                    frame.Y = 26;
                    frame.Height = 26;
                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                //Vector2 drawOrigin = new(glowTex.Width() * 0.5f, Projectile.height * 0.5f);

                /*for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin; //+ new Vector2(0f, Projectile.gfxOffY);

                    Main.spriteBatch.Draw(glowTex.Value, drawPos, frame, ColorHelper.AdditiveWhite * (0.6f - (0.055f * k)), Projectile.rotation, origin, scale * (1f - (0.075f * k)), flip, 0f);
                }*/

                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);
                Main.spriteBatch.Draw(glowTex.Value, pos - Main.screenPosition, frame, ColorHelper.AdditiveWhite, rotation, origin, scale, flip, 0);

                pos += diff;
            }
            return false;
        }
    }
    
    public class ConstellationPlayer : ModPlayer 
    {
        public int extraSegments = 0;
        public int maxExtraSegments = 12;
        public int segmentTimer = 60;

        public override void ResetEffects()
        {
            maxExtraSegments = 12;
            if (Player.HasBuff(BuffID.StarInBottle))
                maxExtraSegments += 3;
            if (!Main.dayTime)
                maxExtraSegments += 3;
        }

        public override void PreUpdate()
        {
            if (segmentTimer <= 0) extraSegments = 0;
            if (extraSegments > maxExtraSegments) extraSegments = maxExtraSegments;

            segmentTimer--;
        }
    }

    public class ConstellationTag : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public static readonly int TagDamage = 3;

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }

    public class ConstellationDebuffNPC : GlobalNPC
    {
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
                return;

            var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            if (npc.HasBuff<ConstellationTag>())
            {
                modifiers.FlatBonusDamage += ConstellationTag.TagDamage * projTagMultiplier;
            }
        }
    }
}
