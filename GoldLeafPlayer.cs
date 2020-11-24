using GoldLeaf.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace GoldLeaf
{
	public class GoldLeafPlayer : ModPlayer
    {
        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            void DrawGlowmasks(PlayerDrawInfo info)
            {
                if (info.drawPlayer.HeldItem.modItem is Items.IGlowingItem) (info.drawPlayer.HeldItem.modItem as Items.IGlowingItem).DrawGlowmask(info);
            }
        }
    }
}