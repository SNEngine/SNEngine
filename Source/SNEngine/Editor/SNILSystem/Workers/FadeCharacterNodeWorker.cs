using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.CharacterSystem.Animations.Fade;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Editor.SNILSystem.Workers
{
    public class FadeCharacterNodeWorker : SNILWorker
    {
        public override void ApplyParameters(BaseNode node, Dictionary<string, string> parameters)
        {
            // Check that this is actually a FadeCharacterNode or its subclass
            if (!(node is FadeCharacterNode fadeNode))
            {
                return;
            }

            // Handle special case for ease parameter using the editor method
            if (parameters.ContainsKey("ease") || parameters.ContainsKey("_ease"))
            {
                string easeValue = parameters.ContainsKey("ease") ? parameters["ease"] : parameters["_ease"];

                // Try to call the ApplyEase_Editor method if it exists
                var applyEaseMethod = node.GetType().GetMethod("ApplyEase_Editor",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                if (applyEaseMethod != null)
                {
                    applyEaseMethod.Invoke(node, new object[] { easeValue });
                    // Remove the ease parameter so it's not processed as a field
                    parameters.Remove("ease");
                    parameters.Remove("_ease");
                }
            }

            // Get all fields including from base classes
            var fields = GetAllFields(node.GetType());

            foreach (var kvp in parameters)
            {
                var field = fields.FirstOrDefault(f =>
                    f.Name.Equals(kvp.Key, System.StringComparison.OrdinalIgnoreCase) ||
                    f.Name.Equals("_" + kvp.Key, System.StringComparison.OrdinalIgnoreCase));

                if (field != null)
                {
                    object val = ConvertValue(kvp.Value, field.FieldType);
                    if (val != null || !field.FieldType.IsValueType)
                    {
                        field.SetValue(node, val);
                    }
                }
            }
        }

        private static FieldInfo[] GetAllFields(System.Type type)
        {
            var fields = new List<FieldInfo>();
            while (type != null && type != typeof(object))
            {
                fields.AddRange(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
                type = type.BaseType;
            }
            return fields.ToArray();
        }

        private static object ConvertValue(string value, System.Type targetType)
        {
            if (targetType == typeof(string)) return value;
            if (targetType == typeof(int)) return int.TryParse(value, out int i) ? i : 0;
            if (targetType == typeof(float))
            {
                // Try parsing with different number styles to handle various formats like 0.0, 0,5, etc.
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float f))
                {
                    return f;
                }
                // Fallback to default parsing
                return float.TryParse(value, out float f2) ? f2 : 0f;
            }
            if (targetType == typeof(bool)) return bool.TryParse(value, out bool b) ? b : false;
            if (targetType.IsEnum)
            {
                try
                {
                    // Handle Ease enum specifically since it's from DOTween
                    if (targetType == typeof(Ease))
                    {
                        // Try to parse the Ease enum value with more robust handling
                        // First, try direct parsing
                        try
                        {
                            return System.Enum.Parse(targetType, value, true);
                        }
                        catch
                        {
                            // If direct parsing fails, try to find the enum value by name ignoring case
                            foreach (var enumValue in System.Enum.GetValues(targetType))
                            {
                                if (enumValue.ToString().Equals(value, System.StringComparison.OrdinalIgnoreCase))
                                {
                                    return enumValue;
                                }
                            }
                            // If still not found, return default (Linear is common)
                            return Ease.Linear;
                        }
                    }
                    return System.Enum.Parse(targetType, value, true);
                }
                catch
                {
                    // If parsing fails completely, try to get the first enum value as default
                    try
                    {
                        return System.Enum.GetValues(targetType).GetValue(0);
                    }
                    catch
                    {
                        // If all fails, return default value for common enums
                        if (targetType == typeof(Ease))
                            return Ease.Linear;
                        return null;
                    }
                }
            }

            if (typeof(Object).IsAssignableFrom(targetType))
            {
                // For Character objects, try to find by name first
                if (targetType == typeof(SNEngine.CharacterSystem.Character))
                {
                    // Try to find character by name in the project
                    string[] guids = AssetDatabase.FindAssets($"t:Character {value}");
                    if (guids.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        return AssetDatabase.LoadAssetAtPath(path, targetType);
                    }
                    // If not found by name, try exact match
                    else
                    {
                        guids = AssetDatabase.FindAssets($"t:Character");
                        foreach (string guid in guids)
                        {
                            string path = AssetDatabase.GUIDToAssetPath(guid);
                            var character = AssetDatabase.LoadAssetAtPath(path, targetType) as SNEngine.CharacterSystem.Character;
                            if (character != null && character.name.Equals(value, System.StringComparison.OrdinalIgnoreCase))
                            {
                                return character;
                            }
                        }
                    }
                }
                else
                {
                    string filter = $"t:{targetType.Name} {value}";
                    string[] guids = AssetDatabase.FindAssets(filter);
                    if (guids.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        return AssetDatabase.LoadAssetAtPath(path, targetType);
                    }
                }
            }

            return null;
        }
    }
}