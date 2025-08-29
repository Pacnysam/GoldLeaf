using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace GoldLeaf.Tiles.MusicBoxes
{
    public class GroveMusicBox : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(Item.createTile = TileType<GroveMusicBoxT>());
        }
    }

    internal class GroveMusicBoxT : BaseMusicBox
	{
        public GroveMusicBoxT() : base(ItemType<GroveMusicBox>(), "Sounds/Music/WhisperingGrove", true) { }
    }
}
