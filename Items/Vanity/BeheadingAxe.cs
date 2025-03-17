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
namespace GoldLeaf.Items.Vanity
{
    //[AutoloadEquip(EquipType.Head)]
    public class BeheadingAxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 32;

            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Blue;

            Item.headSlot = /*Main.LocalPlayer.armor[0].Clone().headSlot; */ EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            Item.vanity = true;
        }

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
                EquipLoader.AddEquipTexture(Mod, EmptyTexString, EquipType.Head, this);
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            //Main.LocalPlayer.armor[0].headSlot
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
        }

        public override void UpdateVanity(Player player)
        {
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = true;
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
            ArmorIDs.Head.Sets.PreventBeardDraw[Item.headSlot] = false;
        }
    }

    public class BeheadedPlayer : ModPlayer 
    {
        public bool axed = false;
        public bool axeEquipped = false;

        public override void ResetEffects()
        {
            //axed = false;
            axeEquipped = Player.armor[0].type == ItemType<BeheadingAxe>() && Player.armor[10].type == ItemID.None || Player.armor[10].type == ItemType<BeheadingAxe>();
        }

        public override void PostUpdateEquips()
        {
            if (axeEquipped) 
            {
                if (!axed)
                {
                    for (int k = 0; k < 24; k++)
                    {
                        Dust.NewDustPerfect(new Vector2(Player.Center.X, Player.Top.Y + 22 + Main.rand.NextFloat(-1f, 2f)), DustID.Blood, new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-1, -3)), Scale: Main.rand.NextFloat(0.8f, 1.2f));
                    }

                    AddScreenshake(Player, 12);
                    SoundStyle sound1 = new("GoldLeaf/Sounds/SE/HollowKnight/MawlekExplode") { Volume = 1f, Pitch = 0.2f, PitchVariance = 0.4f };
                    SoundStyle sound2 = new("GoldLeaf/Sounds/SE/DeadCells/Crit") { Volume = 0.65f, Pitch = -0.2f, PitchVariance = 0.6f };
                    SoundEngine.PlaySound(sound1, Player.Center);
                    SoundEngine.PlaySound(sound2, Player.Center);
                    
                    //Gore.NewGorePerfect(null, new Vector2(Player.Center.X, Player.Top.Y + 22), new Vector2(Player.Center.X, Player.Top.Y + 22));
                    axed = true;
                }
            }
            else 
            { 
                axed = false; 
            }
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (axed)
            {
                drawInfo.hideEntirePlayer = true;
            }
        }
    }
}
