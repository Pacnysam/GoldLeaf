using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;
using GoldLeaf.Effects.Dusts;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using GoldLeaf.Core;
using Terraria.Audio;
using Microsoft.CodeAnalysis;
using static Terraria.Item;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Nightshade;
using static GoldLeaf.Core.GoldLeafPlayer;

namespace GoldLeaf.Items.Pickups
{
    public class HeartTiny : ModItem
    {
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Small Heart");

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Heart);

            Item.width = 8;
            Item.height = 8;
            Item.alpha = 200;
            Item.color = new Color(200, 200, 200, 200);

            ItemID.Sets.ItemsThatShouldNotBeInInventory[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.IgnoresEncumberingStone[Item.type] = true;
            ItemID.Sets.IsAPickup[Item.type] = true;
        }

        public override bool OnPickup(Player player)
        {
            player.Heal(5);
            SoundEngine.PlaySound(SoundID.Grab, player.Center);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;

            spriteBatch.Draw(tex, new Vector2(Item.position.X - Main.screenPosition.X, Item.position.Y - Main.screenPosition.Y + 2), new Rectangle(0, 0, tex.Width, tex.Height), new Color(200, 200, 200, 200), rotation, tex.Size() * 0.5f, scale += Main.essScale * 0.001f, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostUpdate()
        {
            float glo = 70 * 0.005f;
            glo *= Main.essScale * 0.5f;
            Lighting.AddLight((int)((Item.position.X + (Item.width / 2)) / 16f), (int)((Item.position.Y + (Item.height / 2)) / 16f), 0.75f * glo, 0.15f * glo, 0.15f * glo);
        }
    }

    public class HeartLarge : ModItem
    {
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Big Heart");

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Heart);

            Item.width = 16;
            Item.height = 16;
            Item.alpha = 200;
            Item.color = new Color(200, 200, 200, 200);

            ItemID.Sets.ItemsThatShouldNotBeInInventory[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.IgnoresEncumberingStone[Item.type] = true;
            ItemID.Sets.IsAPickup[Item.type] = true;
        }

        public override bool OnPickup(Player player)
        {
            player.Heal(40);
            SoundEngine.PlaySound(SoundID.Grab, player.Center);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;

            spriteBatch.Draw(tex, new Vector2 (Item.position.X - Main.screenPosition.X, Item.position.Y - Main.screenPosition.Y + 4), new Rectangle(0, 0, tex.Width, tex.Height), new Color(200, 200, 200, 200), rotation, tex.Size() * 0.5f, scale + (Main.essScale * 0.002f), SpriteEffects.None, 0f);
            return false;
        }

        public override void PostUpdate()
        {
            float glo = 180 * 0.01f;
            glo *= Main.essScale * 0.5f;
            Lighting.AddLight((int)((Item.position.X + (Item.width / 2)) / 16f), (int)((Item.position.Y + (Item.height / 2)) / 16f), 1f * glo, 0.2f * glo, 0.2f * glo);
        }
    }

    public class StarTiny : ModItem
    {
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Small Star");

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Star);

            Item.width = 8;
            Item.height = 8;
            Item.alpha = 200;
            Item.color = new Color(200, 200, 200, 200);

            ItemID.Sets.ItemsThatShouldNotBeInInventory[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.IgnoresEncumberingStone[Item.type] = true;
            ItemID.Sets.IsAPickup[Item.type] = true;
        }

        public override bool OnPickup(Player player)
        {
            player.ManaEffect(20);
            player.statMana += 20;
            SoundEngine.PlaySound(SoundID.Grab, player.Center);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;

            spriteBatch.Draw(tex, new Vector2(Item.position.X - Main.screenPosition.X, Item.position.Y - Main.screenPosition.Y + 2), new Rectangle(0, 0, tex.Width, tex.Height), new Color(200, 200, 200, 200), rotation, tex.Size() * 0.5f, scale += Main.essScale * 0.001f, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostUpdate()
        {
            float glo = 70 * 0.005f;
            glo *= Main.essScale * 0.5f;
            Lighting.AddLight((int)((Item.position.X + (Item.width / 2)) / 16f), (int)((Item.position.Y + (Item.height / 2)) / 16f), 0.15f * glo, 0.15f * glo, 0.75f * glo);
        }
    }

    public class StarLarge : ModItem
    {
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Big Star");

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Star);

            Item.width = 16;
            Item.height = 16;
            Item.alpha = 200;
            Item.color = new Color(200, 200, 200, 200);

            ItemID.Sets.ItemsThatShouldNotBeInInventory[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.IgnoresEncumberingStone[Item.type] = true;
            ItemID.Sets.IsAPickup[Item.type] = true;
        }

        public override bool OnPickup(Player player)
        {
            player.ManaEffect(200);
            player.statMana += 200;
            SoundEngine.PlaySound(SoundID.Grab, player.Center);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;

            spriteBatch.Draw(tex, new Vector2(Item.position.X - Main.screenPosition.X, Item.position.Y - Main.screenPosition.Y + 4), new Rectangle(0, 0, tex.Width, tex.Height), new Color(200, 200, 200, 200), rotation, tex.Size() * 0.5f, scale + (Main.essScale * 0.002f), SpriteEffects.None, 0f);
            return false;
        }

        public override void PostUpdate()
        {
            float glo = 180 * 0.01f;
            glo *= Main.essScale * 0.5f;
            Lighting.AddLight((int)((Item.position.X + (Item.width / 2)) / 16f), (int)((Item.position.Y + (Item.height / 2)) / 16f), 0.2f * glo, 0.2f * glo, 1f * glo);
        }
    }
}
