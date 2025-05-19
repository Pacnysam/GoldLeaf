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

namespace GoldLeaf.Items.Blizzard
{
    public class SafetyBlanket : ModItem
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;

            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;

            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SafetyBlanketPlayer>().safetyBlanket = true;
            player.GetModPlayer<SafetyBlanketPlayer>().safetyBlanketItem = Item;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite, rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<FrostCloth>(), 6);
            recipe.AddIngredient(ItemID.PurificationPowder, 25);
            recipe.AddTile(TileID.Loom);
            recipe.AddOnCraftCallback(RecipeCallbacks.AuroraMajor);
            recipe.Register();
        }
    }

    public class SafetyBlanketBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;

            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
        }
    }

    public class SafetyBlanketPlayer : ModPlayer
    {
        public int safetyBuffImmuneType = -1;
        public int safetyBuffImmuneTime = 0;
        public bool safetyBlanket = false;
        public Item safetyBlanketItem;

        public override void ResetEffects()
        {
            safetyBlanket = false;
            safetyBlanketItem = null;
        }

        public override void PostUpdateBuffs()
        {
            if (safetyBuffImmuneTime > 0) 
            {
                Player.buffImmune[safetyBuffImmuneType] = true;
                safetyBuffImmuneTime--; 
            }
        }

        public override void Load()
        {
            On_Player.AddBuff += SafetyBuff;
        }

        public override void Unload()
        {
            On_Player.AddBuff -= SafetyBuff;
        }

        private static void SafetyBuff(On_Player.orig_AddBuff orig, Player self, int type, int timeToAdd, bool quiet, bool foodHack)
        {
            if (!self.dead && self.GetModPlayer<SafetyBlanketPlayer>().safetyBlanket && IsValidDebuff(type, timeToAdd) && !self.HasBuff(BuffType<SafetyBlanketBuff>()))
            {
                self.GetModPlayer<SafetyBlanketPlayer>().safetyBuffImmuneType = type;
                self.GetModPlayer<SafetyBlanketPlayer>().safetyBuffImmuneTime = Math.Clamp(timeToAdd / 3, 30, 120);

                self.ShadowDodge();
                self.shadowDodgeTimer = 90;

                if (Main.myPlayer == self.whoAmI)
                {
                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/HyperLightDrifter/Deflect") { Volume = 0.4f });
                    Projectile proj = Projectile.NewProjectileDirect(self.GetSource_Accessory(self.GetModPlayer<SafetyBlanketPlayer>().safetyBlanketItem), self.MountedCenter, new Vector2(0, -8.5f), ProjectileType<SafetyBlanketEffect>(), 0, 0, self.whoAmI);
                    proj.scale = 0.01f;
                }

                type = BuffType<SafetyBlanketBuff>();
                timeToAdd = Math.Clamp(timeToAdd, TimeToTicks(3), TimeToTicks(16));
            }
            orig(self, type, timeToAdd, quiet, foodHack);
        }
    }

    public class SafetyBlanketEffect : ModProjectile
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>("GoldLeaf/Textures/Flares/FlareSmall");
        }
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;

            Projectile.damage = 0;

            Projectile.scale = 0f;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.9f;

            if (Projectile.ai[1] < 20 && Projectile.timeLeft % 3 == 0)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustType<AuroraTwinkle>(), Vector2.Zero, 0, ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 15f), Main.rand.NextFloat(0.25f, 0.5f));
                dust.noLight = true;
                dust.velocity *= 0.2f;
                dust.rotation = Main.rand.NextFloat(-12, 12);

                //dust.customData = Projectile;
            }

            if (Projectile.scale >= 1.25f) Projectile.ai[0] = 1;

            if (Projectile.ai[0] == 0)
            {
                Projectile.scale += 0.075f;
                Projectile.ai[2] = Projectile.scale * 0.75f;
            } 
            else
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] >= 30)
                {
                    Projectile.scale -= Projectile.scale * 0.05f;
                    Projectile.alpha += 8;
                }
                else if (Projectile.ai[1] >= 10)
                {
                    Projectile.scale = 1;
                }
                else 
                {
                    Projectile.scale -= Projectile.scale * 0.025f;
                }
            }

            if (Projectile.alpha >= 255 || (Projectile.scale <= 0.01f && Projectile.ai[0] != 0)) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color color = ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 5f); color.A = 0;
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, color * Math.Clamp(Math.Abs((Projectile.velocity.Y)/2), 0, 1), Projectile.rotation, tex.Size() / 2, Projectile.scale * 1.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glowTex.Value, Projectile.Center - Main.screenPosition, null, color * Math.Clamp(Math.Abs((Projectile.velocity.Y)/2), 0, 1), Projectile.rotation, glowTex.Size() / 2, Math.Abs(Projectile.velocity.Y / 3), SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White * ((float)(255 - Projectile.alpha)/255), Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
