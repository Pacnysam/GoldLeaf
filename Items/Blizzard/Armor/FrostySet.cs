using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System.Diagnostics.Metrics;
using System;
using GoldLeaf.Effects.Dusts;
using Terraria.DataStructures;
using ReLogic.Content;
using Terraria.Localization;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using GoldLeaf.Items.Pickups;
using Terraria.Graphics.Renderers;
using static GoldLeaf.GoldLeaf;

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

        public override void ArmorSetShadows(Player player) => player.armorEffectDrawOutlines = !player.HasBuff(BuffType<SnapFreezeBuff>());

        public override void SetStaticDefaults()
        {
            ArmorSets.FaceMask[Item.headSlot] = true;

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
            //color = ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 0.05f);
            color = ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 0.05f) * (0.875f + (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f);

            Color glowColor = ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 0.05f); glowColor.A = 0;
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

            Item.defense = 5;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<AuroraCluster>(), 16);
            recipe.AddTile(TileID.Anvils);
            recipe.AddOnCraftCallback(RecipeCallbacks.AuroraMajor);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Body, EquipType.Waist)]
    public class FrostyRobe : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(manaMax, magicDamage * 100);

        private static readonly int manaMax = 40;
        private static readonly float magicDamage = 0.12f;

        //public int frontEquip = -1;
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
                //frontEquip = EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Waist}", EquipType.Front, this); //TODO draw this manually
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
            //ArmorIDs.Body.Sets.IncludedCapeFront[Item.bodySlot] = frontEquip;
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
            //color = ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 0.05f);
            //color = ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 0.05f) * (0.875f + (float)Math.Sin(GoldLeafWorld.rottime) * 0.125f);

            Color glowColor = ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 0.05f); glowColor.A = 0;
            glowMask = frontEquip;
            glowMaskColor = glowColor;
        }*/

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<FrostCloth>(), 8);
            recipe.AddIngredient(ItemType<AuroraCluster>(), 12);
            recipe.AddTile(TileID.Loom);
            recipe.AddOnCraftCallback(RecipeCallbacks.AuroraMajor);
            recipe.Register();
        }
    }
    
    public class FrostyPlayer : ModPlayer 
    {
        public bool frostySet = false;
        public float frostyDamageBonus = 0.1f;
        //public int frostyCooldown = 0;

        public override void Load()
        {
            ControlsPlayer.DoubleTapPrimaryEvent += SnapFreeze;
        }
        public override void Unload()
        {
            ControlsPlayer.DoubleTapPrimaryEvent -= SnapFreeze;
        }

        public override void ResetEffects()
        {
            frostySet = false;
        }

        /*public override void PostUpdateEquips()
        {
            if (frostySet && frostyCooldown > 0) frostyCooldown--;
        }*/
        
        private static void SnapFreeze(Player player) 
        {
            if (player.GetModPlayer<FrostyPlayer>().frostySet && !player.HasBuff(BuffType<SnapFreezeBuff>()))
            {
                float num = 8000f;
                int target = -1;
                for (int i = 0; i < 200; i++)
                {
                    float distanceCheck = Vector2.Distance(player.MountedCenter, Main.npc[i].Center);
                    if (distanceCheck < num && distanceCheck < 900f && Vector2.Distance(Main.MouseWorld, Main.npc[i].Center) <= 200)
                    {
                        target = i;
                        num = distanceCheck;
                    }
                }
                if (target != -1 && IsTargetValid(Main.npc[target])) 
                {
                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        ModPacket packet = Instance.GetPacket();
                        packet.Write((byte)MessageType.SnapFreeze);
                        packet.Write((byte)player.whoAmI);
                        packet.Send(-1, player.whoAmI);
                    }

                    Main.npc[target].AddBuff(BuffType<SnapFreezeBuff>(), TimeToTicks(10));

                    player.AddBuff(BuffType<SnapFreezeBuff>(), TimeToTicks(15));

                    float seed = Main.rand.NextFloat(0f, 8f);

                    for (float k = 0; k < 6.28f; k += (6.28f / 24))
                    {
                        Dust dust = Dust.NewDustPerfect(player.MountedCenter, DustType<AuroraTwinkle>(), Vector2.One.RotatedBy(k) * 6f /* (float)((Math.Sin(k * 3)/2) + 1f)*/, 5, ColorHelper.AuroraAccentColor(seed + (k * 5.4f)), Main.rand.NextFloat(0.6f, 0.9f));
                        dust.rotation = Main.rand.NextFloat(-18f, 18f);
                        dust.noLight = true;
                        dust.customData = player;
                    }
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.MountedCenter, Vector2.Zero, ProjectileType<AuroraStar>(), 0, 0, Main.myPlayer, 3.6f, 0.9f);
                        proj.rotation = Main.rand.NextFloat(-0.015f, 0.015f);
                    }

                    if (Main.netMode != NetmodeID.Server)
                        SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Reflect") { Volume = 0.9f }, player.Center);
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.HasBuff(BuffType<SnapFreezeBuff>()))
            {
                modifiers.ArmorPenetration += 5;
                modifiers.ScalingBonusDamage += 0.25f;
            }
            if (target.HasBuff(BuffType<SnapFreezeBuff>()) && (target.life) <= target.lifeMax / 4 && !target.boss)
            {
                Projectile.NewProjectileDirect(Player.GetSource_OnHit(target), target.Center, new Vector2(0, -8.5f), ProjectileType<SnapFreezeEffect>(), 0, 0, Player.whoAmI);
                target.RequestBuffRemoval(BuffType<SnapFreezeBuff>());

                if (Main.netMode != NetmodeID.Server)
                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/IceSmash") { Volume = 1f, Pitch = 0.2f, PitchVariance = 0.2f });

                ReduceBuffTime(Player, BuffType<SnapFreezeBuff>(), TimeToTicks(10));

                int i = Item.NewItem(Player.GetSource_Loot(), target.Center, ItemType<StarLarge>(), 1, true, 0, true);
                Main.item[i].playerIndexTheItemIsReservedFor = Player.whoAmI;
                Main.item[i].timeSinceTheItemHasBeenReservedForSomeone = 0;

                if (Main.netMode == NetmodeID.MultiplayerClient && i >= 0)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 1f);

                modifiers.SetInstantKill();
            }
        }

        /*public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.active)
            {
                ReduceBuffTime(Player, BuffType<SnapFreezeBuff>(), TimeToTicks(3));
            }
        }*/

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
            BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<GoldLeafNPC>().movementSpeed *= 0.75f;
        }
    }
    
    public class SnapFreezeNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private static Asset<Texture2D> maskTex;
        private static Asset<Texture2D> darkMaskTex;
        private static Asset<Texture2D> bloomTex;
        private static Asset<Texture2D> darkBloomTex;
        public override void Load()
        {
            maskTex = Request<Texture2D>("GoldLeaf/Items/Blizzard/Armor/SnapFreezeMask");
            darkMaskTex = Request<Texture2D>("GoldLeaf/Items/Blizzard/Armor/SnapFreezeMaskDark");
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
            { 
                snapFreezeTime += 0.1f;
                
                //Color color = ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 0.05f) * ((Math.Clamp(snapFreezeTime, 0, 5) / 5) * 0.35f);
                //Lighting.AddLight((int)npc.Center.X/16, (int)npc.Center.Y/16, color.R / 255, color.G / 255, color.B / 255);
            }

            /*if (!npc.HasBuff(BuffType<SnapFreezeBuff>()) && snapFreezeTime > 0)
            {
                snapFreezeTime = Math.Clamp(snapFreezeTime, 0, 1);
                snapFreezeTime -= 0.05f;
            }*/
        }

        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            if (npc.HasBuff(BuffType<SnapFreezeBuff>()) && npc.life > 0 && Main.netMode != NetmodeID.Server)
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, npc.Center);
            }
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (npc.HasBuff(BuffType<SnapFreezeBuff>()) && npc.life > 0)
            {
                Projectile proj = Projectile.NewProjectileDirect(npc.GetSource_OnHurt(player), npc.Center, Vector2.Zero, ProjectileType<AuroraStar>(), 0, 0, -1, npc.scale, 0.875f);
                proj.rotation = Main.rand.NextFloat(-0.015f, 0.015f);
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (npc.HasBuff(BuffType<SnapFreezeBuff>()) && npc.life > 0 )//&& Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile proj = Projectile.NewProjectileDirect(npc.GetSource_OnHurt(Main.player[projectile.owner]), npc.Center, Vector2.Zero, ProjectileType<AuroraStar>(), 0, 0, -1, npc.scale, 0.875f);
                proj.rotation = Main.rand.NextFloat(-0.015f, 0.015f);
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            Color color = ColorHelper.AuroraAccentColor(Main.GlobalTimeWrappedHourly * 3f);

            if (npc.HasBuff(BuffType<SnapFreezeBuff>())) 
            { 
                drawColor = NPC.buffColor(drawColor, color.R / 255f, color.G / 255f, color.B / 255f, 1f);
            }
        }
        
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.Draw(darkBloomTex.Value, npc.Center - screenPos, new Rectangle(0, 0, darkBloomTex.Width(), darkBloomTex.Height()), Color.Black * (Math.Clamp(snapFreezeTime, 0, 5) / 5) * 1.1f, 0, darkBloomTex.Size() / 2, npc.scale * Math.Clamp(snapFreezeTime, 0, 1) * (((float)Math.Sin(GoldLeafWorld.rottime * 2f) * 0.4f) + 1f) * 1.25f, SpriteEffects.None, 0f);
            spriteBatch.Draw(bloomTex.Value, npc.Center - screenPos, new Rectangle(0, 0, bloomTex.Width(), bloomTex.Height()), ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 3) with { A = 0 } * (Math.Clamp(snapFreezeTime, 0, 5)/5) * 0.75f, 0, bloomTex.Size() / 2, npc.scale * 0.5f * Math.Clamp(snapFreezeTime, 0, 1) * (((float)Math.Sin(GoldLeafWorld.rottime * 2f) * 0.4f) + 1f), SpriteEffects.None, 0f);
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.HasBuff(BuffType<SnapFreezeBuff>()))
            {
                float yOffset = (npc.GetGlobalNPC<FrostNPC>().frostVisualTime > -30) ? -18 : 0;
                float bufftime = (float)npc.buffTime[npc.FindBuffIndex(BuffType<SnapFreezeBuff>())]/60/10;

                spriteBatch.Draw(maskTex.Value, npc.Top + new Vector2(0, -16 + yOffset) - screenPos, new Rectangle(0, 0, maskTex.Width(), maskTex.Height()), ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 15) with { A = 0 }, 0, maskTex.Size()/2, (3.2f - Math.Clamp(snapFreezeTime*2, 0, 2)) + (float)(Math.Sin(GoldLeafWorld.rottime * 3f) * 0.1f), SpriteEffects.None, 0f);
                spriteBatch.Draw(maskTex.Value, npc.Top + new Vector2(0, -16 + yOffset) - screenPos, new Rectangle(0, 0, maskTex.Width(), maskTex.Height()), Color.White, 0, maskTex.Size() / 2, 3f - Math.Clamp(snapFreezeTime * 2, 0, 2), SpriteEffects.None, 0f);
                spriteBatch.Draw(maskTex.Value, npc.Top + new Vector2(0, -16 + yOffset) - screenPos, new Rectangle(0, 0, maskTex.Width(), maskTex.Height()), ColorHelper.AdditiveWhite() * (1 - Math.Clamp(snapFreezeTime, 0, 1)), 0, maskTex.Size() / 2, 3f - Math.Clamp(snapFreezeTime*2, 0, 2), SpriteEffects.None, 0f);
                spriteBatch.Draw(darkMaskTex.Value, npc.Top + new Vector2(0, -16 + yOffset) - screenPos, new Rectangle(0, 0, maskTex.Width(), maskTex.Height() - (int)(maskTex.Height() * bufftime)), Color.White, 0, maskTex.Size()/2, 3f - Math.Clamp(snapFreezeTime*2, 0, 2), SpriteEffects.None, 0f);
            }
        }
    }

    public class SnapFreezeEffect : SafetyBlanketEffect 
    {
        public override string Texture => "GoldLeaf/Items/Blizzard/Armor/SnapFreezeMask";
    }

    public class AuroraStar : ModProjectile //TODO: this should not be a projectile
    {
        private static Asset<Texture2D> darkBloomTex;
        public override void Load()
        {
            darkBloomTex = Request<Texture2D>("GoldLeaf/Textures/GlowBlack");
        }

        public override string Texture => "GoldLeaf/Textures/Flares/FlareSmall";

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;

            Projectile.damage = 0;

            Projectile.scale = 0f;
            Projectile.timeLeft = 20;

            Projectile.ai[1] = 0.9f;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.ai[0] *= Projectile.ai[1];
            Projectile.rotation += Projectile.rotation * 0.94f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color color = ColorHelper.AuroraColor(Main.GlobalTimeWrappedHourly * 15); color.A = 0;
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(darkBloomTex.Value, Projectile.Center - Main.screenPosition, null, Color.Black * Projectile.ai[0] * 0.8f, Projectile.rotation, darkBloomTex.Size() / 2, Projectile.ai[0] * 0.7f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, color * Projectile.ai[0], Projectile.rotation, tex.Size() / 2, Projectile.ai[0], SpriteEffects.None, 0f);
            return false;
        }
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
