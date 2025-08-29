using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Core;
using GoldLeaf.Effects.Dusts;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using GoldLeaf.Items.VanillaBossDrops;
using GoldLeaf.Items.Accessories;
using Terraria.DataStructures;
using Steamworks;
using Terraria.Audio;
using static GoldLeaf.GoldLeaf;

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

                CameraSystem.QuickScreenShake(player.MountedCenter, null, 20, 7.5f, 24, 1000);
                CameraSystem.QuickScreenShake(player.MountedCenter, (0f).ToRotationVector2(), 12.5f, 12f, 18, 1000);

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
        }

        public override bool? CanHitNPC(NPC target) => false;

        public override void AI()
        {
            if (++Projectile.frameCounter >= 4)
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
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, ColorHelper.AdditiveWhite(180) * Projectile.Opacity, Projectile.rotation, frame.Size()/2, Projectile.scale, SpriteEffects.None, 0f);

            for (int k = 1; k <= 3; k++)
            {
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, ColorHelper.AdditiveWhite(0) * (0.5f - (k * 0.125f)) * Projectile.Opacity, Projectile.rotation, frame.Size() / 2, (1 + (k * 0.1f)) * Projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}
