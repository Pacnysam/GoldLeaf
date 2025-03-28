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
using System;
using GoldLeaf.Effects.Dusts;
using Terraria.Graphics.Shaders;

namespace GoldLeaf.Items.Dyes
{
    internal class AuroraDye : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                GameShaders.Armor.BindShader(
                    Item.type,
                    GameShaders.Armor.GetSecondaryShader(1, Main.LocalPlayer).UseColor(ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 3f).R/255, ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 3f).G/255, ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 3f).B/255) 
                );
            }

            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            int dye = Item.dye;

            Item.CloneDefaults(ItemID.AcidDye);

            Item.dye = dye;
        }
    }
}
