using System;
using static Terraria.ModLoader.ModContent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Items.Accessories;
using Terraria;
using GoldLeaf.Tiles.Grove;
using System.Threading.Channels;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.DataStructures;
using GoldLeaf.Items.Underground;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GoldLeaf.Items.Accessories;

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

        public override void Drop(int i, int j, int type)
        {
            if (TileID.Sets.CountsAsGemTree[type] && Main.rand.NextBool(10) && (!Main.tile[i - 1, j].HasTile && !Main.tile[i + 1, j].HasTile && !Main.tile[i, j - 1].HasTile))
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ItemType<Sediment>());
            }
        }

        /*public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (TileID.Sets.CountsAsGemTree[type] && Main.tile[i, j - 1].TileType != type && Main.rand.NextBool(16)) 
            {
                Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, ItemType<Sediment>());
            }
        }*/

        /*public override bool ShakeTree(int x, int y, TreeTypes treeType) //apparently this doesnt support gemtrees yet so im just gonna keep the old stuff for now
        {
            if (treeType == TreeTypes.Custom && WorldGen.genRand.NextBool(10))
            {
                Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), x * 16, y * 16, 16, 16, ItemType<Sediment>());
                return true;
            }
            return false;
        }*/

        //this is grabbed from spirit
        /// <summary>
		/// Code from Vortex, thanks for slogging through vanilla to get this!
		/// </summary>
        public static void DrawSlopedGlowMask(int i, int j, Texture2D texture, Color drawColor, Vector2 positionOffset, bool overrideFrame = false)
        {
            Tile tile = Main.tile[i, j];
            int frameX = tile.TileFrameX;
            int frameY = tile.TileFrameY;

            if (overrideFrame)
            {
                frameX = 0;
                frameY = 0;
            }

            int width = 16;
            int height = 16;
            Vector2 location = new Vector2(i * 16, j * 16);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 offsets = -Main.screenPosition + zero + positionOffset;
            Vector2 drawCoordinates = location + offsets;

            if ((tile.Slope == 0 && !tile.IsHalfBlock) || (Main.tileSolid[tile.TileType] && Main.tileSolidTop[tile.TileType])) //second one should be for platforms
                Main.spriteBatch.Draw(texture, drawCoordinates, new Rectangle(frameX, frameY, width, height), drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            else if (tile.IsHalfBlock)
                Main.spriteBatch.Draw(texture, new Vector2(drawCoordinates.X, drawCoordinates.Y + 8), new Rectangle(frameX, frameY, width, 8), drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            else
            {
                byte b = (byte)tile.Slope;
                Rectangle frame;
                Vector2 drawPos;

                if (b == 1 || b == 2)
                {
                    int length;
                    int height2;

                    for (int a = 0; a < 8; ++a)
                    {
                        if (b == 2)
                        {
                            length = 16 - a * 2 - 2;
                            height2 = 14 - a * 2;
                        }
                        else
                        {
                            length = a * 2;
                            height2 = 14 - length;
                        }

                        frame = new Rectangle(frameX + length, frameY, 2, height2);
                        drawPos = new Vector2(i * 16 + length, j * 16 + a * 2) + offsets;
                        Main.spriteBatch.Draw(texture, drawPos, frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                    }

                    frame = new Rectangle(frameX, frameY + 14, 16, 2);
                    drawPos = new Vector2(i * 16, j * 16 + 14) + offsets;
                    Main.spriteBatch.Draw(texture, drawPos, frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                }
                else
                {
                    int length;
                    int height2;

                    for (int a = 0; a < 8; ++a)
                    {
                        if (b == 3)
                        {
                            length = a * 2;
                            height2 = 16 - length;
                        }
                        else
                        {
                            length = 16 - a * 2 - 2;
                            height2 = 16 - a * 2;
                        }

                        frame = new Rectangle(frameX + length, frameY + 16 - height2, 2, height2);
                        drawPos = new Vector2(i * 16 + length, j * 16) + offsets;
                        Main.spriteBatch.Draw(texture, drawPos, frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                    }

                    drawPos = new Vector2(i * 16, j * 16) + offsets;
                    frame = new Rectangle(frameX, frameY, 16, 2);
                    Main.spriteBatch.Draw(texture, drawPos, frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                }
            }
        }
    }
}
