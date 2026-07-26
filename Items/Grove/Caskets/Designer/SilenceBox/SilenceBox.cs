using GoldLeaf.Tiles.MusicBoxes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Grove.Caskets.Designer.SilenceBox
{
    public class SilenceBox() : BaseMusicBox(TileType<SilenceBoxTile>()) { }
    public class SilenceBoxTile : BaseMusicBoxTile
	{
        public SilenceBoxTile() : base(ItemType<SilenceBox>(), "Sounds/Music/Silence", true) { }

        public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
        {
            return;
        }
    }
}
