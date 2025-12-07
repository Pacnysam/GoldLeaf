using System;
using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using static GoldLeaf.Core.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using GoldLeaf.Items.Grove;
using System.Diagnostics.Metrics;
using GoldLeaf.Effects.Dusts;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using GoldLeaf.Items.Accessories;
using ReLogic.Content;
using GoldLeaf.Items.Blizzard.Armor;
using System.IO;
using Terraria.ModLoader.IO;
using static GoldLeaf.GoldLeaf;

namespace GoldLeaf.Items.Blizzard
{
    public class FrostNPC : GlobalNPC
    {
        private static Asset<Texture2D> numeralTex;
        public override void Load()
        {
            numeralTex = Request<Texture2D>("GoldLeaf/Items/Blizzard/FrozenNumerals");
        }

        public override bool InstancePerEntity => true;

        const int FREEZETIME = 240;

        public int frost;
        public int defrostTimer;
        
        public int frostVisualTime = -30;
        public float frozenNumeralSize = 1f;

        public override void SetDefaults(NPC entity)
        {
            entity.buffImmune[BuffType<ColdSlowBuff>()] = !entity.CanBeStunned();
            entity.buffImmune[BuffType<FreezeBuff>()] = !entity.CanBeStunned();
        }

        public override void PostAI(NPC npc)
        {
            frozenNumeralSize = (frozenNumeralSize > 1f ? frozenNumeralSize * 0.9f : 1f);

            if (frost >= 8 && !npc.buffImmune[BuffType<FreezeBuff>()])
            {
                //if (Main.netMode != NetmodeID.MultiplayerClient)
                    //npc.netUpdate = true;

                npc.AddBuff(BuffType<FreezeBuff>(), Math.Clamp(FREEZETIME - defrostTimer / 2, 30, FREEZETIME));
                
                if (Main.netMode != NetmodeID.Server) 
                {
                    if (defrostTimer <= 180)
                    {
                        SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/FrostFreeze") { Volume = 1.25f, PitchVariance = 0.4f }, npc.Center);
                        SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Frost") { Volume = 0.7f, PitchVariance = 0.6f }, npc.Center);
                    }
                    else
                        SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Monolith/Chop") { Volume = 0.45f, PitchVariance = 0.4f }, npc.Center);
                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Frost") { Volume = 0.7f, PitchVariance = 0.6f }, npc.Center);
                }

                defrostTimer = FREEZETIME * 2;
                frost = 0;

                /*for (float k = 0; k < 6.28f; k += (6.28f / 12))
                {
                    Dust dust = Dust.NewDustPerfect(npc.Center, DustType<FrostShard>(), Vector2.One.RotatedBy(k) * Main.rand.NextFloat(1f, 2f) + new Vector2(0, -Main.rand.NextFloat(1.5f, 2.5f)), 0, new Color(79, 180, 255), 1f);
                    dust.color = Color.White;
                    //dust.rotation = Main.rand.NextFloat(-18f, 18f);
                    //dust.noGravity = true;
                    //dust.customData = npc;
                }*/

                int smokeCount = Main.rand.Next(40, 60);
                for (int j = 0; j < smokeCount; j++)
                {
                    Dust dust = Dust.NewDustDirect(npc.Center, 0, 0, DustType<SnowCloud>());
                    dust.velocity = Main.rand.NextVector2Circular((npc.width / 5.5f), (npc.height / 6f)) * Main.rand.NextFloat(0.5f, 1f);
                    dust.scale = Main.rand.NextFloat(0.65f, 1.15f);
                    dust.alpha = 40 + Main.rand.Next(80);
                    dust.rotation = Main.rand.NextFloat(-6.28f, 6.28f);
                }
            }
            else if (defrostTimer > 0 && !npc.HasBuff(BuffType<FreezeBuff>()))
            {
                defrostTimer--;
            }
            if (defrostTimer < 0) 
            {
                defrostTimer = 0;
            }

            frostVisualTime--;
        }

        public static void AddFrost(NPC npc, int amount = 1)
        {
            if (!npc.HasBuff(BuffType<FreezeBuff>()) && !npc.buffImmune[BuffType<FreezeBuff>()])
            {
                npc.GetGlobalNPC<FrostNPC>().frost += amount;
                
                npc.GetGlobalNPC<FrostNPC>().frozenNumeralSize = Math.Clamp(npc.GetGlobalNPC<FrostNPC>().frost / 2f, 1.5f, 3f);
                npc.GetGlobalNPC<FrostNPC>().frostVisualTime = TimeToTicks(3);

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    ModPacket packet = GoldLeaf.Instance.GetPacket();
                    packet.Write((byte)MessageType.FrostSync);
                    packet.Write((byte)Main.myPlayer);
                    packet.Write((byte)npc.whoAmI);

                    packet.Write((byte)npc.GetGlobalNPC<FrostNPC>().frost);
                    packet.Write((byte)npc.GetGlobalNPC<FrostNPC>().defrostTimer);
                    packet.Send();
                }
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.HasBuff(BuffType<ColdSlowBuff>())) 
            {
                drawColor = NPC.buffColor(drawColor, 79f / 255, 180f / 255, 1f, 1f); 
            }
            if (npc.HasBuff(BuffType<FreezeBuff>()))
            {
                drawColor = NPC.buffColor(drawColor, 30f / 255, 90f / 255, 192f / 255, 1f);
            }
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int frostAmount = ((!npc.HasBuff(BuffType<FreezeBuff>()) && frost > 0) && frostVisualTime > -30 ? frost : 8);
            Rectangle rect = new(0, frostAmount * 16, 32, 16);

            float yOffset = npc.HasBuff(BuffType<SnapFreezeBuff>()) ? 6 : 0;

            if ((!npc.HasBuff(BuffType<FreezeBuff>()) && frost > 0) || frostVisualTime > -30)
            {
                spriteBatch.Draw(numeralTex.Value, npc.Top + new Vector2(0, -16 + yOffset) - screenPos, rect, Color.White with { A = 160 } * MathHelper.Clamp(frost, 0.625f + (float)(Math.Sin(GoldLeafWorld.rottime) * 0.125f), 1f) * MathHelper.Clamp((frostVisualTime + 30) / 60f, 0f, 1f), 0, rect.Size() / 2, frozenNumeralSize + (float)-(Math.Sin(GoldLeafWorld.rottime) * 0.1f), SpriteEffects.None, 0f);
            }
        }
    }

    public class ColdSlowBuff : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff_" + BuffID.Chilled;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;

            BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<GoldLeafNPC>().movementSpeed *= 0.5f;

            if (Main.rand.NextBool(8))
            {
                Dust dust = Dust.NewDustDirect(npc.Center, 0, 0, DustType<SnowCloud>());
                dust.velocity = Main.rand.NextVector2Circular((npc.width / 4.5f), (npc.height / 5f)) * Main.rand.NextFloat(0.2f, 0.4f);
                dust.scale = Main.rand.NextFloat(0.95f, 1.65f);
                dust.alpha = 80 + Main.rand.Next(60);
                dust.rotation = Main.rand.NextFloat(-6.28f, 6.28f);
            }
        }
    }

    public class FreezeBuff : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff_" + BuffID.Frozen;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;

            BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<GoldLeafNPC>().movementSpeed *= 0f;
            npc.GetGlobalNPC<GoldLeafNPC>().stunned = true;

            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(npc.Center, 0, 0, DustType<SnowCloud>());
                dust.velocity = Main.rand.NextVector2Circular((npc.width / 4.5f), (npc.height / 5f)) * Main.rand.NextFloat(0.2f, 0.4f);
                dust.scale = Main.rand.NextFloat(0.95f, 1.65f);
                dust.alpha = 70 + Main.rand.Next(40);
                dust.rotation = Main.rand.NextFloat(-6.28f, 6.28f);
            }
        }
    }
}
