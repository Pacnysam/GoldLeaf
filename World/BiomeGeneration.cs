using Microsoft.Xna.Framework;
using System.Collections.Generic;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using GoldLeaf.Items.Placeable;

namespace GoldLeaf.World
{
    class BiomeGen : ModWorld
    {

        public override void ModifyWorldGenTasks(List<GenPass> tasks,  ref float totalWeight)
        {
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Temple"));
            if (ShiniesIndex != -1)
            {
                tasks.Insert(ShiniesIndex - 1, new PassLegacy("Planting the Grove", delegate (GenerationProgress progress)
                {
                    for (int i = 0; i < Main.maxTilesX / 500; ++i) //Repeats ~8 times on a small world (i.e. 8 biomes/world)
                          {
                        Point pos = new Point(WorldGen.genRand.Next(70, Main.maxTilesX - 70), WorldGen.genRand.Next((int)Main.worldSurface + 40, Main.maxTilesY - 800)); //Position of the biome
                              if (!AreaContains(pos, 60, TileID.BlueDungeonBrick) && !AreaContains(pos, 50, TileID.GreenDungeonBrick) && !AreaContains(pos, 50, TileID.PinkDungeonBrick) && !AreaContains(pos, 50, TileID.LihzahrdBrick) && !AreaContains(pos, 20, TileID.Ash)) //checks for unwanted blocks
                                  GenerateGrove(pos, WorldGen.genRand.Next(2)); 
                              else
                            i--; //Repeat until valid placement
                          }
                }));
            }
        }

        public void GenerateGrove(Point pos,  int size)
        {
            int[] types = new int[] { TileType<GroveGrassT>(), TileType<GroveStoneT>(), WallID.None, WallType<AutumnWallT>() };
            int reps = WorldGen.genRand.Next(4,  15); //number of holes
            int sizeCircle = WorldGen.genRand.Next(4,  20); //size of holes
            int[] randSiz = new int[] { -16,  16 }; //random offset
            if (size == 1) //Size 2
            {
                reps = WorldGen.genRand.Next(8,  12);
                sizeCircle = WorldGen.genRand.Next(6,  26);
                randSiz = new int[] { -24,  26 };
            }
            else if (size == 2) //Size 3
            {
                reps = WorldGen.genRand.Next(12,  18);
                sizeCircle = WorldGen.genRand.Next(7,  32);
                randSiz = new int[] { -30,  32 };
            }
            List<Point> centres = new List<Point>(); //List of centres
            for (int i = 0; i < reps; ++i) //Places actual holes/outlines
            {
                Point placePos = new Point(pos.X + WorldGen.genRand.Next(randSiz[0],  randSiz[1]),  pos.Y + WorldGen.genRand.Next(randSiz[0],  randSiz[1]));
                SmoothRunner(placePos,  sizeCircle + 24,  types[1],  types[2]);
                SmoothWallRunner(pos,  sizeCircle,  types[2]);
                centres.Add(placePos);
            }
            /*for (int i = 0; i < reps; ++i) //Places actual holes/outlines
            {
                Point placePos = new Point(pos.X + WorldGen.genRand.Next(randSiz[0], randSiz[1]), pos.Y + WorldGen.genRand.Next(randSiz[0], randSiz[1]));
                SmoothRunner(placePos, sizeCircle, types[0], types[2]);
                centres.Add(placePos);
                SmoothTunnel(placePos, sizeCircle - 1);
            }*/
            for (int i = 0; i < reps; ++i) //Places actual holes/outlines
            {
                Point placePos = centres[i];
                SmoothRunner(placePos, sizeCircle, types[0], types[2]);
                centres.Add(placePos);
                SmoothTunnel(placePos, sizeCircle - 1);
            }


            //Places walls
            for (int i = 0; i < (int)(reps *6); ++i)
            {
                Point placePos = new Point(pos.X + WorldGen.genRand.Next(randSiz[0], randSiz[1]), pos.Y + WorldGen.genRand.Next(randSiz[0], randSiz[1]));
                SmoothWallRunner(placePos,  WorldGen.genRand.Next(3,  8),  types[3]);
            }

            ShapeData shapeData = new ShapeData();
            WorldUtils.Gen(pos, new ModShapes.InnerOutline(shapeData), Actions.Chain(new Actions.SetTile((ushort)types[0]), new Actions.SetFrames(frameNeighbors: true)));
        }

        
        public void DirectSmoothRunner(Point position,  int size,  int type,  int wallID)
        {
            for (int x = position.X - size; x <= position.X + size; x++)
            {
                for (int y = position.Y - size; y <= position.Y + size; y++)
                {
                    if (Vector2.Distance(new Vector2(position.X,  position.Y),  new Vector2(x,  y)) <= size)
                    {
                        WorldGen.PlaceTile(x,  y,  type);
                        WorldGen.PlaceWall(x,  y,  wallID);
                    }
                }
            }
        }

        public void SmoothRunner(Point position,  int size,  int type,  int wallID) //Overrides blocks in a circle
        {
            for (int x = position.X - (int)(size); x <= position.X + (int)(size); x++)
            {
                for (int y = position.Y - size; y <= position.Y + size; y++)
                {
                    if (Vector2.Distance(new Vector2(position.X,  position.Y),  new Vector2(x,  y)) <= size)
                    {
                        Main.tile[x,  y].type = (ushort)type;
                        Main.tile[x,  y].wall = (ushort)wallID;
                    }
                }
            }
        }

        public void SmoothTunnel(Point pos,  int size) //Tunnels in a circle shape
        {
            for (int x = pos.X - (int)(size); x <= pos.X + (int)(size); x++)
            {
                for (int y = pos.Y - size; y <= pos.Y + size; y++)
                {
                    if (Vector2.Distance(new Vector2(pos.X,  pos.Y),  new Vector2(x,  y)) <= size)
                    {
                        WorldGen.KillTile(x,  y);
                    }
                }
            }
        }

        public int BlockLining(double x,  double y,  int repeats,  int tileType,  bool random,  int max,  int min = 3)
        {
            for (double i = x; i < x + repeats; i++)
            {
                if (random)
                {
                    for (double k = y; k < y + Main.rand.Next(min,  max); k++)
                    {
                        WorldGen.PlaceTile((int)i,  (int)k,  tileType);
                    }
                }
                else
                {
                    for (double k = y; k < y + max; k++)
                    {
                        WorldGen.PlaceTile((int)i,  (int)k,  tileType);
                    }
                }
            }
            return repeats;
        }


            public void SmoothWallRunner(Point position,  int size,  int wallID) //Walls...but in a circle
        {
            for (int x = position.X - size; x <= position.X + size; x++)
            {
                for (int y = position.Y - size; y <= position.Y + size; y++)
                {
                    if (Vector2.Distance(new Vector2(position.X,  position.Y),  new Vector2(x,  y)) <= size)
                    {
                        Main.tile[x,  y].wall = (ushort)wallID;
                    }
                }
            }
        }

        public bool ContainsWall(Point position, int size, int wallID) //Checks if a wall of a specific ID is within a radius
        {
            for (int xPos = position.X - size; xPos <= position.X + size; xPos++)
            {
                for (int yPos = position.Y - size; yPos <= position.Y + size; yPos++)
                {
                    if (Vector2.Distance(new Vector2(position.X, position.Y), new Vector2(xPos, yPos)) <= size && Main.tile[xPos, yPos].wall == wallID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool AreaContains(Point position,  int size,  int type) //Checks if a tile of a specific type is within a radius
        {
            for (int xPos = position.X - size; xPos <= position.X + size; xPos++)
            {
                for (int yPos = position.Y - size; yPos <= position.Y + size; yPos++)
                {
                    if (Vector2.Distance(new Vector2(position.X,  position.Y),  new Vector2(xPos,  yPos)) <= size && Main.tile[xPos,  yPos].type == type)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool AreaContains(Point position,  int size) //Same but if there is any active tile
        {
            for (int xPos = position.X - size; xPos <= position.X + size; xPos++)
            {
                for (int yPos = position.Y - size; yPos <= position.Y + size; yPos++)
                {
                    if (Vector2.Distance(new Vector2(position.X,  position.Y),  new Vector2(xPos,  yPos)) <= size && Main.tile[xPos,  yPos].active() == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void DebugPlacement(int x,  int y,  string debug = "Placed: ") //Debug
        {
            WorldGen.PlaceTile(x,  y,  TileID.Meteorite);
            ErrorLogger.Log(debug + " " + x + " " + y);
        }
    }
}

