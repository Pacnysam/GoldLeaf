using GoldLeaf.Items.Vanity;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using Terraria.ModLoader;
using GoldLeaf.Core;
using Microsoft.Xna.Framework;

namespace GoldLeaf.Items.Granite
{
	public class GraniteVoice : ModItem
	{
        private static Asset<Texture2D> glowTex;
        public override void Load()
        {
            glowTex = Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            ItemSets.Glowmask[Type] = (glowTex, Color.White with { A = 120 }, true);
        }

        public override void SetDefaults()
		{
			Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Green;

            Item.width = 34;
            Item.height = 38;

            Item.accessory = true;
			Item.vanity = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GraniteVoicePlayer>().graniteVoice = true;
        }
        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<GraniteVoicePlayer>().graniteVoice = true;
        }
    }

    public class GraniteVoicePlayer : ModPlayer
    {
        public bool graniteVoice = false;
        public override void ResetEffects()
        {
            graniteVoice = false;
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (graniteVoice)
            {
                modifiers.DisableSound();
            }
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if (graniteVoice)
            {
                SoundEngine.PlaySound(SoundID.NPCHit41 with { Volume = 1.1f }, Player.position);
            }
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (graniteVoice)
            {
                playSound = false;
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (graniteVoice)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath43 with { Volume = 1.1f }, Player.position);
            }
        }
    }
}