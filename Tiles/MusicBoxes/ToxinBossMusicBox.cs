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
    public class ToxinBossMusicBox : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(Item.createTile = TileType<ToxinBossMusicBoxT>());
        }
    }

    internal class ToxinBossMusicBoxT : BaseMusicBox
	{
        public ToxinBossMusicBoxT() : base(ItemType<ToxinBossMusicBox>(), "Sounds/Music/ToxinBoss", true) { }
    }
}
