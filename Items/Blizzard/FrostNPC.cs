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
using GoldLeaf.Items.Misc.Accessories;
using ReLogic.Content;

namespace GoldLeaf.Items.Blizzard
{
    public class FrostNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        const int FREEZETIME = 240;

        public int frost = 0;
        public int defrostTimer = 0;

        public override void SetDefaults(NPC entity)
        {
            entity.buffImmune[BuffID.Chilled] = !GoldLeafNPC.CanBeStunned(entity);
            entity.buffImmune[BuffID.Frozen] = !GoldLeafNPC.CanBeStunned(entity);
        }

        public override void PostAI(NPC npc)
        {
            if (frost >= 8 && !npc.buffImmune[BuffID.Frozen])
            { 
                npc.AddBuff(BuffID.Frozen, FREEZETIME - defrostTimer);
                
                if (defrostTimer <= 180)
                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Frost") { Volume = 1.15f, PitchVariance = 0.6f }, npc.Center);
                else
                    SoundEngine.PlaySound(new SoundStyle("GoldLeaf/Sounds/SE/Monolith/Chop") { Volume = 0.75f, PitchVariance = 0.4f }, npc.Center);

                defrostTimer = FREEZETIME;
                frost = 0;
            }
            else if (defrostTimer > 0 && !npc.HasBuff(BuffID.Frozen))
            {
                defrostTimer--;
            }
            if (defrostTimer < 0) 
            {
                defrostTimer = 0;
            }
        }

        public static void AddFrost(NPC npc, int amount)
        {
            if (!npc.HasBuff(BuffID.Frozen))
                npc.GetGlobalNPC<FrostNPC>().frost += amount;
        }
        public static void AddFrost(NPC npc)
        {
            if (!npc.HasBuff(BuffID.Frozen))
                npc.GetGlobalNPC<FrostNPC>().frost++;
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.HasBuff(BuffID.Chilled)) drawColor = NPC.buffColor(drawColor, 79f / 255, 180f / 255, 1f, 1f);
            if (npc.HasBuff(BuffID.Frozen)) drawColor = NPC.buffColor(drawColor, 30f / 255, 90f / 255, 192f / 255, 1f);
        }
    }
}
