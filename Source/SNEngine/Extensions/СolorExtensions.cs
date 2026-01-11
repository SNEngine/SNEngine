using UnityEngine;

namespace SNEngine.Extensions
{
    public static class ColorExtensions
    {
        public static Color SetAlpha(this Color color, float alpha)
        {
            color.a = Mathf.Clamp01(alpha);
            return color;
        }
    }
}