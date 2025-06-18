using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using Microsoft.Xna.Framework;
using GoldLeaf.Effects.Dusts;
using GoldLeaf.Tiles.Decor;
using Terraria.Enums;
using Terraria.ObjectData;
using GoldLeaf.Tiles.Grove;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.GameContent.Metadata;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using Terraria.GameContent.ObjectInteractions;
using GoldLeaf.Items.Granite;
using GoldLeaf.Tiles.Granite;

namespace GoldLeaf.Items.Accessories
{
	public class OxeyeDaisy : ModItem
	{
		public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Oxeye Daisy");
		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs("Loves me... loves me not...");

		public override void SetDefaults()
		{
			Item.value = Item.sellPrice(0, 0, 30, 0);
			Item.rare = ItemRarityID.White;

			Item.width = 30;
			Item.height = 32;

            /*Item.createTile = TileType<OxeyeDaisyT>();
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.consumable = true;*/

            ItemID.Sets.IsAMaterial[Item.type] = false;

			Item.accessory = true;
		}

        public override void AddRecipes()
        {
            Recipe oxeyeDye = Recipe.Create(ItemID.BrightSilverDye, 1)
                .AddIngredient(ItemType<OxeyeDaisy>())
                .AddTile(TileID.DyeVat)
                .Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OxeyePlayer>().oxeyeDaisy = true;
            player.GetModPlayer<OxeyePlayer>().oxeyeItem = Item;
            player.GetModPlayer<OxeyePlayer>().cooldown--;
        }
    }

    /*public class OxeyeDaisyPlaceable : ModItem
    {
        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 30, 0);
            Item.rare = ItemRarityID.White;

            Item.width = 16;
            Item.height = 28;

            Item.DefaultToPlaceableTile(TileType<OxeyeDaisyT>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<OxeyeDaisy>())
                .AddCondition(Condition.InGraveyard)
                .Register();
        }
    }*/

    public class OxeyePlayer : ModPlayer
	{
        public bool oxeyeDaisy = false;
        public bool lovesMe = true;
        public int cooldown;
        public Item oxeyeItem;

        public override void ResetEffects()
        {
            oxeyeDaisy = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (oxeyeDaisy) 
            {
                if (cooldown <= 0)
                {
                    lovesMe = !lovesMe;
                    Gore.NewGore(null, Player.Center, new Vector2(Main.rand.NextFloat(-0.2f, -0.65f) * Player.direction, Main.rand.NextFloat(-0.4f, -0.8f)), GoreType<OxeyePetal>());

                    if (target.life <= 0)
                    {
                        cooldown = 600;
                        if (lovesMe && Main.myPlayer == Player.whoAmI)
                        {
                            CombatText.NewText(new Rectangle((int)Player.Center.X, (int)Player.Center.Y - 12, Player.width / 4, Player.height / 4), new Color(235, 99, 139, 100), "Loves me!", true, true);
                            Player.AddBuff(BuffID.Lovestruck, 600);
                            int i = Item.NewItem(Player.GetSource_Accessory(oxeyeItem), target.Hitbox, ItemID.Heart, 1, true, 0, true);
                            Main.item[i].playerIndexTheItemIsReservedFor = Player.whoAmI;

                            if (Main.netMode == NetmodeID.MultiplayerClient && i >= 0)
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 1f);

                            //NewItemPerfect(target.Center, new Vector2(0, -2f), ItemID.Heart);


                            for (int k = 0; k < Main.rand.Next(8 + (target.width / 20), 10 + (target.width / 20)); k++)
                                Gore.NewGore(target.GetSource_Death(), target.Center, new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6)), 331, Main.rand.NextFloat(0.8f, 1.2f));
                                //Gore.NewGorePerfect(null, target.Center, new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6)), 331, Main.rand.NextFloat(0.8f, 1.2f));
                        }
                        else
                        {
                            CombatText.NewText(new Rectangle((int)Player.Center.X, (int)Player.Center.Y - 12, Player.width / 4, Player.height / 4), new Color(211, 63, 62, 100), "Loves me not!", true, true);
                            Player.AddBuff(BuffID.Wrath, 600);
                        }
                    }
                    else
                    {
                        cooldown = 15;
                        if (lovesMe)
                        {
                            CombatText.NewText(new Rectangle((int)Player.Center.X, (int)Player.Center.Y - 12, Player.width / 5, Player.height / 5), new Color(163, 209, 159, 100), "Loves me...", false, true);
                        }
                        else
                        {
                            CombatText.NewText(new Rectangle((int)Player.Center.X, (int)Player.Center.Y - 12, Player.width / 5, Player.height / 5), new Color(163, 209, 159, 100), "Loves me not...", false, true);
                        }
                    }
                }
            }            
        }
    }
    
    public class OxeyeDaisyT : ModTile 
    {
        public override void SetStaticDefaults()
        {
            HitSound = SoundID.Grass;
            DustType = DustID.Grass;

            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;

            AddMapEntry(new Color(235, 255, 240));
            RegisterItemDrop(ItemType<OxeyeDaisy>(), 0);

            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            //TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            Main.tileNoFail[Type] = true;

            TileObjectData.newTile.AnchorValidTiles = [TileID.Grass, TileID.JungleGrass, TileID.MushroomGrass, TileID.CorruptGrass, TileID.CrimsonGrass, TileID.CorruptJungleGrass, TileID.CrimsonJungleGrass, TileID.HallowedGrass, TileType<GroveGrassT>()];
            TileObjectData.newTile.AnchorAlternateTiles = [TileID.ClayPot, TileID.PlanterBox];

            TileObjectData.newTile.CoordinateHeights = [28];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.DrawYOffset = -10;

            //TileID.Sets.ReplaceTileBreakUp[Type] = true;
            TileID.Sets.IgnoredInHouseScore[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;

            TileObjectData.addTile(Type);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void MouseOver(int i, int j)
        {
            Player Player = Main.LocalPlayer;
            Player.cursorItemIconID = ItemType<OxeyeDaisy>();
            Player.noThrow = 2;
            Player.cursorItemIconEnabled = true;
        }

        public override bool RightClick(int i, int j)
        {
            WorldGen.KillTile(i, j);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendTileSquare(-1, i, j);
            return true;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer && Main._shouldUseWindyDayMusic && Main.IsItDay() && Main.rand.NextBool(10) && !Main.gamePaused)
            {
                Dust dust = Dust.NewDustDirect(new Vector2(i * 16, (j - 1) * 16), 16, 16, DustType<LightDustFast>(), 0, 0, 0, new Color(231, 168, 16), Main.rand.NextFloat(0.35f, 0.5f));
                dust.velocity = new Vector2(Main.windSpeedCurrent * 0.025f, Main.rand.NextFloat(-0.4f, -0.65f));
                
                /*if (Main.tile[i, j].WallType != 0)
                    dust.noLight = true;*/
            }
            if (closer && Main._shouldUseWindyDayMusic && Main.IsItDay() && Main.rand.NextBool(45) && !Main.gamePaused)
            {
                Gore.NewGore(new EntitySource_TileUpdate(i, j), new Vector2(i * 16f, j * 16f), new Vector2(Main.windSpeedCurrent * 0.005f, Main.rand.NextFloat(-0.025f, -0.125f)), GoreType<OxeyePetal>(), 1f);
            }
        }
    }
    
    public class OxeyeDaisyFake : OxeyeDaisyT
    {
        public override string Texture => "GoldLeaf/Items/Accessories/OxeyeDaisyT";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            FlexibleTileWand.RubblePlacementSmall.AddVariations(ItemType<OxeyeDaisy>(), Type, 0);
            //RegisterItemDrop(ItemType<OxeyeDaisy>());
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
            => Item.NewItem(null, new Rectangle(i * 16, j * 16, 32, 32), new Item(ItemType<OxeyeDaisy>()), false, true);

        public override bool CanDrop(int i, int j) => false;
    }

    public class OxeyePetal : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.numFrames = 8;
            gore.behindTiles = true;
            gore.timeLeft = Gore.goreTime * 3;
            ChildSafety.SafeGore[gore.type] = true;
        }

        public override bool Update(Gore gore)
        {
            if (Collision.SolidCollision(gore.position, 2, 2))
            {
                gore.alpha++;
                if (gore.alpha > 250)
                    gore.active = false;
                return true;
            }

            gore.position.X += gore.velocity.X + Main.windSpeedCurrent * 2f;

            gore.rotation = gore.velocity.ToRotation() + MathHelper.PiOver2;
            gore.position += gore.velocity;
            //gore.position.Y += gore.velocity.Y;
            gore.velocity.Y += 0.03f;

            
            gore.frameCounter++;
            if (gore.frameCounter > 7)
            {
                gore.frameCounter = 0;
                gore.frame++;
                if (gore.frame > 7) gore.frame = 0;
            }
            return false;
        }

        /*public override Color? GetAlpha(Gore gore, Color lightColor)
        {
            if (Collision.SolidCollision(gore.position, 2, 2))
            {
                return Color.Lerp(lightColor, Color.Transparent, gore.alpha / 750f);
            }
            return null;
        }*/
    }
}