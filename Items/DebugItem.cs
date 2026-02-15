using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using GoldLeaf.Core;
using Terraria.Audio;
using System.Diagnostics.Metrics;
using GoldLeaf.Items.Pickups;
using Terraria.GameContent.Drawing;
using GoldLeaf.Items.Nightshade;
using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Ocean.Jellyfisher;

namespace GoldLeaf.Items
{
    public class DebugItem : ModItem
    {
        public int temp, temp2, temp3 = 0;
        public float tempFloat = 0.9f;
        public Vector2 tempVec2 = new();

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
            if (player.altFunctionUse != 2) //primary
            {
                //player.GetModPlayer<GoldLeafPlayer>().ScreenMoveTime = temp;
                //player.GetModPlayer<GoldLeafPlayer>().ScreenMoveHold = false;

                //player.GetModPlayer<CameraSystem>().ScreenMoveTarget = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                Dust.NewDustPerfect(Main.MouseWorld, DustType<JellyLightningDust>(), tempVec2);
            }
            else //secondary
            {
                //player.GetModPlayer<CameraSystem>().ScreenMoveTime = temp;
                //player.GetModPlayer<CameraSystem>().ScreenMoveHold = false;
                //player.GetModPlayer<CameraSystem>().ScreenMovePan = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                tempVec2 = Main.MouseWorld;
            }
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
}
