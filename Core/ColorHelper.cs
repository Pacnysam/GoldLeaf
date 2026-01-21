using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace GoldLeaf.Core
{
    public static class ColorHelper
    {
        public static Color AdditiveWhite(int alpha = 0) => new(255, 255, 255) { A = (byte)alpha };
        
        public static Color Prefix(bool good = true) => good? new(120, 190, 120) : new(190, 120, 120);

        public readonly struct Gradient(List<(Color, float)> points)
        {
            private struct ColorPoint(Color color, float position)
            {
                public Color color = color;
                public float position = position;
            }

            public Color GetColor(float position)
            {
                List<(Color, float)> colorPoints = [.. points.OrderBy(point => point.Item2)];

                if (colorPoints.First().Item2 > 0f) colorPoints.Insert(0, (colorPoints.First().Item1, 0f));
                if (colorPoints.Last().Item2 < 1f) colorPoints.Add(new(colorPoints.Last().Item1, 1f));

                for (int i = 0; i < colorPoints.Count - 1; i++)
                {
                    ColorPoint color = new(colorPoints.ElementAt(i).Item1, colorPoints.ElementAt(i).Item2);
                    ColorPoint nextColor = new(colorPoints.ElementAt(i + 1).Item1, colorPoints.ElementAt(i + 1).Item2);

                    if (nextColor.position > position)
                    {
                        float pos = Utils.Remap(position, color.position, nextColor.position, 0f, 1f);
                        return color.color.Lerp(nextColor.color, pos);
                    }
                }
                return colorPoints.Last().Item1;
            }
        }

        public static Color AuroraColor(float Timer = default)
        {
            if (Timer == default) Timer = Main.GlobalTimeWrappedHourly * 3f;
            
            var auroraGreen = new Color(0, 255, 189);
            var auroraBlue = new Color(0, 164, 242);
            var auroraPurple = new Color(122, 63, 255);

            return new Gradient([(auroraGreen, 0f), (auroraBlue, 0.25f), (auroraPurple, 0.5f), (auroraBlue, 0.75f), (auroraGreen, 1f)]).GetColor(Timer/8f % 1);
        }
        public static Color AuroraAccentColor(float Timer = default)
        {
            if (Timer == default) Timer = Main.GlobalTimeWrappedHourly * 3f;

            var auroraGreen = new Color(0, 255, 189);
            var auroraBlue = new Color(0, 164, 242);
            var auroraPurple = new Color(122, 63, 255);

            return new Gradient([(auroraPurple, 0f), (auroraBlue, 0.25f), (auroraGreen, 0.5f), (auroraBlue, 0.75f), (auroraPurple, 1f)]).GetColor(Timer / 8f % 1);
        }

        public static Color Lerp(this Color baseColor, Color targetColor, float amount) => Color.Lerp(baseColor, targetColor, amount);

        public static Color MultiplyAlpha(this Color color, float alpha) => new Color(color.R, color.G, color.B, (int)((color.A / 255f) * Math.Clamp(alpha, 0f, 1f) * 255));
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
