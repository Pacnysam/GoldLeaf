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

namespace GoldLeaf.Items.Dyes
{
    public class SunstoneDye : ModItem
    {
        private static Asset<Texture2D> noiseTex;
        public override void Load()
        {
            noiseTex = Request<Texture2D>("GoldLeaf/Textures/Noise/Manifold");
        }
        public override void SetStaticDefaults()
        {
            ArmorShaderData arouraDyeShaderData = new(Request<Effect>("GoldLeaf/Effects/SunstoneDye"), "SunstoneDyePass");

            if (!Main.dedServ)
            {
                GameShaders.Armor.BindShader(
                    Item.type,
                    arouraDyeShaderData
                ).UseImage(noiseTex).UseColor(255f/255, 185f/255, 67f/255).UseSecondaryColor(30f/255, 222f/255, 139f/255);
            }

            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            int dye = Item.dye;

            Item.CloneDefaults(ItemID.AcidDye);

            Item.dye = dye;
        }

        /*public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ItemType<Sunstone>());
            recipe.AddTile(TileID.DyeVat);
            recipe.Register();
        }*/
    }
}
