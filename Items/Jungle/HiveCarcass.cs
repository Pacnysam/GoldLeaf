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
using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Pickups;
using GoldLeaf.Items.Sky;
namespace GoldLeaf.Items.Jungle
{
    internal class HiveCarcass : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 26;

            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;

            ItemID.Sets.IsAMaterial[Item.type] = false;

            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GoldLeafPlayer>().hiveCarcass = true;
        }
    }

    public class HiveCarcassItem : GlobalItem
    {
        public override bool OnPickup(Item item, Player player)
        {
            if (player.GetModPlayer<GoldLeafPlayer>().hiveCarcass && ItemSets.HeartPickup[item.type])
            {
                if (item.type == ItemType<HeartTiny>())
                    player.AddBuff(BuffID.Honey, TimeToTicks(5));
                else if (item.type == ItemType<HeartLarge>())
                    player.AddBuff(BuffID.Honey, TimeToTicks(15));
                else
                    player.AddBuff(BuffID.Honey, TimeToTicks(10));

                SoundEngine.PlaySound(SoundID.Item112 with { Pitch = 0.3f, PitchVariance = 0.3f, Volume = 0.65f });
            }
            return base.OnPickup(item, player);
        }

        public override void PostUpdate(Item item)
        {
            if (Main.LocalPlayer.GetModPlayer<GoldLeafPlayer>().hiveCarcass && ItemSets.HeartPickup[item.type] && Main.rand.NextBool(6))
            {
                Dust dust = Dust.NewDustDirect(item.position, item.width, item.height, DustID.Honey2, 0, Main.rand.NextFloat(-0.5f, 1.5f), 140);
                dust.velocity.X *= 0.35f;
                dust.fadeIn = 0.5f;
            }
        }

        public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            base.PostDrawInWorld(item, spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);

            if (Main.LocalPlayer.GetModPlayer<GoldLeafPlayer>().hiveCarcass) 
            {
                Main.GetItemDrawFrame(item.type, out Texture2D texture, out Rectangle frame);
                Vector2 origin = frame.Size() / 2;
                Vector2 drawPos = item.Bottom - Main.screenPosition - new Vector2(0, origin.Y);

                spriteBatch.Draw(texture, drawPos, frame, Color.Gold.Alpha(40) * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 8f) * 0.35f + 0.65f) * 0.85f, rotation, origin, scale, SpriteEffects.None, 0f);
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ItemID.BottledHoney, 5);
            recipe.AddIngredient(ItemID.Bottle, 5);
            recipe.AddIngredient(ItemType<HiveCarcass>());
            recipe.Register();
        }
    }
}
