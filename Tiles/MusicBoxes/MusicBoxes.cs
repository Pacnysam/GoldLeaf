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
    public class GroveMusicBox() : BaseMusicBox(TileType<GroveMusicBoxTile>()) { }
    public class GroveMusicBoxTile() : BaseMusicBoxTile(ItemType<GroveMusicBox>(), "Sounds/Music/WhisperingGrove", true) { }

    public class GroveBossMusicBox() : BaseMusicBox(TileType<GroveBossMusicBoxTile>()) { }
    public class GroveBossMusicBoxTile() : BaseMusicBoxTile(ItemType<GroveBossMusicBox>(), "Sounds/Music/GroveBoss", true) { }

    public class ChalcedonyMusicBox() : BaseMusicBox(TileType<ChalcedonyMusicBoxTile>()) { }
    public class ChalcedonyMusicBoxTile() : BaseMusicBoxTile(ItemType<ChalcedonyMusicBox>(), "Sounds/Music/ChalcedonyQuarry", true) { }

    /*public class WhisperingGalleryMusicBox() : BaseMusicBox(TileType<WhisperingGalleryMusicBoxTile>()) { }
    public class WhisperingGalleryMusicBoxTile() : BaseMusicBoxTile(ItemType<WhisperingGalleryMusicBox>(), "Sounds/Music/HeavensGate", true) { }*/
}
