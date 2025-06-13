using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Effects.Dusts;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using GoldLeaf.Core;
using Mono.Cecil;
using Terraria.DataStructures;
using System;
using System.Diagnostics.Metrics;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;

namespace GoldLeaf.Items.Grove.Wood
{
    public class EchobarkSword : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ShadewoodSword);

            Item.damage = 13;
            Item.DamageType = DamageClass.Melee;

            Item.width = Item.height = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 5.25f;

            Item.value = Item.sellPrice(0, 0, 0, 25);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<Echobark>(), 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class EchobarkBow : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ShadewoodBow);

            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;

            Item.width = 16;
            Item.height = 32;
            Item.knockBack = 0.25f;
            Item.shootSpeed = 7f;

            Item.value = Item.sellPrice(0, 0, 0, 25);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<Echobark>(), 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class EchobarkHammer : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ShadewoodHammer);

            Item.damage = 9;
            Item.DamageType = DamageClass.Melee;

            Item.width = Item.height = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 5.75f;

            Item.hammer = 42;

            Item.value = Item.sellPrice(0, 0, 0, 25);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<Echobark>(), 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}