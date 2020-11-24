using System;
using System.IO;
using System.Linq;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Dusts;

namespace GoldLeaf.Items.Placeable
{
    public class GroveGrassT : ModTile
    {
        int GroveGrassDrawOffset = 0;
        int GroveGrassDrawOffDir = 1;
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][TileType<GroveStoneT>()] = true;
            Main.tileMerge[Type][TileType<GroveBrickT>()] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.Grass[Type] = true;
            AddMapEntry(new Color(190, 99, 37));
            Main.tileBrick[Type] = true;
            drop = ItemType<GroveStone>();
            minPick = 65;
            soundType = SoundID.Grass;
            soundStyle = 6;
        }

        public override bool CanExplode(int i, int j)
        {
            return true;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!effectOnly)
            {
                fail = true;
                Main.tile[i, j].type = (ushort)TileType<GroveStoneT>();
                WorldGen.SquareTileFrame(i, j, true);
            }
        }

        public override void RandomUpdate(int i, int j)
        {
            WorldGen.SpreadGrass(i, j, TileType<GroveStoneT>(), TileType<GroveGrassT>(), true, Main.tile[i, j].color());
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)   //light colors
        {
            r = 0.190f * 3f;
            g = 0.099f * 3f;
            b = 0.037f * 3f;
        }
        
        public override void FloorVisuals(Player player)
        {
            Vector2 playerFeet = player.Center + new Vector2(-8, player.height / 2);
            if (player.velocity.X != 0)
            {
                if (Main.rand.Next(6) == 0) Dust.NewDust(playerFeet, 16, 1, DustType<AutumnLeaves>(), 0, 0.6f, Scale: 1.2f);
            }
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen) zero = Vector2.Zero;
            if (GroveGrassDrawOffDir == 1)
            {
                if (GroveGrassDrawOffset >= 9) GroveGrassDrawOffDir = -1;
                GroveGrassDrawOffset++;
            }
            if (GroveGrassDrawOffDir == -1)
            {
                if (GroveGrassDrawOffset <= 4) GroveGrassDrawOffDir = 1;
                GroveGrassDrawOffset--;
            }
            int height = tile.frameY == 36 ? 18 : 16;
            Main.spriteBatch.Draw(mod.GetTexture("Items/Placeable/GroveGrassGlow"), new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.frameX, tile.frameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }

    public class GroveGrass : ModItem
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
            item.createTile = TileType<GroveGrassT>();
            item.maxStack = 999;
        }
    }
}