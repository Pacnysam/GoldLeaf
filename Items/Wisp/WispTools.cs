using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GoldLeaf.Items.Wisp
{
	public class WispHammer : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Wispy.");
		}

		public override void SetDefaults()
		{
			item.damage = 8;
			item.melee = true;
			item.width = 30;
			item.height = 30;
			item.useTime = 12;
			item.useAnimation = 12;
			item.hammer = 65;
			item.useStyle = 1;
			item.knockBack = 1f;
			item.rare = 2;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
		}
	}

    public class WispPick : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Wispful.");
        }

        public override void SetDefaults()
        {
            item.damage = 5;
            item.melee = true;
            item.width = 30;
            item.height = 30;
            item.useTime = 8;
            item.useAnimation = 8;
            item.pick = 65;
            item.useStyle = 1;
            item.knockBack = 1f;
            item.rare = 2;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }
    }

    public class WispAxe : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Of Wisp.");
		}

		public override void SetDefaults()
		{
			item.damage = 7;
			item.melee = true;
			item.width = 30;
			item.height = 30;
			item.useTime = 10;
			item.useAnimation = 10;
			item.axe = 13;
			item.useStyle = 1;
			item.knockBack = 1f;
			item.rare = 2;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
		}
	}
}