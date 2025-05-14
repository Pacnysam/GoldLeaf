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
using GoldLeaf.Items.Misc.Accessories;
using ReLogic.Content;

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
            player.GetModPlayer<NitrogenPerfumePlayer>().NitrogenPerfume = true;
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
        public bool NitrogenPerfume = false;
        public Item perfumeItem;

        public override void ResetEffects()
        {
            NitrogenPerfume = false;
            perfumeItem = null;
        }
    }
}
