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
using GoldLeaf.Items.Vanity;
using Terraria.Localization;
using GoldLeaf.Items.Misc.Weapons;

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

        public override void SetStaticDefaults()
        {
            //ArmorIDs.Face.Sets.PreventHairDraw[Item.headSlot] = false;
            //ArmorIDs.Face.Sets.DrawInFaceHeadLayer[Item.headSlot] = true;
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

            Item.defense = 3;
            //Item.vanity = true;

            Item.faceSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Face);
            //Item.headSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);

            ItemID.Sets.ShimmerTransformToItem[Item.type] = ItemType<FrigidMask>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<AuroraCluster>(), 16);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class FrostyRobe : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(manaMax, magicDamage * 100);

        private static readonly int manaMax = 40;
        private static readonly float magicDamage = 0.1f;

        public int frontEquip = -1;
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
                frontEquip = EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Waist}", EquipType.Front, this); //TODO draw this manually
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
            ArmorIDs.Body.Sets.IncludedCapeFront[Item.bodySlot] = frontEquip;
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

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<FrostCloth>(), 9);
            recipe.AddIngredient(ItemType<AuroraCluster>(), 12);
            recipe.AddTile(TileID.Loom);
            recipe.Register();
        }
    }

    public class FrostyPlayer : ModPlayer 
    {
        public bool frostySet = false;
        public float frostyDamageBonus = 0.1f;
        public int frostyCooldown = 0;

        public override void Load()
        {
            GoldLeafPlayer.DoubleTapPrimaryEvent += SnapFreeze;
        }

        public override void ResetEffects()
        {
            frostySet = false;
        }

        public override void PostUpdateEquips()
        {
            if (frostySet && frostyCooldown > 0) frostyCooldown--;
        }
        
        private void SnapFreeze(Player player) 
        {
            if (player.GetModPlayer<FrostyPlayer>().frostySet && player.GetModPlayer<FrostyPlayer>().frostyCooldown <= 0)
            {
                frostyCooldown = 180;

                for (float k = 0; k < 6.28f; k += 0.15f)
                    Dust.NewDustPerfect(Main.LocalPlayer.Center, DustType<ArcticDust>(), Vector2.One.RotatedBy(k) * 2);
            }
        }

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

    public class FrigidMask : ModItem //idk how to make FrostyMask draw on accessory face layer, im convinced player layers were created to spite me specifically
    {
        public override string Texture => "GoldLeaf/Items/Blizzard/Armor/FrostyMask";

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
                EquipLoader.AddEquipTexture(Mod, $"GoldLeaf/Items/Blizzard/Armor/FrigidMask_Head", EquipType.Face, this);
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;

            Item.rare = ItemRarityID.Gray;

            ItemID.Sets.TrapSigned[Item.type] = true;

            Item.vanity = true;
            Item.faceSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Face);
            Item.accessory = true;
        }

        private void SetupDrawing()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Face);
        }

        public override void SetStaticDefaults()
        {
            SetupDrawing();
        }
    }
}
