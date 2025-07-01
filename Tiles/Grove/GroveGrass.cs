using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using System;
using GoldLeaf.Effects.Dusts;
using GoldLeaf.Tiles.Grove;
using Terraria.DataStructures;
using Terraria.Audio;
using GoldLeaf.Core;
using ReLogic.Content;
using Terraria.GameContent;

namespace GoldLeaf.Tiles.Grove
{
    public class GroveGrassSeeds : ModItem 
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemSets.Glowmask[Type] = (TextureAssets.Item[Type], Color.White * 0.4f, true);
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.White;
            Item.placeStyle = 0;
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 0, 5, 0);

            ItemID.Sets.GrassSeeds[Item.type] = true;
            ItemID.Sets.AlsoABuildingItem[Item.type] = true;
        }

        /*public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            spriteBatch.Draw(TextureAssets.Item[Item.type].Value, Item.Center - Main.screenPosition, null, Color.White * 0.4f, rotation, TextureAssets.Item[Item.type].Value.Size() / 2, scale, SpriteEffects.None, 0f);
        }*/

        public override void PostUpdate()
        {
            if (!Main.dedServ)
                Lighting.AddLight(new Vector2(Item.position.X / 16f, Item.position.Y / 16f), new Vector3(190/255, 99/255, 37/255) * 0.8f);
        }

        public override bool? UseItem(Player player)
		{
			if (Main.myPlayer != player.whoAmI)
				return false;

			Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);

			if (tile.HasTile && (tile.TileType == TileID.Mud || tile.TileType == TileID.JungleGrass || tile.TileType == TileID.MushroomGrass) && player.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY, TileReachCheckSettings.Simple))
			{
				WorldGen.PlaceTile(Player.tileTargetX, Player.tileTargetY, TileType<GroveGrassT>(), forced: true);
				player.inventory[player.selectedItem].stack--;

				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendTileSquare(player.whoAmI, Player.tileTargetX, Player.tileTargetY);
			}

			return true;
		}
    }

    public class GroveGrassT : ModTile
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        private float glow = 0;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;

			Main.tileMerge[Type][TileID.Grass] = true;
			Main.tileMerge[TileID.Grass][Type] = true;

            TileID.Sets.JungleSpecial[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = TileID.Mud;

            TileID.Sets.Grass[Type] = true;
			TileID.Sets.Conversion.Grass[Type] = true;
            TileID.Sets.Conversion.JungleGrass[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;

            AddMapEntry(new Color(190, 99, 37));
            RegisterItemDrop(ItemID.MudBlock);

            DustType = DustType<AutumnGrass>();

            //soundType = SoundID.Grass;
            //soundStyle = 6;

            /*//SoundType = SoundID.Tink;
            //SoundStyle = 1;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            //Main.tileMerge[Type][TileType<GroveGrassT>()] = true;
            //Main.tileMerge[Type][TileType<GroveBrickT>()] = true;
            RegisterItemDrop(ItemType<Echoslate>());
            AddMapEntry(new Color(118, 108, 98));
            Main.tileBlockLight[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;*/
        }

        public override bool CanExplode(int i, int j)
        {
            WorldGen.KillTile(i, j, false, false, true);
            return true;
        }

        public override void RandomUpdate(int i, int j)
        {
            WorldGen.SpreadGrass(i+1, j, TileID.Mud, TileType<GroveGrassT>(), true); //bruh this sucks i needa redo this later on god
            WorldGen.SpreadGrass(i-1, j, TileID.Mud, TileType<GroveGrassT>(), true);
            WorldGen.SpreadGrass(i, j+1, TileID.Mud, TileType<GroveGrassT>(), true);
            WorldGen.SpreadGrass(i, j-1, TileID.Mud, TileType<GroveGrassT>(), true);
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

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.570f * 0.3f;
            g = 0.394f * 0.3f;
            b = 0.111f * 0.3f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GoldLeafTile.DrawSlopedGlowMask(i, j, glowTex.Value, Color.White * 0.4f, Vector2.Zero);
        }

        public override void FloorVisuals(Player player)
        {
            Vector2 playerFeet = player.Center + new Vector2(-8, player.height / 2);
            if (player.velocity.X != 0)
            {
                if (Main.rand.NextBool(6)) 
                {
                    Dust dust = Dust.NewDustDirect(playerFeet, 16, 1, DustType<AutumnGrass>(), 0, Main.rand.NextFloat(-0.05f, -0.3f), Scale: 0.9f);
                    if (dust.velocity.Y > 0)
                        dust.velocity.Y = 0;
                }
            }
        }

        /*public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            int height = tile.TileFrameY == 36 ? 18 : 16;
            //if (tile.Slope() == 0 && !tile.halfBrick())
            {
                Main.spriteBatch.Draw(Request<Texture2D>(Texture + "Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White * glow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

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
            
        }*/
    }
}