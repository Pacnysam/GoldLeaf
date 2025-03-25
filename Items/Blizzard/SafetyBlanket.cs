using System;
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
using GoldLeaf.Effects.Dusts;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using GoldLeaf.Items.Misc.Accessories;

namespace GoldLeaf.Items.Blizzard
{
    public class SafetyBlanket : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;

            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;

            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SafetyBlanketPlayer>().safetyBlanket = true;
            player.GetModPlayer<SafetyBlanketPlayer>().safetyBlanketItem = Item;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<FrostCloth>(), 6);
            recipe.AddIngredient(ItemID.PurificationPowder, 25);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    public class SafetyBlanketBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;

            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
        }
    }

    public class SafetyBlanketPlayer : ModPlayer
    {
        public bool safetyBlanket = false;
        public Item safetyBlanketItem;

        public override void ResetEffects()
        {
            safetyBlanket = false;
            safetyBlanketItem = null;
        }

        public override void Load()
        {
            //On_Player.AddBuff_DetermineBuffTimeToAdd += SafetyBuffTime;
            On_Player.AddBuff += SafetyBuff;
        }

        public override void Unload()
        {
            //On_Player.AddBuff_DetermineBuffTimeToAdd -= SafetyBuffTime;
            On_Player.AddBuff -= SafetyBuff;
        }

        /*private static int SafetyBuffTime(On_Player.orig_AddBuff_DetermineBuffTimeToAdd orig, Player self, int type, int time1)
        {
            int buffTime = orig(self, type, time1);

            if (type == BuffType<SafetyBlanketBuff>())
            {
                Main.NewText("Buff duration decided");
                return buffTime / 2;
            }
            return buffTime;
        }*/

        private static void SafetyBuff(On_Player.orig_AddBuff orig, Player self, int type, int timeToAdd, bool quiet, bool foodHack)
        {
            if (self.GetModPlayer<SafetyBlanketPlayer>().safetyBlanket && IsValidDebuff(type, timeToAdd) && !self.HasBuff(BuffType<SafetyBlanketBuff>()))
            {
                if (Main.myPlayer == self.whoAmI)
                {
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact with { Volume = 1.35f });
                    Projectile.NewProjectile(self.GetSource_Accessory(self.GetModPlayer<SafetyBlanketPlayer>().safetyBlanketItem), self.MountedCenter, Vector2.Zero, ProjectileType<SafetyBlanketEffect>(), 0, 0, self.whoAmI);
                }

                type = BuffType<SafetyBlanketBuff>();

                Main.NewText("Buff type changed");
            }
            orig(self, type, timeToAdd, quiet, foodHack);
        }
    }

    public class SafetyBlanketEffect : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BrainOfConfusion);

            Projectile.width = 24;
            Projectile.height = 28;

            Projectile.damage = 0;
        }
    }
}
