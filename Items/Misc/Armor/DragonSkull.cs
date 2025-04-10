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
using GoldLeaf.Items.Misc.Accessories;
using Terraria.Audio;
using Terraria.DataStructures;
using System.Collections.Generic;
using GoldLeaf.Items.Vanity.Watcher;
using ReLogic.Content;
using Terraria.Localization;

namespace GoldLeaf.Items.Misc.Armor
{
    [AutoloadEquip(EquipType.Head)]
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
            player.GetModPlayer<MinionSpeedPlayer>().sentrySpeed += SentrySpeed/100;
            player.GetModPlayer<DragonSkullPlayer>().dragonSkull = true;
        }

        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.PreventBeardDraw[Item.headSlot] = false;
        }
    }
    
    public class DragonSkullLayer : PlayerDrawLayer
    {
        private static Asset<Texture2D> tex;
        public override void Load()
        {
            tex = Request<Texture2D>("GoldLeaf/Items/Misc/Armor/DragonSkull_Head");
        }

        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.FaceAcc);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo) //taken from slr
        {
            Player player = drawInfo.drawPlayer;

            if (player.armor[0].type == ItemType<DragonSkull>() && player.armor[10].type == ItemID.None || player.armor[10].type == ItemType<DragonSkull>())
            {
                int frame = player.bodyFrame.Y / player.bodyFrame.Height;
                int height = tex.Height() / 20;

                Vector2 pos = (player.MountedCenter - Main.screenPosition + new Vector2(0, player.gfxOffY - 3)).ToPoint16().ToVector2() + player.headPosition;

                drawInfo.DrawDataCache.Add(new DrawData(tex.Value, pos, new Rectangle(0, frame * height, tex.Width(), height),
                    drawInfo.colorArmorHead,
                    player.headRotation,
                    new Vector2(tex.Width() * 0.5f, tex.Height() * 0.025f),
                    1f, drawInfo.playerEffect, 0)
                {
                    shader = drawInfo.cHead
                });
            }
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
    }
}
