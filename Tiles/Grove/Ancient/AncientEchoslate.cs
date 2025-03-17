using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using GoldLeaf.Items.Blizzard.Armor;

namespace GoldLeaf.Tiles.Grove.Ancient
{
    public class AncientEchoslate : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<AncientEchoslateT>());
            Item.value = Item.sellPrice(0, 0, 0, 2);

            ItemID.Sets.ShimmerTransformToItem[Item.type] = ItemType<Echoslate>();
        }
    }

    public class AncientEchoslateT : ModTile
    {
        public override void SetStaticDefaults()
        {
            LocalizedText name = CreateMapEntryName();
            RegisterItemDrop(ItemType<AncientEchoslate>());
            AddMapEntry(new Color(108, 98, 88), name);

            HitSound = SoundID.Tink with { Pitch = -0.65f };

            Main.tileSolid[Type] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileBlockLight[Type] = true;

            TileID.Sets.JungleSpecial[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = TileID.Mud;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust dust;
            Vector2 position = new(i * 16, j * 16);
            dust = Main.dust[Dust.NewDust(position, 16, 16, DustID.Stone, 0f, 0f, 0, new Color(255, 255, 255), 1f)];
            return true;
        }

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            WorldGen.TileMergeAttempt(-2, TileID.Mud, ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }
    }
}