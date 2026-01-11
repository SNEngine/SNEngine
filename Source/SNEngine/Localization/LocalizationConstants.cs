using SNEngine.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SNEngine.Localization
{
    public static class LocalizationConstants
    {
        private static readonly Dictionary<string, string> _localizedStrings = new Dictionary<string, string>()
        {
            {"%APPNAME%", Application.productName},
            {"%VERSION%", Application.version },
            {"%COMPANYNAME%", Application.companyName},
        };

        public static string GetValue (string key)
        {
            if (_localizedStrings.TryGetValue(key, out var value))
            {
                return value;
            }
            NovelGameDebug.LogError($"[LocalizationConstants] key {key} not found");
            return key;
        }

        
    }
}
