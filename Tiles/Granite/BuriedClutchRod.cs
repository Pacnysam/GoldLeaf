using GoldLeaf.Core;
using GoldLeaf.Items.Granite;
using GoldLeaf.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Utilities;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Tiles.Granite
{
    public class BuriedClutchRod : ModTile
    {
        private static Asset<Texture2D> glowTex;
        private static Asset<Texture2D> backTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
            backTex = Request<Texture2D>("GoldLeaf/Textures/Masks/Mask4");
        }
        
        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(109, 73, 64), Language.GetText("Mods.GoldLeaf.Items.Granite.ClutchRod.DisplayName"));
            RegisterItemDrop(ItemType<ClutchRod>(), 0);

            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.FriendlyFairyCanLureTo[Type] = true;

            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileOreFinderPriority[Type] = 475;
            Main.tileShine2[Type] = true;

            TileObjectData.newTile.Origin = new Point16(0, 1);

            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.CoordinateWidth = 20;

            DustType = DustID.Granite;

            TileObjectData.addTile(Type);
        }

        public override bool IsTileSpelunkable(int i, int j) => true;

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        /*public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            {
                Color colour = Color.White * MathHelper.Lerp(0.2f, 1f, (float)((Math.Sin(SpiritMod.GlobalNoise.Noise(i * 0.2f, j * 0.2f) * 3f + Main.GlobalTimeWrappedHourly * 1.3f) + 1f) * 0.5f));

                Texture2D glow = ModContent.Request<Texture2D>("SpiritMod/Tiles/Furniture/NeonLights/BlueLightsLarge_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

                spriteBatch.Draw(glow, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), colour);
            }
        }*/

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            {
                if (TileDrawing.IsVisible(tile)) 
                {
                    Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                    spriteBatch.Draw(glowTex.Value, new Vector2((i * 16) -2, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
                }
            }

            /*Texture2D tex = glowTex.Value;
            Color color = ColorHelper.AdditiveWhite * (1 - (float)Math.Sin(GoldLeafWorld.rottime) * 0.5f);
            Vector2 position = new(i * 16, j * 16);

            for (float k = 0f; k < 1f; k += 1 / 3f)
            {
                Main.spriteBatch.Draw
                (
                    tex,
                    position + new Vector2(0f, 0.8f).RotatedBy((k + (GoldLeafWorld.rottime + i) / 4) * ((float)Math.PI * 2f) + (0.5f - (float)Math.Sin(GoldLeafWorld.rottime) * 0.5f)),
                    new Rectangle(0, 0, tex.Width, tex.Height),
                    color * 0.4f,
                    0,
                    tex.Size() * 0.5f,
                    1f,
                    SpriteEffects.None,
                    0f
                );
            }*/
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemType<ClutchRod>();
            player.noThrow = 2;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            {
                if (TileDrawing.IsVisible(tile) && tile.TileFrameY != 0 && Main.LocalPlayer.Distance(new Vector2(i, j).ToWorldCoordinates()) < 300)
                {
                    float alpha = Math.Clamp(1f - (Main.LocalPlayer.Distance(new Vector2(i, j).ToWorldCoordinates()) / 250f), 0, (float)((Math.Sin(GoldLeafWorld.rottime * 0.5f) / 4f) + 0.5f));
                    Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

                    spriteBatch.Draw(backTex.Value, new Vector2(i * 16 - (int)Main.screenPosition.X + 6, j * 16 - (int)Main.screenPosition.Y + 18) + zero, null, new Color(0, 192, 255){ A = 0 } * alpha, 0f, backTex.Size()/2, (float)(Math.Sin(GoldLeafWorld.rottime)/8f) + 0.5f, SpriteEffects.None, 0f);
                }
            }
            return true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];

            float alpha = Math.Clamp(1f - (Main.LocalPlayer.Distance(new Vector2(i, j).ToWorldCoordinates()) / 300f), -1f, (float)((Math.Sin(GoldLeafWorld.rottime * 0.5f) / 4f) + 0.3f));

            r = (0f / 255) * alpha;
            g = (192f / 255) * alpha;
            b = (255f / 255) * alpha;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer && !Main.gamePaused && Main.LocalPlayer.Distance(new Vector2(i, j).ToWorldCoordinates()) < 250 && TileDrawing.IsVisible(Framing.GetTileSafely(i, j)))
            {
                Vector2 position = new Vector2(i, j) * 16 + new Vector2(Main.rand.NextFloat(16f, 24f), 4);

                /*if (Main.rand.NextBool(16)) 
                {
                    var gore = Gore.NewGoreDirect(new EntitySource_TileUpdate(i, j), position, new Vector2(Main.rand.NextFloat(-0.6f, 0.6f), Main.rand.NextFloat(-0.6f, 0.6f)), GoreID.LightningBunnySparks, Main.rand.NextFloat(0.65f, 1.15f));
                    gore.alpha = Main.rand.Next(60, 120);
                    gore.position.X -= (gore.Width * gore.scale) / 2;
                    gore.position.Y += (gore.Height * gore.scale) / 2;
                }*/
                
                if (Main.rand.NextBool(3 + (int)(Main.LocalPlayer.Distance(new Vector2(i, j).ToWorldCoordinates())/50)))
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(i * 16, (j * 16) - 6), 0, 8, DustID.Electric, Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-1.5f, 1.5f), 0, Color.White, Main.rand.NextFloat(0.25f, 0.75f));
                    dust.noGravity = true;
                    dust.noLight = true;
                }
            }
        }

        public override bool RightClick(int i, int j)
        {
            WorldGen.KillTile(i, j);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendTileSquare(-1, i, j);
            return true;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            for (int k = 0; k < 12; k++) 
            {
                Dust dust = Dust.NewDustDirect(new Vector2(i, j) * 16, 16, 16, DustID.Electric, 0f, Main.rand.NextFloat(-2.4f, -6f), 0, Color.White, Main.rand.NextFloat(0.75f, 1.25f));
                dust.noGravity = true;
                dust.fadeIn = 0.8f;
                //dust.velocity.X *= 0.8f;
                dust.rotation = Main.rand.NextFloat(-8f, 8f);
            }

            RecipeSystem.LearnRecipie(GetInstance<ClutchRod>().Item);
        }
    }
    /*public class ClutchRodPlaceable : ModItem
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }
        public override void SetStaticDefaults()
        {
            ItemSets.Glowmask[Type] = (glowTex, ColorHelper.AdditiveWhite, true);
        }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 30, 0);
            Item.rare = ItemRarityID.White;

            Item.width = 16;
            Item.height = 28;

            Item.DefaultToPlaceableTile(TileType<BuriedClutchRod>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<ClutchRod>())
                .AddCondition(Condition.InGraveyard)
                .Register();
        }
    }*/
}