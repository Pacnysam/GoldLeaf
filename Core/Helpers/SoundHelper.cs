using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.WorldGen;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.GoldLeaf;

namespace GoldLeaf.Core.Helpers
{
    internal class SoundHelper
    {
        public static void SendSound(string soundLocation, Vector2 position, float volume = 1f, float pitch = 0f)
        {
            if (Main.netMode != NetmodeID.SinglePlayer) 
            {
                ModPacket packet = Instance.GetPacket();
                packet.Write((byte)MessageType.SoundSync);
                packet.Write((byte)Main.LocalPlayer.whoAmI);
                packet.Write(soundLocation);
                packet.WriteVector2(position);
                packet.Write(volume);
                packet.Write(pitch);
                packet.Send();
            }
        }
    }
}
