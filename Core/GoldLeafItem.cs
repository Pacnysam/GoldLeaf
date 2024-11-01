using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core
{
    public partial class GoldLeafItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override void SetDefaults(Item item)
        {
            switch (item.type)
            {
                case ItemID.SlimeStaff:
                    {
                        item.rare = ItemRarityID.Blue;
                        item.damage = 10;
                        break;
                    }
            }
        }
    }
}
