using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Extensions
{
    public static class ColorExtensions
    {

        public static string ToColorTag(this Color color, string source = "")
        {
            return GetTag(color, source);
        }

        public static string ToColorTag(this Color32 color, string source = "")
        {
            return GetTag(color, source);
        }

        private static string GetTag (Color color, string source = "")
        {
            string hex = ColorUtility.ToHtmlStringRGB(color);

            return $"<color=#{hex}>{source}</color>";
        }
    }
}
