using GoldLeaf.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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
				Action<PlayerDrawInfo> layerTarget = DrawGlowmasks;
				PlayerLayer layer = new PlayerLayer("GoldLeaf", "GoldLeaf Item Layer", layerTarget);
				layers.Insert(layers.IndexOf(layers.FirstOrDefault(n => n.Name == "Arms")), layer);

				if (info.drawPlayer.HeldItem.modItem is Items.IGlowingItem) (info.drawPlayer.HeldItem.modItem as Items.IGlowingItem).DrawGlowmask(info);
			}
		}
    }
}