﻿using Terraria;
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
using GoldLeaf.Items.Misc;
using Terraria.GameContent.Drawing;
using GoldLeaf.Items.Nightshade;

namespace GoldLeaf.Items
{
    public class DebugItem : ModItem
    {
        public int temp, temp2, temp3 = 0;

        public override string Texture => "GoldLeaf/Items/Blizzard/ArcticFlower";
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.staff[Item.type] = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.rare = ItemRarityID.Quest;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                //player.GetModPlayer<GoldLeafPlayer>().ScreenMoveTime = temp;
                //player.GetModPlayer<GoldLeafPlayer>().ScreenMoveHold = false;
                player.GetModPlayer<GoldLeafPlayer>().ScreenMoveTarget = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
            }
            else
            {
                player.GetModPlayer<GoldLeafPlayer>().ScreenMoveTime = temp;
                player.GetModPlayer<GoldLeafPlayer>().ScreenMoveHold = false;
                player.GetModPlayer<GoldLeafPlayer>().ScreenMovePan = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
            }

            

            //Vector3 coords = Helper.ScreenCoord(new Vector3(Main.MouseScreen.X, Main.MouseScreen.Y, 0));
            //Main.NewText("cursor coords: (" + coords.X + "," + coords.Y + ")");

            /*ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, (ParticleOrchestraType)temp,
                new ParticleOrchestraSettings { PositionInWorld = Main.MouseWorld },
                Main.LocalPlayer.whoAmI);
            */

            //Gore.NewGorePerfect(Terraria.Entity.GetSource_None(), Main.MouseWorld, Vector2.Zero, GoreType<RingGoreRewrite>(), Scale: 0.7f + Main.rand.NextFloat(temp1, temp2) / 30f);
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
}
