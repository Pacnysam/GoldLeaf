using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using Terraria.DataStructures;
using Terraria.Localization;

namespace GoldLeaf.Core
{
    /*internal abstract class PacketHandler
    {
        internal byte HandlerType { get; set; }

        public abstract void HandlePacket(BinaryReader reader, int fromWho);

        protected PacketHandler(byte handlerType)
        {
            HandlerType = handlerType;
        }

        protected ModPacket GetPacket(byte packetType, int fromWho)
        {
            var p = Mod.GetPacket();
            p.Write(HandlerType);
            p.Write(packetType);
            if (Main.netMode == NetmodeID.Server)
            {
                p.Write((byte)fromWho);
            }
            return p;
        }
    }*/
}
