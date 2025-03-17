using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Items.Nightshade;
using Microsoft.Xna.Framework;
using GoldLeaf.Items.Pickups;
using GoldLeaf.Effects.Dusts;
using Microsoft.Build.Tasks;
using GoldLeaf.Tiles.Decor;
using Terraria.Enums;
using Terraria.ObjectData;
using GoldLeaf.Tiles.Grove;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.GameContent.Metadata;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria.GameContent;
using System;
using Terraria.GameContent.ObjectInteractions;

namespace GoldLeaf.Items
{
    public class ExclamationIcon : ModItem
    {
        public override string Texture => "GoldLeaf/Textures/Specific/Exclamation";

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Deprecated[Item.type] = true;
        }
    }

    public class LeafIcon : ModItem
    {
        public override string Texture => "GoldLeaf/icon_small";

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Deprecated[Item.type] = true;
        }
    }
}
