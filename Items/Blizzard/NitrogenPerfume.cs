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
using GoldLeaf.Items.Blizzard.Armor;

namespace GoldLeaf.Items.Blizzard
{
    public class NitrogenPerfume : ModItem
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 38;

            Item.value = Item.sellPrice(0, 2, 80, 0);
            Item.rare = ItemRarityID.Orange;

            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<NitrogenPerfumePlayer>().nitrogenPerfume = true;
            player.GetModPlayer<NitrogenPerfumePlayer>().perfumeItem = Item;
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            float brightness = Lighting.Brightness((int)Item.Center.X / 16, (int)Item.Center.Y / 16);
            if (brightness <= 0.3f)
                spriteBatch.Draw(glowTex.Value, Item.Center - Main.screenPosition, null, ColorHelper.AdditiveWhite * (0.3f - brightness), rotation, glowTex.Size() / 2, scale, SpriteEffects.None, 0f);
        }
    }

    public class NitrogenPerfumePlayer : ModPlayer
    {
        public bool nitrogenPerfume = false;
        public Item perfumeItem;

        public override void ResetEffects()
        {
            nitrogenPerfume = false;
            perfumeItem = null;
        }

        public override void Load()
        {
            FirstStrikePlayer.OnFirstStrike += NitrogenPerfume;
            FirstStrikePlayer.ModifyFirstStrike += NitrogenPerfumeModifications;
        }
        public override void Unload()
        {
            FirstStrikePlayer.OnFirstStrike -= NitrogenPerfume;
            FirstStrikePlayer.ModifyFirstStrike -= NitrogenPerfumeModifications;
        }

        private void NitrogenPerfumeModifications(Player player, NPC target, NPC.HitModifiers hit)
        {
            if (player.GetModPlayer<NitrogenPerfumePlayer>().nitrogenPerfume && GoldLeafNPC.CanBeStunned(target))
            {
                hit.SetCrit();
            }
        }
        private void NitrogenPerfume(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (player.GetModPlayer<NitrogenPerfumePlayer>().nitrogenPerfume && GoldLeafNPC.CanBeStunned(target))
            {
                int smokeCount = Main.rand.Next(50, 75);
                for (int j = 0; j < smokeCount; j++)
                {
                    var dust = Dust.NewDustDirect(target.Center, 0, 0, DustType<SnowCloud>());
                    dust.velocity = Main.rand.NextVector2Circular((target.width / 6.5f), (target.height / 7.5f)) * Main.rand.NextFloat(0.5f, 1f);
                    dust.scale = Main.rand.NextFloat(0.65f, 1.15f);
                    dust.alpha = 10 + Main.rand.Next(20);
                    dust.rotation = Main.rand.NextFloat(6.28f);
                }

                target.AddBuff(BuffID.Frozen, TimeToTicks(2.5f));
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Frost") { Pitch = 0.45f, PitchVariance = 0.6f, Volume = 0.85f }, target.Center);

                //target.GetGlobalNPC<FrostNPC>().defrostTimer = 0;
                //FrostNPC.AddFrost(target, 8);
            }
        }
    }
}
