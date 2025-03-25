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
    public class UndergroveMusicBox : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(Item.createTile = TileType<UndergroveMusicBoxT>());
        }
    }

    internal class UndergroveMusicBoxT : BaseMusicBox
	{
        public UndergroveMusicBoxT() : base(ItemType<UndergroveMusicBox>()) { }
    }
}
