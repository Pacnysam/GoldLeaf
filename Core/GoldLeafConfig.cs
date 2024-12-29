using Terraria.ModLoader.Config;

using GoldLeaf.Core;
using static Terraria.ModLoader.ModContent;
using static GoldLeaf.Core.Helper;
using System.ComponentModel;

namespace GoldLeaf.Core
{
    public class GameplayConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Label("Critical Strike Gimmicks")]
        [Tooltip("Enables additional class based critical strike gimmicks")]
        [DefaultValue(true)]
        public bool ClassGimmicks = true;

        [Label("Gem Staff Reworks")]
        [Tooltip("Reworks gem staves")]
        [ReloadRequired]
        [DefaultValue(true)]
        public bool OreStaffReworks = true;

        [Label("Buff Changes")]
        [Tooltip("Modifies several buffs")]
        [ReloadRequired]
        [DefaultValue(true)]
        public bool BuffChanges = true;
    }

    public class GraphicsConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("Screenshake")]
        [Tooltip("Screenshake Intensity")]
        [Range(0f, 1f)]
        [Slider]
        [DefaultValue(1f)]
        public float ShakeIntensity = 1;

        [Label("Crafting Visuals")]
        [Tooltip("Enables visual effects when crafting an item")]
        [DefaultValue(true)]
        public bool OnCraftEffects = true;

        [Label("Coolbuffs Support")]
        [Tooltip("Changes buff textures to support the Cool Buffs resource pack")]
        [ReloadRequired]
        [DefaultValue(false)]
        public bool CoolBuffs = false;

        /*[Label("Radcap Support")]
        [Tooltip("Changes textures to support the Radcap resource pack")]
        [ReloadRequired]
        [DefaultValue(false)]
        public bool Radcap = false;*/
    }
}
