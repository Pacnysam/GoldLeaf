using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;

namespace GoldLeaf.Tiles.MusicBoxes
{
    public class SilenceMusicBox : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(Item.createTile = TileType<SilenceMusicBoxT>());
        }
    }

    internal class SilenceMusicBoxT : BaseMusicBox
	{
        public SilenceMusicBoxT() : base(ItemType<SilenceMusicBox>(), true) { }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            return;
        }

    }
}
