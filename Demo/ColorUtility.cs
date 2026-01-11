using UnityEngine;

namespace CoreGame.Utilities
{
    public static class ColorUtility
    {
        public static Color Darken(Color color, float percent)
        {
            float r = color.r * (1f - percent);
            float g = color.g * (1f - percent);
            float b = color.b * (1f - percent);
            return new Color(r, g, b, color.a);
        }
    }
}