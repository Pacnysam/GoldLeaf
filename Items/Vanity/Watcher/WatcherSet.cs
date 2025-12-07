using GoldLeaf.Core;
using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Blizzard.Armor;
using GoldLeaf.Items.Granite;
using GoldLeaf.Items.Grove;
using GoldLeaf.Prefixes;
using GoldLeaf.Tiles.Grove;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Transactions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Vanity.Watcher
{
    [AutoloadEquip(EquipType.Head)]
    public class WatcherEyedrops : ModItem
    {
        public override string Texture => "GoldLeaf/Items/Vanity/Watcher/WatcherEyedrops";

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
        }

        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 24;

            Item.value = Item.sellPrice(0, 7, 50, 0);
            Item.rare = ItemRarityID.Purple;

            Item.vanity = true;
        }
        
        public int glowSlot = -1;
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
                glowSlot = EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}Glow", EquipType.Head, this, "WatcherHeadGlow");
        }
        public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
        {
            //glowMask = EquipLoader.GetEquipSlot(Mod, "WatcherHeadGlow", EquipType.Head);
            glowMaskColor = Color.White.Alpha(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.5f + 0.5f) * 0.35f;
        }
    } //TODO: Glowmasks

    [AutoloadEquip(EquipType.Body)]
    public class WatcherCloak : ModItem
    {
        public int glowSlot = -1;
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
                glowSlot = EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}Glow", EquipType.Legs, this, "WatcherLegsGlow");
                //frontSlot = EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Front}", EquipType.Front, this, "WatcherFront");
            }
        }

        public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
        {
            if (shadow == 0)
            {
                //glowMask = EquipLoader.GetEquipSlot(Mod, "WatcherLegsGlow", EquipType.Legs);
                glowMaskColor = Color.White.Alpha(1f - (MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.5f + 0.5f)) * 0.35f;
            }
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            robes = true;

            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
        }

        public override bool IsVanitySet(int head, int body, int legs)
        {
            return head == ItemType<WatcherEyedrops>();
        }

        public override void SetStaticDefaults()
        {
            ArmorIDs.Body.Sets.HidesArms[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.HidesTopSkin[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.HidesBottomSkin[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.HidesHands[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.DisableHandOnAndOffAccDraw[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.DisableBeltAccDraw[Item.bodySlot] = true;

            //ArmorIDs.Body.Sets.IncludeCapeFrontAndBack[Item.bodySlot] = new ArmorIDs.Body.Sets.IncludeCapeFrontAndBackInfo { backCape = glowSlot, frontCape = frontSlot };
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;

            Item.value = Item.sellPrice(0, 7, 50, 0);
            Item.rare = ItemRarityID.Purple;

            Item.bodySlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            Item.vanity = true;
        }
    } //TODO: Glowmasks

    public class WatcherPlayer : ModPlayer
    {
        public bool WatcherDrops => Player.armor[0].type == ItemType<WatcherEyedrops>() && Player.armor[10].type == ItemID.None || Player.armor[10].type == ItemType<WatcherEyedrops>();
        public bool WatcherCloak => Player.armor[1].type == ItemType<WatcherCloak>() && Player.armor[11].type == ItemID.None || Player.armor[11].type == ItemType<WatcherCloak>();
        public bool WatcherSet => WatcherDrops && WatcherCloak;

        private float ShadowOpacity = 0f;
        private Color CustomShadowColor = Color.White;

        public override void PostUpdateMiscEffects()
        {
            ShadowOpacity = MathHelper.Lerp(ShadowOpacity, (Player.velocity.Length() >= 1 && WatcherSet) ? 1f : 0f, 0.15f);
        }

        public override void DrawPlayer(Camera camera)
        {
            if (!WatcherSet)
                return;

            int skip = 2;
            int totalShadows = Math.Min(Player.availableAdvancedShadowsCount, 18);
            CustomShadowColor = new Color(255, 120, 235).Alpha(40) * 0.75f;

            float num7 = Main.GlobalTimeWrappedHourly * 8f;
            float globalTime = Main.GlobalTimeWrappedHourly;
            globalTime %= 5f;
            globalTime /= 2.5f;
            if (globalTime >= 1f)
            {
                globalTime = 2f - globalTime;
            }
            globalTime = globalTime * 0.5f + 0.5f;
            CustomShadowColor = new Color(255, 120, 235).Alpha(40) * 0.7f;
            for (float k = 0f; k < 1f; k += 0.25f)
            {
                Main.PlayerRenderer.DrawPlayer(camera, Player, Player.position + new Vector2(0f, 2.5f /*3f + MathF.Sin(Main.GlobalTimeWrappedHourly * 10f) * 0.5f*/).RotatedBy((k + (num7 * -1.75f)) * ((float)Math.PI * 2f)) * globalTime, Player.fullRotation, Player.fullRotationOrigin, 1, 1f);
            }
            CustomShadowColor = Color.White;

            if (ShadowOpacity < 0.01)
                return;

            for (int i = totalShadows - totalShadows % skip; i > 0; i -= skip)
            {
                EntityShadowInfo advancedShadow = Player.GetAdvancedShadow(i);
                float shadow = Utils.Remap((float)i / totalShadows, 0, 1, 0.5f, 1f, clamped: true);
                CustomShadowColor = Color.Black * 0.1f * MathHelper.SmoothStep(0.95f, 0f, i / 28f) * ShadowOpacity;
                Main.PlayerRenderer.DrawPlayer(camera, Player, advancedShadow.Position, advancedShadow.Rotation, advancedShadow.Origin, shadow);
                CustomShadowColor = Color.Lerp(new Color(255, 120, 235), new Color(123, 59, 239), i / 14f).Alpha(40) * MathHelper.SmoothStep(0.95f, 0f, i / 28f) * ShadowOpacity;
                Main.PlayerRenderer.DrawPlayer(camera, Player, advancedShadow.Position, advancedShadow.Rotation, advancedShadow.Origin, shadow);
            }
            CustomShadowColor = Color.White;
        }

        public override void TransformDrawData(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            if (CustomShadowColor == Color.White)
                return;

            for (int i = drawInfo.DrawDataCache.Count - 1; i >= 0; i--)
            {
                DrawData value = drawInfo.DrawDataCache[i];

                value.color = CustomShadowColor;

                if (player.dye[2].type != ItemID.None)
                {
                    if (!ItemID.Sets.NonColorfulDyeItems.Contains(player.dye[2].type))
                        value.color = value.color.MultiplyRGBA(Color.White);
                    
                    value.shader = player.dye[2].dye;
                }
                drawInfo.DrawDataCache[i] = value;
            }
        }
        
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (WatcherSet)
            {
                modifiers.DisableSound();
            }
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if (WatcherSet && Player.statLife > 0)
            {
                //SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/StarSlot") { Variants = [1, 2, 3] }, Player.Center);
                //SoundEngine.PlaySound(SoundID.DD2_LightningBugHurt, Player.position);
                SoundEngine.PlaySound(SoundID.DD2_KoboldIgnite with { Volume = 1.1f }, Player.Center);
                SoundEngine.PlaySound(SoundID.NPCHit52 with { Volume = 0.95f, Pitch = 0.35f }, Player.position);
            }
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (WatcherSet)
            {
                playSound = false;
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (WatcherSet)
            {
                SoundEngine.PlaySound(SoundID.NPCHit52 with { Volume = 0.8f }, Player.position);
                SoundEngine.PlaySound(SoundID.DD2_KoboldIgnite with { Volume = 1.1f }, Player.position);
                SoundEngine.PlaySound(SoundID.Item68, Player.position);
            }
        }
    } //TODO: movement and idle particles

    public class WatcherPunch : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => false;

        private static Asset<Texture2D> tex;
        private static Asset<Texture2D> glowTex;

        public override void Load()
        {
            tex = Request<Texture2D>(Texture);
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        private readonly Color[] afterimageColors = [new Color(14, 14, 15), new Color(47, 41, 76), new Color(95, 69, 132), new Color(169, 98, 215)];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;

            Projectile.width = 12;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            //Projectile.extraUpdates = 1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (Projectile.ai[0] > 40) Projectile.ai[0] = 40;

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            Projectile.velocity *= 0.9f;

            if (++Projectile.frameCounter >= 1 + Projectile.ai[0] / 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }

            if (Projectile.frame > 5) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new(tex.Width() * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                Main.spriteBatch.Draw(glowTex.Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(tex.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), afterimageColors[k], Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(tex.Value, Projectile.position, new Microsoft.Xna.Framework.Rectangle?(tex.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            return true;
        }
    }
}
