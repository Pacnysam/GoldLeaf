using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using System;
using GoldLeaf.Effects.Dusts;

namespace GoldLeaf.Tiles.Grove
{
    public class GroveGrass : ModItem
    {
        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.width = 16;
            Item.height = 16;
            Item.value = 100;
            Item.useStyle = 1;
            Item.rare = 1;
            Item.autoReuse = true;
            Item.createTile = TileType<GroveGrassT>();
            Item.maxStack = 9999;
        }
    }

    public class GroveGrassSeeds : ModItem 
    {
        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Green;
            Item.placeStyle = 0;
            Item.width = 22;
            Item.height = 20;
            Item.value = Item.buyPrice(0, 0, 5, 0);
        }

        public override bool? UseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
                return false;

            Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
            if (tile.HasTile && tile.TileType == TileID.Mud ) //& player.IsWithinSnappngRangeToTile(Player.tileTargetX, Player.tileTargetY)
            {
                WorldGen.PlaceTile(Player.tileTargetX, Player.tileTargetY, TileType<GroveGrassT>(), forced: true);
                player.inventory[player.selectedItem].stack--;
            }

            return null;
        }
    }

    public class GroveGrassT : ModTile
    {
        int GroveGrassDrawOffset = 0;
        int GroveGrassDrawOffDir = 1;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][TileID.Mud] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.Grass[Type] = true;
            AddMapEntry(new Color(190, 99, 37));
            Main.tileBrick[Type] = true;
            RegisterItemDrop(ItemType<GroveStone>());
            //soundType = SoundID.Grass;
            //soundStyle = 6;

            /*//SoundType = SoundID.Tink;
            //SoundStyle = 1;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            //Main.tileMerge[Type][TileType<GroveGrassT>()] = true;
            //Main.tileMerge[Type][TileType<GroveBrickT>()] = true;
            RegisterItemDrop(ItemType<GroveStone>());
            AddMapEntry(new Color(118, 108, 98));
            Main.tileBlockLight[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;*/
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
                Main.tile[i, j].TileType = TileID.Mud;
                WorldGen.SquareTileFrame(i, j, true);
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)   //light colors
        {
            r = 0.570f;
            g = 0.394f;
            b = 0.111f;
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
            int height = tile.TileFrameY == 36 ? 18 : 16;
            Main.spriteBatch.Draw(Request<Texture2D>(Texture + "Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}