using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using GoldLeaf.Core;
using ReLogic.Content;
using Terraria.ID;

namespace GoldLeaf.Effects.Dusts
{
    internal class EveDust : ModDust
    {
        public override void SetStaticDefaults()
        {
            UpdateType = 33;
        }

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 8, 8, 8);

            dust.alpha = 80;
            dust.velocity *= 0.7f;
            dust.velocity.Y += 1f;
        }
    }

    public class EveCrystalDust : ModDust
    {
        public override bool MidUpdate(Dust dust)
        {
            Tile tile = Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16];
            if (tile.HasTile && Main.tileSolid[tile.TileType])
            {
                dust.velocity *= -0.4f;
            }

            if (dust.velocity.Length() > 3f) dust.velocity = Vector2.Normalize(dust.velocity) * 3f;
            return true;
        }

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 8, 8, 8);

            //dust.fadeIn = 1.5f;
            dust.scale = 1f;
        }
    }
}
