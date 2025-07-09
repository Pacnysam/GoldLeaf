using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Core;
using GoldLeaf.Effects.Dusts;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using GoldLeaf.Items.VanillaBossDrops;
using GoldLeaf.Items.Accessories;
using Terraria.DataStructures;
using Steamworks;

namespace GoldLeaf.Items.Misc
{
    public class RoyalCanister : ModItem
    {
        public override void SetStaticDefaults() //temp item, remove once weather rocket is added
        {
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.BloodMoonStarter);
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item155;

            Item.consumable = true;

            Item.width = 22;
            Item.height = 34;
        }

        public override bool CanUseItem(Player player) => !Main.slimeRain;

        public override bool? UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Main.StartSlimeRain();
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            return true;
        }
    }
}
