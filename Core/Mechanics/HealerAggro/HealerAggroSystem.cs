using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core.Mechanics.HealerAggro
{
    public class HealerAggroPlayer : ModPlayer 
    {
        public int healingAggro = 0;
        public static int MaxHealingAggro => 1000;
        public static int PersecutionDamageMult => 25;
        public int HealingAggroStage 
        {
            get
            {
                if (Player.HasBuff(BuffType<HealerAggroBuff>()))
                {
                    if (healingAggro >= MaxHealingAggro) return 3;
                    else if (healingAggro >= (MaxHealingAggro * 0.5f)) return 2;
                    else return 1;
                }
                else return 0;
            }
        }

        public override void PreUpdateBuffs() => healingAggro = Math.Clamp(healingAggro, 0, MaxHealingAggro);
        public override void ResetEffects()
        {
            if (!Player.HasBuff<HealerAggroBuff>())
                healingAggro = 0;
        }

        public static void AddHealingAggro(Player player, int amount = 10, int buffTime = 600)
        {
            player.AddBuff(BuffType<HealerAggroBuff>(), buffTime);
            player.GetModPlayer<HealerAggroPlayer>().healingAggro += amount;
        }
    }

    public class HealerAggroBuff : ModBuff
    {
        public override string Texture => CoolBuffTex(base.Texture);

        private static Asset<Texture2D> coolOutlineTex;
        public override void Load() => coolOutlineTex = Request<Texture2D>("GoldLeaf/Core/Mechanics/HealerAggro/HealerAggroBuffCoolOutline");

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            if (Main.LocalPlayer.TryGetModPlayer(out HealerAggroPlayer healerAggroPlayer))
            {
                if (healerAggroPlayer.HealingAggroStage != 0)
                {
                    buffName = Language.GetTextValue("Mods.GoldLeaf.Buffs.HealerAggroBuff.Names.Level" + healerAggroPlayer.HealingAggroStage);
                    if (healerAggroPlayer.HealingAggroStage >= 2)
                        tip += "\n" + Language.GetTextValue("Mods.GoldLeaf.Buffs.HealerAggroBuff.Level2Tooltip");
                    if (healerAggroPlayer.HealingAggroStage == 3)
                        tip += "\n" + Language.GetTextValue("Mods.GoldLeaf.Buffs.HealerAggroBuff.Level3Tooltip", HealerAggroPlayer.PersecutionDamageMult);
                }
            }
        }

        public override void SetStaticDefaults()
        {
            BuffID.Sets.LongerExpertDebuff[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;

            BuffSets.IsRemovable[Type] = false;
            BuffSets.RemoveCleanseTooltip[Type] = true;

            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (Main.LocalPlayer.TryGetModPlayer(out HealerAggroPlayer healerAggroPlayer))
            {
                if (healerAggroPlayer.HealingAggroStage == 0)
                {
                    player.DelBuff(buffIndex);
                    buffIndex--;
                }
                else
                {
                    if (healerAggroPlayer.HealingAggroStage >= 2)
                        player.aggro += healerAggroPlayer.healingAggro;
                    if (healerAggroPlayer.HealingAggroStage >= 3)
                        player.GetModPlayer<GoldLeafPlayer>().damageResistance -= HealerAggroPlayer.PersecutionDamageMult * 0.01f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            if (Main.LocalPlayer.TryGetModPlayer(out HealerAggroPlayer healerAggroPlayer))
            {
                float healingAggro = healerAggroPlayer.healingAggro;
                
                drawParams.MouseRectangle.Height -= 64;
                drawParams.SourceRectangle = drawParams.Texture.Frame(1, 3, 0, healerAggroPlayer.HealingAggroStage - 1);
                drawParams.TextPosition.Y -= 64;

                if (GetInstance<VisualConfig>().CoolBuffs)
                {
                    ColorHelper.Gradient gradient = new([(new Color(235, 171, 81), 0.25f), (new Color(240, 129, 77), 0.5f), (new Color(240, 129, 77), 0.75f), (new Color(244, 82, 72), 1f)]);
                    spriteBatch.Draw(coolOutlineTex.Value, drawParams.Position, drawParams.SourceRectangle, gradient.GetColor((float)healingAggro / HealerAggroPlayer.MaxHealingAggro) * Main.buffAlpha[buffIndex], 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                spriteBatch.Draw(drawParams.Texture, drawParams.Position, drawParams.SourceRectangle, drawParams.DrawColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }
    }
}
