using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Items.FallenStar;
using GoldLeaf.Items.Blizzard.Armor;
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Items.FishWeapons;
using Terraria.Audio;
using GoldLeaf.Effects.Dusts;
using Microsoft.Xna.Framework;

namespace GoldLeaf
{
    public partial class GoldLeaf
    {
        internal enum MessageType : byte
        {
            OverhealthSync,
            ConstellationSync,
            FrostSync,
            SnapFreeze,
            ControlsPlayer,
            DoubleTapPacket,
            QuetzalRiftDetonate,
            SentryDetonate,
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType msgType = (MessageType)reader.ReadByte();
            byte player = reader.ReadByte();

            switch (msgType)
            {
                case MessageType.OverhealthSync:
                    OverhealthManager overhealthPlayer = Main.player[player].GetModPlayer<OverhealthManager>();
                    overhealthPlayer.ReceivePlayerSync(reader);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        overhealthPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MessageType.ConstellationSync:
                    
                    ConstellationPlayer starWhipPlayer = Main.player[player].GetModPlayer<ConstellationPlayer>();
                    starWhipPlayer.ReceivePlayerSync(reader);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        starWhipPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MessageType.FrostSync:
                    int frostNpcWhoAmI = reader.ReadByte();
                    int frost = reader.ReadByte();
                    int defrost = reader.ReadByte();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = Instance.GetPacket();
                        packet.Write((byte)MessageType.FrostSync);
                        packet.Write((byte)player);
                        packet.Write((byte)frostNpcWhoAmI);
                        packet.Write((byte)frost);
                        packet.Write((byte)defrost);
                        packet.Send(-1, player);
                    }
                    Main.npc[frostNpcWhoAmI].GetGlobalNPC<FrostNPC>().frost = frost;
                    Main.npc[frostNpcWhoAmI].GetGlobalNPC<FrostNPC>().defrostTimer = defrost;

                    break;
                case MessageType.SnapFreeze:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = Instance.GetPacket();
                        packet.Write((byte)MessageType.SnapFreeze);
                        packet.Write((byte)player);
                        packet.Send(-1, player);
                    }
                    else
                        SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Reflect") { Volume = 0.9f }, Main.player[player].Center);

                    float seed = Main.rand.NextFloat(0f, 8f);

                    for (float k = 0; k < 6.28f; k += (6.28f / 24))
                    {
                        Dust dust = Dust.NewDustPerfect(Main.player[player].MountedCenter, DustType<AuroraTwinkle>(), Vector2.One.RotatedBy(k) * 6f /* (float)((Math.Sin(k * 3)/2) + 1f)*/, 5, ColorHelper.AuroraAccentColor(seed + (k * 5.4f)), Main.rand.NextFloat(0.6f, 0.9f));
                        dust.rotation = Main.rand.NextFloat(-18f, 18f);
                        dust.noLight = true;
                        dust.customData = Main.player[player];
                    }
                    break;
                case MessageType.ControlsPlayer:
                    ControlsPlayer controlsPlayer = Main.player[player].GetModPlayer<ControlsPlayer>();
                    controlsPlayer.ReceivePlayerSync(reader);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        controlsPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;
                case MessageType.DoubleTapPacket:
                    int keyDir = reader.ReadByte();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = Instance.GetPacket();
                        packet.Write((byte)MessageType.DoubleTapPacket);
                        packet.Write((byte)player);
                        packet.Write((byte)keyDir);
                        packet.Send(-1, player);
                    }
                    Main.player[player].GetModPlayer<ControlsPlayer>().DoubleTap(Main.LocalPlayer, keyDir);
                    break;
                case MessageType.QuetzalRiftDetonate:
                    int quetzalProjWhoAmI = reader.ReadByte();
                    Projectile quetzalRift = Main.projectile[quetzalProjWhoAmI];

                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = Instance.GetPacket();
                        packet.Write((byte)MessageType.QuetzalRiftDetonate);
                        packet.Write((byte)player);
                        packet.Write((byte)quetzalProjWhoAmI);
                        packet.Send(-1, player);
                    }
                    if (quetzalRift.ModProjectile is QuetzalRift)
                    {
                        (quetzalRift.ModProjectile as QuetzalRift).Detonate();
                    }
                    break;
                case MessageType.SentryDetonate:
                    
                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = Instance.GetPacket();
                        packet.Write((byte)MessageType.SentryDetonate);
                        packet.Write((byte)player);
                        packet.Send(-1, player);
                    }
                    else 
                    {
                        SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.75f }, Main.player[player].position);

                        CameraSystem.QuickScreenShake(Main.player[player].MountedCenter, null, 20, 7.5f, 24, 1000);
                        CameraSystem.QuickScreenShake(Main.player[player].MountedCenter, (0f).ToRotationVector2(), 12.5f, 12f, 18, 1000);
                    }
                    break;
                default:
                    Logger.WarnFormat("GoldLeaf: Unknown Message type: {0}", msgType);
                    break;
            }
        }
    }
}
