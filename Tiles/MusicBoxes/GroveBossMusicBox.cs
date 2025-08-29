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
    public class GroveBossMusicBox : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(Item.createTile = TileType<GroveBossMusicBoxT>());
        }
    }

    internal class GroveBossMusicBoxT : BaseMusicBox
	{
        public GroveBossMusicBoxT() : base(ItemType<GroveBossMusicBox>(), "Sounds/Music/GroveBoss", true) { }
    }
}
