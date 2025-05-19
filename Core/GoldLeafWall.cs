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
using Terraria.GameContent;

namespace GoldLeaf.Core
{
    public class GoldLeafWall : GlobalWall
    {
        public static void DrawSimpleWall(int i, int j, Texture2D texture, Color drawColor, Vector2 positionOffset, bool overrideFrame = false)
        {
            Tile tile = Main.tile[i, j];
            int frameX = tile.WallFrameX;
            int frameY = tile.WallFrameY;

            if (overrideFrame)
            {
                frameX = 0;
                frameY = 0;
            }

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 offsets = -Main.screenPosition + zero;

            Main.spriteBatch.Draw(texture, new Vector2(i * 16, j * 16) + offsets, new Rectangle(frameX, frameY, 32, 32), drawColor, 0, new Vector2(8, 8), 1f, SpriteEffects.None, 0f);
        }
    }
}
