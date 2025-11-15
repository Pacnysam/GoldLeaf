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
using System.IO;
using GoldLeaf.Core.CrossMod;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;
using Terraria.GameContent.Animations;
using Terraria.ModLoader.IO;
using GoldLeaf.Core.Mechanics;

namespace GoldLeaf.Items.Sky
{
    public class Constellation : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ConstellationTag.TagDamageMult);

        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            Item.AddElements([Element.Celestial]);
            ItemSets.Glowmask[Type] = (glowTex, ColorHelper.AdditiveWhite(200) * 0.85f, true);
        }

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ProjectileType<ConstellationP>(), 21, 1.5f, 3.25f, 40);

            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.autoReuse = true;
        }

        public override float UseSpeedMultiplier(Player player) 
            => MathHelper.Lerp(12/9f, 1f, (float)player.GetModPlayer<ConstellationPlayer>().extraSegments / 10);
        
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
            => damage.Base -= MathHelper.Lerp(6, 0, (float)player.GetModPlayer<ConstellationPlayer>().extraSegments / 10);

        public override bool MeleePrefix() => true;
    }
    
    public class ConstellationP : ModProjectile
    {
        private static Asset<Texture2D> glowTex;
        private static Asset<Texture2D> bloomTex;

        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
            bloomTex = Request<Texture2D>("GoldLeaf/Textures/Masks/Mask0");
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.IsAWhip[Type] = true;

            Projectile.AddElements([Element.Celestial]);
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            Projectile.extraUpdates = 2;
            Projectile.WhipSettings.Segments = 4;
            Projectile.WhipSettings.RangeMultiplier = 0.75f;
        }

        bool hasSetup = false;
        ref float Timer => ref Projectile.ai[0];
        ref float Segments => ref Projectile.ai[1];

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!hasSetup)
            {
                Segments = player.GetModPlayer<ConstellationPlayer>().extraSegments;
                hasSetup = true;
            }

            Projectile.GetWhipSettings(Projectile, out var timeToFlyOut, out var _, out var _);
            Projectile.WhipSettings.Segments = 4 + (int)(Segments * 0.7f);

            float flyTimer = Timer / timeToFlyOut;
            if (Utils.GetLerpValue(0.1f, 0.7f, flyTimer, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, flyTimer, clamped: true) > 0.1f)
            {
                Projectile.WhipPointsForCollision.Clear();
                Projectile.FillWhipControlPoints(Projectile, Projectile.WhipPointsForCollision);
                Rectangle rectangle = Utils.CenteredRectangle(Projectile.WhipPointsForCollision[^1], new Vector2(18f, 20f));
                Vector2 forwardVector = Projectile.WhipPointsForCollision[^2].DirectionTo(Projectile.WhipPointsForCollision[^1]).SafeNormalize(Vector2.Zero);

                /*Dust dust = Dust.NewDustDirect(rectangle.TopLeft(), rectangle.Width, rectangle.Height, DustID.FireworkFountain_Blue, 0f, 0f, 90, default, 0.7f);
                dust.velocity *= Main.rand.NextFloat() * 0.55f;
                dust.noGravity = true;
                dust.scale = MathHelper.Lerp(0.2f, 0.45f, (float)player.GetModPlayer<ConstellationPlayer>().extraSegments / player.GetModPlayer<ConstellationPlayer>().maxExtraSegments) + Main.rand.NextFloat() * 0.4f;
                dust.fadeIn = Main.rand.NextFloat() * 0.75f;
                dust.velocity += forwardVector * 1.65f;*/
            }

            if (Timer == (int)(timeToFlyOut / 2f))
            {
                SoundEngine.FindActiveSound(in SoundID.Item153)?.Stop();
                
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/StarSlot")
                { Variants = [1, 2, 3], Pitch = 0.125f, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew }, Projectile.WhipPointsForCollision[^1]);
                SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, Projectile.WhipPointsForCollision[^1]);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            target.AddBuff(BuffType<ConstellationTag>(), TimeToTicks(5));
            player.MinionAttackTargetNPC = target.whoAmI;

            if (player.GetModPlayer<ConstellationPlayer>().extraSegments < player.GetModPlayer<ConstellationPlayer>().maxExtraSegments && target.IsValid())
            {
                player.GetModPlayer<ConstellationPlayer>().extraSegments++;

                Gore gore = Gore.NewGoreDirect(null, target.Top, Vector2.Zero, GoreType<ConstellationGore>());
                gore.rotation = MathHelper.ToRadians(Main.rand.NextFloat(380, 780)).RandomNegative();
                gore.velocity.X *= 0.65f;
                gore.velocity.Y = Main.rand.NextFloat(-8.5f, -6.5f);
                gore.frame = 2;
                gore.alpha -= Main.rand.Next(40, 70);

                for (int i = 0; i < 5; i++)
                {
                    Gore gore2 = Gore.NewGoreDirect(null, target.Top, Vector2.Zero, GoreType<ConstellationGore>());
                    gore2.rotation = MathHelper.ToRadians(Main.rand.NextFloat(420, 820)).RandomNegative();
                    gore2.velocity.X *= 1.45f;
                    gore2.velocity.Y = Main.rand.NextFloat(-4f, -1.5f);
                    gore2.frame = 0;
                    gore2.alpha += Main.rand.Next(-10, 15);
                }
            }
            else
            {
                Projectile.damage = (int)(Projectile.damage * 0.75f);

                Gore gore = Gore.NewGoreDirect(null, target.Top, Vector2.Zero, GoreType<ConstellationGore>());
                gore.rotation = MathHelper.ToRadians(Main.rand.NextFloat(320, 840)).RandomNegative();
                gore.velocity.X *= 0.65f;
                gore.velocity.Y = Main.rand.NextFloat(-6f, -4.5f);
                gore.frame = 1;
                gore.alpha -= Main.rand.Next(10, 30);

                for (int i = 0; i < 3; i++)
                {
                    Gore gore2 = Gore.NewGoreDirect(null, target.Top, Vector2.Zero, GoreType<ConstellationGore>());
                    gore2.rotation = MathHelper.ToRadians(Main.rand.NextFloat(580, 780)).RandomNegative();
                    gore2.velocity.X *= 1.45f;
                    gore2.velocity.Y = Main.rand.NextFloat(-4f, -1.5f);
                    gore2.frame = 0;
                    gore2.alpha += Main.rand.Next(0, 30);
                }
            }

            if (player.GetModPlayer<ConstellationPlayer>().extraSegments > 0)
                player.AddBuff(BuffType<ConstellationTag>(), TimeToTicks(5));

            SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Kirby/SuperStar/MirrorReflect") { Pitch = -0.5f + (player.GetModPlayer<ConstellationPlayer>().extraSegments * 0.05f), Volume = 0.7f }, player.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = [];
            Projectile.FillWhipControlPoints(Projectile, list);
            Vector2 pos = list[0];

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.instance.LoadProjectile(Type);

            for (int i = 0; i < list.Count - 1; i++)
            {
                Rectangle frame = new(0, 0, 18, 20); //handle
                Vector2 origin = new(9, 4);
                float scale = 1;
                if (i == list.Count - 2) //tip
                {
                    frame.Y = 68; //whip length minus tip
                    frame.Height = 24;

                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else if (i > 10) //far segment
                {
                    frame.Y = 50;
                    frame.Height = 18;
                }
                else if (i > 7) //mid segment
                {
                    frame.Y = 36;
                    frame.Height = 14;
                }
                else if (i > 4) //first segment
                {
                    frame.Y = 26;
                    frame.Height = 10;
                }
                else if (i > 0) //right above handle
                {
                    frame.Y = 20;
                    frame.Height = 6;
                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates());
                Color glowColor = ColorHelper.AdditiveWhite(200);

                Color color1 = new(81, 166, 243) { A = 0 };
                Color color2 = new(255, 241, 83) { A = 0 };

                if (i > 0 && i < list.Count - 2)
                {
                    color = Color.Lerp(color1, color2, MathHelper.Lerp(0f, 1f, Math.Clamp(i / (list.Count - 1f), 0f, 1f))) with { A = 255 };
                    glowColor = color.MultiplyAlpha(0.85f - MathHelper.Lerp(0f, 1f, Math.Clamp(i / (list.Count - 1f), 0f, 0.4f)));
                }
                Color bloomColor = Color.Lerp(color1, color2, MathHelper.Lerp(0f, 1f, Math.Clamp(i / (list.Count - 1f), 0f, 1f)));

                if (i == list.Count - 2)
                {
                    /*for (int k = 0; k < oldPos.Count; k++)
                    {
                        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, oldPos[k] - Main.screenPosition, frame, color1 with { A = 120 } * (0.55f - (0.05f * k)), oldRot[k], frame.Size() / 2f, scale * (1f - (0.035f * k)), flip);
                    }*/

                    Vector2 forwardVector = list[^2].DirectionTo(list[^1]).SafeNormalize(Vector2.Zero);
                    Main.EntitySpriteDraw(bloomTex.Value, pos + (forwardVector * 12f) - Main.screenPosition, null, color1 * 0.175f, rotation, bloomTex.Size() / 2, scale * 0.275f, SpriteEffects.None);
                }
                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip);
                Main.EntitySpriteDraw(glowTex.Value, pos - Main.screenPosition, frame, glowColor * 0.85f, rotation, origin, scale, flip);
                if (i > 0)
                {
                    Main.EntitySpriteDraw(bloomTex.Value, pos - Main.screenPosition, null, bloomColor * 0.15f, rotation, bloomTex.Size() / 2, 
                        scale * MathHelper.Lerp(0.145f, 0.25f, Math.Clamp(i / (list.Count - 1f), 0f, 1f)), SpriteEffects.None);
                }

                pos += diff;
            }
            return false;
        }
    }

    public class ConstellationPlayer : ModPlayer
    {
        public int extraSegments = 0;
        public int maxExtraSegments = 10;
        //public int segmentTimer = 60;

        public override void ResetEffects()
        {
            maxExtraSegments = 16; //maxExtraSegments = 10;
        }

        public override void PreUpdate()
        {
            //if (segmentTimer <= 0) extraSegments = 0;
            if (extraSegments > maxExtraSegments) extraSegments = maxExtraSegments;

            //segmentTimer--;
        }

        public override void PostUpdateBuffs()
        {
            if (!Player.HasBuff(BuffType<ConstellationTag>()))
                extraSegments = 0;

            if (extraSegments > 0)
                Player.whipRangeMultiplier += 0.065f * extraSegments;

            /*if (Player.HasBuff(BuffID.StarInBottle))
                maxExtraSegments += 3;
            if (!Main.dayTime)
                maxExtraSegments += 3;*/
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)GoldLeaf.MessageType.ConstellationSync);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)extraSegments);
            packet.Write((byte)maxExtraSegments);
            //packet.Write((byte)segmentTimer);
            packet.Send(toWho, fromWho);
        }
        public void ReceivePlayerSync(BinaryReader reader)
        {
            extraSegments = reader.ReadByte();
            maxExtraSegments = reader.ReadByte();
            //segmentTimer = reader.ReadByte();
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            ConstellationPlayer clone = (ConstellationPlayer)targetCopy;
            clone.extraSegments = extraSegments;
            clone.maxExtraSegments = maxExtraSegments;
            //clone.segmentTimer = segmentTimer;
        }
        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            ConstellationPlayer clone = (ConstellationPlayer)clientPlayer;

            if (extraSegments != clone.extraSegments || maxExtraSegments != clone.maxExtraSegments /*|| segmentTimer != clone.segmentTimer*/)
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }
    }

    public class ConstellationGlobalProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => ProjectileID.Sets.IsAWhip[entity.type];

        public bool hasSetup = false;
        public int segments = 0;

        public override bool PreAI(Projectile projectile)
        {
            if (!hasSetup)
            {
                projectile.TryGetOwner(out Player player);
                if (player != null)
                {
                    segments = player.GetModPlayer<ConstellationPlayer>().extraSegments;
                    projectile.netUpdate = true;
                }
                hasSetup = true;

                Projectile.GetWhipSettings(projectile, out var timeToFlyOut, out var _, out var _);

                if (projectile.type == ProjectileType<ConstellationP>())
                    projectile.WhipSettings.RangeMultiplier += 0.035f * segments;

                /*if (projectile.type == ProjectileType<ConstellationP>())
                    projectile.WhipSettings.RangeMultiplier += 0.115f * segments;
                else
                    projectile.WhipSettings.RangeMultiplier += 0.0725f * segments;*/
            }
            return true;
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(segments);
        }
        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            segments = binaryReader.ReadInt32();
        }
    }

    public class ConstellationTag : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public static readonly float TagDamageMult = 25;

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
            Main.debuff[Type] = false;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            SummonTagNPC gNPC = npc.GetGlobalNPC<SummonTagNPC>();
            gNPC.tagDamageMult += TagDamageMult * 0.01f;
        }
    }
    
    public class ConstellationGore : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.position -= new Vector2(22, 23) * gore.scale;
            gore.numFrames = 3;
            gore.behindTiles = false;
            gore.timeLeft = 30;
            gore.scale *= 0.1f;
            ChildSafety.SafeGore[gore.type] = true;
        }

        public override Color? GetAlpha(Gore gore, Color lightColor)
        {
            Color color1 = new(255, 241, 51);
            Color color2 = new(81, 166, 243);

            Color color = Color.Lerp(color1, color2, Math.Clamp((30 - gore.timeLeft) / 25f, 0f, 1f)).MultiplyAlpha(gore.alpha/255f - 0.3f);
            return color * gore.Opacity();
        }

        public override bool Update(Gore gore)
        {
            gore.position += gore.velocity;

            gore.rotation = MathHelper.Lerp(gore.rotation, gore.rotation * 0.45f, 0.075f);
            gore.velocity *= 0.94f;
            gore.velocity.Y += 0.175f;

            gore.alpha += 6;

            if (--gore.timeLeft < 10)
            {
                gore.scale = ((gore.scale - 1) / 3f) + 1;
            }
            else
            {
                gore.scale = MathHelper.Lerp(gore.scale, 1, 0.075f);
            }

            if (gore.alpha > 245)
                gore.active = false;
            return false;
        }
    }
}