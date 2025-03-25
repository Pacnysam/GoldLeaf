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

namespace GoldLeaf.Items.Blizzard
{
	public class ArcticFlower : ModItem
	{
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;
        }

        public override void SetDefaults()
		{
			Item.damage = 35;
			Item.DamageType = DamageClass.Summon;
            Item.shoot = ProjectileType<ArcticWraith>();
            Item.buffType = BuffType<ArcticWraithBuff>();
            Item.mana = 10;
			Item.width = 30;
			Item.height = 36;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(0, 1, 20, 0);
            Item.rare = ItemRarityID.Blue;
			Item.autoReuse = false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);

            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            return false;
        }

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BorealWood, 15);
			recipe.AddRecipeGroup("GoldLeaf:GoldBars", 6);
            recipe.AddIngredient(ItemType<AuroraCluster>(), 14);
            recipe.AddIngredient(ItemID.Shiverthorn, 2);
            recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
    }

    public class ArcticWraith : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.DamageType = DamageClass.MagicSummonHybrid;
        }

        public override void AI()
        {
           
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            /*Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * (float)(((float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length) / 2);

                Main.spriteBatch.Draw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0f);
            }*/
            return true;
        }
    }

    public class ArcticWraithBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // If the minions exist reset the buff time, otherwise remove the buff from the player
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ArcticWraith>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}