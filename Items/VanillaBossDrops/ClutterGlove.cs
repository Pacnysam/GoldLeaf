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
using GoldLeaf.Items.Grove;
using Terraria.GameContent.ItemDropRules;
using Terraria.UI;
using Terraria.ModLoader.IO;
using System.Collections.Generic;

namespace GoldLeaf.Items.VanillaBossDrops
{
    public abstract class ClutterGlove : ModItem //TODO fix like, ALL of this
    {
        public override void SetDefaults()
        {
            Item.damage = 9;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 2;
            Item.crit = 6;

            Item.width = 28;
            Item.height = 38;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Blue;

            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;

            Item.shootSpeed = 11f;
            Item.useAmmo = ItemType<EveDroplet>();
            Item.shoot = ProjectileType<EveDropletP>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-3, 3))), type, damage, knockback, player.whoAmI);

            if (type == ProjectileType<EveDropletP>()) { Main.projectile[p].timeLeft = Main.rand.Next(115, 200); Main.projectile[p].velocity *= 0.7f; Main.projectile[p].damage -= 2; }
            if (type == ProjectileType<ClutterScale>() || type == ProjectileType<ClutterTissue>()) { Main.projectile[p].damage += 13; }
            if (type == ProjectileID.SpikyBall) { Main.projectile[p].velocity *= 0.5f; Main.projectile[p].damage -= 5; }
            if (type == ProjectileID.SporeTrap) { Main.projectile[p].velocity *= Main.rand.NextFloat(0.3f, 0.5f); Main.projectile[p].timeLeft = 900; Main.projectile[p].GetGlobalProjectile<GoldLeafProjectile>().gravity = 0.005f; }
            if (type == ProjectileID.HornetStinger) { Main.projectile[p].GetGlobalProjectile<GoldLeafProjectile>().gravity = 0.12f; Main.projectile[p].damage += 12; }

            Main.projectile[p].DamageType = Item.DamageType;
            return false;
        }

        /*public override void OnConsumeAmmo(Item ammo, Player player)
        {
            switch (ammo.type)
            {
                case ItemID.Stinger:
                case ItemID.JungleSpores:
                    {
                        Helper.TryTakeItem(player, ammo.type, 1);
                        break;
                    }
            }
        }*/

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.Bone) { type = ProjectileID.BoneGloveProj; velocity *= 0.65f; }
        }
    }

    public class ClutterGloveCorruption : ClutterGlove 
    {
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (ammo.type == ItemID.ShadowScale)
            {
                return Main.rand.NextBool();
            }
            else
            { 
                return true; 
            }
        }
    }

    public class ClutterGloveCrimson : ClutterGlove
    {
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (ammo.type == ItemID.TissueSample)
            {
                return Main.rand.NextBool();
            }
            else
            {
                return true;
            }
        }
    }

    public class ClutterScale : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 16;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.ArmorPenetration = 12;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 1;

            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            if (Projectile.Counter() > 15)
                Projectile.velocity.Y += 0.08f;

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            Projectile.spriteDirection = Projectile.direction;
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * (float)(((float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length) / 2);

                Main.spriteBatch.Draw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0f);
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.7f);
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, Projectile.Center);

            for (int k = 0; k < 12; ++k)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, Main.rand.NextFloat(-3, 3) + Projectile.velocity.X / 3, Main.rand.NextFloat(-3, 3) + Projectile.velocity.Y / 3, 0, new Color(), 1f);
                Main.dust[dust].noGravity = true;
            }
        }
    }

    public class ClutterTissue : ModProjectile
    {
        private int bounces = 2;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 16;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.15f;

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            Projectile.spriteDirection = Projectile.direction;
            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * (float)(((float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length) / 2);

                Main.spriteBatch.Draw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0f);
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.DD2_LightningBugHurt, Projectile.Center);

            target.SimpleStrikeNPC((int)(damageDone * 0.7), hit.HitDirection, hit.Crit, 0, DamageClass.Ranged);
        }

        /*public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.Hurt(info.DamageSource, (int)(info.Damage * 0.7), info.HitDirection, true);
        }*/

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 12; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimslime, Main.rand.NextFloat(-3, 3) + Projectile.velocity.X / 3, Main.rand.Next(-3, 3) + Projectile.velocity.Y / 3);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }

            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimslime);

            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);

            Projectile.velocity *= 0.7f;

            if (bounces <= 0)
            {
                Projectile.Kill(); return true;
            }
            else
            {
                bounces--; return false;
            }
        }
    }

    public class ClutterAmmo : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            switch (item.type)
            {
                /*case ItemID.StoneBlock:
                    {
                        item.ammo = ItemType<EveDroplet>();
                        item.shoot = ProjectileID.PewMaticHornShot;
                        break;
                    }*/
                case ItemID.SpikyBall:
                    {
                        item.ammo = ItemType<EveDroplet>();
                        break;
                    }
                case ItemID.ShadowScale:
                    {
                        item.ammo = ItemType<EveDroplet>();
                        item.shoot = ProjectileType<ClutterScale>();
                        item.value = Item.buyPrice(0, 0, 7, 50);
                        break;
                    }
                case ItemID.TissueSample:
                    {
                        item.ammo = ItemType<EveDroplet>();
                        item.shoot = ProjectileType<ClutterTissue>();
                        item.value = Item.buyPrice(0, 0, 7, 50);
                        break;
                    }
                case ItemID.Stinger:
                    {
                        item.ammo = ItemType<EveDroplet>();
                        item.shoot = ProjectileID.HornetStinger;
                        item.consumable = true;
                        break;
                    }
                case ItemID.JungleSpores:
                    {
                        item.ammo = ItemType<EveDroplet>();
                        item.shoot = ProjectileID.SporeTrap;
                        item.consumable = true;
                        break;
                    }
                case ItemID.Bone:
                    {
                        item.ammo = ItemType<EveDroplet>();
                        break;
                    }
            }
        }

        /*public override void AddRecipes()
        {
            Recipe tissueToScale = Recipe.Create(ItemID.ShadowScale, 5)
                .AddIngredient(ItemID.TissueSample, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .AddCondition(GoldLeafConditions.HasClutterGlove)
                .Register();
            Recipe scaleToTissue = Recipe.Create(ItemID.TissueSample, 5)
                .AddIngredient(ItemID.ShadowScale, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .AddCondition(GoldLeafConditions.HasClutterGlove)
                .Register();
        }*/

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            int glove = ItemType<ClutterGlove>();
            if (!WorldGen.crimson) glove = ItemType<ClutterGloveCorruption>(); else glove = ItemType<ClutterGloveCrimson>();

            if (item.ammo == ItemType<EveDroplet>())
            {
                string[] text =
                [
                    Language.GetTextValue("Mods.GoldLeaf.Items.ClutterGlove.ClutterTooltip" /*+ ItemID.Search.GetName(item.type)*/)
                ];

                TooltipLine tooltipLine = tooltips.Find(n => n.Name == "Ammo");

                if (tooltipLine != null && GoldLeafConditions.HasClutterGlove.IsMet())
                {
                    int index = tooltips.IndexOf(tooltipLine);
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (text[i] != string.Empty)
                            tooltips.Insert(index + 1, new TooltipLine(Mod, "ClutterTooltip", /*"[i/s1:" + glove + "]: " +*/ text[i]));
                    }
                }
                tooltips.Remove(tooltipLine);
            }
        }
    }
}
