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
using GoldLeaf.Items.Granite;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using System.Collections.Generic;
using Terraria.Graphics.Renderers;
using System.Linq;

namespace GoldLeaf.Items.Vanity.Watcher
{
    [AutoloadEquip(EquipType.Head)]
    public class WatcherEyedrops : ModItem
    {
        public override string Texture => "GoldLeaf/Items/Vanity/Watcher/WatcherEyedrops";

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
        public List<Vector2> oldPos = [];
        public RenderTarget2D target;
        public bool WatcherSet => watcherDrops && watcherCloak;

        const int oldPosLength = 15;
        public void Load(Mod mod)
        {
            if (!Main.dedServ)
            {
                target = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            }
        }

        public override void ResetEffects()
        {
            watcherDrops = Player.armor[0].type == ItemType<WatcherEyedrops>() && Player.armor[10].type == ItemID.None || Player.armor[10].type == ItemType<WatcherEyedrops>();
            watcherCloak = Player.armor[1].type == ItemType<WatcherCloak>() && Player.armor[11].type == ItemID.None || Player.armor[11].type == ItemType<WatcherCloak>();
        }

        public override void PreUpdate()
        {
            if (watcherCloak && punchCooldown > 0) punchCooldown--;
            if (WatcherSet && dustCooldown > 0) dustCooldown--;
            
            if (Player.velocity.Length() > 0)
                oldPos.Add(Player.Center);

            if (oldPos.Count > oldPosLength || (oldPos.Count > 0 && Player.velocity.Length() <= 1))
            {
                oldPos.RemoveAt(0);
            }
        }

        /*public override void Load()
        {
            On_Main.DrawPlayers_AfterProjectiles += WatcherAfterImage;
        }
        public override void Unload()
        {
            On_Main.DrawPlayers_AfterProjectiles -= WatcherAfterImage;
        }*/

        private void WatcherAfterImage(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            if (Main.dedServ || Main.spriteBatch == null || Main.gameMenu || Main.graphics.GraphicsDevice == null)
                return;

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            foreach (Player player in Main.player.Where(x => x.active && x != null))
            {
                if (!player.dead && player.GetModPlayer<WatcherPlayer>().WatcherSet && !player.outOfRange)
                {
                    for (int i = player.GetModPlayer<WatcherPlayer>().oldPos.Count - 1; i > 0; i--)
                    {
                        Color color = Color.Lerp(new Color(145, 41, 184), new Color(47, 41, 76), i / (float)oldPosLength);
                        Vector2 offset = new(player.width / 2, player.height / 2);

                        DrawData data = new(Request<Texture2D>("GoldLeaf/Textures/Glow0").Value, player.GetModPlayer<WatcherPlayer>().oldPos[i] - Main.screenPosition, null, color * (i / (float)oldPosLength) * (player.GetModPlayer<WatcherPlayer>().oldPos.Count / (float)oldPosLength), player.fullRotation, Request<Texture2D>("GoldLeaf/Textures/Flares/FlareSmall").Size() / 2, 1f, SpriteEffects.None, 0f);
                        
                        if (player.dye[2].type != ItemID.None)
                        {
                            data.color = Color.White * (i / (float)oldPosLength) * (player.GetModPlayer<WatcherPlayer>().oldPos.Count / (float)oldPosLength) * 0.5f;
                            //GameShaders.Armor.GetShaderFromItemId(player.dye[2].type).Apply(player, data);
                            GameShaders.Armor.GetSecondaryShader(player.dye[2].dye, player).Apply(player, data);
                        }

                        data.Draw(Main.spriteBatch);

                        //Main.spriteBatch.Draw(Request<Texture2D>("GoldLeaf/Textures/Flares/FlareSmall").Value, player.GetModPlayer<WatcherPlayer>().oldPos[i] - Main.screenPosition, null, color * (i / (float)oldPosLength) * (player.GetModPlayer<WatcherPlayer>().oldPos.Count / (float)oldPosLength), player.fullRotation, Request<Texture2D>("GoldLeaf/Textures/Flares/FlareSmall").Size()/2, 1f, SpriteEffects.None, 0f);

                        //Main.spriteBatch.Draw(player.GetModPlayer<WatcherPlayer>().target, player.GetModPlayer<WatcherPlayer>().oldPos[i] - Main.screenPosition, player.Size.ToRectangle(), color, player.fullRotation, Vector2.Zero, 1f + (1f - Main.GameZoomTarget) * 0.5f, SpriteEffects.None, 0f);
                    }
                }
            }

            /*for (int k = 0; k < 255; k++)
            {
                Player player = Main.player[k];
                //WatcherPlayer truthPlayer = player.GetModPlayer<WatcherPlayer>();

                if (!Main.gameMenu && player.active && !player.dead && player.GetModPlayer<WatcherPlayer>().WatcherSet && !player.outOfRange)
                {
                    for (int i = player.GetModPlayer<WatcherPlayer>().oldPos.Count - 1; i > 0; i--)
                    {
                        Color color = Color.Lerp(new Color(186, 91, 232), new Color(47, 41, 76), i * 0.1f);
                        
                        Main.spriteBatch.Draw(player.GetModPlayer<WatcherPlayer>().target, player.GetModPlayer<WatcherPlayer>().oldPos[i] - Main.screenPosition, player.Size.ToRectangle(), color, player.fullRotation, Vector2.Zero, 1f + (1f - Main.GameZoomTarget) * 0.5f, SpriteEffects.None, 0f);
                    }
                }
            }*/
            Main.spriteBatch.End();

            orig.Invoke(self);
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
            if (WatcherSet && dustCooldown <= 0 && Player.statMana > 0 && Player.velocity.Y == 0f && Player.grappling[0] == -1 && Math.Abs(Player.velocity.X) >= 3.2f)
            {
                Vector2 position; if (Player.velocity.X > 0) position = Player.BottomLeft; else position = Player.BottomRight;
                float manaPercent = (float)Player.statMana / Player.statManaMax2;
                float manaRatio = (float)Player.statManaMax2 / Player.statMana;

                Dust dust = Dust.NewDustPerfect(position, DustType<LightDust>(), new Vector2(0, Main.rand.NextFloat(-2f, -0.5f)), Main.rand.Next(0, 80), new Color(47, 41, 76) * manaPercent, Main.rand.NextFloat(1.75f, 2.5f));
                dust.shader = GameShaders.Armor.GetSecondaryShader(Player.dye[2].dye, Player);

                if (manaRatio > 20) manaRatio = 20;
                dustCooldown = (int)(Main.rand.Next(1, 4) + manaRatio);
            }
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (WatcherSet)
            {
                modifiers.DisableSound();
            }
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if (WatcherSet && Player.statLife > 0)
            {
                //SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/StarSlot") { Variants = [1, 2, 3] }, Player.Center);
                //SoundEngine.PlaySound(SoundID.DD2_LightningBugHurt, Player.position);
                SoundEngine.PlaySound(SoundID.DD2_KoboldIgnite with { Volume = 1.1f }, Player.Center);
                SoundEngine.PlaySound(SoundID.NPCHit52 with { Volume = 0.95f, Pitch = 0.35f }, Player.position);
            }
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (WatcherSet)
            {
                playSound = false;
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (WatcherSet)
            {
                SoundEngine.PlaySound(SoundID.NPCHit52 with { Volume = 0.8f }, Player.position);
                SoundEngine.PlaySound(SoundID.DD2_KoboldIgnite with { Volume = 1.1f }, Player.position);
                SoundEngine.PlaySound(SoundID.Item68, Player.position);
            }
        }
    }

    /*public class TruthPlayerLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => PlayerDrawLayers.AfterLastVanillaLayer;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<WatcherPlayer>().WatcherSet;
        

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            WatcherPlayer truthPlayer = player.GetModPlayer<WatcherPlayer>();

            if (!Main.gameMenu && player.active && !player.dead && truthPlayer.WatcherSet && !player.outOfRange)
            {
                for (int i = truthPlayer.oldPos.Count - 1; i > 0; i--)
                {
                    Color color = Color.Lerp(new Color(186, 91, 232), new Color(47, 41, 76), i * 0.1f);
                    DrawData data = new(truthPlayer.target, truthPlayer.target.Size(), color);
                    drawInfo.DrawDataCache.Add(data);
                    //Main.spriteBatch.Draw(truthPlayer.target, truthPlayer.target.Size(), color);
                    //Main.PlayerRenderer.DrawPlayer(Main.Camera, player, truthPlayer.oldPos[i]- Main.screenPosition, player.fullRotation, player.fullRotationOrigin);
                }
            }
        }
    }*/

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
