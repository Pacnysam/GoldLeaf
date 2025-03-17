using System;
using static Terraria.ModLoader.ModContent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Items.Misc.Accessories;
using Terraria;
using GoldLeaf.Tiles.Grove;
using System.Threading.Channels;
using Terraria.ObjectData;
using Terraria.Enums;
using GoldLeaf.Items.Gem;
using Terraria.DataStructures;

namespace GoldLeaf.Core
{
    internal class GoldLeafTile : GlobalTile
    {
        private readonly int[] oxeyeSafeTiles = [TileID.Grass, TileID.JungleGrass, TileID.HallowedGrass, TileType<GroveGrassT>()];
        
        public override void RandomUpdate(int i, int j, int type)
        {
            Tile tile = Main.tile[i, j - 1];
            
            if (oxeyeSafeTiles.Contains(Framing.GetTileSafely(i, j).TileType) && j < Main.worldSurface && !TileID.Sets.Platforms[type] && Main.rand.NextBool(4200) && Main._shouldUseWindyDayMusic && !Main.tile[i, j].TopSlope && (Main.tileCut[tile.TileType] || TileID.Sets.BreakableWhenPlacing[tile.TileType] || !tile.HasTile) && !tile.IsActuated && Main.dayTime)
            {
                WorldGen.PlaceTile(i, j - 1, TileType<OxeyeDaisyT>(), false, false);
                NetMessage.SendObjectPlacement(-1, i, j - 1, TileType<OxeyeDaisyT>(), 0, 0, -1, -1);
            }
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem) 
        {
            if (TileID.Sets.CountsAsGemTree[type] && Main.tile[i, j - 1].TileType != type && Main.rand.NextBool(16)) 
            {
                Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, ItemType<Sediment>());
            }
        }

        /*public override bool ShakeTree(int x, int y, TreeTypes treeType) //apparently this doesnt support gemtrees yet so im just gonna keep the old stuff for now
        {
            if (treeType == TreeTypes.Custom && WorldGen.genRand.NextBool(10))
            {
                Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), x * 16, y * 16, 16, 16, ItemType<Sediment>());
                return true;
            }
            return false;
        }*/
    }
}
