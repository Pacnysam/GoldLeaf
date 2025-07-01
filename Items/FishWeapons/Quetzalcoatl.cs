using GoldLeaf.Core;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using GoldLeaf.Items.Dungeon;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using Terraria.Graphics.Shaders;
using Terraria.Graphics;
using Terraria.DataStructures;
using System;
using GoldLeaf.Items.Underground;
using Terraria.GameContent.Drawing;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using GoldLeaf.Items.Grove.Toxin;
using Terraria.Graphics.CameraModifiers;
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Items.Nightshade;
using Steamworks;
using GoldLeaf.Effects.Dusts;
using Terraria.GameContent;
using Terraria.GameInput;

using static GoldLeaf.GoldLeaf;

namespace GoldLeaf.Items.FishWeapons
{
    public class Quetzalcoatl : ModItem
	{
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }
        public override void SetStaticDefaults()
        {
            ItemSets.Glowmask[Type] = (glowTex, Color.White, false);
            Item.AddElements([Element.Arcane, Element.Holy, Element.Earth]);
        }

        public override void SetDefaults()
		{
            Item.shoot = ProjectileType<QuetzalP>();
            Item.DamageType = DamageClass.Magic;
            Item.damage = 91;
            Item.mana = 16;

            Item.width = 54;
			Item.height = 62;

            Item.useTime = Item.useAnimation = 52;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.staff[Item.type] = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = false;

            Item.shootSpeed = 15f;
            Item.knockBack = 9;

			Item.value = Item.sellPrice(0, 7, 50, 0);
            Item.rare = ItemRarityID.Cyan;

            Item.UseSound = SoundID.DD2_BetsyFireballShot with { Volume = 0.9f, Pitch = 0.2f, PitchVariance = 0.5f };
        }

        public override void HoldItem(Player player)
        {
            /*if (player.GetModPlayer<ControlsPlayer>().rightClick)
            {
                if ((player.ownedProjectileCounts[ProjectileType<QuetzalOrb>()] > 0 || player.ownedProjectileCounts[ProjectileType<QuetzalRift>()] > 0) && !player.HasBuff(BuffType<QuetzalBuff>()))
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (int p = 0; p < 300; p++)
                        {
                            Projectile projectile = Main.projectile[p];
                            if (projectile.active && projectile.type == ProjectileType<QuetzalRift>() && projectile.owner == player.whoAmI && (projectile.ai[0] == 0 && projectile.ai[1] == 0))
                            {
                                projectile.ai[0] = 1;
                                projectile.timeLeft = 10;

                                Vector2 vectorToCursor = projectile.Center - player.Center;
                                bool projDirection = projectile.Center.X < player.Center.X;
                                if (projectile.Center.X < player.Center.X)
                                {
                                    vectorToCursor = -vectorToCursor;
                                }
                                player.direction = ((!projDirection) ? 1 : (-1));
                                player.itemRotation = vectorToCursor.ToRotation();
                            }
                        }
                    }
                }

                Item.shoot = ProjectileType<QuetzalOrb>();
                Item.shootSpeed = 28f;
                Item.UseSound = (player.ownedProjectileCounts[ProjectileType<QuetzalOrb>()] > 0 || player.ownedProjectileCounts[ProjectileType<QuetzalRift>()] > 0) ? SoundID.NPCHit52 with { Volume = 1.35f } : new SoundStyle("GoldLeaf/Sounds/SE/Monolith/HolyLaser") { Volume = 0.35f, PitchVariance = 0.4f };
                Item.autoReuse = false;

                Item.useTime = Item.useAnimation = 24;
            }
            else
            {
                Item.shoot = ProjectileType<QuetzalP>();
                Item.shootSpeed = 15f;
                Item.UseSound = SoundID.DD2_EtherianPortalSpawnEnemy with { Volume = 1f, Pitch = 0.8f, PitchVariance = 0.4f };
                Item.autoReuse = true;

                Item.useTime = Item.useAnimation = 52;
            }*/

            if (Main.myPlayer == player.whoAmI && player.GetModPlayer<ControlsPlayer>().rightClick)
            {
                if (player.ownedProjectileCounts[ProjectileType<QuetzalOrb>()] > 0 || player.ownedProjectileCounts[ProjectileType<QuetzalRift>()] > 0)
                {
                    //SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch, player.Center);
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (int p = 0; p < 300; p++)
                        {
                            Projectile projectile = Main.projectile[p];
                            if (projectile.active && projectile.type == ProjectileType<QuetzalRift>() && projectile.owner == player.whoAmI && (projectile.ai[0] == 0 && projectile.ai[1] == 0))
                            {
                                projectile.ai[0] = 1;
                                projectile.timeLeft = 10;

                                Vector2 vectorToCursor = projectile.Center - player.Center;
                                bool projDirection = projectile.Center.X < player.Center.X;
                                if (projectile.Center.X < player.Center.X)
                                {
                                    vectorToCursor = -vectorToCursor;
                                }
                                player.direction = ((!projDirection) ? 1 : (-1));
                                player.itemRotation = vectorToCursor.ToRotation();
                            }
                        }
                    }
                }
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2 && (player.ownedProjectileCounts[ProjectileType<QuetzalOrb>()] > 0 || player.ownedProjectileCounts[ProjectileType<QuetzalRift>()] > 0 || player.HasBuff(BuffType<QuetzalBuff>())))
                return false;

            if (player.altFunctionUse == 2)
            {
                Item.shoot = ProjectileType<QuetzalOrb>();
                Item.shootSpeed = 28f;
                Item.UseSound = (player.ownedProjectileCounts[ProjectileType<QuetzalOrb>()] > 0 || player.ownedProjectileCounts[ProjectileType<QuetzalRift>()] > 0) ? SoundID.NPCHit52 with { Volume = 1.35f } : new SoundStyle("GoldLeaf/Sounds/SE/Monolith/HolyLaser") { Volume = 0.35f, PitchVariance = 0.4f };
            }
            else
            {
                Item.shoot = ProjectileType<QuetzalP>();
                Item.shootSpeed = 15f;
                Item.UseSound = SoundID.DD2_BetsyFireballShot with { Volume = 0.9f, Pitch = 0.2f, PitchVariance = 0.5f };
            }
            return true;
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (player.altFunctionUse != 2)
                return 1f;
            else
                return 0.35f;
        }
        public override float UseAnimationMultiplier(Player player)
        {
            if (player.altFunctionUse != 2)
                return 1f;
            else
                return 0.35f;
        }
        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse != 2)
                mult = 1f;
            else if (player.altFunctionUse == 2 && (player.ownedProjectileCounts[ProjectileType<QuetzalOrb>()] > 0 || player.ownedProjectileCounts[ProjectileType<QuetzalRift>()] > 0))
                mult = 0f;
            else 
                mult = 4f;
        }

        public override bool AltFunctionUse(Player player) => true;
        
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * (Item.height + 10);

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }

        public override bool CanShoot(Player player)
        {
            if (player.altFunctionUse == 2 && (player.ownedProjectileCounts[ProjectileType<QuetzalOrb>()] > 0 || player.ownedProjectileCounts[ProjectileType<QuetzalRift>()] > 0))
            {
                return false;
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy with { Volume = 1f, Pitch = 0.8f, PitchVariance = 0.4f }, position);

            if (player.whoAmI == Main.myPlayer)
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
                    new ParticleOrchestraSettings { PositionInWorld = position },
                    player.whoAmI);
            }

            for (float k = 0; k < Math.PI * 2; k += Main.rand.NextFloat(0.24f, 0.38f))
            {
                Dust dust = Dust.NewDustPerfect(position, DustID.AmberBolt, Vector2.One.RotatedBy(k) * 1.25f, 0, Color.White, Main.rand.NextFloat(0.85f, 1.25f));
                dust.fadeIn = Main.rand.NextFloat(0.5f, 1.5f);
                dust.rotation = Main.rand.NextFloat(-12, 12);
                dust.noGravity = true;
            }

            if (player.altFunctionUse == 2)
            {
                Vector2 cursorDirection = Vector2.Normalize(position - Main.MouseWorld);
                for (int k = 1; k <= 3; k++)
                {
                    for (float n = 0; n <= 6.28f; n += 0.03f)
                    {
                        Vector2 off = new Vector2((float)Math.Cos(n), (float)Math.Sin(n) * 2) * (7.5f + (k * 4));
                        Dust dust = Dust.NewDustPerfect(position + off.RotatedBy(cursorDirection.ToRotation()), DustID.AmberBolt, cursorDirection * -k * 2.5f, 0, Color.White, Main.rand.NextFloat(0.45f, 0.8f));
                        dust.fadeIn = Main.rand.NextFloat(0.5f, 0.85f);
                        dust.rotation = Main.rand.NextFloat(-9, 9);
                        dust.noGravity = true;
                    }
                }
            }
            return true;
        }
        /*public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2 && (player.ownedProjectileCounts[ProjectileType<QuetzalOrb>()] > 0 || player.ownedProjectileCounts[ProjectileType<QuetzalRift>()] > 0))
            {
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch, player.Center);
                if (player.whoAmI == Main.myPlayer)
                {
                    for (int p = 0; p < 300; p++)
                    {
                        if (Main.projectile[p].active && Main.projectile[p].type == ProjectileType<QuetzalRift>() && Main.projectile[p].owner == player.whoAmI)
                        {
                            Main.projectile[p].ai[0] = 1;
                            Main.projectile[p].timeLeft = 10;

                            Vector2 vectorToCursor = Main.projectile[p].Center - player.Center;
                            bool projDirection = Main.projectile[p].Center.X < player.Center.X;
                            if (Main.projectile[p].Center.X < player.Center.X)
                            {
                                vectorToCursor = -vectorToCursor;
                            }
                            player.direction = ((!projDirection) ? 1 : (-1));
                            player.itemRotation = vectorToCursor.ToRotation();
                        }
                    }
                }
                return false;
            }
            return null;
        }*/

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.CrystalSerpent);
            recipe.AddIngredient(ItemID.AmberStaff);
            recipe.AddIngredient(ItemID.LunarTabletFragment, 6);
            recipe.AddIngredient(ItemType<Demitoxin>(), 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
    
	public class QuetzalP : ModProjectile
	{
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

            Projectile.AddElements([Element.Arcane, Element.Holy, Element.Earth]);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.aiStyle = -1;

            Projectile.extraUpdates = 2;

            Projectile.ignoreWater = true;

            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.netImportant = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;

            Projectile.penetrate = -1;

            Projectile.DamageType = DamageClass.Magic;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Inflate(10, 10);
        }

        public float rottime = 0;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!Main.dedServ)
                Lighting.AddLight(Projectile.Center, (255/255) * 0.7f, (114/255) * 0.7f, (57/255) * 0.7f);

            if (Projectile.ai[0] > 0)
            {
                Projectile.scale *= 0.95f;
                Projectile.alpha += 255 / 10;
            }
            else 
            {
                const int homingRange = 175;

                float targetDistance = 8000f;
                int targetEnemy = -1;
                int targetProjectile = -1;

                for (int p = 0; p < 300; p++)
                {
                    float range = Vector2.Distance(Projectile.Center, Main.projectile[p].Center);
                    if (range < targetDistance && range < homingRange && Main.projectile[p].active && Main.projectile[p].type == ProjectileType<QuetzalRift>() && Main.projectile[p].owner == player.whoAmI && Main.projectile[p].ai[0] == 0)
                    {
                        targetProjectile = p;
                        targetDistance = range;
                    }
                }
                for (int i = 0; i < 200; i++)
                {
                    float range = Vector2.Distance(Projectile.Center, Main.npc[i].Center);
                    if (range < targetDistance && range < homingRange && Main.npc[i].active && Main.npc[i].CanBeChasedBy(Projectile, false))
                    {
                        targetEnemy = i;
                        targetDistance = range;
                    }
                }
                if (targetProjectile != -1 && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.projectile[targetProjectile].position, Main.projectile[targetProjectile].width, Main.projectile[targetProjectile].height))
                {
                    Projectile.velocity += Vector2.Normalize(Main.projectile[targetProjectile].Center - Projectile.Center) * 8f;

                    if (Projectile.velocity.Length() > 15f)
                        Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 15f;

                    if (Projectile.Hitbox.Intersects(Main.projectile[targetProjectile].Hitbox))
                    {
                        Main.projectile[targetProjectile].ai[1] = 1;
                        Projectile.timeLeft = 0;
                        Projectile.Kill();
                    }
                }
                else if (targetEnemy != -1 && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[targetEnemy].position, Main.npc[targetEnemy].width, Main.npc[targetEnemy].height))
                {
                    Projectile.velocity += Vector2.Normalize(Main.npc[targetEnemy].Center - Projectile.Center) * 8f;

                    if (Projectile.velocity.Length() > 15f)
                        Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 15f;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.velocity.X += (float)Math.Sin(rottime * 8) * Math.Clamp(Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter * 0.1f, 0f, 1.25f) * -1;
                    Projectile.velocity.Y += (float)Math.Sin(rottime * 8) * Math.Clamp(Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter * 0.1f, 0f, 1.25f);
                }

                Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-4, 4)), DustID.AmberBolt, Vector2.Zero, 0, Color.Orange, Main.rand.NextFloat(0.5f, 0.75f));
                dust.fadeIn = Main.rand.NextFloat(0.7f, 0.95f);
                dust.noGravity = true;
                dust.rotation = Main.rand.NextFloat(-8, 8);

                if (Main.rand.NextBool())
                {
                    Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.AmberBolt, 0, 0, 0, Color.White, Main.rand.NextFloat(0.85f, 1.25f));
                    dust2.fadeIn = Main.rand.NextFloat(1.15f, 1.5f);
                    dust.rotation = Main.rand.NextFloat(-18, 18);
                    dust2.noGravity = true;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void PostAI()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                rottime += (float)Math.PI / 60;
                if (rottime >= Math.PI * 2) rottime = 0;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] == 0 && !target.friendly;
        }

        public override bool PreKill(int timeLeft)
        {
            return Projectile.timeLeft > 0;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Explode();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Explode(target);
        }

        /*public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Explode();

            target.AddBuff(BuffType<AmberStun>(), 20, false);

            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
            new ParticleOrchestraSettings { PositionInWorld = target.MountedCenter },
            Projectile.owner);
        }*/

        private void Explode(NPC target = null) 
        {
            Player player = Main.player[Projectile.owner];

            Projectile.netUpdate = true;

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Volume = 1.2f }, Projectile.Center);
            SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Kirby/MassAttack/SputterStar") { Volume = 0.175f, Pitch = -0.25f, PitchVariance = 0.4f }, Projectile.Center);

            if (target != null)
            {
                target.AddBuff(BuffType<AmberStun>(), 20);

                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
                new ParticleOrchestraSettings { PositionInWorld = target.Center },
                Projectile.owner);
            }

            CameraSystem.QuickScreenShake(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.One), 40f, 5f, distance: 1500);
            //CameraSystem.AddScreenshake(player, 20, Projectile.Center);
            if (Projectile.owner == Main.myPlayer)
            {
                for (float k = 0; k < 10; k++)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-10, 3)), ProjectileType<QuetzalShard>(), Projectile.damage / 2, 0, Projectile.owner, 0f, 0f);
                    proj.GetGlobalProjectile<GoldLeafProjectile>().gravity = Main.rand.NextFloat(0.1f, 0.3f);
                    proj.ai[2] = Main.rand.Next(30, 70) - Math.Abs(proj.velocity.Length());
                    proj.netUpdate = true;
                }

                Projectile.extraUpdates = 0;

                if (Projectile.ai[0] == 0)
                {
                    Projectile.ai[0]++;
                    Projectile.timeLeft = 30;
                    Projectile.velocity = Vector2.Zero;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(QuetzalcoatlDrawer).Draw(Projectile, 1f);
            return false;
        }
    }
    
    public class QuetzalShard : ModProjectile
    {
        public override string Texture => "GoldLeaf/Items/FishWeapons/QuetzalDust";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

            Projectile.AddElements([Element.Arcane, Element.Holy, Element.Earth]);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 80;

            Projectile.extraUpdates = 1;

            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.netImportant = true;

            Projectile.penetrate = -1;

            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Lighting.AddLight(Projectile.Center, (255 / 255) * 0.3f, (114 / 255) * 0.3f, (57 / 255) * 0.3f);

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.ai[0] == 2)
            {
                const int homingRange = 850;

                float targetDistance = 1000f;
                int targetEnemy = -1;

                for (int i = 0; i < 200; i++)
                {
                    float range = Vector2.Distance(Projectile.Center, Main.npc[i].Center);
                    if (range < targetDistance && range < homingRange && Main.npc[i].active && Main.npc[i].CanBeChasedBy(Projectile, false))
                    {
                        targetEnemy = i;
                        targetDistance = range;
                    }
                }
                if (targetEnemy != -1 && Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter >= 10 + (Projectile.ai[2] / 3f))
                {
                    Projectile.velocity += Vector2.Normalize(Main.npc[targetEnemy].Center - Projectile.Center) * (3.5f + (Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter * 0.025f));
                    Projectile.tileCollide = false;
                    Projectile.ai[2]++;
                    Projectile.timeLeft++;

                    if (Projectile.velocity.Length() > 15f + (Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter * 0.05f))
                        Projectile.velocity = Vector2.Normalize(Projectile.velocity) * (15f + (Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter * 0.05f));
                }
                else
                {
                    Projectile.velocity *= 0.97f;
                }
                Projectile.tileCollide = false;
            }
            else
            {
                Projectile.tileCollide = true;
            }

            if (Projectile.ai[0] == 0 || Projectile.ai[0] == 2)
            {
                Projectile.ai[1]++;
                if (Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter > Projectile.ai[2])
                {
                    Pop();
                }
            }
            if (Projectile.ai[0] == 1)
            {
                Projectile.scale *= 0.95f;
                Projectile.alpha += 255 / 10;
            }
            else
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-1.5f, 1.5f)), DustID.AmberBolt, Vector2.Zero, 0, Color.Orange, Main.rand.NextFloat(0.35f, 0.6f));
                dust.fadeIn = Main.rand.NextFloat(0.55f, 0.75f);
                dust.rotation = Main.rand.NextFloat(-8, 8);
                dust.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        private void Pop()
        {
            Projectile.extraUpdates = 0;

            if (Projectile.ai[0] == 0 || Projectile.ai[0] == 2)
            {
                Projectile.ai[0] = 1;
                Projectile.timeLeft = 10;
                Projectile.velocity = Vector2.Zero;
            }

            for (float k = 0; k < Math.PI * 2; k += Main.rand.NextFloat(0.32f, 0.48f))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.AmberBolt, Vector2.One.RotatedBy(k) * 0.85f, 0, Color.White, Main.rand.NextFloat(0.85f, 1.25f));
                dust.fadeIn = Main.rand.NextFloat(0.75f, 1.25f);
                dust.rotation = Main.rand.NextFloat(-12, 12);
                dust.noGravity = true;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return (Projectile.ai[0] == 0 || Projectile.ai[0] == 2) && !target.friendly;
        }

        public override bool PreKill(int timeLeft)
        {
            return Projectile.timeLeft > 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 2)
                Pop();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.ai[0] == 2)
                Pop();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(QuetzalcoatlDrawer).Draw(Projectile, 0.45f * Projectile.scale);
            return false;
        }
    }

    public class QuetzalOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

            Projectile.AddElements([Element.Arcane, Element.Holy, Element.Earth, Element.Explosive]);
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = TimeToTicks(3);

            Projectile.extraUpdates = 2;

            Projectile.DamageType = DamageClass.Magic;

            Projectile.netImportant = true;
        }

        const float teleportRange = 1000f;

        public override bool? CanHitNPC(NPC target) => false;
        
        private ref int Counter => ref Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Vector2 vectorToCursor = Projectile.Center - player.Center;
            bool projDirection = Projectile.Center.X < player.Center.X;
            if (Projectile.Center.X < player.Center.X)
            {
                vectorToCursor = -vectorToCursor;
            }
            player.direction = ((!projDirection) ? 1 : (-1));
            player.itemRotation = vectorToCursor.ToRotation();
            player.itemTime = 5;
            player.itemAnimation = 5;

            Projectile.velocity *= 0.95f;

            if (Projectile.velocity.Length() < 0.5f)
                Projectile.Kill();
            
            if (++Projectile.frameCounter >= 12)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }

            if (!Main.dedServ)
                Lighting.AddLight(Projectile.Center, (255 / 255) * 0.4f + ((float)Math.Sin(GoldLeafWorld.rottime) * 0.5f), (114 / 255) * 0.4f + ((float)Math.Sin(GoldLeafWorld.rottime) * 0.5f), (57 / 255) * 0.4f + ((float)Math.Sin(GoldLeafWorld.rottime) * 0.5f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle? rect = new Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame));
            
            if (Main.myPlayer != Projectile.owner)
            {
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, Color.White with { A = 225 } * 0.5f * Projectile.Opacity, Projectile.rotation, rect.Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
                return false;
            }

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + texture.Size() / 2;
                Color color1 = new(255, 239, 163) { A = 185 };
                Color color2 = new(255, 114, 57) { A = 185 };

                Main.spriteBatch.Draw(texture, drawPos, rect, Color.Lerp(color1, color2, k / (Projectile.oldPos.Length + 2f)) * (0.6f - (k / 10f)), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            }

            for (int k = 1; k < 6; k++)
            {
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, rect, Color.White with { A = 180 } * (0.45f - (k / 15f)), Projectile.rotation, rect.Value.Size() / 2, Projectile.scale * (1 + k / 2) * (1f - MathHelper.Clamp(Projectile.velocity.Length()/3f, 0, 1f)), SpriteEffects.None, 0f);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, Color.White with { A = 225 }, Projectile.rotation, rect.Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<QuetzalRift>(), (int)(Projectile.damage * 1.25f), Projectile.knockBack, Projectile.owner);
                SoundEngine.PlaySound(SoundID.Item105);
            }
            else
                SoundEngine.PlaySound(SoundID.Item105, Projectile.Center);

            for (float k = 0; k < 6.28f; k += Main.rand.NextFloat(0.3f, 0.45f))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(6f).RotatedBy(k), DustID.AmberBolt, Vector2.One.RotatedBy(k) * Main.rand.NextFloat(0.9f, 1.35f), 0, Color.White);
                dust.fadeIn = Main.rand.NextFloat(0.95f, 1.45f);
                dust.rotation = Main.rand.NextFloat(-16, 16);
                dust.velocity.Y *= 1.5f;
                dust.noGravity = true;
            }
            for (float k = 0; k < 6.28f; k += Main.rand.NextFloat(0.18f, 0.24f))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(14.5f).RotatedBy(k), DustID.AmberBolt, Vector2.One.RotatedBy(k) * Main.rand.NextFloat(1.85f, 2.35f), 0, Color.White);
                dust.fadeIn = Main.rand.NextFloat(1.15f, 1.5f);
                dust.rotation = Main.rand.NextFloat(-22, 22);
                dust.velocity.Y *= 2f;
                dust.noGravity = true;
            }
        }
    }
    
    public class QuetzalRift : ModProjectile
    {
        private static Asset<Texture2D> ringTex;
        private static Asset<Texture2D> flareTex;
        private static Asset<Texture2D> flareTex2;
        public override void Load()
        {
            ringTex = Request<Texture2D>("GoldLeaf/Textures/RingGlow4");
            flareTex = Request<Texture2D>("GoldLeaf/Textures/Flares/Flare2");
            flareTex2 = Request<Texture2D>("GoldLeaf/Textures/Flares/Flare4");
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 9;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

            Projectile.AddElements([Element.Arcane, Element.Holy, Element.Earth, Element.Explosive]);
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 40;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = TimeToTicks(15);

            Projectile.netImportant = true;

            Projectile.DamageType = DamageClass.Magic;
        }

        const float teleportRange = 800f;
        public float rottime = 0;

        public override bool? CanHitNPC(NPC target) => false;

        private ref int Counter => ref Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter;

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = 0;
            }

            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = 0;
            }
            return false;
        }

        public override void AI()
        {
            Projectile.netUpdate = true;

            if (Projectile.ai[1] == 1 && Projectile.ai[0] == 0)
            {
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    ModPacket packet = Instance.GetPacket();
                    packet.Write((byte)MessageType.QuetzalRiftDetonate);
                    packet.Write((byte)Main.myPlayer);
                    packet.Write((byte)Projectile.whoAmI);
                }

                Detonate();
            }

            Projectile.velocity.Y = (float)Math.Sin(rottime) * Math.Clamp(Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter * 0.01f, 0f, 1f) * 2.5f;

            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }

            if (Projectile.ai[0] >= 1)
            {
                Projectile.scale *= 0.9f;
                Projectile.alpha += 16;

                if (Projectile.scale <= 0.2f)
                    Projectile.Kill();
            }

            Projectile.localAI[0] = MathHelper.Lerp(Projectile.localAI[0], 1f, 0.05f);
            Projectile.localAI[1] *= 0.9f;

            if (!Main.dedServ)
                Lighting.AddLight(Projectile.Center, (255 / 255) * 0.6f + ((float)Math.Sin(GoldLeafWorld.rottime) * 0.3f), (114 / 255) * 0.6f + ((float)Math.Sin(GoldLeafWorld.rottime) * 0.3f), (57 / 255) * 0.6f + ((float)Math.Sin(GoldLeafWorld.rottime) * 0.3f));
        }

        public void Detonate() 
        {
            Player player = Main.player[Projectile.owner];

            //SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Volume = 1.5f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion with { PitchVariance = 0.6f, Pitch = 0.2f }, Projectile.Center);
            SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Kirby/MassAttack/SputterStar") { Volume = 0.15f, Pitch = 0.4f, PitchVariance = 0.6f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item67, Projectile.Center);

            //CameraSystem.AddScreenshake(Main.LocalPlayer, 45, Projectile.Center);
            
            CameraSystem.QuickScreenShake(Projectile.Center, null, 20f, 5f, 90, 2000);
            CameraSystem.QuickScreenShake(Projectile.Center, (0f).ToRotationVector2(), 20f, 8f, 100, 2000);

            if (!Main.dedServ)
                Lighting.AddLight(Projectile.Center, (255 / 255) * 2f, (114 / 255) * 2f, (57 / 255) * 2f);

            if (Projectile.owner == Main.myPlayer)
            {
                for (float k = 0; k < 18; k++)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Main.rand.NextFloat((float)Math.PI * 2f).ToRotationVector2() * Main.rand.NextFloat(12.5f, 18.5f), ProjectileType<QuetzalShard>(), (int)(Projectile.damage * 0.45f), 0, Projectile.owner, 2);
                    proj.scale = 1.4f;
                    proj.ai[2] = Main.rand.Next(120, 280) - Math.Abs(proj.velocity.Length());
                    proj.extraUpdates = 1;
                    proj.netUpdate = true;
                }

                for (int f = 0; f < 200; f++)
                {
                    float range = Vector2.Distance(Projectile.Center, Main.npc[f].Center);
                    if (range < teleportRange && IsTargetValid(Main.npc[f]))
                    {
                        Main.npc[f].AddBuff(BuffType<AmberStun>(), TimeToTicks(1f));
                        Main.npc[f].SimpleStrikeNPC(Projectile.damage, 0, true, 0, DamageClass.Magic, true, Main.player[Projectile.owner].luck);
                    }
                }
            }
            player.reuseDelay = 30;

            Projectile.timeLeft = 15;
            Projectile.ai[1] = 2;
            Projectile.ai[0] = 0;
            Projectile.localAI[1] = 1f;
            Projectile.netUpdate = true;
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[0] >= 1 && Projectile.Center.Distance(player.Center) < teleportRange /*&& Main.tile[(Projectile.Center/16f).ToPoint()]*/)
            {
                player.Teleport(Projectile.position, 2, 0);
                player.AddBuff(BuffType<QuetzalBuff>(), TimeToTicks(1));
                
                if (player.velocity != Vector2.Zero)
                    player.velocity *= 0.5f;

                if (player.velocity.Y < 0)
                    player.velocity.Y = 0;

                player.velocity.Y -= 7.5f;

                for (float k = 0; k < 6.28f; k += Main.rand.NextFloat(0.3f, 0.45f))
                {
                    Dust dust = Dust.NewDustPerfect(player.Center + new Vector2(6f).RotatedBy(k), DustID.AmberBolt, Vector2.One.RotatedBy(k) * Main.rand.NextFloat(0.9f, 1.35f), 0, Color.White);
                    dust.fadeIn = Main.rand.NextFloat(0.95f, 1.45f);
                    dust.rotation = Main.rand.NextFloat(-16, 16);
                    dust.velocity.Y *= 1.5f;
                    dust.noGravity = true;
                }
                for (float k = 0; k < 6.28f; k += Main.rand.NextFloat(0.18f, 0.24f))
                {
                    Dust dust = Dust.NewDustPerfect(player.Center + new Vector2(14.5f).RotatedBy(k), DustID.AmberBolt, Vector2.One.RotatedBy(k) * Main.rand.NextFloat(1.85f, 2.35f), 0, Color.White);
                    dust.fadeIn = Main.rand.NextFloat(1.15f, 1.5f);
                    dust.rotation = Main.rand.NextFloat(-22, 22);
                    dust.velocity.Y *= 2f;
                    dust.noGravity = true;
                }

                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
                new ParticleOrchestraSettings { PositionInWorld = Projectile.Center },
                Projectile.owner);

                SoundEngine.PlaySound(SoundID.Item68, player.Center);

                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);
            }
            else
            {
                for (float k = 0; k < 6.28f; k += Main.rand.NextFloat(0.3f, 0.45f))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(24f).RotatedBy(k), DustID.AmberBolt, Vector2.One.RotatedBy(k) * Main.rand.NextFloat(0.9f, 1.35f), 0, Color.White);
                    dust.fadeIn = Main.rand.NextFloat(0.95f, 1.45f);
                    dust.rotation = Main.rand.NextFloat(-16, 16);
                    dust.velocity.Y *= 1.5f;
                    dust.velocity *= -1;
                    dust.noGravity = true;
                }
                for (float k = 0; k < 6.28f; k += Main.rand.NextFloat(0.18f, 0.24f))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(40f).RotatedBy(k), DustID.AmberBolt, Vector2.One.RotatedBy(k) * Main.rand.NextFloat(1.85f, 2.35f), 0, Color.White);
                    dust.fadeIn = Main.rand.NextFloat(1.15f, 1.5f);
                    dust.rotation = Main.rand.NextFloat(-22, 22);
                    dust.velocity.Y *= 2f;
                    dust.velocity *= -1;
                    dust.noGravity = true;
                }

                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
                new ParticleOrchestraSettings { PositionInWorld = Projectile.Center },
                Projectile.owner);
            }
        }

        public override void PostAI()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                rottime += (float)Math.PI / 60;
                if (rottime >= Math.PI * 2) rottime = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = 1f;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle? rect = new Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame));

            if (Main.myPlayer != Projectile.owner)
            {
                opacity = 0.5f;
            }

            float sizeMod = 1f;
            if (Projectile.ai[0] >= 1)
            {
                sizeMod = (1f - Projectile.scale);
            }

            Color ringColor1 = new(255, 239, 163) { A = 0 };
            Color ringColor2 = new(255, 114, 57) { A = 0 };

            ringColor1 *= Math.Clamp((teleportRange - Projectile.Center.Distance(Main.player[Projectile.owner].Center)) / 100f, 0, 1);
            ringColor2 *= Math.Clamp((teleportRange - Projectile.Center.Distance(Main.player[Projectile.owner].Center)) / 100f, 0, 1);

            if (Projectile.timeLeft <= 15)
            {
                ringColor1 *= (Projectile.timeLeft / 15f);
                ringColor2 *= (Projectile.timeLeft / 15f);
            }

            Main.spriteBatch.Draw(ringTex.Value, Projectile.Center - Main.screenPosition -(Projectile.velocity * 0.5f), null, Color.Lerp(ringColor1, ringColor2, ((float)Math.Sin(rottime * 2.5f)/2f) + 0.5f) * (Projectile.scale/2f) * 0.85f * Projectile.Opacity * opacity, 0f, ringTex.Size() / 2, 1.28f * Projectile.localAI[0] * (teleportRange/1000f), SpriteEffects.None, 0f);

            if (Projectile.ai[0] == 0)
            {
                if (Main.myPlayer != Projectile.owner)
                {
                    for (int k = 0; k < Projectile.oldPos.Length; k++)
                    {
                        Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + texture.Size() / 2;
                        Color color1 = new(255, 239, 163) { A = 185 };
                        Color color2 = new(255, 114, 57) { A = 185 };

                        Main.spriteBatch.Draw(texture, drawPos, rect, Color.Lerp(color1, color2, k / (Projectile.oldPos.Length + 2f)) * (0.6f - (k / 10f)) * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
                    }
                }
            }

            for (int k = 1; k < 6; k++)
            {
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, rect, Color.White with { A = 180 } * (0.45f - (k / 15f)) * (1 - (sizeMod/2)) * Projectile.Opacity * opacity, Projectile.rotation, rect.Value.Size() / 2, Projectile.scale * (1 + (k / (4 + ((float)Math.Sin(GoldLeafWorld.rottime) * 0.5f)) * Math.Clamp(Projectile.GetGlobalProjectile<GoldLeafProjectile>().counter * 0.01f, 0f, 1f))) * sizeMod, SpriteEffects.None, 0f);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, Color.White with { A = 225 } * Projectile.Opacity * opacity, Projectile.rotation, rect.Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            float opacity = 1f;
            if (Main.myPlayer != Projectile.owner)
            {
                opacity = 0.5f;
            }

            Main.spriteBatch.Draw(flareTex.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 114, 57) { A = 0 } * (Projectile.localAI[1] * 1.65f) * opacity, 0f, flareTex.Size() / 2, 0.5f + (3.5f * Projectile.localAI[1]), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(flareTex2.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 239, 163) { A = 0 } * (Projectile.localAI[1] * 0.9f) * opacity, MathHelper.ToRadians(90), flareTex2.Size() / 2, 1f + (6.5f * (1 - Projectile.localAI[1])), SpriteEffects.None, 0f);
        }
    }

    public class QuetzalBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += 0.7f;
            player.runAcceleration += 0.35f;
        }
    }
    
    public readonly struct QuetzalcoatlDrawer
    {
        private static readonly VertexStrip _vertexStrip = new();

        public void Draw(Projectile proj, float width = 1f)
        {
            MiscShaderData miscShaderData = GameShaders.Misc["RainbowRod"];
            miscShaderData.UseSaturation(-2.4f);
            miscShaderData.UseOpacity(proj.Opacity * (8f * width));
            miscShaderData.Apply();
            /*_vertexStrip.PrepareStripWithProceduralPadding(proj.oldPos, proj.oldRot, Black, BlackWidth, -Main.screenPosition + proj.Size / 2f);
            _vertexStrip.DrawTrail();*/
            _vertexStrip.PrepareStripWithProceduralPadding(proj.oldPos, proj.oldRot, StripColors, StripWidth, -Main.screenPosition + proj.Size / 2f);
            _vertexStrip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        /*private readonly Color Black(float progressOnStrip)
        {
            return Color.Lerp(Color.Black * 0.3f, Color.Black * 0f, Utils.GetLerpValue(0.3f, 1f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.8f, progressOnStrip));
        }
        private readonly float BlackWidth(float progressOnStrip)
        {
            float num = 1.5f;
            float lerpValue = Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(2f, 32f, num);
        }*/

        private Color StripColors(float progressOnStrip)
        {
            Color result = Color.Lerp(new Color(255, 239, 163) { A = 100 }, new Color(255, 114, 57) { A = 200 }, Utils.GetLerpValue(0f, 0.75f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            //result.A /= 2;
            return result;
        }
        private float StripWidth(float progressOnStrip)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(2f, 32f, num);
        }
    }
}