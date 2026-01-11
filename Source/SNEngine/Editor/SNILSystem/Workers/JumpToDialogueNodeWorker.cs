using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Graphs;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Editor.SNILSystem.Workers
{
    public class JumpToDialogueNodeWorker : SNILWorker
    {
        public override void ApplyParameters(BaseNode node, Dictionary<string, string> parameters)
        {
            // Проверяем, что это действительно JumpToDialogueNode или его наследник
            if (node.GetType().Name != "JumpToDialogueNode")
            {
                return;
            }

            foreach (var kvp in parameters)
            {
                var field = node.GetType().GetField(kvp.Key, 
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
                if (field != null && field.FieldType == typeof(DialogueGraph))
                {
                    // Регистрируем отложенную ссылку
                    SNILPostProcessor.RegisterJumpToReference(node, kvp.Key, kvp.Value);
                    SNILDebug.Log($"Registered jump from {node.GUID} to dialogue: {kvp.Value}");
                }
                else if (field != null)
                {
                    // Для других полей устанавливаем значения напрямую
                    object val = ConvertValue(kvp.Value, field.FieldType);
                    if (val != null || !field.FieldType.IsValueType)
                    {
                        field.SetValue(node, val);
                    }
                }
            }
        }

        private static object ConvertValue(string value, System.Type targetType)
        {
            if (targetType == typeof(string)) return value;
            if (targetType == typeof(int)) return int.TryParse(value, out int i) ? i : 0;
            if (targetType == typeof(float)) return float.TryParse(value, out float f) ? f : 0f;
            if (targetType == typeof(bool)) return bool.TryParse(value, out bool b) ? b : false;
            if (targetType.IsEnum) return System.Enum.Parse(targetType, value, true);

            if (typeof(Object).IsAssignableFrom(targetType))
            {
                string filter = $"t:{targetType.Name} {value}";
                string[] guids = AssetDatabase.FindAssets(filter);
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    return AssetDatabase.LoadAssetAtPath(path, targetType);
                }
            }

            return null;
        }
    }
}