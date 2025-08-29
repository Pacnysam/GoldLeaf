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
using Terraria.Localization;
using Terraria.Audio;
using GoldLeaf.Items.Vanity.Watcher;

namespace GoldLeaf.Items.Vanity.Champion
{
    [AutoloadEquip(EquipType.Front, EquipType.Back)]
    public class ChampionsCloak : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 34;

            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;

            Item.accessory = true;
            Item.vanity = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ChampionSetPlayer>().championCloak = true;
        }
        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<ChampionSetPlayer>().championCloak = true;
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class DragonSkull : ModItem
    {
        public override LocalizedText Tooltip => null;
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;

            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;

            Item.vanity = true;
        }

        public override void SetStaticDefaults()
        {
            ItemSets.FaceMask[Item.type] = true;
            ArmorIDs.Head.Sets.PreventBeardDraw[Item.headSlot] = false;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class ChampionsBelt : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 14;

            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;

            Item.vanity = true;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class GymShorts : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 12;

            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;

            Item.vanity = true;
        }

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}_Female", EquipType.Legs, this, Name + "_Female");
            }
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            equipSlot = EquipLoader.GetEquipSlot(Mod, male? Name : Name + "_Female", EquipType.Legs);
        }
    }
    
    public class ChampionSetPlayer : ModPlayer
    {
        public bool championCloak = false;
        public bool championSet = false;
        public override void ResetEffects()
        {
            championSet = IsVanitySet(Player, ItemType<DragonSkull>(), ItemType<ChampionsBelt>(), ItemType<GymShorts>()) && championCloak;
            championCloak = false;
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (championSet)
            {
                modifiers.DisableSound();
            }
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if (championSet)
            {
                switch (Main.rand.Next(1, 3))
                {
                    case 0:
                        SoundEngine.PlaySound(SoundID.NPCHit10 with { Pitch = -1f, PitchVariance = 0.3f, MaxInstances = 0 }, Player.position); break;
                    case 1:
                        SoundEngine.PlaySound(SoundID.Zombie27 with { Volume = 1.15f, Pitch = 0.3f, PitchVariance = 0.4f, MaxInstances = 0 }, Player.position); break;
                    case 2:
                        SoundEngine.PlaySound(SoundID.NPCDeath8 with { Volume = 1.15f, Pitch = 0.15f, PitchVariance = 0.3f, MaxInstances = 0 }, Player.position); break;
                }
            }
        }
        /*public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (championSet)
            {
                playSound = false;
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }*/
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (championSet)
            {
                /*switch (Main.rand.Next(1, 3))
                {
                    case 0:
                        SoundEngine.PlaySound(SoundID.NPCHit10 with { Pitch = -1f, PitchVariance = 0.3f }, Player.position); break;
                    case 1:
                        SoundEngine.PlaySound(SoundID.Zombie27 with { Volume = 1f, Pitch = 0.3f, PitchVariance = 0.4f }, Player.position); break;
                    case 2:
                        SoundEngine.PlaySound(SoundID.NPCDeath8 with { Pitch = 0.15f, PitchVariance = 0.3f }, Player.position); break;
                }*/

                SoundEngine.PlaySound(SoundID.NPCHit56 with { Volume = 1.15f, Pitch = -0.5f, PitchVariance = 0.3f, MaxInstances = 0 }, Player.position);
            }
        }
    }
}
