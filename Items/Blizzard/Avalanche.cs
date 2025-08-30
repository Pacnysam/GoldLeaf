using System;
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
using GoldLeaf.Effects.Dusts;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using GoldLeaf.Items.Accessories;
using ReLogic.Content;

using Terraria.ModLoader.IO;
using System.Collections.Generic;
using System.IO;
using GoldLeaf.Core.CrossMod;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;

namespace GoldLeaf.Items.Blizzard
{
    public class Avalanche : ModItem
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }
        public override void SetStaticDefaults()
        {
            ItemSets.Glowmask[Type] = (glowTex, ColorHelper.AdditiveWhite() * 0.5f, false);
            Item.AddElements([Element.Ice]);
        }

        public int consecutiveHits = 0;
        public bool newPeak = false;
        //public bool hitPrev = false;

        public override bool? CanChooseAmmo(Item ammo, Player player)
        {
            if (ammo.type == ItemID.Snowball)
                return true;

            return base.CanChooseAmmo(ammo, player);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.FlintlockPistol);

            Item.width = 42;
            Item.height = 24;

            Item.useTime = Item.useAnimation = 40;

            Item.crit = -4;

            Item.damage = 8;
            Item.shootSpeed = 9f;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 0.8f;
            Item.autoReuse = true;

            Item.useAmmo = AmmoID.Bullet;

            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Green;

            Item.UseSound = new SoundStyle("GoldLeaf/Sounds/SE/Monolith/Revolver") { Volume = 0.6f, PitchVariance = 0.3f, MaxInstances = 5, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest };
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextBool();
        }

        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            crit = Math.Clamp(100 - Math.Clamp(consecutiveHits * 5, 0, 100), player.GetTotalCritChance(Item.DamageType), 100);
        }

        public override float UseTimeMultiplier(Player player)
        {
            return Math.Clamp(1f - (consecutiveHits * 0.05f), 0.15f, 1f);
        }
        public override float UseAnimationMultiplier(Player player)
        {
            return Math.Clamp(1f - (consecutiveHits * 0.05f), 0.15f, 1f);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10f, -3f);
        }

        /*public override bool CanUseItem(Player player)
        {
            int projCount = 0;
            for (int k = 0; k <= Main.maxProjectiles; k++)
            {
                if (Main.projectile[k].active && Main.projectile[k].owner == player.whoAmI && Main.projectile[k].GetGlobalProjectile<AvalancheProjectile>().shotFromAvalanche)
                    projCount++;

                if (projCount > 2)
                    return false;
            }
            return true;
        }*/

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 28f;
            position.Y -= 7;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            if (type == ProjectileID.Bullet)
                type = ProjectileType<BlizzardBrassBulletP>();
            if (type == ProjectileID.SnowBallFriendly)
            {
                type = ProjectileType<AvalancheSnowball>();
                velocity *= 1.35f;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile burst = Projectile.NewProjectileDirect(source, position + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(0f, 2f), 
                velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-15f, 15f))), ProjectileType<AvalancheSnowBurst>(), damage, knockback, player.whoAmI);
            burst.rotation = burst.velocity.ToRotation() + MathHelper.PiOver2;
            burst.position += burst.velocity * 1.35f;
            burst.scale = Main.rand.NextFloat(0.65f, 1f);
            burst.ai[0] = burst.position.X - player.position.X;
            burst.ai[1] = burst.position.Y - player.position.Y;
            burst.ai[2] = burst.rotation;
            burst.netUpdate = true;

            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-1.6f, 1.6f))), type, damage, knockback);
            proj.GetGlobalProjectile<AvalancheProjectile>().shotFromAvalanche = true;
            proj.GetGlobalProjectile<AvalancheProjectile>().avalancheInstance = Item;
            proj.extraUpdates += Math.Clamp((int)(consecutiveHits / 5f), 0, 3);
            proj.usesLocalNPCImmunity = true;
            proj.localNPCHitCooldown = 10;
            proj.netUpdate = true;
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            consecutiveHits = (int)Math.Clamp(consecutiveHits, 0, 999999999999);

            if (consecutiveHits < player.GetModPlayer<AvalanchePlayer>().avalancheHighScore)
                newPeak = false;

            if (player.GetModPlayer<AvalanchePlayer>().avalancheHighScore < consecutiveHits)
            {
                player.GetModPlayer<AvalanchePlayer>().avalancheHighScore = consecutiveHits;
                if (newPeak == false)
                {
                    SoundEngine.PlaySound(SoundID.ResearchComplete);
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 5, player.width, player.height), new(195, 10, 200), Language.GetTextValue("Mods.GoldLeaf.Items.Avalanche.NewHighScore"), true);
                    newPeak = true;
                }
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Color color = new(195, 10, 200);

            if (Main.LocalPlayer.GetModPlayer<AvalanchePlayer>().avalancheHighScore <= consecutiveHits)
                color = ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 9);

            string[] text =
            [
                Language.GetTextValue("Mods.GoldLeaf.Items.Avalanche.HighScore", consecutiveHits, Main.LocalPlayer.GetModPlayer<AvalanchePlayer>().avalancheHighScore)
            ];

            int index = tooltips.IndexOf(tooltips.Find(n => n.Name == "Tooltip2"));
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != string.Empty)

                    tooltips.Insert(index + 1, new TooltipLine(Mod, "AvalancheHighScore", text[i]) { OverrideColor = color });
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("GoldLeaf:SilverBars", 8);
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddIngredient(ItemType<AuroraCluster>(), 15);
            recipe.AddTile(TileID.Anvils);
            recipe.AddOnCraftCallback(RecipeCallbacks.AuroraMajor);
            recipe.Register();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Color color = ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 3); color.A = 0;

            //spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 3), rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite() * 0.5f, rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);
        }
    }
    
    public class AvalancheProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.DamageType == DamageClass.Ranged;
        }

        public bool shotFromAvalanche = false;
        public Item avalancheInstance = null;
        bool hasHitTarget = false;

        public override void ModifyDamageHitbox(Projectile projectile, ref Rectangle hitbox)
        {
            if (shotFromAvalanche)
                hitbox.Inflate(6, 6);
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(shotFromAvalanche);
        }
        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            shotFromAvalanche = binaryReader.ReadBoolean();
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (shotFromAvalanche && avalancheInstance != null) 
            {
                hasHitTarget = true;

                if (!target.friendly && !target.immortal && !target.dontTakeDamage && target.lifeMax > 5)
                {
                    Main.player[projectile.owner].GetModPlayer<AvalanchePlayer>().streakLossImmuneTime += 8;
                    var avalanche = avalancheInstance.ModItem as Avalanche;
                    Math.Clamp(avalanche.consecutiveHits++, 0, 100);
                    //avalanche.consecutiveHits++;
                    //avalanche.hitPrev = true;
                }
            }
        }

        /*public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (projectile.GetGlobalProjectile<AvalancheProjectile>().shotFromAvalanche && projectile.GetGlobalProjectile<AvalancheProjectile>().avalancheInstance != null)
            {
                projectile.GetGlobalProjectile<AvalancheProjectile>().hasHitTarget = true;

                Main.player[projectile.owner].GetModPlayer<AvalanchePlayer>().streakLossImmuneTime += 15;
                var avalanche = projectile.GetGlobalProjectile<AvalancheProjectile>().avalancheInstance.ModItem as Avalanche;
                Math.Clamp(avalanche.consecutiveHits++, 0, 100);
            }
        }*/

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if ((!hasHitTarget && Main.player[projectile.owner].GetModPlayer<AvalanchePlayer>().streakLossImmuneTime <= 0) && shotFromAvalanche && avalancheInstance != null)
            {
                var avalanche = avalancheInstance.ModItem as Avalanche;

                if (Main.myPlayer == projectile.owner && Main.netMode != NetmodeID.Server && avalanche.consecutiveHits != 0)
                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Monolith/Chop") { Volume = 0.9f });

                avalanche.consecutiveHits = 0;
                //Math.Clamp((avalanche.consecutiveHits -= 2), 0, 100);
                //avalanche.hitPrev = false;
            }
        }
    }

    public class AvalanchePlayer : ModPlayer
    {
        public int avalancheHighScore;
        public int streakLossImmuneTime;

        public override void PostUpdateMiscEffects()
        {
            streakLossImmuneTime = Math.Clamp(--streakLossImmuneTime, 0, 15);
        }

        public override void Initialize()
        {
            avalancheHighScore = 50;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["avalancheHighScore"] = avalancheHighScore;
        }

        public override void LoadData(TagCompound tag)
        {
            avalancheHighScore = tag.GetInt("avalancheHighScore");
        }
    }
    
    public class AvalancheSnowball : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SnowBallFriendly;

        private static Asset<Texture2D> trailTex;
        public override void Load()
        {
            trailTex = Request<Texture2D>("GoldLeaf/Items/Blizzard/AvalancheSnowballTrail");
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SnowBallFriendly);
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Snow, Scale: Main.rand.NextFloat(1.15f, 1.65f));
                dust.velocity *= 0.75f;
                dust.fadeIn = Main.rand.NextFloat(0.85f, 1.25f);
                dust.noGravity = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Inflate(5, 5);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
                FrostNPC.AddFrost(target);

            target.AddBuff(BuffID.Chilled, TimeToTicks(3.5f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + TextureAssets.Projectile[Type].Size() / 2;

                //afterimage
                Main.EntitySpriteDraw(trailTex.Value, drawPos, null, lightColor * Projectile.Opacity * (0.7f - (k * 0.125f)), Projectile.oldRot[k], (trailTex.Size() / 2) - new Vector2(0, 26), Projectile.scale * 0.75f, SpriteEffects.None, 0f);
            }
            return true;
        }
    }

    public class AvalancheSnowBurst : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 80;
            Projectile.damage = 0;
            Projectile.aiStyle = -1;
            Projectile.frameCounter = -1;
            Projectile.alpha = Main.rand.Next(30) * 5;
            Projectile.tileCollide = false;
        }

        Vector2 InitialOffset;
        bool hasSetUp = false;

        public override void AI()
        {
            if (!hasSetUp)
            {
                //Projectile.frame = (int)Projectile.ai[2];
                hasSetUp = true;
                InitialOffset = new Vector2(Projectile.ai[0], Projectile.ai[1]);
                Projectile.rotation = Projectile.ai[2];

                Projectile.netUpdate = true;
            }

            Projectile.velocity = Vector2.Zero;
            Projectile.position = Main.player[Projectile.owner].position + InitialOffset;
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame > Main.projFrames[Type] - 1)
                    Projectile.Kill();
            }
        }

        /*public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Rectangle rect = TextureAssets.Projectile[Type].Frame(1, 4, 0, Projectile.frame);

            Main.EntitySpriteDraw(tex, Projectile.position - Main.screenPosition, rect, lightColor, Projectile.rotation, rect.Size()/2f, Projectile.scale, SpriteEffects.None);
            return false;
        }*/

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        /*public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return lightColor;
        }

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 80, 80);
            dust.position -= dust.frame.Size()/2f * dust.scale;
        }

        public override bool Update(Dust dust)
        {
            if (dust.alpha % 20 == 15)
                dust.frame.Y += dust.frame.Height;

            dust.alpha += 5;

            if (dust.alpha > 255)
                dust.active = false;

            if (dust.customData is Player player)
            {
                dust.velocity = player.velocity;
                dust.position += dust.velocity;
            }
            return false;
        }*/
    }
}
