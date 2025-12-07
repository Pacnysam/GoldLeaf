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
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using Terraria.ID;

namespace GoldLeaf.Core.PlayerLayers
{
    public class ArmorLongCoat : PlayerDrawLayer
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.ArmorLongCoat);
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            return (ItemSets.BodyExtra[player.armor[1].type].Item1 != null && player.armor[11].type == ItemID.None) || ItemSets.BodyExtra[player.armor[11].type].Item1 != null;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            if ((ItemSets.BodyExtra[player.armor[1].type].Item1 != null && player.armor[11].type == ItemID.None) || ItemSets.BodyExtra[player.armor[11].type].Item1 != null)
            {
                Texture2D tex;
                Color color;
                if (player.armor[11].type == ItemID.None)
                {
                    tex = ItemSets.BodyExtra[player.armor[1].type].Item1.Value;
                    color = (ItemSets.BodyExtra[player.armor[1].type].Item2 == default) ? drawInfo.colorArmorBody : ItemSets.BodyExtra[player.armor[1].type].Item2;
                }
                else
                { 
                    tex = ItemSets.BodyExtra[player.armor[11].type].Item1.Value;
                    color = (ItemSets.BodyExtra[player.armor[11].type].Item2 == default) ? drawInfo.colorArmorBody : ItemSets.BodyExtra[player.armor[11].type].Item2;
                }

                Rectangle playerFrame = ItemSets.BodyExtra[player.armor[1].type].Item3 ? player.legFrame : player.bodyFrame;
                int frame = playerFrame.Y / playerFrame.Height;
                int height = tex.Height / 20;

                Vector2 vector = new(-playerFrame.Width / 2 + drawInfo.drawPlayer.width / 2, drawInfo.drawPlayer.height - playerFrame.Height + 4);
                Vector2 position = (drawInfo.Position - Main.screenPosition + vector).Floor() + drawInfo.drawPlayer.bodyPosition + drawInfo.bodyVect;

                drawInfo.DrawDataCache.Add(new DrawData(tex, position, new Rectangle(0, frame * height, tex.Width, height),
                    color,
                    player.bodyRotation,
                    drawInfo.bodyVect,
                    1f, drawInfo.playerEffect, 0)
                {
                    shader = drawInfo.cBody
                });
            }
        }
    }
}
