using GoldLeaf.Core.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.UI.ResourceOverlay.Overhealth
{
	public class OverhealthOverlay : ModResourceOverlay
	{
		public override bool IsLoadingEnabled(Mod mod) => false;
		// This field is used to cache vanilla assets used in the CompareAssets helper method further down in this file
		private Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();

		// These fields are used to cache the result of ModContent.Request<Texture2D>()
		private Asset<Texture2D> heartTexture, greyHeartTexture, barTexture, gayBarTexture;
		private string Directory = "GoldLeaf/UI/ResourceOverlay/Overhealth/";
        public override void Load()
        {
            heartTexture = Request<Texture2D>(Directory + "Heart");
            barTexture = Request<Texture2D>(Directory + "HealthBar");

            greyHeartTexture = Request<Texture2D>(Directory + "HeartGrey");
            gayBarTexture = Request<Texture2D>(Directory + "HealthBarGrey");
        }

        public override bool DisplayHoverText(PlayerStatsSnapshot snapshot, IPlayerResourcesDisplaySet displaySet, bool drawingLife)
        {
            return true;
        }

        public override void PostDrawResource(ResourceOverlayDrawContext context) {
			Asset<Texture2D> asset = context.texture;

			string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
			string barsFolder = "Images/UI/PlayerResourceSets/HorizontalBars/";

			int overhealth = Main.LocalPlayer.GetModPlayer<OverhealthManager>().overhealth;


			// NOTE: CompareAssets is defined below this method's body
			if (asset == TextureAssets.Heart || asset == TextureAssets.Heart2) {
				// Draw over the Classic hearts
				DrawClassicFancyOverlay(context);
			}
			else if (CompareAssets(asset, fancyFolder + "Heart_Fill") || CompareAssets(asset, fancyFolder + "Heart_Fill_B")) {
				// Draw over the Fancy hearts
				DrawClassicFancyOverlay(context);
			}
			else if (CompareAssets(asset, barsFolder + "HP_Fill") || CompareAssets(asset, barsFolder + "HP_Fill_Honey")) {
				// Draw over the Bars life bars
				DrawBarsOverlay(context);
			}
		}

        public override void PostDrawResourceDisplay(PlayerStatsSnapshot snapshot, IPlayerResourcesDisplaySet displaySet, bool drawingLife, Color textColor, bool drawText)
        {
            base.PostDrawResourceDisplay(snapshot, displaySet, drawingLife, textColor, drawText);
        }

		private bool CompareAssets(Asset<Texture2D> existingAsset, string compareAssetPath) {
			// This is a helper method for checking if a certain vanilla asset was drawn
			if (!vanillaAssetCache.TryGetValue(compareAssetPath, out var asset))
				asset = vanillaAssetCache[compareAssetPath] = Main.Assets.Request<Texture2D>(compareAssetPath);

			return existingAsset == asset;
		}

        private void DrawOverhealthHearts(ResourceOverlayDrawContext context, OverhealthManager manager)
        {
            
        }
        private void DrawOverhealthBar(ResourceOverlayDrawContext context)
        {
            
        }
        private void DrawClassicFancyOverlay(ResourceOverlayDrawContext context) {
			// Draw over the Classic / Fancy hearts
			// "context" contains information used to draw the resource
			// If you want to draw directly on top of the vanilla hearts, just replace the texture and have the context draw the new texture
			context.texture = heartTexture ??= ModContent.Request<Texture2D>("ExampleMod/Common/UI/ResourceOverlay/ClassicLifeOverlay");
			context.Draw();
		}

		private void DrawBarsOverlay(ResourceOverlayDrawContext context) {
			// Draw over the Bars life bars
			// "context" contains information used to draw the resource
			// If you want to draw directly on top of the vanilla bars, just replace the texture and have the context draw the new texture
			context.texture = barTexture ??= ModContent.Request<Texture2D>("ExampleMod/Common/UI/ResourceOverlay/BarsLifeOverlay_Fill");
			context.Draw();
		}
	}
}
