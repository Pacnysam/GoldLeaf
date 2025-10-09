using GoldLeaf.Core;
using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Accessories;
using GoldLeaf.Items.VanillaBossDrops;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using static GoldLeaf.GoldLeaf;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Misc
{
    public class SentryDetonator : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 0, 25, 0);

            Item.useStyle = ItemUseStyleID.Thrust;
            Item.useTime = Item.useAnimation = 10;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.width = 30;
            Item.height = 34;
        }

        public override bool CanUseItem(Player player)
        {
            return !player.ZoneOldOneArmy;
        }

        public override bool? UseItem(Player player)
        {
            int sentryKillCount = 0;
            bool explosionEffectActivated = false;
            foreach (Projectile projectile in Main.ActiveProjectiles) 
            {
                if (projectile.owner == player.whoAmI && projectile.sentry)
                {
                    if (ProjectileSets.sentryCanDetonaterExplode[projectile.type])
                    {
                        explosionEffectActivated = true;
                        
                        /*int explosionSmokeCount = Main.rand.Next(8, 12);
                        for (int j = 0; j < explosionSmokeCount; j++)
                        {
                            var dust = Dust.NewDustDirect(projectile.Center, 0, 0, DustType<HotSmoke>());
                            dust.velocity = Main.rand.NextVector2Circular(7.5f, 7.5f) * projectile.scale;
                            dust.scale = Main.rand.NextFloat(0.9f, 1.8f);
                            dust.alpha = 60 + Main.rand.Next(40);
                            dust.rotation = Main.rand.NextFloat(6.28f);
                            dust.position += dust.velocity * 5;
                        }*/
                    }

                    if (Main.LocalPlayer == player && ProjectileSets.sentryCanDetonaterExplode[projectile.type])
                        Projectile.NewProjectile(projectile.GetSource_Death("SentryDetonator"), projectile.Center, Vector2.Zero, ProjectileType<SentryDetonatorExplosion>(), 0, 0, projectile.owner);

                    sentryKillCount++;
                    projectile.Kill();
                }
            }
            if (explosionEffectActivated)
            {
                SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.75f }, player.position);

                CameraSystem.QuickScreenShake(player.MountedCenter, null, 6.5f, 7.5f);
                CameraSystem.QuickScreenShake(player.MountedCenter, (0f).ToRotationVector2(), 12f, 12f);

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    ModPacket packet = Instance.GetPacket();
                    packet.Write((byte)MessageType.SentryDetonate);
                    packet.Write((byte)player.whoAmI);
                    packet.Send(-1, player.whoAmI);
                }
            }

            if (sentryKillCount > 0)
            {
                player.UpdateMaxTurrets();
            }

            if (Main.LocalPlayer == player && !Main.dedServ)
            {
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Monolith/BombClick") { Volume = 0.275f }, player.Center);
            }
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            spriteBatch.Draw(TextureAssets.Item[Type].Value, Item.Center + new Vector2(0, 7) - Main.screenPosition, null, lightColor, rotation, TextureAssets.Item[Type].Size() / 2f, scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class SentryDetonatorExplosion : ModProjectile
    {
        private static Asset<Texture2D> glowTex;
        private static Asset<Texture2D> alphaBloom;
        private static Asset<Texture2D> transparentBloom;
        public override void Load()
        {
            glowTex = Request<Texture2D>("GoldLeaf/Textures/BoomGlow");
            alphaBloom = Request<Texture2D>("GoldLeaf/Textures/GlowAlpha");
            transparentBloom = Request<Texture2D>("GoldLeaf/Textures/Glow0");
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = Projectile.height = 98;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 28;
        }

        public override bool? CanHitNPC(NPC target) => false;

        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.Kill();

                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);

            Main.EntitySpriteDraw(transparentBloom.Value, Projectile.Center - Main.screenPosition, null, Color.Black * 0.9f * (Projectile.timeLeft / 28f), 0, transparentBloom.Size() / 2, (Projectile.timeLeft / 20f * (Projectile.Counter() / 36f)) * 10f * Projectile.scale, SpriteEffects.None);
            for (int k = 1; k <= 3; k++)
            {
                Rectangle frame2 = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame - k);
                Color colorgrad = new Color (255, 184, 40).Lerp(new(187, 37, 16), k / 3f) with { A = 0 };
                Main.EntitySpriteDraw(glowTex.Value, Projectile.Center - Main.screenPosition, frame2, colorgrad * (0.5f - (k * 0.115f)) * Projectile.Opacity * 0.325f * (Projectile.timeLeft / 16f), Projectile.rotation, frame.Size() / 2, (1 + (k * (Projectile.timeLeft / 36f * (Projectile.Counter() / 20f)))) * Projectile.scale, SpriteEffects.None, 0f);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame2, colorgrad * (0.5f - (k * 0.115f)) * Projectile.Opacity * 0.45f * (Projectile.timeLeft / 16f), Projectile.rotation, frame.Size() / 2, (1 + (k * ((Projectile.timeLeft / 36f) * (Projectile.Counter() / 20f)))) * Projectile.scale, SpriteEffects.None, 0f);
            }
            Main.EntitySpriteDraw(glowTex.Value, Projectile.Center - Main.screenPosition, frame, new Color(255, 105, 25) { A = 0 } * Projectile.Opacity * 0.45f, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White * Projectile.Opacity, Projectile.rotation, frame.Size()/2, Projectile.scale, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(alphaBloom.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 173, 65) { A = 0 } * (Projectile.timeLeft / 16f) * 0.325f, 0, alphaBloom.Size() / 2, Projectile.timeLeft / 40f * 6f * Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
