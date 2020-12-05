using Microsoft.Xna.Framework;
using System.Collections.Generic;
using static Terraria.ModLoader.ModContent;
using static Terraria.WorldGen;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using GoldLeaf.Items.Placeable;
using GoldLeaf.Tiles;
using System;
using Terraria.DataStructures;

namespace GoldLeaf.World
{
	class BiomeGen : ModWorld
	{
		int[] tile = new int[] { TileType<GroveGrassT>(), TileType<GroveStoneT>(), WallID.None, WallType<AutumnWallT>() };
		int[] ruintile = new int[] { TileType<GroveBrickT>(), WallType<GroveBrickWallT>(), WallType<GroveBrickLeafWallT>() };

		#region GenerateGrove
		#region LocateGrove
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Temple"));
			if (ShiniesIndex != -1)
			{
				tasks.Insert(ShiniesIndex - 1, new PassLegacy("Planting the Grove", delegate (GenerationProgress progress)
				{
					progress.Message = "Planting the Grove";
					bool success = false;
					int inset = 200;
					int spawnProtect = Main.maxTilesX >> 5;
					int spawnStart = (Main.maxTilesX >> 1) - (spawnProtect >> 1);
					int limit = Main.maxTilesX - spawnProtect - inset;
					int attempts = 0;
					int x = 0;
					int y = (int)WorldGen.worldSurface;
					while (!success)
					{
						attempts++;
						if (attempts > 1000)
						{
							success = true;
							continue;
						}
						x = WorldGen.genRand.Next(inset, limit);
						if (x > spawnStart)
							x += spawnProtect;
						y = (int)WorldGen.worldSurfaceLow;
						while (!Main.tile[x, y].active() && (double)y < Main.worldSurface)
						{
							y++;
						}
						if (Main.tile[x, y].type == TileID.Grass || Main.tile[x, y].type == TileID.Dirt)
						{
							y--;
							if (y > 150 && CanPlaceGrove(x, y))
							{
								success = true;
								continue;
							}
						}
					}
					PlaceGrove(x, y);
				}));
			}
		}

		static bool CanPlaceGrove(int x, int y)
		{
			for (int i = x - 64; i < x + 64; i++)
			{
				for (int j = y - 64; j < y + 64; j++)
				{
					int[] TileArray = { TileID.BlueDungeonBrick, TileID.GreenDungeonBrick, TileID.PinkDungeonBrick, TileID.Cloud, TileID.RainCloud,
						TileID.SnowBlock, TileID.JungleGrass, TileID.Sand, TileID.ClayBlock, TileID.FleshGrass, TileID.CorruptGrass, TileID.Ebonstone, TileID.Crimstone };
					for (int block = 0; block < TileArray.Length; block++)
					{
						if (Main.tile[i, j].type == (ushort)TileArray[block])
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		#endregion LocateGrove

		public void PlaceGrove(int x, int y)
		{
			#region shit
			int numHills = genRand.Next(1, 3);
			for (int j = 0; j < numHills; j++)
			{

			}

			int[] tile = new int[] { TileType<GroveGrassT>(), TileType<GroveStoneT>(), WallID.None, WallType<AutumnWallT>() };
			int[] ruintile = new int[] { TileType<GroveBrickT>(), WallType<GroveBrickWallT>(), WallType<GroveBrickLeafWallT>() };

			Point pos = new Point(x, y);
			int[] bottomPos = new int[] { x, 0 };

			//initial pit
			ReplaceTreeException(x, y, genRand.Next(200, 300), tile[1]);
			bool leftpit = false;
			int PitX;
			int PitY;
			if (Main.rand.Next(2) == 0)
			{
				leftpit = true;
			}

			if (leftpit)
			{
				PitX = x - Main.rand.Next(25, 50);
			}
			else
			{
				PitX = x + Main.rand.Next(25, 50);
			}

			int a = Main.rand.Next(12, 24);
			Point uwwu = new Point(PitX, y-a);

			SmoothRunner(uwwu, a, tile[1], tile[2]);

			for (PitY = y - (16+a); PitY < y + genRand.Next(30, 60); PitY++)
			{
				digTunnel(PitX, PitY, 0, 0, 1, 4, false);
				TileRunner(PitX, PitY, 21, 1, tile[1], false, 0f, 0f, false, true);
			}
			//tunnel off of pit
			int tunnellength = Main.rand.Next(80, 200);
			int TunnelEndX = 0;
			int addheight2 = genRand.Next(-18, 18);
			if (leftpit)
			{
				for (int TunnelX = PitX; TunnelX < PitX + tunnellength; TunnelX++)
				{
					digTunnel(TunnelX, PitY + addheight2, 0, 0 - addheight2, 1, 4, false);
					TileRunner(TunnelX, PitY + addheight2, 18, 1 - addheight2, tile[1], false, 0f, 0f, false, true);
					TunnelEndX = TunnelX;
				}
			}
			else
			{
				for (int TunnelX = PitX; TunnelX > PitX - tunnellength; TunnelX--)
				{
					digTunnel(TunnelX, PitY + addheight2, 0, 0 - addheight2, 1, 4, false);
					TileRunner(TunnelX, PitY + addheight2, 18, 1 - addheight2, tile[1], false, 0f, 0f, false, true);
					TunnelEndX = TunnelX;
				}
			}

			//More pits
			int TrapX;

			for (int TrapNum = 0; TrapNum < 10; TrapNum++)
			{
				if (leftpit)
				{
					TrapX = Main.rand.Next(PitX, PitX + tunnellength);
				}
				else
				{
					TrapX = Main.rand.Next(PitX - tunnellength, PitX);
				}
				for (int TrapY = PitY; TrapY < PitY + genRand.Next(10, 20); TrapY++)
				{
					digTunnel(TrapX, TrapY, 0, 0, 1, 3, false);
					TileRunner(TrapX, TrapY, 17, 1, tile[1], false, 0f, 0f, false, true);
				}
			}

			int uhhh = 0;
			int uwu = genRand.Next(5, 8);
			for (int j = 0; j < uwu; j++)
			{
				//Additional hole and tunnel
				int PittwoX = 0;
				int PittwoY = 0;

					for (PittwoY = PitY; PittwoY < PitY + 40; PittwoY++)
					{
						digTunnel(TunnelEndX, PittwoY, 0, 0 + (uhhh * genRand.Next(60, 80)), 1, 4, false);
						TileRunner(TunnelEndX, PittwoY, 11, 1 + (uhhh * genRand.Next(60, 80)), tile[1], false, 0f, 0f, false, true);
					}
					for (PittwoX = TunnelEndX - 50; PittwoX < TunnelEndX + 50; PittwoX++)
					{
						int addheight = genRand.Next(-12, 12);

						digTunnel(PittwoX, PittwoY + addheight, 0, 0 + (uhhh * genRand.Next(60, 80)), 1, genRand.Next(1, 6), false);
						TileRunner(PittwoX, PittwoY + addheight, 26, 1 + (uhhh * genRand.Next(60, 80)), tile[1], false, 0f, 0f, false, true);
					}
			}

			for (int ex = 0; ex < Main.maxTilesX; ex++) //Grows grass
			{
				for (int ey = 0; ey < Main.maxTilesY; ey++)
				{
					if (Main.tile[ex, ey].active())
					{
						if (Main.tile[ex, ey].type == tile[1] && (!Main.tile[ex, ey - 1].active() || !Main.tile[ex, ey + 1].active()))
						{
							SpreadGrass(ex, ey, tile[1], tile[0], repeat: true, 0);
						}

						if (Main.tile[ex, ey].type == tile[0] && !Main.tile[ex, ey - 1].active() && genRand.Next(5) >= 4)
						{
							PlaceTile(ex, ey - 1, TileType<GroveFlora>(), mute: true, style: genRand.Next(1, 11)); //places flora
						}
					}
				}
			}
			#endregion

			#region border placement
			int groveLeft = 0;
			int groveRight = 0;
			int groveBottom = 0;

			List<Circle> circles = new List<Circle>();

			for (int rx = 0; rx < Main.maxTilesX; rx++) //Find the biome borders
			{
				if (groveLeft != 0) break;

				for (int ry = 0; ry < Main.maxTilesY; ry++)
					if (Main.tile[rx, ry].type == tile[0] || Main.tile[rx, ry].type == tile[1])
					{
						groveLeft = rx;
						break;
					}
			}

			for (int rx = Main.maxTilesX - 1; rx > 0; rx--)
			{
				if (groveRight != 0) break;

				for (int ry = 0; ry < Main.maxTilesY; ry++)
					if (Main.tile[rx, ry].type == tile[0] || Main.tile[rx, ry].type == tile[1])
					{
						groveRight = rx;
						break;
					}
			}

			for (int rx = Main.maxTilesX - 1; rx > 0; rx--)
			{
				for (int ry = Main.maxTilesY - 1; ry > 0; ry--)
					if (Main.tile[rx, ry].type == tile[0] || Main.tile[rx, ry].type == tile[1])
					{
						groveBottom = ry;
						bottomPos[1] = groveBottom;
						break;
					}
			}
			#endregion border placement

			GenerateUndergrove(x, y, groveBottom);
		}
		#endregion GenerateGrove

		#region GenerateUnderGrove
		public void GenerateUndergrove(int x, int y, int topPos)
		{
			//uwu
		}
		#endregion GenerateUnderGrove

		#region OldUndergroveGeneration
		/*
		public override void ModifyWorldGenTasks(List<GenPass> tasks,  ref float totalWeight)
		{
			int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Temple"));
			if (ShiniesIndex != -1)
			{
				tasks.Insert(ShiniesIndex - 1, new PassLegacy("Planting the Undergrove", delegate (GenerationProgress progress)
				{
					for (int i = 0; i < Main.maxTilesX / 1200; ++i) //Repeats ~8 times on a small world (i.e. 8 biomes/world)
						  {
						Point pos = new Point(genRand.Next(180, Main.maxTilesX - 180), genRand.Next((int)Main.worldSurface + 40, Main.maxTilesY - 800)); //Position of the biome
							  if (!AreaContains(pos, 60, TileID.BlueDungeonBrick) && !AreaContains(pos, 50, TileID.GreenDungeonBrick) && !AreaContains(pos, 50, TileID.PinkDungeonBrick) && !AreaContains(pos, 50, TileID.LihzahrdBrick) && !AreaContains(pos, 20, TileID.Ash)) //checks for unwanted blocks
								  GenerateUndergroveOld(pos, genRand.Next(2)); 
							  else
							i--; //Repeat until valid placement
						  }
				}));
			}
		}

		public void GenerateUndergroveOld(Point pos, Point toppos)
		{
			int[] types = new int[] { TileType<GroveGrassT>(), TileType<GroveStoneT>(), WallID.None, WallType<AutumnWallT>() };
			int reps = genRand.Next(4, 7); //number of holes
			int sizeCircle = genRand.Next(6, 10); //size of holes
			int[] randSiz = new int[] { -42, 42 }; //random offset

			List<Point> centres = new List<Point>(); //List of centres

			for (int i = 0; i < reps; ++i) //Places holes
			{
				Point placePos = new Point(pos.X + genRand.Next(randSiz[0], randSiz[1]), pos.Y + genRand.Next(randSiz[0], randSiz[1]));
				SmoothTunnel(placePos, sizeCircle - genRand.Next(1, 3));
				centres.Add(placePos);
			}

			//Places walls
			for (int i = 0; i < (int)(reps *3); ++i)
			{
				Point placePos = new Point(pos.X + genRand.Next(randSiz[0], randSiz[1])*2, pos.Y + genRand.Next(randSiz[0], randSiz[1])*2);
				for (int v = 0; v < (int)(reps /3); ++v)
				{
					Point wallPos = new Point(placePos.X + genRand.Next(-6, 6), placePos.Y + genRand.Next(-6, 6));
					SmoothWallRunner(wallPos, genRand.Next(2, 7), types[3]);
				}
			}

			for (int x = 0; x < Main.maxTilesX; x++) //Grows grass
			{
				for (int y = 0; y < Main.maxTilesY; y++)
				{
					if (Main.tile[x, y].active())
					{
						SpreadGrass(x, y, types[1], types[0], repeat: true, 0);
						if (Main.tile[x, y].type == types[0] && !Main.tile[x, y - 1].active()&& genRand.Next(4) >= 3)
						{
							PlaceTile(x, y - 1, TileType<GroveFlora>(), mute: true, style: genRand.Next(1, 11)); //places flora
						}
					}
				}
			}
		}
		*/
		#endregion

		#region Methods
		public void DirectSmoothRunner(Point position,  int size,  int type,  int wallID)
        {
            for (int x = position.X - size; x <= position.X + size; x++)
            {
                for (int y = position.Y - size; y <= position.Y + size; y++)
                {
                    if (Vector2.Distance(new Vector2(position.X,  position.Y),  new Vector2(x,  y)) <= size)
                    {
                        PlaceTile(x,  y,  type);
                        PlaceWall(x,  y,  wallID);
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
                        KillTile(x,  y);
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
                        PlaceTile((int)i,  (int)k,  tileType);
                    }
                }
                else
                {
                    for (double k = y; k < y + max; k++)
                    {
                        PlaceTile((int)i,  (int)k,  tileType);
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

		public class Circle
		{
			public Point16 position;
			public int radius;
			public bool decorated = false;

			public Circle(Point16 position, int radius)
			{
				this.position = position;
				this.radius = radius;
			}

			public bool Colliding(Circle compare)
			{
				if (Vector2.DistanceSquared(position.ToVector2(), compare.position.ToVector2()) <= (float)Math.Pow(radius + compare.radius + 14, 2))
					return true;
				else return false;
			}

			public Circle Inflate(int amount) => new Circle(position, radius + amount);
		}

		private List<Vector4> GenerateLines(List<Circle> circles)
		{
			List<Vector4> output = new List<Vector4>();

			for (int k = 0; k < circles.Count; k++)
			{
				for (int j = k + 1; j < circles.Count; j++)
				{
					var line = new Vector4(circles[k].position.X, circles[k].position.Y, circles[j].position.X, circles[j].position.Y);
					var lineVector = new Vector2(line.Z - line.X, line.W - line.Y);

					var colliding = false;

					for (int n = 0; n < circles.Count; n++) //first, check against the circles
					{
						Circle check = circles[n].Inflate(11);
						if (n == k || n == j) continue;

						var off = check.position.ToVector2() - line.XY();

						var dot = Vector2.Dot(off, lineVector);
						var distAlong = dot / lineVector.LengthSquared();

						if (distAlong > 0 && distAlong < 1) //we've passed the check against the infinite line
						{
							float dist = Vector2.Distance(line.XY() + lineVector * distAlong, check.position.ToVector2());

							if (dist < check.radius)
							{
								colliding = true;
								break;
							}
						}
					}

					for (int n = 0; n < output.Count; n++) //next, check against the lines we already have to prevent intersections
					{
						var enemyLine = output[n];

						if (line == enemyLine) continue;
						if (line.XY() == enemyLine.XY() || line.XY() == enemyLine.ZW()) continue; //if lines share a point it's fine, because the intersection will be at a sphere
						if (line.ZW() == enemyLine.XY() || line.ZW() == enemyLine.ZW()) continue;

						if (SegmentsColliding(line, enemyLine))
						{
							colliding = true;
							break;
						}
					}

					var caveLength = lineVector.Length() - circles[k].radius - circles[j].radius;
					if (!colliding && caveLength < 80) //final check to add a max length
						output.Add(line);
				}
			}

			return output;
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
            PlaceTile(x,  y,  TileID.Meteorite);
            ErrorLogger.Log(debug + " " + x + " " + y);
        }

		//Zoinked from https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
		private bool SegmentsColliding(Vector4 line1, Vector4 line2)
		{
			var p1 = line1.XY();
			var q1 = line1.ZW();
			var p2 = line2.XY();
			var q2 = line2.ZW();

			int o1 = orientation(p1, q1, p2);
			int o2 = orientation(p1, q1, q2);
			int o3 = orientation(p2, q2, p1);
			int o4 = orientation(p2, q2, q1);

			if (o1 != o2 && o3 != o4)
				return true;

			if (o1 == 0 && onSegment(p1, p2, q1)) return true;
			if (o2 == 0 && onSegment(p1, q2, q1)) return true;
			if (o3 == 0 && onSegment(p2, p1, q2)) return true;
			if (o4 == 0 && onSegment(p2, q1, q2)) return true;

			return false;
		}

		static bool onSegment(Vector2 p, Vector2 q, Vector2 r)
		{
			if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
				q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y))
				return true;

			return false;
		}

		static int orientation(Vector2 p, Vector2 q, Vector2 r)
		{
			int val = (int)((q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y));
			if (val == 0) return 0;

			return (val > 0) ? 1 : 2;
		}

		public static void ReplaceTreeException(float posX, float posY, float radius = 3, int type = 0)
		{
			int xBegin = (int)(posX / 16f - radius);
			int xEnd = (int)(posX / 16f + radius);
			int yBegin = (int)(posY / 16f - radius);
			int yEnd = (int)(posY / 16f + radius);
			if (xBegin < 0)
			{
				xBegin = 0;
			}
			if (xEnd > Main.maxTilesX)
			{
				xEnd = Main.maxTilesX;
			}
			if (yBegin < 0)
			{
				yBegin = 0;
			}
			if (yEnd > Main.maxTilesY)
			{
				yEnd = Main.maxTilesY;
			}

			for (int x = xBegin; x <= xEnd; x++)
			{
				for (int y = yBegin; y <= yEnd; y++)
				{
					float deltaX = Math.Abs((float)x - posX / 16f);
					float deltaY = Math.Abs((float)y - posY / 16f);
					double dist = Math.Sqrt((double)(deltaX * deltaX + deltaY * deltaY));
					if (dist < (double)radius)
					{
						bool changeTile = true;
						if (Main.tile[x, y] != null && Main.tile[x, y].active())
						{
							changeTile = true;
							ushort tile = Main.tile[x, y].type;
							if (Main.tileDungeon[tile] || tile == TileID.LivingWood || tile == TileID.LeafBlock)
							{
								changeTile = false;
							}
							if (changeTile)
							{
								Main.tile[x, y].type = (ushort)type;
							}
						}
					}
				}
			}


		}
		#endregion
	}
}

