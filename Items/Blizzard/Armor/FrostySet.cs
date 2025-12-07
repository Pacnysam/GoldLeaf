using GoldLeaf.Core;
using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Dyes;
using GoldLeaf.Items.Pickups;
using GoldLeaf.Items.Vanity.Watcher;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using static GoldLeaf.GoldLeaf;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Blizzard.Armor
{   //TODO: Rework set bonus
    [AutoloadEquip(EquipType.Head)]
    public class FrostyMask : ModItem
    {
        public override string Texture => "GoldLeaf/Items/Blizzard/Armor/FrostyMask";
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(manaMax);

        private static readonly int manaMax = 20;

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
            ItemSets.FaceMask[Item.type] = true;

            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
            ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += manaMax;
            player.aggro -= 400;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<FrostyRobe>() && legs.type == ItemType<FrostyBoots>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.GoldLeaf.SetBonuses.FrostySet", SetBonusKey);
            player.GetModPlayer<FrostyPlayer>().frostySet = true;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;

            Item.value = Item.sellPrice(0, 0, 80, 0);
            Item.rare = ItemRarityID.Blue;

            Item.defense = 3;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<AuroraCluster>(), 12);
            recipe.AddTile(TileID.Anvils);
            recipe.AddOnCraftCallback(RecipeCallbacks.AuroraMajor);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class FrostyRobe : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(manaMax, magicDamage);

        private static readonly int manaMax = 20;
        private static readonly int magicDamage = 3;

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
                EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Waist}", EquipType.Waist, this, "FrostyBelt");
                backSlot = EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Back}", EquipType.Back, this, "FrostyBack");
            }
        }
        public int backSlot = -1;

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                ItemSets.BodyExtra[Item.type] = (Request<Texture2D>(EquipLoader.GetEquipTexture(EquipType.Waist, Item.waistSlot).Texture), default, false);

            ArmorIDs.Body.Sets.showsShouldersWhileJumping[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.HidesHands[Item.bodySlot] = false;
            ArmorIDs.Body.Sets.IncludedCapeBack[Item.bodySlot] = backSlot;
            ArmorIDs.Body.Sets.IncludedCapeBackFemale[Item.bodySlot] = backSlot;
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += manaMax;
            player.GetDamage(DamageClass.Magic).Flat += magicDamage;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 40;

            Item.value = Item.sellPrice(0, 0, 80, 0);
            Item.rare = ItemRarityID.Blue;

            Item.defense = 3;

            Item.bodySlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            Item.waistSlot = EquipLoader.GetEquipSlot(Mod, "FrostyBelt", EquipType.Waist);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<FrostCloth>(), 8);
            recipe.AddIngredient(ItemType<AuroraCluster>(), 10);
            recipe.AddTile(TileID.Loom);
            recipe.AddOnCraftCallback(RecipeCallbacks.AuroraMajor);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class FrostyBoots : ModItem
    {
        public override string Texture => "GoldLeaf/Items/Blizzard/Armor/FrostyBoots";
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(manaMax, critDamage);

        private static readonly int manaMax = 20;
        private static readonly int critDamage = 25;

        public override void SetStaticDefaults()
        {
            ArmorIDs.Legs.Sets.HidesBottomSkin[Item.legSlot] = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += manaMax;
            player.GetModPlayer<GoldLeafPlayer>().magicCritDamageMod += critDamage * 0.01f;
            player.GetModPlayer<FrostyPlayer>().frostyBoots = true;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;

            Item.value = Item.sellPrice(0, 0, 80, 0);
            Item.rare = ItemRarityID.Blue;

            Item.defense = 3;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<AuroraCluster>(), 10);
            recipe.AddTile(TileID.Anvils);
            recipe.AddOnCraftCallback(RecipeCallbacks.AuroraMajor);
            recipe.Register();
        }
    }

    public class FrostyPlayer : ModPlayer 
    {
        public bool frostyBoots = false;
        public bool frostySet = false;
        public int dustCooldown = 0;
        public bool frostyBoot => Player.armor[2].type == ItemType<FrostyBoots>() && Player.armor[11].type == ItemID.None || Player.armor[12].type == ItemType<FrostyBoots>();

        public override void PreUpdate()
        {
            if (frostyBoot && dustCooldown > 0) dustCooldown--;
        }

        public override void ResetEffects()
        {
            frostySet = false;
        }

        public override void Load() => ControlsPlayer.DoubleTapPrimaryEvent += SnapFreeze;
        public override void Unload() => ControlsPlayer.DoubleTapPrimaryEvent -= SnapFreeze;

        public override void PostUpdateMiscEffects()
        {
            if (frostyBoot && dustCooldown <= 0 && Player.statMana > 0 && Player.velocity.Y == 0f && Player.grappling[0] == -1 && Math.Abs(Player.velocity.X) >= 3.2f)
            {
                Vector2 position = (Player.velocity.X > 0)? Player.BottomLeft : Player.BottomRight;
                float manaPercent = (float)Player.statMana / Player.statManaMax2;
                float manaRatio = MathHelper.Clamp((float)Player.statManaMax2 / Player.statMana, 0f, 20f);

                Dust dust = Dust.NewDustPerfect(position, DustType<TwinkleDust>(), new Vector2(0, Main.rand.NextFloat(-2f, -0.5f)), Main.rand.Next(0, 80), Color.White, Main.rand.NextFloat(0.85f, 1.25f) * (manaPercent * 0.5f + 0.5f));
                dust.customData = new LightDust.LightDustData(Main.rand.NextFloat(0.9f, 0.935f), MathHelper.ToRadians(Main.rand.NextFloat(-4f, 4f)));
                dust.velocity.Y -= Main.rand.NextFloat(0.25f, 1.45f);
                dust.velocity.X += Main.rand.NextFloat(2.5f, 3f) * (Player.velocity.X < 0).ToDirectionInt();
                dust.fadeIn = Main.rand.NextFloat(1.5f, 2.5f);
                dust.shader = (Player.dye[2].dye != 0)? GameShaders.Armor.GetShaderFromItemId(Player.dye[2].type) : GameShaders.Armor.GetShaderFromItemId(ItemType<AuroraDye>());

                dustCooldown = (int)(Main.rand.Next(1, 4) + manaRatio);
            }
        }

        private static void SnapFreeze(Player player) 
        {
            if (player.GetModPlayer<FrostyPlayer>().frostySet && !player.HasBuff(BuffType<SnapFreezeBuff>()))
            {
                float num = 8000f;
                int target = -1;

                foreach (NPC npc in Main.ActiveNPCs)
                {
                    int npcID = npc.whoAmI;

                    float distanceCheck = Vector2.Distance(Main.MouseWorld, Main.npc[npcID].Center);
                    if (distanceCheck < num && distanceCheck < 250f && Vector2.Distance(player.MountedCenter, Main.npc[npcID].Center) <= 1000 && !Main.npc[npcID].HasBuff<SnapFreezeBuff>() && IsTargetValid(Main.npc[npcID]) && Main.npc[npcID].lifeMax >= 25)
                    {
                        target = npcID;
                        num = distanceCheck;
                    }
                }
                if (target != -1) 
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
                modifiers.ArmorPenetration += 20;
                //modifiers.ScalingBonusDamage += 0.25f;
            }
            if (target.HasBuff(BuffType<SnapFreezeBuff>()) && (target.life) <= target.lifeMax / 5 && !target.boss)
            {
                Projectile.NewProjectileDirect(Player.GetSource_OnHit(target), target.Center, new Vector2(0, -8.5f), ProjectileType<SnapFreezeEffect>(), 0, 0, Player.whoAmI);
                target.RequestBuffRemoval(BuffType<SnapFreezeBuff>());

                if (Main.netMode != NetmodeID.Server)
                {
                    //SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/ColdChime") { Volume = 0.3f, PitchVariance = 0.2f });
                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Freeze") { PitchVariance = 0.2f });
                    //SoundEngine.PlaySound(SoundID.DeerclopsIceAttack with { Volume = 0.35f, PitchVariance = 0.2f });
                    //SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/IceSmash") { Volume = 1.15f, Pitch = 0.35f, PitchVariance = 0.2f });
                }
                //Player.ClearBuff(BuffType<SnapFreezeBuff>());
                Player.ReduceBuffTime(BuffType<SnapFreezeBuff>(), TimeToTicks(8));
                
                int i = Item.NewItem(Player.GetSource_Loot(), target.Center, ItemType<StarLarge>(), 1, true, 0, true);
                Main.item[i].playerIndexTheItemIsReservedFor = Player.whoAmI;
                Main.item[i].timeSinceTheItemHasBeenReservedForSomeone = 0;

                if (Main.netMode == NetmodeID.MultiplayerClient && i >= 0)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 1f);

                modifiers.SetInstantKill();
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
}
