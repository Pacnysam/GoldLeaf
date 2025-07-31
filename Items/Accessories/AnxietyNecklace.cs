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
using GoldLeaf.Items.Vanity.Watcher;
using Terraria.DataStructures;
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
            player.runAcceleration *= 2f;
            player.runSlowdown *= 2f;
            player.GetModPlayer<MinionSpeedPlayer>().minionSpeed += Math.Clamp((player.buffTime[buffIndex] - TimeToTicks(2.5f)) * 0.002f, 0f, 0.5f);
        }
    }
    
    public class AnxietyNecklacePlayer : ModPlayer
    {
        public bool anxietyNecklace = false;
        public float anxietyIntensity = 0f;

        public override void ResetEffects()
        {
            anxietyNecklace = false;
        }

        public override void PostUpdateMiscEffects()
        {
            anxietyIntensity = LerpFloat(anxietyIntensity, 0f, 0.01f);

            if (anxietyIntensity > 0f && anxietyIntensity < 1f)
                anxietyIntensity -= 0.0125f;

            if (anxietyIntensity < 0.025f)
                anxietyIntensity = 0f;
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            /*if (anxietyNecklace && Player.GetModPlayer<GoldLeafPlayer>().hasDoneHurtSound == false)
            {
                modifiers.DisableSound();
            }*/
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (/*Player.whoAmI == Main.myPlayer &&*/ anxietyNecklace)
            {
                if (!Main.dedServ /*&& Player.GetModPlayer<GoldLeafPlayer>().hasDoneHurtSound == false*/)
                {
                    //SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Kirby/MassAttack/EvilGrunt") { Pitch = 0.3f, PitchVariance = 0.4f, Volume = 0.6f }, Player.MountedCenter);
                    SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryShot, Player.MountedCenter);
                    Player.GetModPlayer<GoldLeafPlayer>().hasDoneHurtSound = true;
                }
                //Player.GetModPlayer<GoldLeafPlayer>().hasDoneHurtSound = true;

                anxietyIntensity = 2f;
                Player.AddBuff(BuffType<AnxietyNecklaceBuff>(), TimeToTicks(8));
            }
        }
    }
    
    public class AnxietyPlayerLayer : PlayerDrawLayer
    {
        public static Asset<Texture2D> waveTex;
        public override void Load()
        {
            waveTex = Request<Texture2D>("GoldLeaf/Items/Accessories/AnxietyNecklaceWave");
        }

        public override Position GetDefaultPosition() => PlayerDrawLayers.BeforeFirstVanillaLayer;
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<AnxietyNecklacePlayer>().anxietyIntensity > 0f;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            AnxietyNecklacePlayer anxiousPlayer = player.GetModPlayer<AnxietyNecklacePlayer>();
            float intensity = anxiousPlayer.anxietyIntensity;

            Color color1 = new(236, 64, 12) { A = 0 };
            Color color2 = new(162, 24, 15) { A = 0 };

            var drawDataRight = new DrawData(
                waveTex.Value,
                player.MountedCenter - Main.screenPosition + new Vector2(player.width * (1.05f * MathHelper.Clamp(intensity * 3f, 0.35f, 1f)) * MathHelper.Clamp(intensity - 0.35f, 1f, 1.65f), 0) + (new Vector2(Main.rand.NextFloat(8f, 10f) * 3f).RotatedByRandom(MathHelper.TwoPi) * MathHelper.Clamp(intensity - 1.5f, 0f, 0.5f)),
                null,
                Color.Lerp(color1, color2, (float)Math.Sin(GoldLeafWorld.rottime * 4f) * 0.5f + 0.5f) * MathHelper.Clamp(intensity, 0f, 1f),
                0f,
                waveTex.Size() / 2,
                (0.475f + MathHelper.Clamp((intensity - 1.65f) * 2, 0f, 0.35f) + ((float)Math.Sin(GoldLeafWorld.rottime * 4.5f * MathHelper.Clamp(intensity - 0.5f, 0.75f, 1.5f)) * 0.75f + 0.25f) * 0.15f) * MathHelper.Clamp(intensity * 4, 0, 1f),
                SpriteEffects.None,
                0
            ); //main right
            var drawDataRight2 = new DrawData(
                waveTex.Value,
                player.MountedCenter - Main.screenPosition + new Vector2(player.width * (1.1f * MathHelper.Clamp(intensity * 3f, 0.35f, 1f)) * MathHelper.Clamp(intensity - 0.35f, 1f, 1.65f), 0) + (new Vector2(Main.rand.NextFloat(8f, 10f) * 3f).RotatedByRandom(MathHelper.TwoPi) * MathHelper.Clamp(intensity - 1.5f, 0f, 0.5f)),
                null,
                Color.Lerp(color1, color2, (float)Math.Cos(GoldLeafWorld.rottime * 4f) * 0.5f + 0.5f) * MathHelper.Clamp(intensity, 0f, 0.55f),
                0f,
                waveTex.Size() / 2,
                (0.525f + MathHelper.Clamp((intensity - 1.65f) * 2, 0f, 0.35f) + ((float)Math.Cos(GoldLeafWorld.rottime * 4.5f * MathHelper.Clamp(intensity - 0.5f, 0.75f, 1.5f)) * 0.75f + 0.25f) * 0.125f) * MathHelper.Clamp(intensity * 4, 0, 1f),
                SpriteEffects.None,
                0
            ); //secondary right
            var drawDataRight3 = new DrawData(
                waveTex.Value,
                player.MountedCenter - Main.screenPosition + new Vector2(player.width * (1.15f * MathHelper.Clamp(intensity * 3f, 0.35f, 1f)) * MathHelper.Clamp(intensity - 0.35f, 1f, 1.65f), 0) + (new Vector2(Main.rand.NextFloat(8f, 10f) * 3f).RotatedByRandom(MathHelper.TwoPi) * MathHelper.Clamp(intensity - 1.5f, 0f, 0.5f)),
                null,
                Color.Lerp(color1, color2, (float)Math.Sin(GoldLeafWorld.rottime * 4f) * 0.5f - 0.5f) * MathHelper.Clamp(intensity, 0f, 0.275f),
                0f,
                waveTex.Size() / 2,
                (0.575f + MathHelper.Clamp((intensity - 1.65f) * 2, 0f, 0.35f) + ((float)Math.Cos(GoldLeafWorld.rottime * 4.5f * MathHelper.Clamp(intensity - 0.5f, 0.75f, 1.5f)) * 0.75f + 0.25f) * 0.15f) * MathHelper.Clamp(intensity * 4, 0, 1f),
                SpriteEffects.None,
                0
            ); //tertiary right
            var drawDataLeft = new DrawData(
                waveTex.Value,
                player.MountedCenter - Main.screenPosition + new Vector2(player.width * (-1.05f * MathHelper.Clamp(intensity * 3f, 0.35f, 1f)) * MathHelper.Clamp(intensity - 0.35f, 1f, 1.65f), 0) + (new Vector2(Main.rand.NextFloat(8f, 10f) * 3f).RotatedByRandom(MathHelper.TwoPi) * MathHelper.Clamp(intensity - 1.5f, 0f, 0.5f)),
                null,
                Color.Lerp(color1, color2, (float)Math.Sin(GoldLeafWorld.rottime * 4f) * 0.5f + 0.5f) * MathHelper.Clamp(intensity, 0f, 1f),
                0f,
                waveTex.Size() / 2,
                (0.475f + MathHelper.Clamp((intensity - 1.65f) * 2, 0f, 0.35f) + ((float)Math.Sin(GoldLeafWorld.rottime * 4.5f * MathHelper.Clamp(intensity - 0.5f, 0.75f, 1.5f)) * 0.75f + 0.25f) * 0.15f) * MathHelper.Clamp(intensity * 4, 0, 1f),
                SpriteEffects.FlipHorizontally,
                0
            ); //main left
            var drawDataLeft2 = new DrawData(
                waveTex.Value,
                player.MountedCenter - Main.screenPosition + new Vector2(player.width * (-1.1f * MathHelper.Clamp(intensity * 3f, 0.35f, 1f)) * MathHelper.Clamp(intensity - 0.35f, 1f, 1.65f), 0) + (new Vector2(Main.rand.NextFloat(8f, 10f) * 3f).RotatedByRandom(MathHelper.TwoPi) * MathHelper.Clamp(intensity - 1.5f, 0f, 0.5f)),
                null,
                Color.Lerp(color1, color2, (float)Math.Cos(GoldLeafWorld.rottime * 4f) * 0.5f + 0.5f) * MathHelper.Clamp(intensity, 0f, 0.55f),
                0f,
                waveTex.Size() / 2,
                (0.525f + MathHelper.Clamp((intensity - 1.65f) * 2, 0f, 0.35f) + ((float)Math.Cos(GoldLeafWorld.rottime * 4.5f * MathHelper.Clamp(intensity - 0.5f, 0.75f, 1.5f)) * 0.75f + 0.25f) * 0.125f) * MathHelper.Clamp(intensity * 4, 0, 1f),
                SpriteEffects.FlipHorizontally,
                0
            ); //secondary left
            var drawDataLeft3 = new DrawData(
                waveTex.Value,
                player.MountedCenter - Main.screenPosition + new Vector2(player.width * (-1.15f * MathHelper.Clamp(intensity * 3f, 0.35f, 1f)) * MathHelper.Clamp(intensity - 0.35f, 1f, 1.65f), 0) + (new Vector2(Main.rand.NextFloat(8f, 10f) * 3f).RotatedByRandom(MathHelper.TwoPi) * MathHelper.Clamp(intensity - 1.5f, 0f, 0.5f)),
                null,
                Color.Lerp(color1, color2, (float)Math.Sin(GoldLeafWorld.rottime * 4f) * 0.5f - 0.5f) * MathHelper.Clamp(intensity, 0f, 0.275f),
                0f,
                waveTex.Size() / 2,
                0.575f + MathHelper.Clamp((intensity - 1.65f) * 2, 0f, 0.35f) + ((float)Math.Cos(GoldLeafWorld.rottime * 4.5f * MathHelper.Clamp(intensity - 0.5f, 0.75f, 1.5f)) * 0.75f + 0.25f) * 0.15f,
                SpriteEffects.FlipHorizontally,
                0
            ); //tertiary left

            drawInfo.DrawDataCache.Add(drawDataRight);
            drawInfo.DrawDataCache.Add(drawDataLeft);
            drawInfo.DrawDataCache.Add(drawDataRight2);
            drawInfo.DrawDataCache.Add(drawDataLeft2);
            drawInfo.DrawDataCache.Add(drawDataRight3);
            drawInfo.DrawDataCache.Add(drawDataLeft3);
        }
    }
}
