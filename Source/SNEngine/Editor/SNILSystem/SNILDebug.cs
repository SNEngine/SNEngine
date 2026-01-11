using System.Collections.Generic;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem
{
    public class SNILDebug
    {
        private static readonly Dictionary<DebugType, string> _colorsDebug = new Dictionary<DebugType, string>()
        {
            {DebugType.Message, "#6a6cde" },
            {DebugType.Error, "#de6a6a" },
            {DebugType.Warning, "#ded26a" }
        };


        public static void Log(object logTarget)
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
            string message = FormatMessage(logTarget, DebugType.Warning);

            Debug.LogWarning(message);
        }

        private static string FormatMessage(object message, DebugType debugType)
        {
            return $"<color={_colorsDebug[debugType]}>[SNIL {debugType}]</color> {message}";
        }
    }

    internal enum DebugType
    {
        Message,
        Error,
        Warning,
    }
}