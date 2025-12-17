using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace GoldLeaf.Core
{
    public static class ColorHelper
    {
        public static Color AdditiveWhite(int alpha = 0) => new(255, 255, 255) { A = (byte)alpha };
        
        public static Color Prefix(bool good = true) => good? new(120, 190, 120) : new(190, 120, 120);
        
        public static Color AuroraColor()
        {
            float timer = (Main.GlobalTimeWrappedHourly * 3f) % 9;
            var auroraGreen = new Color(0, 255, 189);
            var auroraBlue = new Color(0, 164, 242);
            var auroraPurple = new Color(122, 63, 255);

            if (timer < 2)
                return Color.Lerp(auroraGreen, auroraBlue, timer / 2);
            else if (timer < 4)
                return Color.Lerp(auroraBlue, auroraPurple, (timer - 2) / 2);
            else if (timer < 6)
                return Color.Lerp(auroraPurple, auroraBlue, (timer - 4) / 2);
            else
                return Color.Lerp(auroraBlue, auroraGreen, (timer - 6) / 2);
        }

        public static Color AuroraColor(float Timer)
        {
            float timer = Timer % 9;
            var auroraGreen = new Color(0, 255, 189);
            var auroraBlue = new Color(0, 164, 242);
            var auroraPurple = new Color(122, 63, 255);

            //return Gradient(Timer / 8f, [auroraBlue, auroraPurple, auroraBlue, auroraGreen, auroraBlue]);

            if (timer < 2)
                return Color.Lerp(auroraGreen, auroraBlue, timer / 2);
            else if (timer < 4)
                return Color.Lerp(auroraBlue, auroraPurple, (timer - 2) / 2);
            else if (timer < 6)
                return Color.Lerp(auroraPurple, auroraBlue, (timer - 4) / 2);
            else
                return Color.Lerp(auroraBlue, auroraGreen, (timer - 6) / 2);
        }

        public static Color AuroraAccentColor()
        {
            float timer = Main.GlobalTimeWrappedHourly % 9;
            var auroraGreen = new Color(0, 255, 189);
            var auroraBlue = new Color(0, 164, 242);
            var auroraPurple = new Color(122, 63, 255);

            if (timer < 2)
                return Color.Lerp(auroraPurple, auroraBlue, timer / 2);
            else if (timer < 4)
                return Color.Lerp(auroraBlue, auroraGreen, (timer - 2) / 2);
            else if (timer < 6)
                return Color.Lerp(auroraGreen, auroraBlue, (timer - 4) / 2);
            else
                return Color.Lerp(auroraBlue, auroraPurple, (timer - 6) / 2);
        }

        /*public struct ColorGradient()
        {
            public struct ColorPoint(Color color, float position)
            {
                public Color color = color;
                public float position = position;
            }
            
            private ColorPoint[] colorList { get; private set; }

            public Color GetColor(float progress)
            {
                Array.Sort(colorList, (x, y) => x.position.CompareTo(y.position));
                float adjustedProgress = Utils.Remap(progress, 0f, 1f, 0f, colorList.Length - 1);
                
                for (int i = 0; i < colorList.Length; i++) 
                {
                    float adjustedIndex = Utils.Remap(i, 0, colorList.Length - 1, 0f, 1f);

                    if (i == colorList.Length - 1)
                    {
                        return Color.Lerp(colorList[i].color, colorList[0].color, adjustedIndex);
                    }
                    else if (progress >= colorList[i].position && progress < colorList[i + 1].position)
                    {
                        return Color.Lerp(colorList[i].color, colorList[i + 1].color, adjustedIndex);
                    }
                }
                return colorList[0].color;
            }
        }*/

        /*public static Color Gradient(float position, ReadOnlySpan<Color> colors, bool loop = true)
        {
            //if (loop) colors.Add(colors[0]);
            
            float adjustedPosition = Math.Clamp(position * colors.Length, 0, colors.Length);
            int first = (int)adjustedPosition; int last = Math.Min((int)adjustedPosition + 1, colors.Length);
            
            return Color.Lerp(colors[first], colors[last], adjustedPosition % 1);
        }*/

        public static Color AuroraAccentColor(float Timer)
        {
            float timer = Timer % 9;
            var auroraGreen = new Color(0, 255, 189);
            var auroraBlue = new Color(0, 164, 242);
            var auroraPurple = new Color(122, 63, 255);

            if (timer < 2)
                return Color.Lerp(auroraPurple, auroraBlue, timer / 2);
            else if (timer < 4)
                return Color.Lerp(auroraBlue, auroraGreen, (timer - 2) / 2);
            else if (timer < 6)
                return Color.Lerp(auroraGreen, auroraBlue, (timer - 4) / 2);
            else
                return Color.Lerp(auroraBlue, auroraPurple, (timer - 6) / 2);
        }

        public static Color Lerp(this Color baseColor, Color targetColor, float amount) => Color.Lerp(baseColor, targetColor, amount);

        public static Color MultiplyAlpha(this Color color, float alpha)
        {
            return new Color(color.R, color.G, color.B, (int)((color.A / 255f) * Math.Clamp(alpha, 0f, 1f) * 255));
        }
        public static Color Alpha(this Color color, int alpha = 0) => color with { A = (byte)Math.Clamp(alpha, 0, 255) };
        public static Color Alpha(this Color color, float alpha) => color with { A = (byte)(Math.Clamp(alpha, 0f, 1f) * 255) };

        public static Color GemColor(int gem)
        {
            switch (gem)
            {
                case 1: //amethyst
                case ItemID.Amethyst:
                    {
                        return new Color(193, 47, 246);
                    }
                case 2: //topaz
                case ItemID.Topaz:
                    {
                        return new Color(246, 188, 0);
                    }
                case 3: //sapphire
                case ItemID.Sapphire:
                    {
                        return new Color(86, 135, 255);
                    }
                case 4: //emerald
                case ItemID.Emerald:
                    {
                        return new Color(41, 206, 131);
                    }
                case 5: //ruby
                case ItemID.Ruby:
                    {
                        return new Color(237, 26, 30);
                    }
                case 6: //diamond
                case ItemID.Diamond:
                    {
                        return Color.White;
                    }
                case 7: //amber
                case ItemID.Amber:
                    {
                        return new Color(244, 133, 27);
                    }
            }
            return Color.White;
        }

        public static Color RarityColor(int rarity)
        {
            switch (rarity)
            {
                case ItemRarityID.Gray:
                    {
                        return Colors.RarityTrash;
                    }
                case ItemRarityID.White:
                    {
                        return Color.White;
                    }
                case ItemRarityID.Blue:
                    {
                        return Colors.RarityBlue;
                    }
                case ItemRarityID.Green:
                    {
                        return Colors.RarityGreen;
                    }
                case ItemRarityID.Orange:
                    {
                        return Colors.RarityOrange;
                    }
                case ItemRarityID.LightRed:
                    {
                        return Colors.RarityRed;
                    }
                case ItemRarityID.Pink:
                    {
                        return Colors.RarityPink;
                    }
                case ItemRarityID.LightPurple:
                    {
                        return Colors.RarityPurple;
                    }
                case ItemRarityID.Lime:
                    {
                        return Colors.RarityLime;
                    }
                case ItemRarityID.Yellow:
                    {
                        return Colors.RarityYellow;
                    }
                case ItemRarityID.Cyan:
                    {
                        return Colors.RarityCyan;
                    }
                case ItemRarityID.Red:
                    {
                        return Colors.RarityDarkRed;
                    }
                case ItemRarityID.Purple:
                    {
                        return Colors.RarityDarkPurple;
                    }
                case ItemRarityID.Expert:
                    {
                        return Main.DiscoColor;
                    }
                case ItemRarityID.Master:
                    {
                        return Main.mcColor;
                    }
                case ItemRarityID.Quest:
                    {
                        return Colors.RarityAmber;
                    }
            }
            return Color.White;
        }

        /// <summary>
        /// Overhealth color (R:19,G:223,B:229,A:255).
        /// </summary>
        public static Color Overhealth => new(19, 223, 229, 255);
        /// <summary>
        /// SlimeBlue color (R:0,G:80,B:255,A:125-175).
        /// </summary>
        public static Color SlimeBlue => new(0, 80, 255, 255);
        /// <summary>
		/// SlimeBlueSimple color (R:112,G:172,B:244,A:255).
		/// </summary>
        public static Color SlimeBlueSimple => new(112, 172, 244, 255);
    }
}
