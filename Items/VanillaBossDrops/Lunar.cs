using Terraria;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Effects.Dusts;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using GoldLeaf.Core;
using Mono.Cecil;
using Terraria.DataStructures;
using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.Enums;
using Terraria.Audio;
using GoldLeaf.Items.Underground;
using System.IO;
using Terraria.ModLoader.IO;
using Terraria.Chat;

namespace GoldLeaf.Items.VanillaBossDrops
{
    public class Lunar : ModItem
    {
        public override string Texture => "GoldLeaf/Items/VanillaBossDrops/LunarFull";

        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(2, 9) { NotActuallyAnimating = true });

            //ItemSets.Glowmask[Type] = (glowTex, ColorHelper.AdditiveWhite(), true);

            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 12;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 26;

            Item.useAmmo = AmmoID.FallenStar;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 25;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;

            Item.damage = 15;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.knockBack = 4.25f;
            Item.channel = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, 1, 0, 0);

            Item.shoot = ProjectileType<LunarP>();
            Item.shootSpeed = 14f;
            Item.GetGlobalItem<GoldLeafItem>().critDamageMod = 0.5f;
        }

        public override bool NeedsAmmo(Player player) => false;
        public override bool CanConsumeAmmo(Item ammo, Player player) => false;

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;

            if (Main.bloodMoon || Main.pumpkinMoon || Main.snowMoon)
                frame.Y = (texture.Height / 9 * 8);
            else
                frame.Y = texture.Height / 9 * Main.moonPhase;

            spriteBatch.Draw(texture, position, frame, Item.GetAlpha(drawColor), 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Rectangle frame = new(0, texture.Height / 9 * Main.moonPhase, texture.Width, texture.Height / 9);

            if (Main.bloodMoon || Main.pumpkinMoon || Main.snowMoon)
                frame.Y = (texture.Height / 9 * 8);

            Vector2 position = new(Item.position.X - Main.screenPosition.X + Item.width / 2, Item.position.Y - Main.screenPosition.Y + Item.height / 2);

            spriteBatch.Draw(texture, position, frame, Item.GetAlpha(lightColor), rotation, frame.Size() / 2, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glowTex.Value, position, frame, ColorHelper.AdditiveWhite(120) * 0.45f, rotation, frame.Size() / 2, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
    
    public class LunarP : ModProjectile
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 18f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 240f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 13.5f;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        const int MaxCharge = 60;
        public ref float MoonTimer => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;

            Projectile.aiStyle = ProjAIStyleID.Yoyo;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (!Main.dayTime)
                hitbox.Inflate(25, 25);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!Main.dayTime && (player.ZoneOverworldHeight || player.ZoneSkyHeight) && player.channel && player.HasItem(ItemID.FallenStar)) 
            {
                MoonTimer++;
                
                if (MoonTimer >= MaxCharge) 
                {
                    MoonTimer = 0;
                    //SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/StarSlot") { Variants = [1, 2, 3] }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.NPCDeath7, Projectile.Center);
                    player.ConsumeItem(ItemID.FallenStar);
                    Projectile.localAI[2] = 3f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Main.moonPhase = ++Main.moonPhase % 8;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.WorldData);
                        }
                    }
                }
            }
            else
            {
                MoonTimer--;
            }
            MoonTimer = Math.Clamp(MoonTimer, 0, MaxCharge);
        }

        public override void PostAI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.localAI[2] = MathHelper.Lerp(Projectile.localAI[2], 0f, 0.085f);

            //Projectile.alpha += (Main.dayTime ? -8 : 8);
            if (Projectile.ai[0] == -1f || Main.dayTime)
            {
                Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], 0f, 0.15f);
            }
            else
            {
                Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], 1f, 0.075f);
            }
            //Projectile.alpha = Math.Clamp(Projectile.alpha, 0, 255);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D moonTex = TextureAssets.Moon[Main.moonType].Value;
            
            if (Main.pumpkinMoon)
                moonTex = TextureAssets.PumpkinMoon.Value;
            else if (Main.snowMoon)
                moonTex = TextureAssets.SnowMoon.Value;

            Rectangle frame = new(0, (moonTex.Height / 8) * Main.moonPhase, moonTex.Width, moonTex.Height / 8);

            Main.EntitySpriteDraw(moonTex, Projectile.Center - Main.screenPosition, new(0, 0, moonTex.Width, moonTex.Height / 8), new Color(50, 50, 50) * Projectile.localAI[1] * 0.5f, ((float)Math.Sin(GoldLeafWorld.rottime * 5f) * 0.25f * Projectile.localAI[2]) + (-Projectile.velocity.X * 0.015f), frame.Size() / 2, Projectile.localAI[1] + ((float)Math.Sin(GoldLeafWorld.rottime) * 0.1f), SpriteEffects.None, 0f);
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Main.EntitySpriteDraw(glowTex.Value, Projectile.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite() * Projectile.Opacity, 0f, glowTex.Size() / 2, 1f, SpriteEffects.None, 0f);

            Texture2D moonTex = TextureAssets.Moon[Main.moonType].Value;

            Color color = ColorHelper.AdditiveWhite(180) * 0.4f;

            if (Main.bloodMoon)
                color = Color.IndianRed with { A = 180 };
            else if (Main.pumpkinMoon)
                moonTex = TextureAssets.PumpkinMoon.Value;
            else if (Main.snowMoon)
                moonTex = TextureAssets.SnowMoon.Value;

            Rectangle frame = new(0, (moonTex.Height / 8) * Main.moonPhase, moonTex.Width, moonTex.Height / 8);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + TextureAssets.Projectile[Projectile.type].Size() / 2;

                Main.EntitySpriteDraw(moonTex, drawPos, frame, color * (1f - (k / 5f)) * Projectile.localAI[1], ((float)Math.Sin(GoldLeafWorld.rottime * 5f) * 0.25f * Projectile.localAI[2]) + (-Projectile.velocity.X * 0.015f), frame.Size() / 2, Projectile.localAI[1] + ((float)Math.Sin(GoldLeafWorld.rottime) * 0.1f), SpriteEffects.None, 0f);
            }

            Main.EntitySpriteDraw(moonTex, Projectile.Center - Main.screenPosition, frame, color with { A = 0 } * Math.Clamp(Projectile.localAI[2] - 0.5f, 0f, 1f), -Projectile.velocity.X * 0.015f, frame.Size() / 2, 4f - Projectile.localAI[2], SpriteEffects.None, 0f);
        }
    }
}
