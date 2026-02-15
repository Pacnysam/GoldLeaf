using GoldLeaf.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using static GoldLeaf.Core.Helpers.DrawHelper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Items.Vanity
{
    public class AcrylicTurretPaint : ModItem
    {
        private static Asset<Texture2D> paintTex;
        public override void Load()
        {
            paintTex = Request<Texture2D>(Texture + "Color");
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.CritterShampoo);

            Item.width = 36;
            Item.height = 32;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Item.type].Value;
            Vector2 origin = tex.Size() / 2;
            Vector2 drawPos = Item.Bottom - Main.screenPosition - new Vector2(0, origin.Y);

            spriteBatch.Draw(tex, drawPos, null, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
            DrawData data = new(paintTex.Value, drawPos, null, alphaColor, rotation, origin, scale, SpriteEffects.None, 0f);

            if (Main.LocalPlayer.GetModPlayer<TurretPaintPlayer>().cSentry > 0)
            {
                spriteBatch.StartBlendState(BlendState.AlphaBlend, DrawContext.InWorld, SpriteSortMode.Immediate);
                
                GameShaders.Armor.GetShaderFromItemId(Main.LocalPlayer.GetModPlayer<TurretPaintPlayer>().dyeItem).Apply(Item, data);
                data.Draw(spriteBatch);

                spriteBatch.ResetBlendState();
            }
            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D tex = TextureAssets.Item[Item.type].Value;

            spriteBatch.Draw(tex, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            DrawData data = new(paintTex.Value, position, frame, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);

            if (Main.LocalPlayer.GetModPlayer<TurretPaintPlayer>().cSentry > 0)
            {
                spriteBatch.StartBlendState(BlendState.AlphaBlend, DrawContext.UI, SpriteSortMode.Immediate);
                
                GameShaders.Armor.GetShaderFromItemId(Main.LocalPlayer.GetModPlayer<TurretPaintPlayer>().dyeItem).Apply(Item, data);
                data.Draw(spriteBatch);
                
                spriteBatch.ResetBlendState(DrawContext.UI);
            }
            return false;
        }
    }

    public class TurretPaintPlayer : ModPlayer
    {
        public bool TurretPaintOn => cSentry > 0;
        public bool turretPaint;
        public int dyeItem = 0;
        public int cSentry = 0;

        public override void ResetEffects()
        {
            if (!turretPaint)
            {
                cSentry = 0;
                dyeItem = 0;
            }

            turretPaint = false;
        }

        public override void Load()
        {
            On_Player.UpdateItemDye += PickSentryPaint;
            //On_Main.GetProjectileDesiredShader += ApplySentryPaint;
        }
        public override void Unload()
        {
            On_Player.UpdateItemDye -= PickSentryPaint;
            //On_Main.GetProjectileDesiredShader -= ApplySentryPaint;
        }

        private void PickSentryPaint(On_Player.orig_UpdateItemDye orig, Player self, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            orig(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);

            if (armorItem.type == ItemType<AcrylicTurretPaint>())
            {
                self.GetModPlayer<TurretPaintPlayer>().cSentry = dyeItem.dye;
                self.GetModPlayer<TurretPaintPlayer>().dyeItem = dyeItem.type;
            }
        }
        private int ApplySentryPaint(On_Main.orig_GetProjectileDesiredShader orig, Projectile proj)
        {
            if (proj.sentry && proj.owner != 255)
            {
                return Main.player[proj.owner].GetModPlayer<TurretPaintPlayer>().cSentry;
            }
            return orig(proj);
        }
    }
}
