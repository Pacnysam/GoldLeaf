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
                new Color(204, 100, 140),
                new Color(100, 129, 133),
                new Color(153, 73, 132)
            ];
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(24, 32, 0, 0, animationTime: 10);
            Item.potion = true;
            Item.healLife = 125;
            Item.rare = ItemRarityID.Orange;
        }

        public override void ModifyPotionDelay(Player player, ref int baseDelay)
        {
            baseDelay = TimeToTicks(20);
        }

        public override void GetHealLife(Player player, bool quickHeal, ref int healValue)
        {
            healValue = 0;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Main.GetItemDrawFrame(Item.type, out Texture2D texture, out Rectangle frame);
            Color color = new(0, 189, 161) { A = 40 };
            float timer = Item.timeSinceItemSpawned * 0.15f;

            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, color * (0.65f + (float)(-Math.Sin(timer) * 0.15f)),
                rotation, frame.Size() / 2, scale * (1.125f + (float)(Math.Sin(timer * 0.5f) * 0.15f)), SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, color * (0.65f + (float)(Math.Sin(timer * 0.5f) * 0.15f)),
                rotation, frame.Size() / 2, scale * (1.125f + (float)(-Math.Sin(timer * 0.5f) * 0.15f)), SpriteEffects.None, 0f);

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tipLine = tooltips.Find(n => n.Name == "HealLife");

            if (tipLine != null)
            {
                tipLine.Text = HealDesc.Value;
            }
        }

        public override void OnConsumeItem(Player player) //TODO: change to its own buff
        {
            player.AddBuff(BuffType<VampirePotionBuff>(), TimeToTicks(5));
        }
    }
}
