using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Debugging
{
    public static class XNodeExtensionsDebug
    {
        private static readonly Dictionary<DebugType, string> _colorsDebug = new Dictionary<DebugType, string>()
        {
            {DebugType.Message, "#69d667" },
            {DebugType.Error, "#d66767" },
            {DebugType.Warning, "#d6d267" }
        };


        public static void Log (object logTarget)
        {
            string message = FormatMessage(logTarget, DebugType.Message);

            Debug.Log(message);
        }

        public static void LogError(object logTarget)
        {
            string message = FormatMessage(logTarget, DebugType.Error);

            Debug.LogError(message);
        }

        public static void LogWarning(object logTarget)
        {
            string message = FormatMessage(logTarget, DebugType .Warning);

            Debug.LogWarning(message);
        }

        private static string FormatMessage (object message, DebugType debugType)
        {
            return $"<color={_colorsDebug[debugType]}>[XNode Extensions {debugType}]</color> {message}";
        }
    }

    public enum DebugType
    {
        Message,
        Error,
        Warning,
    }
}
