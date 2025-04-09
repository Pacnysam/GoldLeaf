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
    internal class AuroraDye : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorShaderData arouraDyeShaderData = new(Request<Effect>("GoldLeaf/Effects/AuroraDye"), "AuroraDyePass");
            //arouraDyeShaderData.UseColor(1, 1, 1).UseSecondaryColor(1, 1, 1);

            if (!Main.dedServ)
            {
                GameShaders.Armor.BindShader(
                    Item.type,
                    arouraDyeShaderData
                );
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
            recipe.Register();
        }
    }
}
