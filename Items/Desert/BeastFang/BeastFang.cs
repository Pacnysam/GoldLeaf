using GoldLeaf.Core;
using GoldLeaf.Core.CrossMod;
using GoldLeaf.Core.Mechanics;
using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Sky;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static GoldLeaf.Core.CrossMod.RedemptionHelper;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Desert.BeastFang
{
	public class BeastFang : ModItem
	{
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(BeastFangBuff.TagDamage);

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ExtractinatorMode[Type] = ItemID.DesertFossil;
        }
        public override void SetDefaults()
		{
            Item.width = 22;
            Item.height = 24;

            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;

            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<BeastFangPlayer>().beastFang = true;
        }

        public override void UpdateInventory(Player player)
        {
            if (!player.GetModPlayer<BeastFangPlayer>().hasObtainedBeastFang)
                player.GetModPlayer<BeastFangPlayer>().hasObtainedBeastFang = true;
        }
    }

    public class BeastFangBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);
        public static int TagDamage => 4;

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            SummonTagNPC summonTagNPC = npc.GetGlobalNPC<SummonTagNPC>();
            summonTagNPC.tagDamage += TagDamage;
        }
    }

    public class BeastFangPlayer : ModPlayer
    {
        public bool beastFang = false;
        public bool hasObtainedBeastFang = false;

        public override void Load() => On_Main.DrawInterface_1_2_DrawEntityMarkersInWorld += NonSummonTagMarker;
        public override void Unload() => On_Main.DrawInterface_1_2_DrawEntityMarkersInWorld -= NonSummonTagMarker;

        private void NonSummonTagMarker(On_Main.orig_DrawInterface_1_2_DrawEntityMarkersInWorld orig)
        {
            if (!Main.LocalPlayer.dead && Main.LocalPlayer.HeldItem.IsWeapon() && !Main.LocalPlayer.HeldItem.CountsAsClass(DamageClass.Summon) && Main.LocalPlayer.GetModPlayer<BeastFangPlayer>().beastFang)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.ZoomMatrix);

                Texture2D texture = TextureAssets.Extra[ExtrasID.WhipMarkers].Value;
                int minionAttackTargetNPC = Main.LocalPlayer.MinionAttackTargetNPC;
                Rectangle screenRect = new((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);

                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.active && npc.Hitbox.Intersects(screenRect))
                    {
                        Vector2 vector = npc.Center - Main.screenPosition;
                        if (Main.LocalPlayer.gravDir == -1f)
                        {
                            vector.Y = (float)Main.screenHeight - vector.Y;
                        }
                        Vector2 position = vector + Vector2.Zero;
                        if (npc.whoAmI == minionAttackTargetNPC)
                        {
                            int frameY = (int)(Main.GlobalTimeWrappedHourly * 10f) % 4;
                            Rectangle rectangle = texture.Frame(1, 4, 0, frameY, 0, -2);
                            Vector2 origin = rectangle.Size() / 2f;
                            Color color = Color.White * 0.7f;
                            color.A /= 2;
                            Main.spriteBatch.Draw(texture, position, rectangle, color, 0f, origin, 1f, SpriteEffects.None, 0f);
                        }
                    }
                }
            }

            orig?.Invoke();
        }

        public override void ResetEffects() => beastFang = false;

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (beastFang && !item.CountsAsClass(DamageClass.Summon))
            {
                target.AddBuff(BuffType<BeastFangBuff>(), TimeToTicks(5));
                Player.MinionAttackTargetNPC = target.whoAmI;
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (beastFang && !(proj.DamageType.CountsAsClass(DamageClass.Summon) || proj.IsMinionOrSentryRelated || ProjectileID.Sets.IsAWhip[proj.type]))
            {
                target.AddBuff(BuffType<BeastFangBuff>(), TimeToTicks(5));
                Player.MinionAttackTargetNPC = target.whoAmI;
            }
        }
        
        public override void Initialize() => hasObtainedBeastFang = false;
        public override void SaveData(TagCompound tag) => tag["hasObtainedBeastFang"] = hasObtainedBeastFang;
        public override void LoadData(TagCompound tag) => hasObtainedBeastFang = tag.GetBool("hasObtainedBeastFang");
    }
}