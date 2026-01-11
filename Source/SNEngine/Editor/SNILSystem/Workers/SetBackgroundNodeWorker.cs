using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.BackgroundSystem;
using SNEngine.Editor.SNILSystem.ResourceFinder;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Editor.SNILSystem.Workers
{
    public class SetBackgroundNodeWorker : SNILWorker
    {
        public override void ApplyParameters(BaseNode node, Dictionary<string, string> parameters)
        {
            // Проверяем, что это действительно SetBackgroundNode или его наследник
            if (!(node is SetBackgroundNode setBackgroundNode))
            {
                return;
            }

            // Получаем все поля, включая из базовых классов
            var fields = GetAllFields(node.GetType());

            foreach (var kvp in parameters)
            {
                var field = fields.FirstOrDefault(f =>
                    f.Name.Equals(kvp.Key, System.StringComparison.OrdinalIgnoreCase) ||
                    f.Name.Equals("_" + kvp.Key, System.StringComparison.OrdinalIgnoreCase));

                if (field != null)
                {
                    if (field.FieldType == typeof(Sprite))
                    {
                        // Для спрайтов ищем по имени или пути
                        string spritePath = SNILResourceFinder.FindResourcePath(kvp.Value, typeof(Sprite));
                        if (!string.IsNullOrEmpty(spritePath))
                        {
                            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                            if (sprite != null)
                            {
                                field.SetValue(node, sprite);
                            }
                            else
                            {
                                SNILDebug.LogWarning($"Could not load sprite from path: {spritePath}");
                            }
                        }
                        else
                        {
                            SNILDebug.LogWarning($"Could not find sprite with name or path: {kvp.Value}");
                        }
                    }
                    else
                    {
                        object val = ConvertValue(kvp.Value, field.FieldType);
                        if (val != null || !field.FieldType.IsValueType)
                        {
                            field.SetValue(node, val);
                        }
                    }
                }
                else
                {
                    // Если поле не найдено по имени, пробуем найти поле _sprite (для SetBackgroundNode)
                    if (kvp.Key.Equals("sprite", System.StringComparison.OrdinalIgnoreCase) ||
                        kvp.Key.Equals("_sprite", System.StringComparison.OrdinalIgnoreCase))
                    {
                        var spriteField = fields.FirstOrDefault(f => 
                            f.Name.Equals("_sprite", System.StringComparison.OrdinalIgnoreCase) ||
                            f.Name.Equals("Sprite", System.StringComparison.OrdinalIgnoreCase));
                        
                        if (spriteField != null && spriteField.FieldType == typeof(Sprite))
                        {
                            string spritePath = SNILResourceFinder.FindResourcePath(kvp.Value, typeof(Sprite));
                            if (!string.IsNullOrEmpty(spritePath))
                            {
                                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                                if (sprite != null)
                                {
                                    spriteField.SetValue(node, sprite);
                                }
                                else
                                {
                                    SNILDebug.LogWarning($"Could not load sprite from path: {spritePath}");
                                }
                            }
                            else
                            {
                                SNILDebug.LogWarning($"Could not find sprite with name or path: {kvp.Value}");
                            }
                        }
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