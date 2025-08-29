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
    public class ToxinGroveMusicBox : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(Item.createTile = TileType<ToxinGroveMusicBoxT>());
        }
    }

    internal class ToxinGroveMusicBoxT : BaseMusicBox
	{
        public ToxinGroveMusicBoxT() : base(ItemType<ToxinGroveMusicBox>(), "Sounds/Music/ToxinGrove", true) { }
    }
}
