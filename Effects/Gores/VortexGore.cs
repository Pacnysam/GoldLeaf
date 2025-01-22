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
    public abstract class VortexGore : ModGore
    {
        float rotMult = 3f;

        public override string Texture => "GoldLeaf/Textures/Vortex1";

        public override void OnSpawn(Gore gore, IEntitySource source)
        {



            gore.numFrames = 1;
            gore.behindTiles = false;
            //gore.timeLeft = Gore.goreTime * 3;
            //gore.velocity = Vector2.Zero;
            ChildSafety.SafeGore[gore.type] = true;
        }

        public override bool Update(Gore gore)
        {
            gore.rotation = gore.timeLeft * rotMult;
            gore.alpha = gore.timeLeft * 3;
            gore.scale *= 1.04f;

            if (gore.alpha < 3)
            {
                gore.frameCounter = 0;
                gore.frame++;
                if (gore.frame > 7) gore.frame = 0;
            }
            return false;
        }
    }
}
