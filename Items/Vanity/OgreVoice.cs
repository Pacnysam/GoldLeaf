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
using static tModPorter.ProgressUpdate;
using Terraria.Graphics.Effects;
using GoldLeaf.Items.Accessories;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.GameContent.Drawing;

namespace GoldLeaf.Items.Vanity
{
    public class OgreVoice : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;

            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;

            Item.accessory = true;
            Item.vanity = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OgreVoicePlayer>().ogrephone = true;
        }
        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<OgreVoicePlayer>().ogrephone = true;
        }
    }

    public class OgreVoicePlayer : ModPlayer
    {
        public bool ogrephone = false;
        public override void ResetEffects()
        {
            ogrephone = false;
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (ogrephone)
            {
                modifiers.DisableSound();
            }
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if (ogrephone)
            {
                SoundEngine.PlaySound(SoundID.DD2_OgreHurt, Player.position);

                /*if (Main.rand.NextBool())
                    SoundEngine.PlaySound(SoundID.DD2_OgreHurt, Player.position);
                else
                    SoundEngine.PlaySound(SoundID.DD2_OgreRoar, Player.position);*/
            }
        }
        /*public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (ogrephone)
            {
                playSound = false;
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }*/
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (ogrephone)
            {
                SoundEngine.PlaySound(SoundID.DD2_OgreRoar, Player.position);
                SoundEngine.PlaySound(SoundID.DD2_OgreDeath, Player.position);
            }
        }
    }
}
