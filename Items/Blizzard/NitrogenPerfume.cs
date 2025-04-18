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
            //player.GetModPlayer<NitrogenPerfumePlayer>().NitrogenPerfume = true;
        }
    }
}
