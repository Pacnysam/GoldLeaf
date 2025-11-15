using GoldLeaf.Core;
using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Vanity.Champion;
using GoldLeaf.Tiles.Grove;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

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
            ItemSets.FaceMask[Item.type] = true;
            ArmorIDs.Head.Sets.PreventBeardDraw[Item.headSlot] = false;
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }
    }
    [AutoloadEquip(EquipType.Body)]
    public class GrantCuffs : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 18;

            Item.value = Item.buyPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Blue;

            Item.vanity = true;
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class GrantPants : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 16;

            Item.value = Item.buyPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Blue;

            Item.vanity = true;
        }

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}_Female", EquipType.Legs, this, Name + "_Female");
            }
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            equipSlot = EquipLoader.GetEquipSlot(Mod, male ? Name : Name + "_Female", EquipType.Legs);
        }
    }
    [AutoloadEquip(EquipType.Front, EquipType.Back)]
    public class GrantCloak : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 32;

            Item.value = Item.buyPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Blue;

            Item.accessory = true;
            Item.vanity = true;
        }
        public override void SetStaticDefaults()
        {
            ArmorIDs.Front.Sets.DrawsInNeckLayer[Item.frontSlot] = false;
        }
    }
}