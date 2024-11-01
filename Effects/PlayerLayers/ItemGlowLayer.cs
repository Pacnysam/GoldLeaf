using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace GoldLeaf.Effects
{
    internal class ItemGlowLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.ArmOverItem);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (player.active && !player.outOfRange && false)
            {
                Texture2D texture = TextureAssets.Item[player.inventory[player.selectedItem].type].Value;
                Vector2 offset = new (texture.Width / 2, 0);
                Vector2 itemLocation = player.itemLocation + new Vector2(0, texture.Height * 0.5f);

                Vector2 origin = new (-8, (texture.Height / 4) / 2);
                if (player.direction == -1)
                    origin = new Vector2(texture.Width + 8, (texture.Height / 4) / 2);

                SpriteEffects effect = player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                var drawPos = new Vector2((int)(itemLocation.X + offset.X), (int)(itemLocation.Y + offset.Y)) - Main.screenPosition;
                var source = new Rectangle(0, texture.Height / 4, texture.Width, texture.Height / 4);
                var drawData = new DrawData(texture, drawPos, source, player.HeldItem.GetAlpha(Color.White), player.itemRotation, origin, player.HeldItem.scale, effect, 0);
                drawInfo.DrawDataCache.Add(drawData);

                if (player.inventory[player.selectedItem].color != default)
                {
                    drawData = new DrawData(texture, drawPos, source, player.HeldItem.GetColor(Color.White), player.itemRotation, origin, player.HeldItem.scale, effect, 0);
                    drawInfo.DrawDataCache.Add(drawData);
                }
            }
        }
    }
}
