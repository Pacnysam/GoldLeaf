using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using GoldLeaf.Core;
using Microsoft.Xna.Framework;

namespace GoldLeaf.Effects.Gores
{
    public class WhirlpoolGore : ModGore
    {
        public override string Texture => "GoldLeaf/Textures/Vortex1";

        public override Color? GetAlpha(Gore gore, Color lightColor)
        {
            return new Color(42, 43, 152);
        }

        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            ChildSafety.SafeGore[gore.type] = true;
            gore.alpha = 255;
        }

        public override bool Update(Gore gore)
        {
            gore.rotation = GoldLeafWorld.rottime * -6;
            gore.scale *= 1.05f;
            gore.alpha -= 6;

            if (gore.alpha < 20) 
            {
                gore.active = false;
            }

            return false;
        }
    }
}
