using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core
{
    public partial class GoldLeafItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
    }
}
