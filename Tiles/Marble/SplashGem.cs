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

namespace GoldLeaf.Tiles.Marble
{
    public class SplashGemItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<SplashGem>());
            Item.value = Item.sellPrice(0, 0, 5, 0);
        }
    }

    public class SplashGem : ModTile
    {
        public override void SetStaticDefaults()
        {
            LocalizedText name = CreateMapEntryName();
            RegisterItemDrop(ItemType<SplashGemItem>());
            AddMapEntry(new Color(37, 176, 195), name);

            HitSound = new SoundStyle("GoldLeaf/Sounds/SE/HollowKnight/JellyfishMiniDeath") { PitchVariance = 0.4f };
            DustType = DustType<SplashGemDust>();

            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileMerge[TileID.Marble][Type] = true;

            TileID.Sets.CanPlaceNextToNonSolidTile[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = TileID.Marble;
        }

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            WorldGen.TileMergeAttempt(-2, TileID.Marble, ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            if (item.type == ItemType<SplashGemItem>())
            {
                for (float k = 0; k < 6.28f; k += 6.28f / 30)
                {
                    Dust dust = Dust.NewDustPerfect(new Vector2(i * 16 + 8, j * 16 + 8), DustID.BubbleBurst_Blue, Vector2.One.RotatedBy(k) * 0.9f, Scale: 1.5f);
                    dust.noGravity = true;
                }
                SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/HollowKnight/JellyfishEggPop") { PitchVariance = 0.4f }, new Vector2(i * 16, j * 16));
            }
        }

        /*public override bool CreateDust(int i, int j, ref int type)
        {
            Dust dust;
            Vector2 position = new(i * 16, j * 16);
            dust = Main.dust[Dust.NewDust(position, 16, 16, DustID.Stone, 0f, 0f, 0, new Color(255, 255, 255), 1f)];
            return true;
        }*/

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j - 1].LiquidType == LiquidID.Water && Main.tile[i, j - 1].LiquidAmount > 0 && Main.rand.NextBool(60))
                Gore.NewGore(null, new Vector2(i * 16, j * 16 - 12), new Vector2(Main.rand.NextFloat(-0.8f, 0.8f), Main.rand.NextFloat(-0.4f, -1.2f)), GoreType<SplashBubble>());
        }

        public override void FloorVisuals(Player player)
        {
            if (Main.rand.NextBool(10) && Math.Abs(player.velocity.X) > 2)
            {
                Gore.NewGore(null, player.Bottom, new Vector2(0f, Main.rand.NextFloat(-0.6f, -1f)), GoreType<SplashBubble>());
                //SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/HollowKnight/JellyfishEggPop") { PitchVariance = 0.8f, MaxInstances = 2, Pitch = 1f, Volume = 0.5f }, player.MountedCenter);
            }
        }
    }

    /*public class SplashFootstepPlayer : ModPlayer
    {
        public int splashFootstepTimer;

        public override void PostUpdateMiscEffects()
        {
            if(splashFootstepTimer > 0)
            {
                splashFootstepTimer--;
            }
        }
    }*/

    public class SplashBubble : ModGore
    {
        public override void SetStaticDefaults()
        {
            UpdateType = 412;
        }

        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.numFrames = 3;
            gore.frame = (byte)Main.rand.Next(3);

            gore.position -= new Vector2(9, 9) * gore.scale;
            ChildSafety.SafeGore[gore.type] = true;

            gore.velocity.Y *= Main.rand.Next(90, 150) * 0.01f;
            gore.velocity.X *= Main.rand.Next(40, 90) * 0.01f;
            gore.timeLeft = Main.rand.Next(120, 240);
            gore.sticky = true;
        }

        /*public override bool Update(Gore gore)
        {
            gore.alpha = 50;
            gore.velocity.X = (gore.velocity.X * 50f + Main.WindForVisuals * 2f + Main.rand.NextFloat(-10, 11) * 0.1f) / 51f;
            gore.velocity.Y = (gore.velocity.Y * 50f + -0.25f + Main.rand.NextFloat(-10, 11) * 0.2f) / 51f;
            gore.rotation = gore.velocity.X * 0.3f;
            if (TextureAssets.Gore[gore.type].IsLoaded)
            {
                Rectangle rectangle2 = new((int)gore.position.X, (int)gore.position.Y, (int)(12 * gore.scale), (int)(12 * gore.scale));
                for (int k = 0; k < 255; k++)
                {
                    if (Main.player[k].active && !Main.player[k].dead)
                    {
                        Rectangle value2 = new((int)Main.player[k].position.X, (int)Main.player[k].position.Y, Main.player[k].width, Main.player[k].height);
                        if (rectangle2.Intersects(value2))
                        {
                            gore.timeLeft = 0;
                        }
                    }
                }
                if (Collision.SolidCollision(gore.position, (int)(TextureAssets.Gore[gore.type].Width() * gore.scale), (int)(TextureAssets.Gore[gore.type].Height() * gore.scale)))
                {
                    gore.timeLeft = 0;
                }
            }
            if (gore.timeLeft > 0)
            {
                if (Main.rand.NextBool(2))
                {
                    gore.timeLeft--;
                }
                if (Main.rand.NextBool(50))
                {
                    gore.timeLeft -= 5;
                }
                if (Main.rand.NextBool(100))
                {
                    gore.timeLeft -= 10;
                }
            }
            else
            {
                gore.alpha = 255;
                if (TextureAssets.Gore[gore.type].IsLoaded)
                {
                    for (int l = 0; l < (18 * gore.scale * 0.8f); l++)
                    {
                        Dust dust = Dust.NewDustDirect(gore.position, (int)(18 * gore.scale), (int)(18 * gore.scale), DustID.BubbleBurst_Blue);
                        dust.noGravity = true;
                        dust.alpha = 100;
                        dust.scale = gore.scale;
                    }
                }
            }

            return true;
        }*/
    }
}