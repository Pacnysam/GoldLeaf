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
using GoldLeaf.Items.Accessories;
using Terraria.Audio;
using Terraria.DataStructures;
using System.Collections.Generic;
using GoldLeaf.Items.Vanity.Watcher;
using ReLogic.Content;
using Terraria.Localization;

namespace GoldLeaf.Items.Armor
{
    /*[AutoloadEquip(EquipType.Head)]
    public class DragonSkull : ModItem
    {
        private readonly float SentrySpeed = 20;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SentrySpeed);

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;

            Item.value = Item.buyPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Orange;

            Item.headSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            Item.defense = 9;
            //Item.vanity = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<MinionSpeedPlayer>().sentrySpeed += SentrySpeed / 100;
            player.GetModPlayer<DragonSkullPlayer>().dragonSkull = true;
        }

        public override void SetStaticDefaults()
        {
            ArmorSets.FaceMask[Item.headSlot] = true;
            ArmorIDs.Head.Sets.PreventBeardDraw[Item.headSlot] = false;
        }
    }

    public class DragonSkullPlayer : ModPlayer
    {
        public bool dragonSkull = false;

        public override void ResetEffects()
        {
            dragonSkull = false;
        }

        public override void PostUpdateEquips()
        {
            if (dragonSkull)
            {
                Player.maxTurrets += Player.maxMinions;
                Player.maxMinions = -1;
            }
        }
    }*/
}
