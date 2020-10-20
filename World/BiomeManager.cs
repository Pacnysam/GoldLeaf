using Microsoft.Xna.Framework;
using GoldLeaf.Items.Placeable;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.World
{
    public class BiomeManager : ModPlayer
    {
        public bool ZoneGrove = false;

        public bool FountainGrove = false;

        public override void UpdateBiomes()
        {
            ZoneGrove = (GoldleafWorld.groveTiles > 50);
        }

        public override bool CustomBiomesMatch(Player other)
        {
            BiomeManager modOther = other.GetModPlayer<BiomeManager>();
            bool allMatch = true;
            allMatch &= ZoneGrove == modOther.ZoneGrove;
            return allMatch;
        }

        public override void CopyCustomBiomesTo(Player other)
        {
            BiomeManager modOther = other.GetModPlayer<BiomeManager>();
            modOther.ZoneGrove = ZoneGrove;
        }

        public override void SendCustomBiomes(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = ZoneGrove;
            writer.Write(flags);
        }

        public override void ReceiveCustomBiomes(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            ZoneGrove = flags[0];
        }

        public override void PreUpdate()
        {
            if (ZoneGrove && Main.rand.Next(20) == 0)
            {
                Dust.NewDustPerfect(Main.screenPosition - Vector2.One * 100 + new Vector2(Main.rand.Next(Main.screenWidth + 200), Main.rand.Next(Main.screenHeight + 200)),
                DustType<Dusts.GroveDust>(), Vector2.Zero, 0, new Color(255, 249, 166) * 0.06f, 2);
            }

            if (ZoneGrove && Main.rand.Next(20) == 0)
            {
                Dust.NewDustPerfect(Main.screenPosition - Vector2.One * 100 + new Vector2(Main.rand.Next(Main.screenWidth + 200), Main.rand.Next(Main.screenHeight + 200)),
                DustType<Dusts.GroveDust2>(), Vector2.Zero, 0, new Color(255, 249, 166) * 0.06f, 2);
            }
        }
    }

    public partial class GoldleafWorld : ModWorld
    {
        public static int groveTiles;

        public override void TileCountsAvailable(int[] tileCounts)
        {
            groveTiles = tileCounts[TileType<Items.Placeable.GroveGrassT>()] + tileCounts[TileType<Items.Placeable.GroveStoneT>()];
        }

        public override void ResetNearbyTileEffects()
        {
            Main.LocalPlayer.GetModPlayer<BiomeManager>().FountainGrove = false;
        }
    }
}