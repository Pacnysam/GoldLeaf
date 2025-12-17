using GoldLeaf.Core.Helpers;
using GoldLeaf.Items.Nightshade;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Biomes;

namespace GoldLeaf.NPCs.Night.Nightshade
{
    public class NightshadeTest : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 10000;
            NPC.defense = 20;
            NPC.value = Item.buyPrice(0, 1, 0, 0);
            NPC.knockBackResist = 0f;
            NPC.width = 54; NPC.height = 30;
            NPC.damage = 1;
            NPC.lavaImmune = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit52;
            NPC.DeathSound = SoundID.Item68;
            NPC.color = Color.DeepSkyBlue;
            NPC.noGravity = true;

            SpawnModBiomes = [GetInstance<WhisperingGroveSurface>().Type];
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];

            NPC.TargetClosest(true);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemType<VampireBat>()));
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Texture2D alphaBloom = Request<Texture2D>("GoldLeaf/Textures/GlowAlpha").Value;

            spriteBatch.Draw(alphaBloom, NPC.Center - Main.screenPosition, null, NPC.color with { A = 0 } * 0.5f, NPC.rotation, alphaBloom.Size() / 2, NPC.scale * 0.875f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, NPC.color with { A = 200 }, NPC.rotation, texture.Size() / 2, NPC.scale + 0.1f, SpriteEffects.None, 0f);
            spriteBatch.StartBlendState(DrawHelper.SubtractiveBlend);
            for (int k = 0; k < 8; k++)
            {
                spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, NPC.color * 0.7f, NPC.rotation, texture.Size() / 2, NPC.scale * (1f - k * 0.075f), SpriteEffects.None, 0f);
            }
            spriteBatch.ResetBlendState();
            return false;
        }
    }
}
