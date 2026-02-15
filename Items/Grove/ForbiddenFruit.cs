using GoldLeaf.Core;
using GoldLeaf.Core.CrossMod;
using GoldLeaf.Items.Grove.Wood.Armor;
using GoldLeaf.Items.Potions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static GoldLeaf.Core.ColorHelper;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Grove
{
    public class ForbiddenFruit : ModItem
    {
        public LocalizedText HealDesc => this.GetLocalization(nameof(HealDesc));

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

            _ = HealDesc;

            ItemID.Sets.IsFood[Type] = true;
            ItemID.Sets.FoodParticleColors[Item.type] = [
                new Color(212, 66, 74),
                new Color(145, 110, 120),
                new Color(217, 93, 76),
                new Color(167, 68, 121)
            ];
        }

        const int QuickHealPriority = 125;
        const int BaseSicknessTime = 20;
        const int LifestealBuffTime = 10;

        public override void SetDefaults()
        {
            Item.DefaultToFood(24, 32, 0, 0, animationTime: 10);
            Item.potion = true;
            Item.healLife = QuickHealPriority;
            Item.rare = ItemRarityID.Orange;
        }

        public override void ModifyPotionDelay(Player player, ref int baseDelay) => baseDelay = TimeToTicks(BaseSicknessTime);

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tipLine = tooltips.Find(n => n.Name == "HealLife");

            if (tipLine != null)
            {
                tipLine.Text = HealDesc.Value;
            }
        }

        public override void UpdateInventory(Player player) => Item.healLife = QuickHealPriority;
        public override bool CanUseItem(Player player) => player.FindBuffIndex(BuffID.PotionSickness) == -1;
        public override bool? UseItem(Player player)
        {
            int heal = Item.healLife;
            Item.healLife = 0;
            player.AddBuff(BuffID.PotionSickness, (int)player.PotionDelayModifier.ApplyTo(TimeToTicks(BaseSicknessTime)));
            player.AddBuff(BuffType<VampirePotionBuff>(), TimeToTicks(LifestealBuffTime));
            return base.UseItem(player);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Gradient gradient = QuickGradient([new Color(185, 49, 122), new Color(210, 19, 61), new Color(235, 116, 79)]);
            Main.GetItemDrawFrame(Item.type, out Texture2D texture, out Rectangle frame);
            float timer = Item.timeSinceItemSpawned * 0.15f;

            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, gradient.GetColor(0.35f).Alpha(20) * (0.65f + (float)(Math.Sin(timer * 0.5f) * 0.15f) * 0.65f),
                rotation, frame.Size() / 2, scale * (1.125f + (float)(Math.Sin(timer * 0.5f) * 0.105f + 0.125f)), SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, gradient.GetColor(0.5f).Alpha(20) * (0.65f + (float)(Math.Sin(timer * 0.5f) * 0.15f) * 0.8f),
                rotation, frame.Size() / 2, scale * (1.1f + (float)(Math.Sin(timer * 0.5f) * 0.125f + 0.05f)), SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, gradient.GetColor(0.85f).Alpha(20) * (0.65f + (float)(-Math.Sin(timer) * 0.15f)),
                rotation, frame.Size() / 2, scale * (1.05f + (float)(Math.Sin(timer * 0.5f) * 0.15f)), SpriteEffects.None, 0f);
            return true;
        }
    }
}
