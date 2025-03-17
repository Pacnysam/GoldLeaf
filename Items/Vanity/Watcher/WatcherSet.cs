using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Items.Grove;
using System.Diagnostics.Metrics;
using System;
using GoldLeaf.Effects.Dusts;
using Terraria.Graphics.Effects;
using Terraria.DataStructures;
using GoldLeaf.Tiles.Grove;
using ReLogic.Content;

namespace GoldLeaf.Items.Vanity.Watcher
{
    [AutoloadEquip(EquipType.Head)]
    public class WatcherEyedrops : ModItem
    {
        public override string Texture => "GoldLeaf/Items/Vanity/Watcher/WatcherEyedrops";

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
                EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
        }

        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 24;

            Item.value = Item.sellPrice(0, 7, 50, 0);
            Item.rare = ItemRarityID.Purple;

            Item.vanity = true;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class WatcherCloak : ModItem
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
                EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            robes = true;

            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
        }

        public override void SetStaticDefaults()
        {
            ArmorIDs.Body.Sets.HidesArms[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.HidesTopSkin[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.HidesBottomSkin[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.HidesHands[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.DisableHandOnAndOffAccDraw[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.DisableBeltAccDraw[Item.bodySlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;

            Item.value = Item.sellPrice(0, 7, 50, 0);
            Item.rare = ItemRarityID.Purple;

            Item.bodySlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            Item.vanity = true;
        }
    }

    public class WatcherPlayer : ModPlayer
    {
        public bool watcherDrops = false;
        public bool watcherCloak = false;
        public int punchCooldown = 0;
        public int dustCooldown = 0;

        public override void ResetEffects()
        {
            watcherDrops = Player.armor[0].type == ItemType<WatcherEyedrops>() && Player.armor[10].type == ItemID.None || Player.armor[10].type == ItemType<WatcherEyedrops>();
            watcherCloak = Player.armor[1].type == ItemType<WatcherCloak>() && Player.armor[11].type == ItemID.None || Player.armor[11].type == ItemType<WatcherCloak>();
        }

        public override void PreUpdate()
        {
            if (watcherCloak && punchCooldown > 0) punchCooldown--;
            if (watcherCloak && watcherDrops && dustCooldown > 0) dustCooldown--;
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (watcherCloak && punchCooldown <= 0 && velocity.Length() >= 8f)
            {
                Vector2 vectorToCursor = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                Vector2 punchVelocity = Vector2.Normalize(vectorToCursor - Player.Center) * 7.5f;

                Projectile.NewProjectile(source, new Vector2(Player.Center.X, Player.Center.Y + 6), punchVelocity, ProjectileType<WatcherPunch>(), 0, 0, Player.whoAmI, item.useTime);
                punchCooldown = item.useTime / 2;
            }
            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }

        public override void PostUpdateMiscEffects()
        {
            if (watcherCloak && watcherDrops && dustCooldown <= 0 && Player.statMana > 0 && Player.velocity.Y == 0f && Player.grappling[0] == -1 && Math.Abs(Player.velocity.X) >= 3.2f)
            {
                Vector2 position; if (Player.velocity.X > 0) position = Player.BottomLeft; else position = Player.BottomRight;
                float manaPercent = (float)Player.statMana / Player.statManaMax2;
                float manaRatio = (float)Player.statManaMax2 / Player.statMana;

                Dust dust = Dust.NewDustPerfect(position, DustType<LightDust>(), new Vector2(0, Main.rand.NextFloat(-2f, -0.5f)), Main.rand.Next(0, 80), new Color(47, 41, 76) * manaPercent, Main.rand.NextFloat(0.7f, 1.2f));

                if (manaRatio > 20) manaRatio = 20;
                dustCooldown = (int)(Main.rand.Next(1, 4) + manaRatio);
            }
        }
    }

    public class WatcherPunch : ModProjectile
    {
        private static Asset<Texture2D> tex;
        private static Asset<Texture2D> glowTex;

        public override void Load()
        {
            tex = Request<Texture2D>(Texture);
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        private readonly Color[] afterimageColors = [new Color(14, 14, 15), new Color(47, 41, 76), new Color(95, 69, 132), new Color(169, 98, 215)];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;

            Projectile.width = 12;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            //Projectile.extraUpdates = 1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (Projectile.ai[0] > 40) Projectile.ai[0] = 40;

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            Projectile.velocity *= 0.9f;

            if (++Projectile.frameCounter >= 1 + Projectile.ai[0] / 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }

            if (Projectile.frame > 5) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new(tex.Width() * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                Main.spriteBatch.Draw(glowTex.Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(tex.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), afterimageColors[k], Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(tex.Value, Projectile.position, new Microsoft.Xna.Framework.Rectangle?(tex.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            return true;
        }
    }
}
