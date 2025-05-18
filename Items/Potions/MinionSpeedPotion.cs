using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Tiles.Decor;
using GoldLeaf.Core;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Blizzard;
using System;
using Terraria.ModLoader.IO;
using System.IO;

namespace GoldLeaf.Items.Potions
{
    public class MinionSpeedPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.RagePotion);

            Item.width = 20;
            Item.height = 30;

            Item.buffType = BuffType<MinionSpeedPotionBuff>();
            Item.buffTime = TimeToTicks(8, 0);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ItemType<AuroraCluster>());
            recipe.AddIngredient(ItemID.Daybloom);
            recipe.AddTile(TileID.Bottles);
            recipe.Register();
        }
    }

    public class MinionSpeedPotionBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<MinionSpeedPlayer>().minionSpeed += .25f;
        }
    }

    /*public class MinionSpeedPotionProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private int minionSpeedTimer;
        private bool minionIsFaster;

        public override void Load()
        {
            //On_Projectile.Update
        }

        public override bool PreAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            if (player != null && player.HasBuff(BuffType<MinionSpeedPotionBuff>()) && projectile.minion)
            {
                int projType = projectile.type;

                if (projectile.aiStyle > 0 && projectile.ModProjectile != null)
                    projectile.type = projectile.ModProjectile.AIType;

                projectile.VanillaAI();

                if (projectile.aiStyle > 0 && projectile.ModProjectile != null)
                    projectile.type = projType;
            }
            ProjectileLoader.AI(projectile);
            return true;
        }

        public override void PostAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];

            if (player != null && player.HasBuff(BuffType<MinionSpeedPotionBuff>()) && projectile.minion)
            {
                minionSpeedTimer++;
                projectile.extraUpdates++;
                minionIsFaster = true;
            }

            if (player != null && !player.HasBuff(BuffType<MinionSpeedPotionBuff>()) && projectile.minion && minionIsFaster) 
            {
                if (projectile.extraUpdates > 1) projectile.extraUpdates--;
                minionIsFaster = false;
                minionSpeedTimer = 0;
            }

            if (player != null && player.HasBuff(BuffType<MinionSpeedPotionBuff>()) && projectile.minion)
            {
                minionSpeedTimer++;

                //projectile.extraUpdates = (int)(projectile.extraUpdates * 1.25f);

                if (minionSpeedTimer == 3)
                { 
                    //projectile.extraUpdates *= 2;
                    projectile.extraUpdates++;
                    minionIsFaster = true;
                }
                if (minionSpeedTimer == 4)
                {
                    //projectile.extraUpdates /= 2;
                    if (projectile.extraUpdates > 0) projectile.extraUpdates--;
                    minionSpeedTimer = 0;
                    minionIsFaster = false;
                }
            }
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(minionIsFaster);
            binaryWriter.Write(minionSpeedTimer);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            minionIsFaster = bitReader.ReadBit();
            minionSpeedTimer = binaryReader.ReadInt32();
        }
    }*/
}
