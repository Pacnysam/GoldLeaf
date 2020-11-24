using GoldLeaf.Items.Placeable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Tiles
{
    internal class GroveFlora : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.RandomStyleRange = 11;
            TileObjectData.newTile.AnchorAlternateTiles = new int[] { TileType<GroveGrassT>()};
            TileObjectData.addTile(Type);
            soundType = SoundID.Grass;
            dustType = DustType<Dusts.AutumnLeaves>();
            AddMapEntry(new Color(208, 158, 68));
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D texture = mod.GetTexture("Items/Placeable/GroveGrassShine");
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero + new Vector2(8, 8);

            int length = 10;
            float rotation = -(float)Math.PI / 2 + ((float)Math.Sin(SharedShine.shineCounter * (2 * (float)Math.PI) / 3 + ((i % 7) * 2 * (float)Math.PI) / 7) * (float)Math.PI / 4);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            for (int d = 0; d < length; d++)
            {
                spriteBatch.Draw(texture,
                    pos + new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * d * 2, //position
                    new Rectangle(0, 0, 2, 2), //source Rectangle
                    Color.Lerp(new Color(255, 255, 255, 255), new Color(0, 0, 0, 0), (float)d / (float)length), //transparency
                    rotation, //rotation
                    new Vector2(1, 1), //origin
                    3f, //scale
                    SpriteEffects.None, 0f);
            }
            return true;
        }
    }

    public class SharedShine : ModWorld
    {
        public static float shineAngle;
        public static float shineCounter;
        public static float shineLength;
        public override void PreUpdate()
        {
            shineCounter += (float)Math.PI / 240;
            shineAngle = -(float)Math.PI / 2 + ((float)Math.Sin(shineCounter) * (float)Math.PI / 4);
            shineLength = Math.Abs((float)Math.Sin(shineCounter * 1.5f));
        }
    }
}