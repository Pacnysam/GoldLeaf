using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Core;
using GoldLeaf.Items.Grove;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.DataStructures;
using GoldLeaf.Effects.Dusts;
using System;
using ReLogic.Content;
using GoldLeaf.Items.Grove.Boss;
using System.IO;
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Items.Nightshade;
using GoldLeaf.Core.CrossMod;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;

namespace GoldLeaf.Items.Ocean
{
    public class Jellyfisher : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

            //ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;
            Item.AddElements([Element.Water, Element.Thunder]);
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item87;
            //Item.shoot = ProjectileType<JellyfishSentry>();
            Item.DamageType = DamageClass.Summon;

            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.mana = 10;
            Item.damage = 13;
            Item.fishingPole = 0;

            Item.value = Item.sellPrice(0, 4, 15, 0);
            Item.rare = ItemRarityID.Blue;
            Item.knockBack = 2.5f;

            Item.width = 40;
            Item.height = 30;

            /*Item.CloneDefaults(ItemID.ReinforcedFishingPole);
            Item.fishingPole = 20;

            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 40;
            Item.height = 30;
            Item.knockBack = 2.5f;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 4, 15, 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = false;*/
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                if (player.ownedProjectileCounts[ProjectileID.BobberReinforced] > 0)
                    return false;
                /*if (player.ownedProjectileCounts[ProjectileType<JellyfishBobber>()] > 0)
                    return false;*/

                Item.UseSound = SoundID.Item87;
                //Item.shoot = ProjectileType<JellyfishSentry>();
                Item.DamageType = DamageClass.Summon;

                Item.useTime = Item.useAnimation = 30;

                Item.mana = 10;
                Item.damage = 13;
                Item.fishingPole = 0;

                Item.value = Item.sellPrice(0, 4, 15, 0);
                Item.rare = ItemRarityID.Blue;
                Item.knockBack = 2.5f;

                Item.width = 40;
                Item.height = 30;
            }
            else
            {
                Item.UseSound = SoundID.Item1;
                //Item.shoot = ProjectileType<JellyfishBobber>();

                Item.useTime = Item.useAnimation = 8;

                Item.mana = 0;
                Item.damage = 0;
                Item.fishingPole = 20;

                Item.value = Item.sellPrice(0, 4, 15, 0);
                Item.rare = ItemRarityID.Blue;
                Item.knockBack = 2.5f;

                Item.width = 40;
                Item.height = 30;
            }
            return base.CanUseItem(player);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2)
                position = Main.MouseWorld + new Vector2(0, Main.screenHeight / 2f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                for (float k = 0; k < MathHelper.TwoPi; k += MathHelper.TwoPi/ 40f)
                {
                    Dust dust = Dust.NewDustPerfect(Main.MouseWorld, DustID.Cloud, Vector2.One.RotatedBy(k) * 1.75f, 120, Color.White, 1.25f);
                    dust.fadeIn = 1f;
                    dust.noGravity = true;
                }
            }
            return true;
        }

        public override bool? UseItem(Player player)
        {
            Item.NetStateChanged();
            return base.UseItem(player);
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult = 0;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.ReinforcedFishingPole);
            recipe.AddIngredient(ItemID.Coral, 8);
            recipe.AddIngredient(ItemID.Seashell, 2);
            recipe.AddRecipeGroup("GoldLeaf:JellyfishBait");
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}