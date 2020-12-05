using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace GoldLeaf.Items.Wisp
{
	public class WispHook : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wisp Hook");
			Tooltip.SetDefault("Damages enemies\nMinions target damaged enemies");
		}
		public override void SetDefaults()
		{
			item.width = 54;
            item.damage = 22;
            item.mana = 20;
            item.summon = true;
			item.height = 16;
			item.useTime = 10;
			item.useAnimation = 10;
			item.shoot = ProjectileType<WispHookP>();
			item.autoReuse = false;
			item.knockBack = 2;
			item.shootSpeed = 15f;
			item.useStyle = 1;
            item.noUseGraphic = true;
			item.rare = 4;
			item.UseSound = SoundID.Item1;
		}
	}

    public class WispHookP : ModProjectile
    {
        bool a = false;
        public override void SetDefaults()
        {
            projectile.scale = 1f;
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = 7;
			projectile.extraUpdates = 1;
            projectile.penetrate = 2;
            projectile.timeLeft = 1200;
            projectile.CloneDefaults(ProjectileID.Hook);
        }

		

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wisp Hook");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;        //The recording mode
        }
        public override float GrappleRange()
        {
            return 360f;
        }

		public override void GrappleRetreatSpeed(Player player, ref float speed) { speed = 9.5f; }
		public override void GrapplePullSpeed(Player player, ref float speed) { speed = 30f; }
		public override void NumGrappleHooks(Player player, ref int numHooks) { numHooks = 2; }

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Player player = Main.player[projectile.owner];
			int num = -1;
			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].CanBeChasedBy(player, false) && Main.npc[i] == target)
				{
					num = i;
				}
			}
			{
				player.MinionAttackTargetNPC = num;
			}
		}

		public override void AI()
        {
            Player p = Main.player[(int)projectile.ai[0]];
            if (a == false)
            {
                a = true;
                if (p.statMana >= 20)
                {
                    p.statMana -= 20;
                }
                else
                {
                    projectile.Kill();
                }
            }
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = GetTexture("GoldLeaf/Items/Wisp/WispHookChain");
            Vector2 position = projectile.Center;
            Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;
            Rectangle? sourceRectangle = new Rectangle?();
            Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            float num1 = texture.Height;
            Vector2 vector24 = mountedCenter - position;
            float rotation = (float)Math.Atan2(vector24.Y, vector24.X) - 1.57f;
            bool flag = true;
            if (float.IsNaN(position.X) && float.IsNaN(position.Y))
                flag = false;
            if (float.IsNaN(vector24.X) && float.IsNaN(vector24.Y))
                flag = false;
            while (flag)
            {
                if (vector24.Length() < num1 + 1.0)
                {
                    flag = false;
                }
                else
                {
                    Vector2 vector21 = vector24;
                    vector21.Normalize();
                    position += vector21 * num1;
                    vector24 = mountedCenter - position;
                    Color color2 = Lighting.GetColor((int)position.X / 16, (int)(position.Y / 16.0));
                    color2 = projectile.GetAlpha(color2);
                    Main.spriteBatch.Draw(texture, position - Main.screenPosition, sourceRectangle, color2, rotation, origin, 1f, SpriteEffects.None, 0.0f);
                }
            }
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D face = mod.GetTexture("Items/Wisp/WispHookPGlow");
            spriteBatch.Draw(face, new Vector2(projectile.position.X - Main.screenPosition.X + projectile.width * 0.5f, projectile.position.Y - Main.screenPosition.Y + projectile.height - face.Height * 0.5f + 2f), new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height), Color.White, projectile.rotation, face.Size(), projectile.scale, SpriteEffects.None, 0f);
        }
    }
}
