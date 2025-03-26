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
using GoldLeaf.Items.Vanity;
using Terraria.Localization;
using GoldLeaf.Items.Misc.Weapons;
using Terraria.Audio;
using Terraria.WorldBuilding;
using Terraria.Graphics.Shaders;
using GoldLeaf.Items.Pickups;

namespace GoldLeaf.Items.Blizzard.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class FrostyMask : ModItem
    {
        public override string Texture => "GoldLeaf/Items/Blizzard/Armor/FrostyMask";
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(manaMax);

        private static readonly int manaMax = 40;

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
                EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Face, this);
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
        }

        public override void SetStaticDefaults()
        {
            //ArmorIDs.Face.Sets.PreventHairDraw[Item.headSlot] = false;
            //ArmorIDs.Face.Sets.DrawInFaceHeadLayer[Item.headSlot] = true;
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
            ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.aggro -= 200;
            player.statManaMax2 += manaMax;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<FrostyRobe>();
        }

        /*public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
        {
            //color = ColorHelper.AuroraColor(GoldLeafWorld.Timer * 0.05f);
            color = ColorHelper.AuroraColor(GoldLeafWorld.Timer * 0.05f) * (0.875f + (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f);

            Color glowColor = ColorHelper.AuroraColor(GoldLeafWorld.Timer * 0.05f); glowColor.A = 0;
            glowMask = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            glowMaskColor = glowColor * (0.3f - (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f);
        }*/

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.GoldLeaf.SetBonuses.FrostySet", player.GetModPlayer<FrostyPlayer>().frostyDamageBonus * 100, SetBonusKey);
            player.GetDamage(DamageClass.Magic) += player.GetModPlayer<FrostyPlayer>().frostyDamageBonus;
            player.GetModPlayer<FrostyPlayer>().frostySet = true;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;

            Item.value = Item.sellPrice(0, 0, 80, 0);
            Item.rare = ItemRarityID.Blue;

            Item.defense = 3;
            //Item.vanity = true;

            Item.faceSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Face);
            //Item.headSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);

            //ItemID.Sets.ShimmerTransformToItem[Item.type] = ItemType<FrigidMask>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<AuroraCluster>(), 16);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class FrostyRobe : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(manaMax, magicDamage * 100);

        private static readonly int manaMax = 40;
        private static readonly float magicDamage = 0.1f;

        public int frontEquip = -1;
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
                frontEquip = EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Waist}", EquipType.Front, this); //TODO draw this manually
            }
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            robes = true;

            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
        }

        public override void SetStaticDefaults()
        {
            ArmorIDs.Body.Sets.showsShouldersWhileJumping[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.HidesHands[Item.bodySlot] = false;
            ArmorIDs.Body.Sets.IncludedCapeFront[Item.bodySlot] = frontEquip;
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += manaMax;
            player.GetDamage(DamageClass.Magic) += magicDamage;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 40;

            Item.value = Item.sellPrice(0, 0, 80, 0);
            Item.rare = ItemRarityID.Blue;

            Item.defense = 5;
            //Item.vanity = true;

            Item.bodySlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
        }

        /*public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
        {
            //color = ColorHelper.AuroraColor(GoldLeafWorld.Timer * 0.05f);
            //color = ColorHelper.AuroraColor(GoldLeafWorld.Timer * 0.05f) * (0.875f + (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f);

            Color glowColor = ColorHelper.AuroraColor(GoldLeafWorld.Timer * 0.05f); glowColor.A = 0;
            glowMask = frontEquip;
            glowMaskColor = glowColor;
        }*/

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<FrostCloth>(), 8);
            recipe.AddIngredient(ItemType<AuroraCluster>(), 12);
            recipe.AddTile(TileID.Loom);
            recipe.Register();
        }
    }

    public class FrostyPlayer : ModPlayer 
    {
        public bool frostySet = false;
        public float frostyDamageBonus = 0.1f;
        public int frostyCooldown = 0;

        public override void Load()
        {
            GoldLeafPlayer.DoubleTapPrimaryEvent += SnapFreeze;
        }

        public override void ResetEffects()
        {
            frostySet = false;
        }

        public override void PostUpdateEquips()
        {
            if (frostySet && frostyCooldown > 0) frostyCooldown--;
        }
        
        private void SnapFreeze(Player player) 
        {
            if (player.GetModPlayer<FrostyPlayer>().frostySet && !player.HasBuff(BuffType<SnapFreezeBuff>()))
            {
                float num = 8000f;
                int target = -1;
                for (int i = 0; i < 200; i++)
                {
                    float distanceCheck = Vector2.Distance(player.MountedCenter, Main.npc[i].Center);
                    if (distanceCheck < num && distanceCheck < 750f && Vector2.Distance(Main.MouseWorld, Main.npc[i].Center) <= 50)
                    {
                        target = i;
                        num = distanceCheck;
                    }
                }
                if (target != -1 && IsTargetValid(Main.npc[target])) 
                {
                    Main.npc[target].AddBuff(BuffType<SnapFreezeBuff>(), TimeToTicks(10));

                    player.AddBuff(BuffType<SnapFreezeBuff>(), TimeToTicks(20));

                    for (float k = 0; k < 6.28f; k += 0.15f)
                        Dust.NewDustPerfect(player.MountedCenter, DustType<ArcticDust>(), Vector2.One.RotatedBy(k) * 2);

                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Deltarune/IceSpell") { Volume = 0.5f });

                    /*Vector2 value = Main.npc[target].Center - player.MountedCenter;
                    float num4 = 25f;
                    float num5 = (float)Math.Sqrt((double)(value.X * value.X + value.Y * value.Y));
                    if (num5 > num4)
                    {
                        num5 = num4 / num5;
                    }
                    value *= num5;

                    if (Main.myPlayer == Player.whoAmI)
                    {
                        SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Deltarune/IceSpell") { Volume = 0.35f });
                        //Projectile.NewProjectile(player.GetSource_FromThis(), player.MountedCenter, value, ProjectileType<SnapFreezeBeam>(), 0, 0f, player.whoAmI);
                    }*/
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!(target.HasBuff(BuffType<SnapFreezeBuff>()) && target.life > 0 && target.life <= target.lifeMax / 5 && !target.boss && Main.myPlayer == Player.whoAmI))
            {
                modifiers.ScalingBonusDamage += 0.2f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.HasBuff(BuffType<SnapFreezeBuff>()) && target.life > 0 && target.life <= target.lifeMax / 5 && !target.boss && Main.myPlayer == Player.whoAmI)
            {
                target.StrikeInstantKill();
                Projectile.NewProjectileDirect(Player.GetSource_OnHit(target), target.Center, new Vector2(0, -8.5f), ProjectileType<SnapFreezeEffect>(), 0, 0, Player.whoAmI);
                SoundEngine.PlaySound(SoundID.DeerclopsIceAttack /*with { Volume = 0.85f, Pitch = -0.3f, PitchVariance = 0.8f }*/);

                ReduceBuffTime(Player, BuffType<SnapFreezeBuff>(), TimeToTicks(10));

                int i = Item.NewItem(Player.GetSource_FromThis(), target.Center, ItemType<StarLarge>());
                Main.item[i].playerIndexTheItemIsReservedFor = Player.whoAmI;
            } 
            else if (target.life <= 0) 
            {
                ReduceBuffTime(Player, BuffType<SnapFreezeBuff>(), TimeToTicks(5));
            }
        }

        /*public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo) //wtf
        {
            Player player = drawInfo.drawPlayer;

            //base.ModifyDrawInfo(ref drawInfo);

            Texture2D tex = Request<Texture2D>("GoldLeaf/Items/Blizzard/Armor/FrostyMask_Head").Value;

            var data2 = new DrawData(
                    tex,
                    drawInfo.Position,
                    null,
                    new Color(255, 255, 255, 0),
                    0f,
                    tex.Size() / 2,
                    1,
                    SpriteEffects.None,
                    0
                )
            {
                shader = drawInfo.cHead
            };
            drawInfo.DrawDataCache.Add(data2);
        }*/
    }

    public class FrostyMaskLayer : PlayerDrawLayer
    {
        private static Asset<Texture2D> tex;
        public override void Load()
        {
            tex = Request<Texture2D>("GoldLeaf/Items/Blizzard/Armor/FrostyMask_Head");
        }

        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.FaceAcc);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo) //taken from slr
        {
            Player player = drawInfo.drawPlayer;

            if ((player.armor[0].type == ItemType<FrostyMask>() && player.armor[10].type == ItemID.None) || player.armor[10].type == ItemType<FrostyMask>())
            {
                int frame = (player.bodyFrame.Y / player.bodyFrame.Height);
                int height = (tex.Height() / 20);

                Vector2 pos = (player.MountedCenter - Main.screenPosition + new Vector2(0, player.gfxOffY - 3)).ToPoint16().ToVector2() + player.headPosition;

                drawInfo.DrawDataCache.Add(new DrawData(tex.Value, pos, new Rectangle(0, frame * height, tex.Width(), height),
                    drawInfo.colorArmorHead,
                    player.headRotation,
                    new Vector2(tex.Width() * 0.5f, tex.Height() * 0.025f),
                    1f, drawInfo.playerEffect, 0)
                {
                    shader = drawInfo.cHead
                });
            }
        }
    }

    public class SnapFreezeBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;

            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
        }
    }

    public class SnapFreezeNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private static Asset<Texture2D> maskTex;
        private static Asset<Texture2D> bloomTex;
        private static Asset<Texture2D> darkBloomTex;
        public override void Load()
        {
            maskTex = Request<Texture2D>("GoldLeaf/Items/Blizzard/Armor/SnapFreezeMask");
            bloomTex = Request<Texture2D>("GoldLeaf/Textures/Masks/Mask1");
            darkBloomTex = Request<Texture2D>("GoldLeaf/Textures/GlowBlack");
        }

        float snapFreezeTime = 0f;

        public override void ResetEffects(NPC npc)
        {
            if (!npc.HasBuff(BuffType<SnapFreezeBuff>())) 
                snapFreezeTime = 0f;
        }

        public override void PostAI(NPC npc)
        {
            if (npc.HasBuff(BuffType<SnapFreezeBuff>()))
                snapFreezeTime += 0.1f;

            /*if (!npc.HasBuff(BuffType<SnapFreezeBuff>()) && snapFreezeTime > 0)
            {
                snapFreezeTime = Math.Clamp(snapFreezeTime, 0, 1);
                snapFreezeTime -= 0.05f;
            }*/
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            Color color = ColorHelper.AuroraAccentColor(GoldLeafWorld.Timer * 0.05f);

            if (npc.HasBuff(BuffType<SnapFreezeBuff>())) drawColor = NPC.buffColor(drawColor, color.R/255f, color.G/255f, color.B/255f, 1f);
        }
        

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.Draw(darkBloomTex.Value, npc.Center - screenPos, new Rectangle(0, 0, darkBloomTex.Width(), darkBloomTex.Height()), Color.Black * (Math.Clamp(snapFreezeTime, 0, 5) / 5) * 1.1f, 0, darkBloomTex.Size() / 2, npc.scale * Math.Clamp(snapFreezeTime, 0, 1) * ((float)Math.Sin(GoldLeafWorld.rottime * 2) * 0.5f + 1f) * 0.75f, SpriteEffects.None, 0f);
            spriteBatch.Draw(bloomTex.Value, npc.Center - screenPos, new Rectangle(0, 0, bloomTex.Width(), bloomTex.Height()), ColorHelper.AuroraColor(GoldLeafWorld.Timer * 0.05f) with { A = 0 } * (Math.Clamp(snapFreezeTime, 0, 5)/5) * 0.75f, 0, bloomTex.Size() / 2, npc.scale * 0.5f * Math.Clamp(snapFreezeTime, 0, 1), SpriteEffects.None, 0f);
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.HasBuff(BuffType<SnapFreezeBuff>()))
            {
                //var effects = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                //Vector2 frame = new(TextureAssets.Npc[npc.type].Width() / 2, TextureAssets.Npc[npc.type].Height() / Main.npcFrameCount[npc.type] / 2);

                float bufftime = (float)npc.buffTime[npc.FindBuffIndex(BuffType<SnapFreezeBuff>())]/60/10;

                //spriteBatch.Draw(TextureAssets.Npc[npc.type].Value, npc.Center, npc.frame, new Color(0, 126, 179) { A = 0 } * (1 - Math.Clamp(snapFreezeTime, 0, 1)), npc.rotation, frame, npc.scale * (2f - Math.Clamp(snapFreezeTime, 0, 1)), effects, 0f);
                
                spriteBatch.Draw(maskTex.Value, npc.Top + new Vector2(0, -16) - screenPos, new Rectangle(0, 0, maskTex.Width(), maskTex.Height()), ColorHelper.AuroraColor(GoldLeafWorld.Timer * 0.25f) with { A = 0 }, 0, maskTex.Size()/2, (3.2f - Math.Clamp(snapFreezeTime*2, 0, 2)) + (float)(Math.Sin(GoldLeafWorld.rottime * 3f) * 0.1f), SpriteEffects.None, 0f);
                spriteBatch.Draw(maskTex.Value, npc.Top + new Vector2(0, -16) - screenPos, new Rectangle(0, 0, maskTex.Width(), maskTex.Height()), Color.White, 0, maskTex.Size() / 2, 3f - Math.Clamp(snapFreezeTime * 2, 0, 2), SpriteEffects.None, 0f);
                spriteBatch.Draw(maskTex.Value, npc.Top + new Vector2(0, -16) - screenPos, new Rectangle(0, 0, maskTex.Width(), maskTex.Height()), ColorHelper.AdditiveWhite * (1 - Math.Clamp(snapFreezeTime, 0, 1)), 0, maskTex.Size() / 2, 3f - Math.Clamp(snapFreezeTime*2, 0, 2), SpriteEffects.None, 0f);
                spriteBatch.Draw(maskTex.Value, npc.Top + new Vector2(0, -16 + (bufftime*2)) - screenPos, new Rectangle(0, 0, maskTex.Width(), maskTex.Height() - (int)(maskTex.Height() * bufftime)), Color.DimGray, 0, maskTex.Size()/2, 3f - Math.Clamp(snapFreezeTime*2, 0, 2), SpriteEffects.None, 0f);
            }
        }
    }

    public class SnapFreezeEffect : SafetyBlanketEffect 
    {
        public override string Texture => "GoldLeaf/Items/Blizzard/Armor/SnapFreezeMask";
    }

    /*public class SnapFreezeBeam : ModProjectile 
    {
        public override string Texture => EmptyTexString;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ShadowBeamFriendly);
            Projectile.damage = 0;
        }

        public override void AI()
        {
            Dust.NewDustPerfect(Projectile.Center, DustType<ArcticDust>());
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //target.AddBuff(BuffType<SnapFreezeBuff>(), TimeToTicks(10));
            Projectile.Kill();
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            return false;
        }
    }*/
}
