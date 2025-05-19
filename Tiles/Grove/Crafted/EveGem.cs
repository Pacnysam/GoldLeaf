using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using GoldLeaf.Tiles.Grove.Ancient;
using Terraria.ObjectData;
using Terraria.Audio;
using GoldLeaf.Effects.Dusts;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using GoldLeaf.Items.Accessories;
using Terraria.GameContent;
using System;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Tiles.Marble;

namespace GoldLeaf.Tiles.Grove.Crafted
{
    public class EveGemItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<EveGem>());
            Item.value = Item.sellPrice(0, 0, 5, 0);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(20)
            .AddIngredient(ItemType<SplashGemItem>(), 20)
            .AddIngredient(ItemType<EveDroplet>())
            .AddTile(TileID.HeavyWorkBench)
            .Register();
        }
    }

    public class EveGem : ModTile
    {
        public override void SetStaticDefaults()
        {
            LocalizedText name = CreateMapEntryName();
            RegisterItemDrop(ItemType<EveGemItem>());
            AddMapEntry(new Color(243, 106, 74), name);

            HitSound = new SoundStyle("GoldLeaf/Sounds/SE/HollowKnight/JellyfishMiniDeath") { PitchVariance = 0.4f };
            DustType = DustType<EveCrystalDust>();

            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileMerge[TileID.Marble][Type] = false;

            TileID.Sets.CanPlaceNextToNonSolidTile[Type] = false;
            TileID.Sets.ChecksForMerge[Type] = false;
            TileID.Sets.NeedsGrassFramingDirt[Type] = TileID.Marble;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            if (item.type == ItemType<EveGemItem>())
            {
                for (float k = 0; k < 6.28f; k += 6.28f / 30)
                {
                    Dust dust = Dust.NewDustPerfect(new Vector2(i * 16 + 8, j * 16 + 8), DustID.BubbleBurst_Pink, Vector2.One.RotatedBy(k) * 0.9f, Scale: 1.5f);
                    dust.noGravity = true;
                }
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/HollowKnight/JellyfishEggPop") { PitchVariance = 0.4f }, new Vector2(i * 16, j * 16));
            }
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j - 1].LiquidType == LiquidID.Lava && Main.tile[i, j - 1].LiquidAmount > 0 && Main.rand.NextBool(60))
                Gore.NewGore(null, new Vector2(i * 16, j * 16 - 12), new Vector2(Main.rand.NextFloat(-0.8f, 0.8f), Main.rand.NextFloat(-0.4f, -1.2f)), GoreType<EveBubble>());
        }

        public override void FloorVisuals(Player player)
        {
            if (Main.rand.NextBool(10) && Math.Abs(player.velocity.X) > 2)
            {
                Gore.NewGore(null, player.Bottom, new Vector2(0f, Main.rand.NextFloat(-0.6f, -1f)), GoreType<EveBubble>());
                //SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/HollowKnight/JellyfishEggPop") { PitchVariance = 0.8f, MaxInstances = 2, Pitch = 1f, Volume = 0.5f }, player.MountedCenter);
            }
        }
    }
    public class EveBubble : SplashBubble
    {
        public override void SetStaticDefaults()
        {
            UpdateType = 417;
        }
    }
}