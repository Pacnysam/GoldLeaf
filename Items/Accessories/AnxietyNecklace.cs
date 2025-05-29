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
using ReLogic.Content;
using Terraria.Localization;
namespace GoldLeaf.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class AnxietyNecklace : ModItem
    {
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }
        public override void SetStaticDefaults()
        {
            ItemSets.Glowmask[Type] = (glowTex, ColorHelper.AdditiveWhite(), true);
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(1);

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 46;

            Item.value = Item.sellPrice(0, 6, 50, 0);
            Item.rare = ItemRarityID.Yellow;

            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.maxMinions++;
            player.GetModPlayer<AnxietyNecklacePlayer>().anxietyNecklace = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PanicNecklace)
                .AddIngredient(ItemID.PygmyNecklace)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }

    public class AnxietyNecklaceBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += 1f;
            player.runAcceleration += 0.15f;
            player.GetModPlayer<MinionSpeedPlayer>().minionSpeed += 0.5f;
        }
    }

    public class AnxietyNecklacePlayer : ModPlayer
    {
        public bool anxietyNecklace = false;

        public override void ResetEffects()
        {
            anxietyNecklace = false;
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (Player.whoAmI == Main.myPlayer && anxietyNecklace)
            {
                Player.AddBuff(BuffType<AnxietyNecklaceBuff>(), TimeToTicks(5));
            }
        }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (Player.whoAmI == Main.myPlayer && anxietyNecklace)
            {
                Player.AddBuff(BuffType<AnxietyNecklaceBuff>(), TimeToTicks(5));
            }
        }
    }
}
