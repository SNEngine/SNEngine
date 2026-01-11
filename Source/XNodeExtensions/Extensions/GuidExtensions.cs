using System;

namespace SiphoinUnityHelpers.XNodeExtensions.Extensions
{
    public static class GuidExtensions
    {
        public static string ToShortGUID(this Guid guid, int length = 15)
        {
            string formatted = guid.ToString("N");

            if (length <= 0) return string.Empty;
            if (length > formatted.Length) return formatted;

            return formatted.Substring(0, length);
        }
    }
}