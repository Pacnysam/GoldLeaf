using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Placeable
{
    public class AutumnWallT : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = false;
            dustType = DustID.Grass;
            soundType = SoundID.Grass;
            AddMapEntry(new Color(190, 99, 37));
        }
        public static Vector2 TileAdj => Lighting.lightMode > 1 ? Vector2.Zero : Vector2.One * 12;
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (i > Main.screenPosition.X / 16 && i < Main.screenPosition.X / 16 + Main.screenWidth / 16 && j > Main.screenPosition.Y / 16 && j < Main.screenPosition.Y / 16 + Main.screenHeight / 16)
            {
                Texture2D tex = GetTexture("GoldLeaf/Items/Placeable/AutumnWallTFlow");

                float offset = i * j % 6.28f;
                float sin = (float)Math.Sin(GoldleafWorld.rottime + offset);
                spriteBatch.Draw(tex, (new Vector2(i + 0.5f, j + 0.5f) + TileAdj) * 16 + new Vector2(1, 0.5f) * sin * 1.2f - Main.screenPosition,
                    new Rectangle(i % 4 * 26, 0, 24, 24), Lighting.GetColor(i, j), offset + sin * 0.06f, new Vector2(12, 12), 1 + sin / 14f, 0, 0);
            }
        }
    }

    public class AutumnWall : ModItem
    {
        public override void SetDefaults()
        {
            item.consumable = true;
            item.width = 16;
            item.height = 16;
            item.value = 100;
            item.useStyle = 1;
            item.rare = 1;
            item.useTime = 10;
            item.useAnimation = 15;
            item.autoReuse = true;
            item.createWall = WallType<AutumnWallT>();
            item.maxStack = 999;
        }
    }
}