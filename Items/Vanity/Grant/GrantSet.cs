using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Items.Grove;
using System.Diagnostics.Metrics;
using System;
using GoldLeaf.Effects.Dusts;
using Terraria.Graphics.Effects;
using Terraria.DataStructures;
using GoldLeaf.Tiles.Grove;
using ReLogic.Content;
using Terraria.Localization;

namespace GoldLeaf.Items.Vanity.Grant
{
    [AutoloadEquip(EquipType.Head)]
    public class GrantMask : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;

            Item.value = Item.buyPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Blue;

            Item.vanity = true;
        }

        public override void SetStaticDefaults()
        {
            ArmorSets.FaceMask[Item.headSlot] = true;
            ArmorIDs.Head.Sets.PreventBeardDraw[Item.headSlot] = false;
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }
    }
}
