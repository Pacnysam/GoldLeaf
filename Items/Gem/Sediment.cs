using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using static GoldLeaf.Core.ColorHelper;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
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
using System.IO;
using Terraria.ModLoader.IO;
using System.Linq;

namespace GoldLeaf.Items.Gem
{
    public class Sediment : ModItem
    {
        private readonly int[] sedimentValidGems = [ItemID.Amethyst, ItemID.Topaz, ItemID.Sapphire, ItemID.Emerald, ItemID.Ruby, ItemID.Diamond, ItemID.Amber];
        private enum Gem : int
        {
            None = 0,
            Amethyst = 1,
            Topaz = 2,
            Sapphire = 3,
            Emerald = 4,
            Ruby = 5,
            Diamond = 6,
            Amber = 7,
            Count
        }
        public int gem;

        public override string Texture => "GoldLeaf/Items/Gem/SedimentFull";

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(2, 8) { NotActuallyAnimating = true });
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.EnchantedBoomerang);

            Item.shoot = ProjectileType<SedimentP>();
            Item.shootSpeed = 8f;

            Item.damage = 15;
            //Item.GetGlobalItem<GoldLeafItem>().throwingDamageType = DamageClass.Melee;
            Item.DamageType = DamageClass.Melee;

            Item.width = 26;
            Item.height = 30;

            Item.knockBack = 4.5f;

            Item.useTime = 20;
            Item.useAnimation = 20;

            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 50, 0);
        }

        public override bool CanUseItem(Player player)
        {
            for (int k = 0; k <= Main.maxProjectiles; k++)
            {
                if (Main.projectile[k].active && Main.projectile[k].owner == player.whoAmI && Main.projectile[k].type == ProjectileType<SedimentP>() && gem != (int)Gem.Diamond)
                    return false;
            }
            return base.CanUseItem(player);
        }

        public override bool ConsumeItem(Player player)
        {
            return false;
        }

        public override void RightClick(Player player)
        {
            Item.NetStateChanged();
            if (gem == (int)Gem.None && sedimentValidGems.Contains(Main.mouseItem.type))
            {
                gem = (Array.IndexOf(sedimentValidGems, Main.mouseItem.type) + 1);
                Main.mouseItem.stack--;

                if (Main.netMode != NetmodeID.Server) 
                {
                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact);
                    SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt);
                }
            }
            else if (gem != (int)Gem.None)
            {
                player.QuickSpawnItem(player.GetSource_ItemUse(Item), sedimentValidGems[gem - 1]);
                gem = (int)Gem.None;

                if (Main.netMode != NetmodeID.Server)
                {
                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath);
                    //SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt);
                }
            }
        }

        #region Stats
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (gem == (int)Gem.Amethyst || gem == (int)Gem.Emerald)
                damage *= 1.25f;
            else if (gem == (int)Gem.Sapphire)
                damage *= 0.85f;
            else if (gem == (int)Gem.Diamond)
                damage *= 0.6f;
            else
                damage *= 1;
        }

        public override bool? CanAutoReuseItem(Player player)
        {
            if (gem == (int)Gem.Diamond)
                return true;
            else
                return false;
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (gem == (int)Gem.Diamond)
                return 0.5f;
            else
                return 1f;
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (gem == (int)Gem.Sapphire)
                return 0.75f;
            else if (gem == (int)Gem.Emerald)
                return 0.35f;
            else
                return 1f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (gem == (int)Gem.Topaz) velocity = (velocity * 1.4f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-10, 10)));
            if (gem == (int)Gem.Emerald) velocity *= 2f;
            if (gem == (int)Gem.Ruby) velocity *= 1.6f;
            if (gem == (int)Gem.Diamond) knockback *= 0.3f;
            if (gem == (int)Gem.Amber) knockback *= 0f;
        }
        #endregion Stats

        public override bool CanRightClick()
        {
            for (int k = 0; k <= Main.maxProjectiles; k++)
            {
                if (Main.projectile[k].active && Main.projectile[k].owner == Main.LocalPlayer.whoAmI && Main.projectile[k].type == ProjectileType<SedimentP>())
                    return false;
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Main.projectile[p].ai[2] = gem;

            if (gem == (int)Gem.Amethyst)
            {
                Main.projectile[p].ArmorPenetration = 5;
            }
            if (gem == (int)Gem.Topaz)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-12, 12))) * 0.85f, ProjectileType<SedimentTopaz>(), damage, knockback, player.whoAmI);
            }
            if (gem == (int)Gem.Emerald)
            {
                Main.projectile[p].velocity.Y -= 5;
            }
            if (gem == (int)Gem.Diamond)
            {
                Main.projectile[p].extraUpdates = 2;
            }
            Main.projectile[p].netUpdate = true;

            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string[] text =
            [
                Language.GetTextValue("Mods.GoldLeaf.Items.Sediment.Gem" + gem)
            ];

            int index = tooltips.IndexOf(tooltips.Find(n => n.Name == "Tooltip0"));
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != string.Empty)

                    tooltips.Insert(index + 1, new TooltipLine(Mod, "SedimentGem", text[i]));
            }
        }

        /*public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.StoneBlock, 20);
            recipe.AddIngredient(ItemID.FallenStar, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }*/

        #region Save & Load Stuff
        protected override bool CloneNewInstances => true;
        public override ModItem Clone(Item item)
        {
            var clone = (Sediment)base.Clone(item);
            clone.gem = gem;
            return clone;
        }

        public override void SaveData(TagCompound tag)
        {
            tag[nameof(gem)] = gem;
        }

        public override void LoadData(TagCompound tag)
        {
            gem = tag.Get<int>(nameof(gem));
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(gem);
        }

        public override void NetReceive(BinaryReader reader)
        {
            gem = reader.ReadInt32();
        }
        #endregion Save & Load Stuff

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;

            frame.Y = texture.Height / (int)Gem.Count * gem;

            spriteBatch.Draw(texture, position, frame, Item.GetAlpha(drawColor), 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Rectangle frame = new(0, (texture.Height / (int)Gem.Count) * gem, texture.Width, (texture.Height / (int)Gem.Count) - 2);

            Vector2 position = new(Item.position.X - Main.screenPosition.X + Item.width / 2, Item.position.Y - Main.screenPosition.Y + Item.height / 2);

            spriteBatch.Draw(texture, position, frame, Item.GetAlpha(lightColor), rotation, frame.Size() / 2, scale, SpriteEffects.None, 0f);
            return true;
        }
    }

    public class SedimentP : ModProjectile
    {
        private int counter;

        private enum Gems : int
        {
            None = 0,
            Amethyst = 1,
            Topaz = 2,
            Sapphire = 3,
            Emerald = 4,
            Ruby = 5,
            Diamond = 6,
            Amber = 7,
            Count
        }
        //private int gem;
        private bool empowered = false;

        public ref float Gem => ref Projectile.ai[2];

        private int emeraldDropCounter = 6;

        private int rubyCounter = 0;
        private int rubyShotCounter = 15;


        public override string Texture => "GoldLeaf/Items/Gem/SedimentFull";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WoodenBoomerang);

            //Projectile.GetGlobalProjectile<GoldLeafProjectile>().throwingDamageType = DamageClass.Melee;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.width = 20;
            Projectile.height = 20;
        }

        public override void OnSpawn(IEntitySource source)
        {
            counter = Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter;

            Gem = (int)Projectile.ai[2];
            Projectile.frame = (int)Projectile.ai[2];

            if (Gem == (int)Gems.Ruby || Gem == (int)Gems.None) 
                empowered = false;
            else 
                empowered = true;
        }


        public override bool PreAI()
        {
            counter = Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter;
            if (counter <= 1) 
            {
                Gem = (int)Projectile.ai[2];
                Projectile.frame = (int)Projectile.ai[2];

                if (Gem == (int)Gems.Ruby || Gem == (int)Gems.None)
                    empowered = false;
                else
                    empowered = true;
            }

            //counter = Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter;

            //if (counter <= 1 && gem == (int)Gem.Ruby) empowered = false;

            //gem = (int)Projectile.ai[2];
            //Projectile.frame = (int)Projectile.ai[2];

            if (Gem == (int)Gems.None) empowered = false;


            if (empowered)
            {
                if (Gem == (int)Gems.Amethyst && counter > 40)
                {
                    empowered = false;
                }
                if (Gem == (int)Gems.Emerald && counter > 40)
                {
                    empowered = false;
                }
                if (Gem == (int)Gems.Ruby && empowered && counter > 1)
                {
                    Projectile.velocity *= 0.825f + (rubyCounter * 0.0005f);
                }
                if (Gem == (int)Gems.Emerald)
                {
                    if (empowered)
                    {
                        Projectile.GetGlobalProjectile<GoldLeafProjectile>().gravity = 0.5f;
                    }
                    else
                    {
                        Projectile.GetGlobalProjectile<GoldLeafProjectile>().gravity = 0f;
                    }
                }

                int dustchance = 6;
                if (Gem == (int)Gems.Diamond) dustchance = 15;

                if (Main.rand.NextBool(dustchance))
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<LightDust>(), Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 0, GemColor((int)Gem), 0.6f);
                }
            }
            return true;
        }

        public override void PostAI()
        {
            if (Gem == (int)Gems.Amethyst && empowered && Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = Projectile.velocity.Length() * Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.MouseWorld) * Projectile.velocity.Length() * 0.5f, 0.3f).SafeNormalize(Vector2.Normalize(Projectile.velocity));
                Projectile.netUpdate = true;
            }
            if (Gem == (int)Gems.Emerald && empowered)
            {
                emeraldDropCounter--;
                if (emeraldDropCounter <= 0)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X, Projectile.Center.Y + Main.rand.NextFloat(-4, 4)), Vector2.Zero, ProjectileType<FallingEmerald>(), (int)(Projectile.damage * 0.7f), Projectile.knockBack * 0.3f, Projectile.owner);
                    emeraldDropCounter = 4;
                }
            }
            if (Gem == (int)Gems.Ruby && empowered && counter > 1)
            {
                rubyShotCounter--;
                rubyCounter++;
                if (rubyShotCounter <= 0)
                {
                    rubyShotCounter = 12;
                    float closest = 8000f;
                    int target = -1;
                    for (int i = 0; i < 200; i++)
                    {
                        float distanceFromTarget = Vector2.Distance(Projectile.Center, Main.npc[i].Center);
                        if (distanceFromTarget < closest && distanceFromTarget < 450f && Main.npc[i].CanBeChasedBy(Projectile, false))
                        {
                            target = i;
                            closest = distanceFromTarget;
                        }
                    }
                    if (target != -1)
                    {
                        bool canHit = Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[target].position, Main.npc[target].width, Main.npc[target].height);
                        if (canHit)
                        {
                            SoundEngine.PlaySound(SoundID.Item28, Projectile.Center);
                            Vector2 shootVelocity = Main.npc[target].Center - Projectile.Center;
                            float num4 = 25f;
                            float num5 = (float)Math.Sqrt((double)(shootVelocity.X * shootVelocity.X + shootVelocity.Y * shootVelocity.Y));
                            if (num5 > num4)
                            {
                                num5 = num4 / num5;
                            }
                            shootVelocity *= num5;

                            Vector2 velocity = shootVelocity.RotatedByRandom(MathHelper.ToRadians(4f));

                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, velocity * 0.55f, ProjectileType<BasicRubyBolt>(), (int)(Projectile.damage * 0.7f), Projectile.knockBack * 0.3f, Projectile.owner);
                        }
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Gem == (int)Gems.Amethyst && empowered)
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.NightsEdge,
                        new ParticleOrchestraSettings { PositionInWorld = Projectile.Center },
                        Projectile.owner);
                empowered = false;
            }
            if (Gem == (int)Gems.Sapphire && empowered)
            {
                empowered = false;
                int explosion = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<SapphireBurst>(), Projectile.damage, 0, Projectile.owner, 60f);
                SoundEngine.PlaySound(SoundID.NPCDeath7, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Projectile.Center);
                target.immune[Projectile.owner] = 4;
            }
            if (Gem == (int)Gems.Emerald && empowered)
            {
                Projectile.velocity.Y -= 9;

                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.TrueNightsEdge,
                    new ParticleOrchestraSettings { PositionInWorld = target.Center },
                    target.whoAmI);
            }
            if (Gem == (int)Gems.Ruby && !empowered)
            {
                empowered = true;
                for (float k = 0; k < 6.28f; k += 0.4f)
                    Dust.NewDustPerfect(Projectile.Center, DustType<LightDust>(), Vector2.One.RotatedBy(k) * 0.65f, 0, GemColor((int)Gem));
            }
            if (Gem == (int)Gems.Diamond && empowered)
            {
                empowered = false;
                target.immune[Projectile.owner] = 0;
                Projectile.damage = 0;
            }
            if (Gem == (int)Gems.Amber && empowered)
            {
                empowered = false;
                target.AddBuff(BuffType<AmberStun>(), 30);
                SoundEngine.PlaySound(SoundID.Item150, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.ChlorophyteLeafCrystalShot,
                    new ParticleOrchestraSettings { PositionInWorld = target.Center },
                    target.whoAmI);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if ((Gem == (int)Gems.Amethyst || Gem == (int)Gems.Emerald) && empowered)
            {
                empowered = false;
                Projectile.GetGlobalProjectile<GoldLeafProjectile>().gravity = 0f;
            }
            if (Gem == (int)Gems.Topaz && empowered)
            {
                TopazBounce(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 8f, (-Projectile.velocity).SafeNormalize(Vector2.UnitX));

                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }

                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
                empowered = false;
                return false;
            }

            return true;
        }

        void TopazBounce(Vector2 hitPoint, Vector2 normal)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Projectile.Center);

            Vector2 spinningpoint = Vector2.Reflect(Projectile.velocity, normal);
            for (int i = 0; i < 6; i++)
            {
                Dust dust = Dust.NewDustPerfect(hitPoint, DustType<LightDust>(), spinningpoint.RotatedBy((float)Math.PI / 4f * Main.rand.NextFloatDirection()) * 0.6f * Main.rand.NextFloat(), 100, GemColor(2), 0.5f);
                //Dust dust = Dust.NewDustPerfect(hitPoint, DustID.GemTopaz, spinningpoint.RotatedBy((float)Math.PI / 4f * Main.rand.NextFloatDirection()) * 0.6f * Main.rand.NextFloat(), 100, Color.Yellow, 1.0f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            if (empowered && Gem != (int)Gems.None)
            {
                //texture = Request<Texture2D>("GoldLeaf/Textures/Slash").Value;
                texture = Request<Texture2D>("GoldLeaf/Items/Gem/SedimentGlow").Value;

                Vector2 drawOrigin = new(texture.Width * 0.5f/* * 0.07f*/, Projectile.height * 0.5f/* * 0.07f*/);
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Main.spriteBatch.Draw(texture, drawPos, null, GemColor((int)Gem) * (0.35f - (k * 0.025f)), Projectile.oldRot[k], drawOrigin, (Projectile.scale * (1f - (k * 0.075f))) /* * 0.07f*/, SpriteEffects.None, 0f);
                }
            }
            else
            {
                Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);
                for (int k = 0; k < Projectile.oldPos.Length / 2; k++)
                {
                    var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(lightColor) * (float)(((float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length) / 2);

                    Main.spriteBatch.Draw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color, Projectile.oldRot[k], drawOrigin, Projectile.scale, effects, 0f);
                }
            }
            return true;
        }
    }

    public class SedimentTopaz : ModProjectile
    {
        public bool empowered = true;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WoodenBoomerang);

            //Projectile.GetGlobalProjectile<GoldLeafProjectile>().throwingDamageType = DamageClass.Melee;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.width = 20;
            Projectile.height = 20;
        }

        public override bool PreAI()
        {
            if (empowered)
            {
                if (Main.rand.NextBool(5))
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<LightDust>(), Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 0, new Color(246, 188, 0), 0.4f);
                }
            }

            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (empowered)
            {
                TopazBounce(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 8f, (-Projectile.velocity).SafeNormalize(Vector2.UnitX));

                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }

                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
                empowered = false;
                return false;
            }

            return true;
        }
        void TopazBounce(Vector2 hitPoint, Vector2 normal)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Projectile.Center);

            Vector2 spinningpoint = Vector2.Reflect(Projectile.velocity, normal);
            for (int i = 0; i < 6; i++)
            {
                Dust dust = Dust.NewDustPerfect(hitPoint, DustType<LightDust>(), spinningpoint.RotatedBy((float)Math.PI / 4f * Main.rand.NextFloatDirection()) * 0.6f * Main.rand.NextFloat(), 100, new Color(246, 188, 0), 0.5f);
                //Dust dust = Dust.NewDustPerfect(hitPoint, DustID.GemTopaz, spinningpoint.RotatedBy((float)Math.PI / 4f * Main.rand.NextFloatDirection()) * 0.6f * Main.rand.NextFloat(), 100, Color.Yellow, 1.0f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            if (empowered)
            {
                //texture = Request<Texture2D>("GoldLeaf/Textures/Slash").Value;
                texture = Request<Texture2D>("GoldLeaf/Items/Gem/SedimentGlow").Value;

                Vector2 drawOrigin = new(texture.Width * 0.5f/* * 0.07f*/, Projectile.height * 0.5f/* * 0.07f*/);
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Main.spriteBatch.Draw(texture, drawPos, null, GemColor(2) * (0.35f - (k * 0.025f)), Projectile.oldRot[k], drawOrigin, (Projectile.scale * (1f - (k * 0.075f))) /* * 0.07f*/, SpriteEffects.None, 0f);
                }
            }
            else
            {
                Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);
                for (int k = 0; k < Projectile.oldPos.Length / 2; k++)
                {
                    var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(lightColor) * (float)(((float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length) / 2);

                    Main.spriteBatch.Draw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color, Projectile.oldRot[k], drawOrigin, Projectile.scale, effects, 0f);
                }
            }
            return true;
        }
    }

    public class SapphireBurst : ModProjectile
    {
        public override string Texture => "GoldLeaf/Textures/Empty";

        public float TimeFade => 1 - Projectile.timeLeft / 20f;
        public float Radius => BezierEase((20 - Projectile.timeLeft) / 20f) * Projectile.ai[0];
        int counter;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 24;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            //Projectile.extraUpdates = 1;

            //Projectile.GetGlobalProjectile<GoldLeafProjectile>().throwingDamageType = DamageClass.Melee;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void OnSpawn(IEntitySource source)
        {
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
                new ParticleOrchestraSettings { PositionInWorld = Projectile.Center },
                Projectile.owner);
        }

        public override bool PreAI()
        {
            counter = Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter;
            return true;
        }

        float rot = 0;
        public override void AI()
        {
            Lighting.AddLight((int)(Projectile.position.X / 16), (int)(Projectile.position.Y / 16), GemColor(3).R / 255, GemColor(3).G / 255, GemColor(3).B / 255);

            if (counter <= 10 /*&& counter % 2 == 0*/)
            {
                for (int k = 0; k < 2; k++)
                {
                    rot += (float)Math.PI / 20;
                    if (rot >= Math.PI * 2) rot = 0;

                    float x = (float)Math.Cos(GoldLeafWorld.rottime + rot + k * 3) * 1.6f;
                    float y = (float)Math.Sin(GoldLeafWorld.rottime + rot + k * 3) * 1.6f;
                    Vector2 pos = (new Vector2(x, y)).RotatedBy(k * 2 * 6.28f);

                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.StardustPunch,
                        new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = pos * (4.8f - (counter * 0.2f)) },
                        Projectile.owner);

                    //Dust d = Dust.NewDustPerfect(Projectile.Center, DustType<LightDust>(), pos * (3.2f - (counter * 0.1f)), 0, GemColor(3), 0.5f);
                }
            }

            if (counter > 12)
            {
                Projectile.damage = 0;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CheckCircularCollision(Projectile.Center, (int)Radius + 40, targetHitbox);
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = Request<Texture2D>("GoldLeaf/Textures/Flares/wavering").Value;
            Color color = GemColor(3) * (1.2f - (counter * 0.05f));
            color.A = 0;

            Main.spriteBatch.Draw
            (
                tex,
                new Vector2
                (
                    Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f,
                    Projectile.position.Y - Main.screenPosition.Y + Projectile.height * 0.5f
                ),
                new Rectangle(0, 0, tex.Width, tex.Height),
                color,
                0f,
                tex.Size() * 0.5f,
                Projectile.scale * 0.16f + (Radius * 0.009f),
                SpriteEffects.None,
                0f
            );
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.velocity += Vector2.Normalize(target.Center - Projectile.Center) * 4.6f * target.knockBackResist;

            target.immune[Projectile.owner] = 8;
        }
    }

    public class FallingEmerald : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Emerald;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 16;

            Projectile.friendly = true;
            Projectile.extraUpdates = 1;

            Projectile.DamageType = DamageClass.Melee;

            Projectile.GetGlobalProjectile<GoldLeafProjectile>().gravity = 0.08f;
            Projectile.GetGlobalProjectile<GoldLeafProjectile>().gravityDelay = 20;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Projectile.Center);

            for (float k = 0; k < 6.28f; k += 0.52f)
                Dust.NewDustPerfect(Projectile.Center, DustType<LightDust>(), Vector2.One.RotatedBy(k) * 0.45f, 0, GemColor(4), 0.35f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin/* + new Vector2(0f, Projectile.gfxOffY)*/;
                Main.spriteBatch.Draw(tex, drawPos, null, Color.White * (1.0f - (0.15f * k)), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }

    public class BasicRubyBolt : ModProjectile
    {
        public override string Texture => "GoldLeaf/Textures/Empty";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.RubyBolt);
            //AIType = ProjectileID.RubyBolt;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 0.5f;
        }

        public override void AI()
        {
            //Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemRuby, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, GemColor(ItemID.Ruby), 0.6f);
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GemRuby, Projectile.velocity * 0.6f, 0, GemColor(ItemID.Ruby), 1f);
            d.noGravity = true;
        }
    }

    public class AmberStun : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);
        
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<GoldLeafNPC>().stunned = true;

            if (!npc.boss)
            {
                npc.velocity *= 0;
                npc.frame.Y = 0;
            }

            if (Main.rand.NextBool(20))
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.AshTreeShake,
                    new ParticleOrchestraSettings { PositionInWorld = npc.Center },
                    npc.whoAmI);
            }
        }
    }
}
