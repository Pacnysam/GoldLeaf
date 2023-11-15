using GoldLeaf.Core;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace GoldLeaf.Effects.Gores.Base
{
    public class RingGore : ModGore
    {
        public float growthDrag = 0.95f;
        public float growthVelocity = 0.1f;
        
        public int fadeSpeed = 14;
        public int fadeAlpha = 225;

        public Color color;

        public float startingScale = 1;
        public float initialScale = 0.1f;

        int phase = 1;
        
        public override string Texture => "GoldLeaf/Textures/RingGlow0";
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            /*startingScale = gore.scale;
            gore.alpha = 0;
            gore.scale *= initialScale;
            gore.velocity *= 0f;*/
        }

        public override bool Update(Gore gore)
        {
            gore.scale *= 1f + growthVelocity;
            growthVelocity *= growthDrag;
            gore.alpha += fadeSpeed; //TODO, fix

            /*if (gore.scale < startingScale * 8)
            {
                gore.scale += growthVelocity;
                gore.alpha += fadeSpeed / 2;
            }
            else 
            {
                gore.scale *= growthDrag;
                gore.alpha += fadeSpeed;
            }
            
            gore.scale += growthVelocity;
            growthVelocity *= growthScale;
            gore.alpha += fadeSpeed;
            */

            if (gore.alpha >= fadeAlpha)
            {
                gore.active = false;
            }

            return true;
        }

        public override Color? GetAlpha(Gore gore, Color lightColor)
        {
            color = new Color(255, 255, 255, gore.alpha);
            return color;
        }
    }
}