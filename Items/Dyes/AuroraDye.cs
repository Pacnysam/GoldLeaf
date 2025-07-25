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
    public class AuroraDye : ModItem
    {
        private static Asset<Texture2D> noiseTex;
        public override void Load()
        {
            noiseTex = Request<Texture2D>("GoldLeaf/Textures/Noise/SwirlNoiseBig");
        }

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                ArmorShaderData arouraDyeShaderData = new(Request<Effect>("GoldLeaf/Effects/AuroraDye"), "AuroraDyePass");
                GameShaders.Armor.BindShader(
                    Item.type,
                    arouraDyeShaderData
                ).UseImage(noiseTex).UseColor(0f, 255f / 255, 135f / 255).UseSecondaryColor(236f / 255, 90f / 255, 255f / 255);
            }

            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            int dye = Item.dye;

            Item.CloneDefaults(ItemID.AcidDye);

            Item.dye = dye;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ItemType<AuroraCluster>());
            recipe.AddTile(TileID.DyeVat);
            recipe.AddOnCraftCallback(RecipeCallbacks.DyeMinor);
            recipe.Register();
        }
    }
}
