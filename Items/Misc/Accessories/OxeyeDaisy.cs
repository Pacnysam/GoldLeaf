using static Terraria.ModLoader.ModContent;
using GoldLeaf.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using GoldLeaf.Items.Nightshade;
using Microsoft.Xna.Framework;
using GoldLeaf.Items.Pickups;
using GoldLeaf.Effects.Dusts;
using Microsoft.Build.Tasks;
using GoldLeaf.Tiles.Decor;
using Terraria.Enums;
using Terraria.ObjectData;
using GoldLeaf.Tiles.Grove;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.GameContent.Metadata;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria.GameContent;

namespace GoldLeaf.Items.Misc.Accessories
{
	public class OxeyeDaisy : ModItem
	{
		public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs("Oxeye Daisy");
		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs("Loves me... loves me not...");

		public override void SetDefaults()
		{
			Item.value = Item.sellPrice(0, 0, 30, 0);
			Item.rare = ItemRarityID.Blue;

			Item.width = 30;
			Item.height = 32;

			Item.accessory = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OxeyePlayer>().oxeyeDaisy = true;
            player.GetModPlayer<OxeyePlayer>().cooldown--;
        }
    }

	public class OxeyePlayer : ModPlayer
	{
        public bool oxeyeDaisy = false;
        public bool lovesMe = true;
        public int cooldown;

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
                    Gore.NewGore(null, Main.LocalPlayer.Center, new Vector2(Main.rand.NextFloat(-0.2f, -0.65f) * Player.direction, Main.rand.NextFloat(-0.4f, -0.8f)), GoreType<OxeyePetal>());

                    if (target.life <= 0)
                    {
                        cooldown = 600;
                        if (lovesMe)
                        {
                            CombatText.NewText(new Rectangle((int)Main.LocalPlayer.Center.X, (int)Main.LocalPlayer.Center.Y - 12, Main.LocalPlayer.width / 4, Main.LocalPlayer.height / 4), new Color(235, 99, 139, 100), "Loves me!", true, true);
                            Main.LocalPlayer.AddBuff(BuffID.Lovestruck, 600);
                            //Item.NewItem(Player.GetSource_OnHit(target), target.Hitbox, ItemID.Heart);
                            NewItemPerfect(target.Center, new Vector2(0, -2f), ItemID.Heart);


                            for (int k = 0; k < Main.rand.Next(8 + (target.width / 20), 10 + (target.width / 20)); k++)
                                Gore.NewGore(null, target.Center, new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6)), 331, Main.rand.NextFloat(0.8f, 1.2f));
                                //Gore.NewGorePerfect(null, target.Center, new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6)), 331, Main.rand.NextFloat(0.8f, 1.2f));
                        }
                        else
                        {
                            CombatText.NewText(new Rectangle((int)Main.LocalPlayer.Center.X, (int)Main.LocalPlayer.Center.Y - 12, Main.LocalPlayer.width / 4, Main.LocalPlayer.height / 4), new Color(211, 63, 62, 100), "Loves me not!", true, true);
                            Main.LocalPlayer.AddBuff(BuffID.Wrath, 600);
                        }
                    }
                    else
                    {
                        cooldown = 15;
                        if (lovesMe)
                        {
                            CombatText.NewText(new Rectangle((int)Main.LocalPlayer.Center.X, (int)Main.LocalPlayer.Center.Y - 12, Main.LocalPlayer.width / 5, Main.LocalPlayer.height / 5), new Color(163, 209, 159, 100), "Loves me...", false, true);
                        }
                        else
                        {
                            CombatText.NewText(new Rectangle((int)Main.LocalPlayer.Center.X, (int)Main.LocalPlayer.Center.Y - 12, Main.LocalPlayer.width / 5, Main.LocalPlayer.height / 5), new Color(163, 209, 159, 100), "Loves me not...", false, true);
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

            AddMapEntry(new Color(235, 255, 240));
            RegisterItemDrop(ItemType<OxeyeDaisy>());

            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            //TileObjectData.newTile.StyleHorizontal = true;
            //TileObjectData.newTile.CoordinatePadding = 2;
            Main.tileNoFail[Type] = true;

            //TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorValidTiles = [TileID.Grass, TileID.JungleGrass, TileID.HallowedGrass, TileType<GroveGrassT>()];

            TileObjectData.newTile.CoordinateHeights = [28];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.DrawYOffset = -10;

            TileID.Sets.ReplaceTileBreakUp[Type] = true;
            TileID.Sets.IgnoredInHouseScore[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;

            TileObjectData.addTile(Type);
        }
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