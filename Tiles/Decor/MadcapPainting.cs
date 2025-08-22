using GoldLeaf.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static System.Net.Mime.MediaTypeNames;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Tiles.Decor
{
	public class MadcapPainting : ModItem
	{
        private static Asset<Texture2D> hoverTex;
        public override void Load()
        {
            hoverTex = Request<Texture2D>(Texture + "Hover");
        }

        public override void SetDefaults() 
		{
			Item.width = 40;
			Item.height = 40;

            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Purple;

			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;

			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			
			Item.createTile = TileType<MadcapPaintingT>();

            Item.value = Item.buyPrice(0, 5, 0, 0);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.IndexOf(tooltips.Find(n => n.Name == "ItemName"));
            tooltips.Insert(index + 1, new TooltipLine(Mod, "Painting" //this is awful but i dont know a better way to do this
                , "                                         \n\n\n\n\n\n\n\n\n\n"));
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name == "ItemName" && line.Mod == "Terraria")
            {
				line.Y -= 2;
                yOffset = -8;
            }
            else if (line.Name == "Painting" && line.Mod == "GoldLeaf")
            {
                Main.spriteBatch.Draw(hoverTex.Value, new Vector2(line.X, line.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				yOffset = -18;
				return false;
            }
			else
			{
				yOffset = 0;
			}
            return true;
        }
    }
	
	public class MadcapPaintingT : ModTile
	{
		public override void SetStaticDefaults()
		{
            LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(99, 50, 30), name);

			RegisterItemDrop(ItemType<MadcapPainting>());

			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Height = 6;
			TileObjectData.newTile.Width = 6;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16,
				16,
				16
			};
			TileObjectData.newTile.AnchorBottom = default;
			TileObjectData.newTile.AnchorTop = default;
			TileObjectData.newTile.AnchorWall = true;
			TileObjectData.addTile(Type);
		}
	}
}