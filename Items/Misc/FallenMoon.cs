using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Core;
using GoldLeaf.Effects.Dusts;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using GoldLeaf.Items.VanillaBossDrops;
using Terraria.DataStructures;
using GoldLeaf.Items.Potions;
using GoldLeaf.Items.Jungle.ToxicPositivity;

namespace GoldLeaf.Items.Misc
{
    public class FallenMoon : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 8, true) { PingPong = true });
            ItemID.Sets.AnimatesAsSoul[Type] = true;

            ItemSets.Glowmask[Type] = (TextureAssets.Item[Type], ColorHelper.AdditiveWhite(180), false);

            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Green;

            Item.alpha = 75;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item4;
            Item.useTurn = false;
            Item.useAnimation = 17;
            Item.useTime = 17;

            Item.consumable = true;
            Item.shoot = ProjectileType<ToxicPositivityEffect>();
            Item.ammo = AmmoID.FallenStar;

            Item.width = 26;
            Item.height = 26;
        }

        public float rottime = 0;

        public override bool CanShoot(Player player) => false;
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override bool? CanBeChosenAsAmmo(Item weapon, Player player)
        {
            if (weapon.type == ItemType<Lunar>())
                return true;

            return base.CanBeChosenAsAmmo(weapon, player);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Main.IsItDay() && !Item.shimmered)
            {
                for (int j = 0; j < 18; j++)
                {
                    Dust dust = TwinkleDust.Spawn(new LightDust.LightDustData(Main.rand.NextFloat(0.9f, 0.985f), MathHelper.ToRadians(Main.rand.NextFloat(-18f, 18f))), Item.position, Item.Hitbox.Size(), Item.velocity * Main.rand.NextFloat(0.9f, 1.25f), 0, new Color(181, 140, 255) { A = 30 } * Main.rand.NextFloat(0.4f, 0.6f), Main.rand.NextFloat(0.2f, 0.7f));
                    dust.fadeIn = Main.rand.NextFloat(1.95f, 2.85f);
                    //Dust.NewDust(Item.position, Item.width, Item.height, DustID.MagicMirror, Item.velocity.X, Item.velocity.Y, 150, default(Color), 1.2f);
                }
                Item.active = false;
                Item.type = ItemID.None;
                Item.stack = 0;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Item.whoAmI);
                }
            }
        }
        public override void PostUpdate()
        {
            rottime += (float)Math.PI / 120;

            if (rottime >= Math.PI * 2)
                rottime = 0;

            if (!Main.dedServ)
                Lighting.AddLight((int)((Item.position.X + (float)Item.width) / 16f), (int)((Item.position.Y + (float)(Item.height / 2)) / 16f), 181 / 255f, 140 / 255f, 255 / 255f);

            if (Item.timeSinceItemSpawned % 16 == 0)
            {
                Dust dust = TwinkleDust.SpawnPerfect(new LightDust.LightDustData(Main.rand.NextFloat(0.9f, 0.95f), MathHelper.ToRadians(Main.rand.NextFloat(-18f, 18f))), Item.Center + new Vector2(0f, Item.height * -0.35f) + Main.rand.NextVector2CircularEdge(Item.width, Item.height * 0.85f) * (0.3f + Main.rand.NextFloat() * 0.5f), new Vector2(0f, (0f - Main.rand.NextFloat()) * 0.3f - 2.5f), 0, new Color(181, 140, 255) { A = 30 } * Main.rand.NextFloat(0.3f, 0.5f), Main.rand.NextFloat(0.45f, 0.65f));
                dust.fadeIn = Main.rand.NextFloat(1.75f, 2.45f);
                //dust.noLight = true;
            }
        }
        public override void UpdateInventory(Player player)
        {
            rottime += (float)Math.PI / 120;

            if (rottime >= Math.PI * 2)
                rottime = 0;
        }

        public override bool CanStackInWorld(Item source) => false;

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Main.GetItemDrawFrame(Item.type, out Texture2D texture, out Rectangle frame);
            
            float num7 = Item.timeSinceItemSpawned / 240f + Main.GlobalTimeWrappedHourly * 0.09f;
            float globalTimeWrappedHourly2 = Main.GlobalTimeWrappedHourly;
            globalTimeWrappedHourly2 %= 5f;
            globalTimeWrappedHourly2 /= 2.5f;
            if (globalTimeWrappedHourly2 >= 1f)
            {
                globalTimeWrappedHourly2 = 2f - globalTimeWrappedHourly2;
            }
            globalTimeWrappedHourly2 = globalTimeWrappedHourly2 * 0.5f + 0.5f;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                spriteBatch.Draw(texture, Item.Center - Main.screenPosition +  new Vector2(0f, 8f).RotatedBy((i + num7) * ((float)Math.PI * 2f)) * globalTimeWrappedHourly2, frame, new Color(214, 51, 145).MultiplyAlpha(0.5f) * 0.55f, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
            }
            for (float k = 0f; k < 1f; k += 0.25f)
            {
                spriteBatch.Draw(texture, Item.Center - Main.screenPosition + new Vector2(0f, 2f).RotatedBy((k + (num7 * -1.75f)) * ((float)Math.PI * 2f)) * globalTimeWrappedHourly2, frame, new Color(210, 164, 255) { A = 120 } * 0.7f, rotation, frame.Size()/2f, scale, SpriteEffects.None, 0f);
            }
            Color glowColor = new Color(181, 140, 255) { A = 30 } * (0.5f + (float)Math.Sin(rottime * 6) * 0.75f);
            
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, alphaColor, rotation, frame.Size()/2, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Load()
        {
            On_NPC.AI_001_Slimes_GenerateItemInsideBody += SlimeLunes;
        }
        public override void Unload()
        {
            On_NPC.AI_001_Slimes_GenerateItemInsideBody -= SlimeLunes;
        }
        private int SlimeLunes(On_NPC.orig_AI_001_Slimes_GenerateItemInsideBody orig, bool isBallooned)
        {
            if (Main.remixWorld && Main.rand.NextBool(20) && NPC.downedBoss1)
                return ItemType<FallenMoon>();

            return orig(isBallooned);
        }
    }

    /*public class FallenMoon : ModProjectile
    {
    }*/
}
