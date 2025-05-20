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

namespace GoldLeaf.Items.Vanity.Champion
{
    [AutoloadEquip(EquipType.Front, EquipType.Back)]
    public class ChampionsCloak : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 34;

            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;

            Item.accessory = true;
            Item.vanity = true;
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class DragonSkull : ModItem
    {
        public override LocalizedText Tooltip => null;
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;

            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;

            Item.vanity = true;
        }

        public override void SetStaticDefaults()
        {
            ArmorSets.FaceMask[Item.headSlot] = true;
            ArmorIDs.Head.Sets.PreventBeardDraw[Item.headSlot] = false;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class ChampionsBelt : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 14;

            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;

            Item.vanity = true;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class GymShorts : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 12;

            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;

            Item.vanity = true;
        }
    }
}
