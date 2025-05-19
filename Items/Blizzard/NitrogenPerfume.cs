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
            FirstStrikePlayer.FirstStrikeEvent += NitrogenPerfumeEffect;
        }
        public override void Unload()
        {
            FirstStrikePlayer.FirstStrikeEvent -= NitrogenPerfumeEffect;
        }

        private void NitrogenPerfumeEffect(Player player, NPC target, NPC.HitModifiers hit)
        {
            if (player.GetModPlayer<NitrogenPerfumePlayer>().nitrogenPerfume)
            {
                hit.SetCrit();

                target.AddBuff(BuffID.Frozen, TimeToTicks(2));
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Frost") { Pitch = 0.45f, PitchVariance = 0.6f, Volume = 0.85f }, target.Center);

                //target.GetGlobalNPC<FrostNPC>().defrostTimer = 0;
                //FrostNPC.AddFrost(target, 8);
            }
        }
    }
}
