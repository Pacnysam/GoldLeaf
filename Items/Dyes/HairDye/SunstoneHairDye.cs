using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Items.Grove;
using System.Diagnostics.Metrics;
using System;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using GoldLeaf.Items.Blizzard;
using Terraria.GameContent.Dyes;

namespace GoldLeaf.Items.Dyes.HairDye
{
    public class SunstoneHairDye : ModItem
    {
        private static Asset<Texture2D> noiseTex;
        public override void Load()
        {
            noiseTex = Request<Texture2D>("GoldLeaf/Textures/Noise/Manifold");
        }

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                HairShaderData sunstoneHairDyeShaderData = new(Request<Effect>("GoldLeaf/Effects/SunstoneDye"), "SunstoneDyePass");

                GameShaders.Hair.BindShader(
                    Item.type,
                    sunstoneHairDyeShaderData
                ).UseImage(noiseTex).UseColor(255f / 255, 185f / 255, 67f / 255).UseSecondaryColor(30f / 255, 222f / 255, 139f / 255);
            }

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
        }
    }
}
