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
using GoldLeaf.Items.Misc.Accessories;
using ReLogic.Content;
using GoldLeaf.Items.Gem;
using Terraria.ModLoader.IO;
using System.Collections.Generic;

namespace GoldLeaf.Items.Blizzard
{
    public class Avalanche : ModItem
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public int consecutiveHits = 0;
        public bool newPeak = false;
        //public bool hitPrev = false;

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.FlintlockPistol);

            Item.width = 42;
            Item.height = 24;

            Item.useTime = Item.useAnimation = 40;

            Item.crit = -4;

            Item.damage = 7;
            Item.shootSpeed = 9f;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 0.8f;
            Item.autoReuse = true;

            Item.useAmmo = AmmoID.Bullet;

            Item.GetGlobalItem<GoldLeafItem>().critDamageMod = -0.5f;

            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Green;

            Item.UseSound = new SoundStyle("GoldLeaf/Sounds/SE/Monolith/Revolver") { Volume = 0.6f, PitchVariance = 0.3f, MaxInstances = 5 };
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextBool();
        }

        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            crit = Math.Clamp(consecutiveHits * 2, 0, 100);
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
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Dust.NewDustPerfect(position, DustType<MuzzleFlash>(), velocity);

            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-1.6f, 1.6f))), type, damage, knockback);
            proj.GetGlobalProjectile<AvalancheProjectile>().shotFromAvalanche = true;
            proj.GetGlobalProjectile<AvalancheProjectile>().avalancheInstance = Item;
            proj.extraUpdates += Math.Clamp((int)(consecutiveHits / 5f), 0, 3);
            proj.usesLocalNPCImmunity = true;
            proj.localNPCHitCooldown = 10;
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

            int index = tooltips.IndexOf(tooltips.Find(n => n.Name == "Tooltip1"));
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

        /*public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Color color = ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 3); color.A = 0;

            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 3), rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, color * (0.3f - (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f), rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);
        }*/
    }
    
    public class AvalancheProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            lateInstantiation = true;
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

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (shotFromAvalanche && avalancheInstance != null) 
            {
                hasHitTarget = true;

                if (!target.friendly && !target.immortal && !target.dontTakeDamage && target.lifeMax > 5)
                {
                    Main.player[projectile.owner].GetModPlayer<AvalanchePlayer>().streakLossImmuneTime += 15;
                    var avalanche = avalancheInstance.ModItem as Avalanche;
                    Math.Clamp(avalanche.consecutiveHits++, 0, 100);
                    //avalanche.consecutiveHits++;
                    //avalanche.hitPrev = true;
                }
            }
        }

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
            streakLossImmuneTime = Math.Clamp(streakLossImmuneTime - 1, 0, 30);
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
}
