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
using GoldLeaf.Items.Armor;
using Terraria.ID;

namespace GoldLeaf.Core.PlayerLayers
{
    public class FaceMaskLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.FaceAcc);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            return ItemSets.FaceMask[player.armor[0].type] && player.armor[10].type == ItemID.None || ItemSets.FaceMask[player.armor[10].type];
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            
            if (ItemSets.FaceMask[player.armor[0].type] && player.armor[10].type == ItemID.None || ItemSets.FaceMask[player.armor[10].type])
            {
                Texture2D tex;
                if (player.armor[10].type == ItemID.None)
                    tex = Request<Texture2D>(EquipLoader.GetEquipTexture(EquipType.Head, player.armor[0].headSlot).Texture).Value;
                else
                    tex = Request<Texture2D>(EquipLoader.GetEquipTexture(EquipType.Head, player.armor[10].headSlot).Texture).Value;

                int frame = player.bodyFrame.Y / player.bodyFrame.Height;
                int height = tex.Height / 20;

                Vector2 vector = new(-drawInfo.drawPlayer.bodyFrame.Width / 2 + drawInfo.drawPlayer.width / 2, drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4);
                Vector2 position = (drawInfo.Position - Main.screenPosition + vector).Floor() + drawInfo.drawPlayer.headPosition + drawInfo.headVect;
                
                drawInfo.DrawDataCache.Add(new DrawData(tex, position, new Rectangle(0, frame * height, tex.Width, height),
                    drawInfo.colorArmorHead,
                    player.headRotation,
                    drawInfo.headVect,
                    1f, drawInfo.playerEffect, 0)
                {
                    shader = drawInfo.cHead
                });
            }
        }
    }
}
