using GoldLeaf.Items.Blizzard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using static GoldLeaf.GoldLeaf;

namespace GoldLeaf.Core
{
    public class ControlsPlayer : ModPlayer
    {
        public delegate void DoubleTapDelegate(Player player);
        public static event DoubleTapDelegate DoubleTapEvent;
        public static event DoubleTapDelegate DoubleTapPrimaryEvent;
        public static event DoubleTapDelegate DoubleTapSecondaryEvent;

        public bool rightClick;
        private bool oldMouseRight = false;
        public bool rightClickListener = false;

        public override void PreUpdate()
        {
            if (Main.myPlayer == Player.whoAmI)
            {
                rightClick = PlayerInput.Triggers.Current.MouseRight;
                if (rightClickListener && rightClick != oldMouseRight)
                {
                    oldMouseRight = rightClick;
                    rightClickListener = false;
                }
            }
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)GoldLeaf.MessageType.ControlsPlayer);
            packet.Write((byte)Player.whoAmI);
            packet.Write(rightClick);
            packet.Send(toWho, fromWho);
        }

        public void ReceivePlayerSync(BinaryReader reader)
        {
            rightClick = reader.ReadBoolean();
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            ControlsPlayer clone = (ControlsPlayer)targetCopy;
            clone.rightClick = rightClick;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            ControlsPlayer clone = (ControlsPlayer)clientPlayer;

            if (rightClick != clone.rightClick)
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }

        public void DoubleTap(Player player, int keyDir)
        {
            DoubleTapEvent?.Invoke(player);

            if ((Main.ReversedUpDownArmorSetBonuses && keyDir == 1) || (!Main.ReversedUpDownArmorSetBonuses && keyDir == 0))
                DoubleTapPrimaryEvent?.Invoke(player);

            if ((Main.ReversedUpDownArmorSetBonuses && keyDir == 0) || (!Main.ReversedUpDownArmorSetBonuses && keyDir == 1))
                DoubleTapSecondaryEvent?.Invoke(player);
        }

        public override void Load()
        {
            On_Player.KeyDoubleTap += DoubleTapKey;
        }
        public override void Unload()
        {
            On_Player.KeyDoubleTap -= DoubleTapKey;
        }

        private static void DoubleTapKey(On_Player.orig_KeyDoubleTap orig, Player self, int keyDir)
        {
            orig(self, keyDir);

            self.GetModPlayer<ControlsPlayer>().DoubleTap(self, keyDir);
            
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Instance.GetPacket();
                packet.Write((byte)MessageType.DoubleTapPacket);
                packet.Write((byte)self.whoAmI);
                packet.Write((byte)keyDir);
                packet.Send();
            }
        }
    }
}
