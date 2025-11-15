using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using static GoldLeaf.GoldLeaf;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core.Helpers
{
    public static class DrawHelper
    {
        public enum DrawContext
        {
            InWorld,
            UI
        }

        public static void StartBlendState(this SpriteBatch spriteBatch, BlendState blendState = null, DrawContext context = DrawContext.InWorld, 
            SpriteSortMode sortMode = SpriteSortMode.Deferred, Effect effect = null, bool end = true, SamplerState sampler = null, RasterizerState rasterizer = null) 
        {
            if (end)
                spriteBatch.End();

            Matrix viewMatrix = Main.GameViewMatrix.TransformationMatrix;

            switch (context)
            {
                case DrawContext.InWorld:
                    sampler = SamplerState.PointClamp;
                    viewMatrix = Main.GameViewMatrix.TransformationMatrix;
                    break;
                case DrawContext.UI:
                    sampler = SamplerState.LinearClamp;
                    viewMatrix = Main.UIScaleMatrix;
                    //rasterizer = RasterizerState.CullCounterClockwise;
                    break;
            }
            spriteBatch.Begin(sortMode, blendState ?? BlendState.AlphaBlend, sampler ?? Main.DefaultSamplerState, DepthStencilState.None, rasterizer ?? Main.Rasterizer, effect, viewMatrix);
        }

        public static void ResetBlendState(this SpriteBatch spriteBatch, DrawContext context = DrawContext.InWorld, bool end = true)
        {
            if (end)
                spriteBatch.End();

            SamplerState sampler = Main.DefaultSamplerState;
            Matrix viewMatrix = Main.GameViewMatrix.TransformationMatrix;
            RasterizerState rasterizer = RasterizerState.CullNone;

            switch (context)
            {
                case DrawContext.InWorld:
                    //sampler = SamplerState.LinearClamp;
                    viewMatrix = Main.GameViewMatrix.TransformationMatrix;
                    break;
                case DrawContext.UI:
                    sampler = SamplerState.LinearClamp;
                    viewMatrix = Main.UIScaleMatrix;
                    break;
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, Main.Rasterizer, null, viewMatrix);
        }

        public readonly static BlendState
            SubtractiveBlend = new()
            {
                ColorSourceBlend = Blend.SourceAlpha,
                ColorDestinationBlend = Blend.One,
                ColorBlendFunction = BlendFunction.ReverseSubtract,
                AlphaSourceBlend = Blend.SourceAlpha,
                AlphaDestinationBlend = Blend.One,
                AlphaBlendFunction = BlendFunction.ReverseSubtract
            };
    }
}
