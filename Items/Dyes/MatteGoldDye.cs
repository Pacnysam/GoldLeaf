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
using ReLogic.Content;
using Terraria.Graphics.Effects;

namespace GoldLeaf.Items.Dyes
{
    internal class MatteGoldDye : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                GameShaders.Armor.BindShader(
                    Item.type,
                    new ArmorShaderData(Request<Effect>("GoldLeaf/Effects/BasicColorDye"), "BasicColorPass").UseColor(255/255f, 191/255f, 123/255f) 
                );
            }
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            int dye = Item.dye;

            Item.CloneDefaults(ItemID.BrownDye);

            Item.dye = dye;
        }
    }
}
