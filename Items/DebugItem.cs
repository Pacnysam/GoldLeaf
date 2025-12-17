using GoldLeaf.Core;
using GoldLeaf.Core.Mechanics;
using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Nightshade;
using GoldLeaf.Items.Pickups;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items
{
    public class DebugItem : ModItem
    {
        public int temp, temp2, temp3 = 0;
        public float tempFloat = 0.9f;

        //public override string Texture => "GoldLeaf/";
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 52;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.staff[Item.type] = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.rare = ItemRarityID.Quest;
            Item.damage = 3;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                OverhealthManager.AddOverhealthPool(player, new VigorPool() { size = 5, timer = 60 });
            }
            else
            {
                
            }
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
}
